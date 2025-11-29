using System;
using System.IO;
using System.Windows.Forms;
using ZidUtilities.CommonCode;

namespace ZidUtilities.TesterWin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void extendedEditor1_Load(object sender, EventArgs e)
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
		
		DECLARE PTR CURSOR FOR
		SELECT EmployeeId, JobCode, LocationName, Salary, Rating, Equity  FROM [dbo].[GetEmployeesForCompensationTool] ()
	
		OPEN PTR
	
		FETCH NEXT FROM PTR
		INTO @EmployeeId, @JobCode, @Location, @Salary, @Rating, @Equity
	
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @JobCodeId = NULL
			
			--validate the jobcode does exists
			IF NOT EXISTS (SELECT Jc.Code FROM dbo.JobCodes Jc WHERE Jc.Code = @JobCode)
			BEGIN
				SELECT @Msg = 'Job code not found: ' + @JobCode + ', for the Employee with Id: ' + CONVERT(VARCHAR(10), @EmployeeId)
				RAISERROR(@Msg, 16, 1)
			END
			--get the JobCodeId
			SELECT @JobCodeId = Jc.Id FROM dbo.JobCodes Jc WHERE Jc.Code = @JobCode
			
			--validate that the location does exists, to be mapped to our location list
			IF NOT EXISTS (SELECT Lc.Name FROM dbo.Location Lc WHERE Lc.Name = @Location)
			BEGIN
				SELECT @Msg = 'Location not found: ' + @Location + ', for the Employee with Id: ' + CONVERT(VARCHAR(10), @EmployeeId)
				RAISERROR(@Msg, 16, 1)
			END
			--get the LocationId
			SELECT @LocationId = Lc.LocationID, @GeoFactor = Gf.Factor FROM dbo.Location Lc INNER JOIN dbo.GeoFactors Gf ON Lc.GeoFactor = Gf.Id WHERE Lc.Name = @Location
			
			
			INSERT INTO dbo.CompensationDetails(CompensationPeriod, Employee, IsElegible, IsPromoted, IsTopPerformer, IsWaitingReclasification, JobCode, Location, NewJobCode, RadfordPercentile, Rating, Salary, Equity, SubmissionRank, Rank, Manager, RecommendedSalaryRaise, RecommendedEquityRaise)
			SELECT
				@CompensationPeriodId,
				E.EmployeeID,
				dbo.DetermineValidityForComp (@EmployeeId, @CompensationPeriodId),--Determine if the current employee is elegible for this comp period
				0,                                     --Promoted Default: false
				0,                                     --Top Performer Default: false
				0,                                     --Waiting Reclasification Default: false
				@JobCodeId,
				@LocationId,
				NULL,                                  --New job code default: false
				Rr.Percentile,                         --Percentile from the current salary(before the raise)
				E.Rating,
				E.Salary,
				E.Equity,
				99,                                    --default level for submission rank
				E.Rank, 							  --employee's rank
				E.ManagerID,                           --employee's manager
				CASE 
					dbo.DetermineElegibility (@EmployeeId, @CompensationPeriodId) 
					WHEN 1 THEN ROUND(@Salary * Rr.RaisePerc * @GeoFactor * @FactorSalary, 3)
					ELSE 0 
				END,                                   --Recommended Salary Raise * @GeoFactor * Factor
				CASE 
					dbo.DetermineElegibility (@EmployeeId, @CompensationPeriodId) 
					WHEN 1 THEN 
						--ROUND(CONVERT(FLOAT, @Equity) * Rr.RaisePerc * @GeoFactor * @FactorEquity, 0)
						ROUND(CONVERT(FLOAT, ISNULL(EM.Equity,0)) * @GeoFactor * @FactorEquity, 0) --[aaguirre]
					ELSE 0 
				END                                    --Recommended Equity Raise * @GeoFactor * Factor
			FROM
				dbo.Employee AS E 
				OUTER APPLY dbo.GetRecommendedRaisePercent (E.JobCode, E.Salary, E.Rating) AS Rr
				LEFT JOIN dbo.JobCodes J ON E.JobCode=J.Code --[aaguirre]
				LEFT JOIN [dbo].[EquityMatrix] EM ON (J.Category = EM.Category AND J.[Level]= EM.[Level] AND E.Rating = EM.Rating) --[aaguirre]
			WHERE
				E.EmployeeID = @EmployeeId
			
			--store the id of this compensation detail to later use
			SET @CompensationDetailId = SCOPE_IDENTITY()
	
			
			--start salary history
			INSERT INTO dbo.SalaryHistory (ActionLog, CompensationDetail, Creation, IsLatest, RadfordPercentile, SalaryRaise, RecommendedSalaryRaise)
			SELECT
				@CreationActionLogId, --Action Log
				@CompensationDetailId,--Compensation Period
				getdate(),            --Creation of the record
				1,                    --Is this the latest(since is the first record, it always is)
				dbo.GetRadfordPercentile 
				(
					@JobCode /*varchar*/,
					@Salary + 
						CASE
							dbo.DetermineElegibility (@EmployeeId, @CompensationPeriodId) 
							WHEN 1 THEN ROUND(@Salary * Rr.RaisePerc * @GeoFactor * @FactorSalary, 3)
							ELSE 0 
						END, 
					@Rating /*int*/
				), 					 --Percentile according to Radford data
				CASE 
					dbo.DetermineElegibility (@EmployeeId, @CompensationPeriodId) 
					WHEN 1 THEN ROUND(@Salary * Rr.RaisePerc * @GeoFactor * @FactorSalary, 3)
					ELSE 0 
				END,                  --Manager Salary Raise
				CASE 
					dbo.DetermineElegibility (@EmployeeId, @CompensationPeriodId) 
					WHEN 1 THEN ROUND(@Salary * Rr.RaisePerc * @GeoFactor * @FactorSalary, 3)
					ELSE 0 
				END                   --Recommended Salary Raise
			FROM
				dbo.GetRecommendedRaisePercent (@JobCode /*int*/, @Salary /*float*/, @Rating /*int*/) AS Rr
			
			--start equity history
			INSERT  INTO dbo.EquityHistory (ActionLog, CompensationDetail, Creation, EquityRaise, RecommendedEquityRaise, IsLatest)
			SELECT
				@CreationActionLogId, --Action Log
				@CompensationDetailId,--Compensation Period
				getdate(),            --Creation of the record
				CASE 
					dbo.DetermineElegibility (@EmployeeId, @CompensationPeriodId) 
					WHEN 1 THEN 
						--ROUND(CONVERT(FLOAT, @Equity) * Rr.RaisePerc * @GeoFactor * @FactorEquity, 0)
						ROUND(CONVERT(FLOAT, ISNULL(EM.Equity,0)) * @GeoFactor * @FactorEquity, 0) --[aaguirre]
					ELSE 0 
				END,                  --Manager Equity Raise
				CASE 
					dbo.DetermineElegibility (@EmployeeId, @CompensationPeriodId) 
					WHEN 1 THEN 
						--ROUND(CONVERT(FLOAT, @Equity) * Rr.RaisePerc * @GeoFactor * @FactorEquity, 0)
						ROUND(CONVERT(FLOAT, ISNULL(EM.Equity,0)) * @GeoFactor * @FactorEquity, 0) --[aaguirre]
					ELSE 0 
				END,                  --Recommended Equity Raise
				1                     --Is this the latest(since is the first record, it always is)
			FROM
				dbo.GetRecommendedRaisePercent (@JobCode /*int*/, @Salary /*float*/, @Rating /*int*/) AS Rr
				JOIN dbo.Employee AS E ON (E.EmployeeID = @EmployeeId) --[aaguirre]
				LEFT JOIN dbo.JobCodes J ON E.JobCode=J.Code --[aaguirre]
				LEFT JOIN [dbo].[EquityMatrix] EM ON (J.Category = EM.Category AND J.[Level]= EM.[Level] AND E.Rating = EM.Rating) --[aaguirre]

				
			--start top performer history
			INSERT INTO dbo.TopPerformerHistory (ActionLog, CompensationDetail, Creation, IsLatest, TopPerformerRaise)
			VALUES(
				@CreationActionLogId, --Action Log
				@CompensationDetailId,--Compensation Period
				getdate(),            --Creation of the record
				1,                    --Is this the latest(since is the first record, it always is)
				0                     --Top performer salary raise always starts as 0, since this is expected to be set by the VP MANUALLY
				)
			
			--To start the team budget history(bith tables) we need to have all the employees in the CompensationDetails table,
			--so this is also pending until the end of the execution of the cursor
			
			FETCH NEXT FROM PTR
			INTO @EmployeeId, @JobCode, @Location, @Salary, @Rating, @Equity
		END
	
		CLOSE PTR
		DEALLOCATE PTR
		
		--From the data calculated before we now populate the team budget history tables
		INSERT INTO dbo.BudgetSalaryHistory (ActionLog, CompensationDetail, Creation, IsLatest, TotalBudget)
		SELECT
			@CreationActionLogId,
			Cd.Id,
			getdate(),
			1,
			ROUND(ISNULL(( SELECT SUM(Cd2.RecommendedSalaryRaise) FROM dbo.GetHierarchyNode (Cd.Employee) Hn INNER JOIN dbo.CompensationDetails Cd2 ON Hn.EmployeeId = Cd2.Employee AND Cd2.CompensationPeriod = @CompensationPeriodId WHERE Hn.EmployeeId != Cd.Employee ), 0), 3)
		FROM
			dbo.CompensationDetails Cd
		WHERE
			Cd.CompensationPeriod = @CompensationPeriodId
	
		INSERT INTO dbo.BudgetEquityHistory (ActionLog, CompensationDetail, Creation, IsLatest, TotalBudget)
		SELECT
			@CreationActionLogId,
			Cd.Id,
			getdate(),
			1,
			ROUND(ISNULL(( SELECT SUM(Cd2.RecommendedEquityRaise) FROM dbo.GetHierarchyNode (Cd.Employee) Hn INNER JOIN dbo.CompensationDetails Cd2 ON Hn.EmployeeId = Cd2.Employee AND Cd2.CompensationPeriod = @CompensationPeriodId WHERE Hn.EmployeeId != Cd.Employee ), 0), 0)
		FROM
			dbo.CompensationDetails Cd
		WHERE
			Cd.CompensationPeriod = @CompensationPeriodId	
			
		--Then we update the complete the compensation details table with the explanation of why each employee has no raise recommendation
		UPDATE
			Cd
		SET
			Cd.RecommendationZero = dbo.GetZeroRecommendationExplanation ( Cd.Employee, Cd.CompensationPeriod )
		FROM
			dbo.CompensationDetails Cd
		WHERE
			Cd.CompensationPeriod = @CompensationPeriodId
			AND Cd.RecommendedSalaryRaise = 0
		
		--refresh the ranks for each employee
		EXEC dbo.RefreshEmployeeRanks 
		
		--adjustments to the equity budget
		DECLARE @CurEqB INT, @MainCompDetail INT, @SecondaryCompDetail INT
		SELECT
			@CurEqB = Beh.TotalBudget,
			@MainCompDetail = Cd.Id
		FROM
			dbo.BudgetEquityHistory Beh INNER JOIN dbo.CompensationDetails Cd ON Cd.Id = Beh.CompensationDetail
		WHERE
			Cd.Employee = 344--main
			AND Cd.CompensationPeriod = @CompensationPeriodId
		
		--ensure main has the whole budget, that was meant to be distributed
		UPDATE
			Beh
		SET
			Beh.AvailableBudget = 0,
			Beh.TotalBudget = @EquityBudget
		FROM
			dbo.BudgetEquityHistory Beh
		WHERE
			Beh.CompensationDetail = @MainCompDetail
		
		--now, we need to adjust secondary's equity with the difference
		SELECT
			@SecondaryCompDetail = Cd.Id
		FROM
			dbo.BudgetEquityHistory Beh INNER JOIN dbo.CompensationDetails Cd ON Cd.Id = Beh.CompensationDetail
		WHERE
			Cd.Employee = 31--secondary
			AND Cd.CompensationPeriod = @CompensationPeriodId		
		
		UPDATE
			Beh
		SET
			Beh.TotalBudget = Beh.TotalBudget + (@CurEqB - @EquityBudget),
			Beh.AvailableBudget = (@CurEqB - @EquityBudget)
		FROM
			dbo.BudgetEquityHistory AS Beh
		WHERE
			Beh.CompensationDetail = @SecondaryCompDetail
			
		
		
			
		
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

            extEditor.Editor.Text = code;
            extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.TransactSQL;
			extEditor.Editor.Refresh();
        }

        private void removeHighlightingToolStripMenuItem_Click(object sender, EventArgs e)
        {
			extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
			extEditor.Editor.Refresh();
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

            extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.CSharp;
			extEditor.Editor.Text = code;
            extEditor.Editor.Refresh();
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

            extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.XML;
            extEditor.Editor.Text = code;
            extEditor.Editor.Refresh();
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
            
			extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.HTML;
            extEditor.Editor.Text = code;
            extEditor.Editor.Refresh();
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

            extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.JavaScript;
            extEditor.Editor.Text = code;
            extEditor.Editor.Refresh();
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
            
            extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.CSS;
            extEditor.Editor.Text = code;
            extEditor.Editor.Refresh();
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

            extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.Json;
            extEditor.Editor.Text = code;
            extEditor.Editor.Refresh();
        }

        private void extEditor_OnRun(string selectedText, CommonCode.ICSharpTextEditor.ToolbarOption btnClicked)
        {
			MessageBox.Show($"You clicked the {btnClicked} button!");
        }
    }
}
