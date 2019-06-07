using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RT {
	public partial class frmPreferences : Form {
		public frmPreferences() {
			InitializeComponent();
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.Hide();
		}

		private void frmPreferences_Load(object sender, EventArgs e) {
			// Get the user's connection information
			txtTargetUsername.Text = Properties.Settings.Default.target_dbUsername;
			txtTargetPassword.Text = RT.security.DecryptString(Properties.Settings.Default.target_dbPassword);

			// Get other preferences
			txtFileDifferFilename.Text = Properties.Settings.Default.fileDiffer_executable;
			txtFileDifferParameters.Text = Properties.Settings.Default.fileDiffer_parameters;

			txtExcludedSchemas.Text = Properties.Settings.Default.schemas_excludedSchemas;

			chkClearIloTasksBetweenScenarios.Checked = false; // @@@ Need to get from a project preference
			txtNumThreads.Text = "1"; // @@@ Need to get from a project preference

			txtExportPath.Text = Properties.Settings.Default.repos_exportPath;
			txtXMLEditorPath.Text = Properties.Settings.Default.repos_xmlEditor;

			txtPLSQLEditorPath.Text = Properties.Settings.Default.plsql_editor;
		}

		private void btnSave_Click(object sender, EventArgs e) {
			// Save the user's new settings...
			Properties.Settings.Default.target_dbUsername = txtTargetUsername.Text;
			Properties.Settings.Default.target_dbPassword = RT.security.EncryptString(txtTargetPassword.Text);

			Properties.Settings.Default.fileDiffer_executable = txtFileDifferFilename.Text;
			Properties.Settings.Default.fileDiffer_parameters = txtFileDifferParameters.Text;

			Properties.Settings.Default.schemas_excludedSchemas = txtExcludedSchemas.Text.Trim();

			Properties.Settings.Default.repos_exportPath = txtExportPath.Text.Trim().TrimEnd('\\') + "\\";
			if (Properties.Settings.Default.repos_exportPath == "\\") {
				Properties.Settings.Default.repos_exportPath = "";
			}

			if (Program.currProject != null) {
				Program.currProject.repository.workingCopyPath = Properties.Settings.Default.repos_exportPath;
			}

			Properties.Settings.Default.repos_xmlEditor = txtXMLEditorPath.Text.Trim();

			Properties.Settings.Default.plsql_editor = txtPLSQLEditorPath.Text.Trim();

			Properties.Settings.Default.Save();

			// Re-open database connections based on the new settings...
			RT.Program.reopenDBConnections();

			// Save any repository preferences to the repository...
			//rtRepos.setPreference("repos.use_ilo", chkClearIloTasksBetweenScenarios.Checked ? "Y" : "N"); // @@@ Need to save project settings
			//rtRepos.setPreference("target.parallelism", txtNumThreads.Text); // @@@ Need to save project settings
			
			this.Hide();
		}
		
		private void btnBrowseForXMLEditorPath_Click(object sender, EventArgs e) {
			ofdXMLEditorPath.ShowDialog();
		}

		private void ofdXMLEditorPath_FileOk(object sender, CancelEventArgs e) {
			txtXMLEditorPath.Text = ofdXMLEditorPath.FileName;
		}

		private void btnBrowseForPLSQLEditorPath_Click(object sender, EventArgs e) {
			ofdPLSQLEditorPath.ShowDialog();
		}

		private void ofdPLSQLEditorPath_FileOk(object sender, CancelEventArgs e) {
			txtPLSQLEditorPath.Text = ofdPLSQLEditorPath.FileName;
		}
	}
}
