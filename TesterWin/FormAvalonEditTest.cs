using System;
using System.IO;
using System.Windows.Forms;
using ZidUtilities.CommonCode;

namespace ZidUtilities.TesterWin
{
    public partial class FormAvalonEditTest : Form
    {
        public FormAvalonEditTest()
        {
            InitializeComponent();
        }

        private void extEditor_Load(object sender, EventArgs e)
        {

        }

        private void loadSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = @"CREATE PROCEDURE [dbo].[StartDistribution](@SalaryBudget FLOAT, @EquityBudget FLOAT, @ReviewPeriod INT, @CutOffDate DATETIME, @TopPerformerBudgetPercent FLOAT, @UserId INT, @LastCompRevisionDateAccepted DATE) AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRANSACTION;

	--before running this SP , we need to have the ranks updated and the employee data completed(JobCode, LocationName, Rating, Salary, Equity)
	DECLARE @NeededBudgetSalary FLOAT, @NeededBudgetEquity FLOAT, @FactorSalary FLOAT, @FactorEquity FLOAT, @EmployeeId INT, @CompensationPeriodId INT, @CompensationDetailId INT, @Msg VARCHAR(200), @GeoFactor FLOAT
	DECLARE @JobCodeId INT, @JobCode VARCHAR(20), @Location VARCHAR(100), @LocationId INT, @CreationActionLogId INT, @Salary FLOAT, @Rating INT, @Equity INT, @LastCompensationPeriodId INT

	SELECT
		@LastCompensationPeriodId = MAX(Cp.Id)
	FROM
		dbo.CompensationPeriods Cp

	--FIXME maybe there is a better way to get the budgets...
	--determine the expected budget for the salary raise, and for the equity raise
	SELECT
		@NeededBudgetSalary = ROUND(SUM( ISNULL(Rr.Raise, 0) * ISNULL(Gf.Factor, 1) ), 3),
		@NeededBudgetEquity = ROUND(SUM( ISNULL(EM.Equity,0) * ISNULL(Gf.Factor, 1) ), 0)
	FROM
		dbo.Employee AS E
		INNER JOIN [dbo].[GetEmployeesForCompensationTool] () AS EmpsForComp ON E.EmployeeID = EmpsForComp.EmployeeId
		LEFT OUTER JOIN dbo.Location L ON E.LocationName = L.Name
		LEFT OUTER JOIN dbo.GeoFactors Gf ON L.GeoFactor = Gf.Id
		OUTER APPLY dbo.GetRecommendedRaisePercent (E.JobCode, E.Salary, E.Rating) AS Rr
		LEFT JOIN dbo.JobCodes J ON E.JobCode=J.Code --[aaguirre]
		LEFT JOIN [dbo].[EquityMatrix] EM ON (J.Category = EM.Category AND J.[Level]= EM.[Level] AND E.Rating = EM.Rating) --[aaguirre]
	WHERE
		dbo.DetermineElegibility (E.EmployeeID, @LastCompensationPeriodId) = 1 --only elegible employees are considered for a raise

	--TODO this should be printed only when debugging, need to remove when it gets to prod
	PRINT 'Needed budget for Salary:'
	PRINT @NeededBudgetSalary
	PRINT 'Needed budget for Equity:'
	PRINT @NeededBudgetEquity

	BEGIN TRY

		IF @NeededBudgetSalary = 0 OR @NeededBudgetEquity = 0
		BEGIN
	   	RAISERROR ('We dont need any money and/or equity for this compensation period!', -- Message text.
	              16, -- Severity.
	              1 -- State.
	              );
		END

		--fold Here we create first a the Compensation period record

			--Deactivate active compensation period, if any(create a log action for this deactivation)
			INSERT INTO dbo.ActionLogs (ActionType, CompensationPeriod, [DATE], Description, Employee)
			SELECT
				9,--Automatic cancellation
				Cp.Id,
				getdate(),
				'Compensation period cancelled automatically, because the creation of a new compensation period',
				@UserId--Employee that is creating the new compensation period
			FROM
				dbo.CompensationPeriods Cp
			WHERE
				Cp.Status = 1--Any compensation period which is active

			UPDATE
				dbo.CompensationPeriods
			SET
				dbo.CompensationPeriods.Status = 3, --Set as Cancelled...
				dbo.CompensationPeriods.EndDate = getdate()
			WHERE
				dbo.CompensationPeriods.Status = 1--Any compensation period which is active

			INSERT INTO dbo.CompensationPeriods (EquityBudget, EquityExpectedBudget, ReviewPeriod, SalaryBudget, SalaryExpectedBudget, StartDate, Status, TopPerformerBudget, CutOffDate, LastCompRevisionAccepted)
			VALUES(
				@EquityBudget,
				@NeededBudgetEquity,
				@ReviewPeriod,
				@SalaryBudget * (1.00 - @TopPerformerBudgetPercent),
				@NeededBudgetSalary,
				getdate(),
				1,--Active
				@SalaryBudget * @TopPerformerBudgetPercent,
				@CutOffDate,
				@LastCompRevisionDateAccepted
				)

			--store the id of this compensation period to later use
			SET @CompensationPeriodId = SCOPE_IDENTITY()

			PRINT 'Compensation Period:'
			PRINT @CompensationPeriodId

			--Create a log action for this Creation of a new compensation period
			INSERT INTO dbo.ActionLogs (ActionType, CompensationPeriod, [DATE], Description, Employee)
			SELECT
				5,--Distribution started
				Cp.Id,
				getdate(),
				'A new compensation period has been created.',
				@UserId --Employee that is creating the new compensation period
			FROM
				dbo.CompensationPeriods Cp

