using System;
using System.Collections.Generic;
using ZidUtilities.CommonCode.Win;
using ZidUtilities.CommonCode.Win.Controls;
using System.Windows.Forms;

namespace ZidUtilities.TesterWin
{
    public partial class FormTokenSelectTest : Form
    {
        public FormTokenSelectTest()
        {
            InitializeComponent();
        }

        private void FormTokenSelectTest_Load(object sender, EventArgs e)
        {
            // Populate theme combo box
            foreach (ZidThemes theme in Enum.GetValues(typeof(ZidThemes)))
            {
                cmbThemes.Items.Add(theme);
                themeComboBox.Items.Add(theme);
            }
            cmbThemes.SelectedItem = ZidThemes.Default;
            themeComboBox.SelectedItem = ZidThemes.Default;

            // Initialize Countries data source (Dictionary)
            var countries = new Dictionary<string, object>
            {
                { "United States", "US" },
                { "United Kingdom", "GB" },
                { "Canada", "CA" },
                { "Australia", "AU" },
                { "Germany", "DE" },
                { "France", "FR" },
                { "Italy", "IT" },
                { "Spain", "ES" },
                { "Japan", "JP" },
                { "China", "CN" },
                { "India", "IN" },
                { "Brazil", "BR" },
                { "Mexico", "MX" },
                { "Argentina", "AR" },
                { "South Africa", "ZA" },
                { "Russia", "RU" },
                { "South Korea", "KR" },
                { "Netherlands", "NL" },
                { "Belgium", "BE" },
                { "Switzerland", "CH" }
            };
            tokenSelectCountries.SetDataSource(countries);

            // Initialize Genres data source (Separate Lists)
            var genreNames = new List<string>
            {
                "Action", "Adventure", "Animation", "Biography", "Comedy",
                "Crime", "Documentary", "Drama", "Family", "Fantasy",
                "Film-Noir", "History", "Horror", "Music", "Musical",
                "Mystery", "Romance", "Sci-Fi", "Sport", "Thriller",
                "War", "Western"
            };
            var genreIds = new List<object>();
            for (int i = 0; i < genreNames.Count; i++)
            {
                genreIds.Add(i + 1);
            }
            tokenSelectGenres.SetDataSource(genreNames, genreIds);

            // Initialize Priority data source (Single Selection)
            var priorities = new Dictionary<string, object>
            {
                { "Critical", 5 },
                { "High", 4 },
                { "Medium", 3 },
                { "Low", 2 },
                { "Trivial", 1 }
            };
            tokenSelectSingle.SetDataSource(priorities);

            // Initialize Team Members data source (Max 3 Tokens)
            var teamMembers = new Dictionary<string, object>
            {
                { "Alice Johnson", "alice@example.com" },
                { "Bob Smith", "bob@example.com" },
                { "Charlie Brown", "charlie@example.com" },
                { "Diana Prince", "diana@example.com" },
                { "Edward Norton", "edward@example.com" },
                { "Fiona Green", "fiona@example.com" },
                { "George Miller", "george@example.com" },
                { "Hannah Lee", "hannah@example.com" }
            };
            tokenSelectMaxTokens.SetDataSource(teamMembers);

            // Initial log messages
            AddLog("TokenSelect Test Form loaded successfully");
            AddLog("Try typing in the controls to see autocomplete in action!");
        }

        private void themeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;


            if (cmb != null && cmb.SelectedItem != null)
            {
                ZidThemes selectedTheme = (ZidThemes)cmb.SelectedItem;

                themeManager1.Theme = selectedTheme;
                themeManager1.ApplyTheme();
                
                //tokenSelectCountries.Theme = selectedTheme;
                //tokenSelectGenres.Theme = selectedTheme;
                //tokenSelectSingle.Theme = selectedTheme;
                //tokenSelectMaxTokens.Theme = selectedTheme;

                //AddLog($"Theme changed to: {selectedTheme}");
            }
        }

        private void TokenSelect_OnTokenAdded(object sender, TokenSelect.TokenEventArgs e)
        {
            AddLog($"[ADDED] Display: '{e.DisplayText}', Value: '{e.Value}'");
            UpdateSelectedValues();
        }

        private void TokenSelect_OnTokenRemoved(object sender, TokenSelect.TokenEventArgs e)
        {
            AddLog($"[REMOVED] Display: '{e.DisplayText}', Value: '{e.Value}'");
            UpdateSelectedValues();
        }

        private void btnGetValues_Click(object sender, EventArgs e)
        {
            AddLog("=== Current Selections ===");

            AddLog($"Countries: {string.Join(", ", tokenSelectCountries.SelectedDisplayTexts)}");
            AddLog($"  Values: {string.Join(", ", tokenSelectCountries.SelectedValues)}");

            AddLog($"Genres: {string.Join(", ", tokenSelectGenres.SelectedDisplayTexts)}");
            AddLog($"  Values: {string.Join(", ", tokenSelectGenres.SelectedValues)}");

            AddLog($"Priority: {string.Join(", ", tokenSelectSingle.SelectedDisplayTexts)}");
            AddLog($"  Values: {string.Join(", ", tokenSelectSingle.SelectedValues)}");

            AddLog($"Team: {string.Join(", ", tokenSelectMaxTokens.SelectedDisplayTexts)}");
            AddLog($"  Values: {string.Join(", ", tokenSelectMaxTokens.SelectedValues)}");

            AddLog("======================");

            UpdateSelectedValues();
        }

        private void btnClearCountries_Click(object sender, EventArgs e)
        {
            tokenSelectCountries.ClearTokens();
            AddLog("Cleared all countries");
        }

        private void btnAddProgrammatic_Click(object sender, EventArgs e)
        {
            tokenSelectGenres.AddToken("Action", 1);
            AddLog("Programmatically added 'Action' genre");
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            logListBox.Items.Clear();
        }

        private void chkReadOnly_CheckedChanged(object sender, EventArgs e)
        {
            tokenSelectCountries.IsReadOnly = chkReadOnly.Checked;
            AddLog($"Countries control read-only: {chkReadOnly.Checked}");
        }

        private void UpdateSelectedValues()
        {
            var allValues = new List<object>();
            allValues.AddRange(tokenSelectCountries.SelectedValues);
            allValues.AddRange(tokenSelectGenres.SelectedValues);
            allValues.AddRange(tokenSelectSingle.SelectedValues);
            allValues.AddRange(tokenSelectMaxTokens.SelectedValues);

            var allTexts = new List<string>();
            allTexts.AddRange(tokenSelectCountries.SelectedDisplayTexts);
            allTexts.AddRange(tokenSelectGenres.SelectedDisplayTexts);
            allTexts.AddRange(tokenSelectSingle.SelectedDisplayTexts);
            allTexts.AddRange(tokenSelectMaxTokens.SelectedDisplayTexts);

            selectedValuesLabel.Text = $"Values: {(allValues.Count > 0 ? string.Join(", ", allValues) : "None")}";
            selectedTextsLabel.Text = $"Texts: {(allTexts.Count > 0 ? string.Join(", ", allTexts) : "None")}";
        }

        private void AddLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            logListBox.Items.Add($"[{timestamp}] {message}");
            logListBox.TopIndex = logListBox.Items.Count - 1;
        }
    }
}