			--store the id of the action to reference to it later
			SET @CreationActionLogId = SCOPE_IDENTITY()

			PRINT 'Action Log:'
			PRINT @CreationActionLogId

		--/fold


		SELECT
			@FactorSalary = cp.FactorSalary,
			@FactorEquity = cp.FactorEquity
		FROM
			dbo.CompensationPeriods cp
		WHERE
			cp.Id = @CompensationPeriodId

		TRUNCATE TABLE RecommendedBefDist
		INSERT INTO RecommendedBefDist
		SELECT
			E.EmployeeID,
			Rr.Percentile,
			Rr.Raise,
			Rr.RaisePerc,
			CASE
				dbo.DetermineElegibility (E.EmployeeID, 1000)
				WHEN 1 THEN ROUND(E.Salary * Rr.RaisePerc * Gf.Factor, 3)
				ELSE 0
			END AS RaiseConsideringElegibility
		FROM
			dbo.Employee AS E LEFT OUTER JOIN dbo.Location L ON E.LocationName = L.Name
			LEFT OUTER JOIN dbo.GeoFactors Gf ON L.GeoFactor = Gf.Id
			OUTER APPLY dbo.GetRecommendedRaisePercent (E.JobCode, E.Salary, E.Rating) AS Rr
		WHERE
			E.Enabled = 1

	END TRY
	BEGIN CATCH
	   IF @@TRANCOUNT > 0
	       ROLLBACK TRANSACTION;

		EXEC dbo.ReThrowErrorWithLogging
			@Debug = 1, --bit
			@logError = 1, --bit
			@ProcedureName = 'StartDistribution' --varchar
	END CATCH

	IF @@TRANCOUNT > 0
	   COMMIT TRANSACTION;
END
";

			if(!File.Exists("SampleSQL.txt"))
				code.WriteToTextFile("SampleSQL.txt");

            extEditor.Text = code;
            extEditor.Syntax = CommonCode.AvalonEdit.SyntaxHighlighting.TransactSQL;
        }

        private void removeHighlightingToolStripMenuItem_Click(object sender, EventArgs e)
        {
			extEditor.Syntax = CommonCode.AvalonEdit.SyntaxHighlighting.None;
        }

        private void loadCToolStripMenuItem_Click(object sender, EventArgs e)
        {
			string code = String.Empty;

            if (File.Exists("SampleCSharp.txt"))
			{
				code = "SampleCSharp.txt".ReadFromTextFile();
            }
			else
			{
                code = "D:\\Just For Fun\\ZidUtilities\\CommonCode\\Extensions.cs".ReadFromTextFile();
				code.WriteToTextFile("SampleCSharp.txt");
            }

            extEditor.Syntax = CommonCode.AvalonEdit.SyntaxHighlighting.CSharp;
			extEditor.Text = code;
        }

        private void loadXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = String.Empty;

            if (File.Exists("SampleXml.txt"))
            {
                code = "SampleXml.txt".ReadFromTextFile();
            }
            else
            {
                code = "C:\\Temp\\DataComparisonReport.xml".ReadFromTextFile();
                code.WriteToTextFile("SampleXml.txt");
            }

            extEditor.Syntax = CommonCode.AvalonEdit.SyntaxHighlighting.XML;
            extEditor.Text = code;
        }

        private void loadHtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = String.Empty;

            if (File.Exists("SampleHtml.txt"))
            {
                code = "SampleHtml.txt".ReadFromTextFile();
            }
            else
            {
                code = "C:\\Temp\\DataComparisonReport.html".ReadFromTextFile();
                code.WriteToTextFile("SampleHtml.txt");
            }

			extEditor.Syntax = CommonCode.AvalonEdit.SyntaxHighlighting.HTML;
            extEditor.Text = code;
        }

        private void loadJavaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = String.Empty;

            if (File.Exists("SampleJavaScript.txt"))
            {
                code = "SampleJavaScript.txt".ReadFromTextFile();
            }
            else
            {
                code = "D:\\Restart\\PlusSalud\\PlusSalud\\Website\\Scripts\\Common.js".ReadFromTextFile();
                code.WriteToTextFile("SampleJavaScript.txt");
            }

            extEditor.Syntax = CommonCode.AvalonEdit.SyntaxHighlighting.JavaScript;
            extEditor.Text = code;
        }

        private void loadCssToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = String.Empty;

            if (File.Exists("SampleCss.txt"))
            {
                code = "SampleCss.txt".ReadFromTextFile();
            }
            else
            {
                code = "D:\\Trash\\Content\\sb-admin.css".ReadFromTextFile();
                code.WriteToTextFile("SampleCss.txt");
            }

            extEditor.Syntax = CommonCode.AvalonEdit.SyntaxHighlighting.CSS;
            extEditor.Text = code;
        }

        private void loadJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = String.Empty;

            if (File.Exists("SampleJson.txt"))
            {
                code = "SampleJson.txt".ReadFromTextFile();
            }
            else
            {
                code = "D:\\Trash\\LogSample.json".ReadFromTextFile();
                code.WriteToTextFile("SampleJson.txt");
            }

            extEditor.Syntax = CommonCode.AvalonEdit.SyntaxHighlighting.Json;
            extEditor.Text = code;
        }

        private void extEditor_OnRun(string selectedText, CommonCode.AvalonEdit.ToolbarOption btnClicked)
        {
			MessageBox.Show($"You clicked the {btnClicked.Name} button!");
        }
    }
}
