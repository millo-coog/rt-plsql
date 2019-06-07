using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace RT {
	static class Program {
		#region Constants
		// Colors
		public static Color PARAMETER_COLOR = Color.FromArgb(192, 224, 255);
		public static Color RETURN_VALUE_COLOR = Color.FromArgb(224, 255, 224);
		
		// Command-line argument names
		public const string MULTIPLE_INSTANCES = "multipleinstances";
		public const string RUN_DB_ARG = "rundb";
		public const string HELP_ARG = "?";

		// Cell value indicators
		public const string MATRIX_CELL_TEXT_INDICATOR = "<matrix...>";
		public const string STRONG_REF_CURSOR_CELL_TEXT_INDICATOR = "<strng rf crsr...>";
		public const string TYPE_CELL_TEXT_INDICATOR = "<type...>";
		#endregion

		#region Form variables
		// Main form
		public static frmMain mainForm;

		// Docked forms
		public static frmNavigation navigationForm;
		public static frmOutput outputForm;
		public static frmTest testForm;
		public static frmSCNGroup scnGroupForm;
		public static frmLibraryItems libraryItemsForm;
		public static frmScenarioGroupStartupHook scenarioGroupStartupForm;
		public static frmScenarioStartupHook scenarioStartupForm;
		public static frmScenarios scenariosForm;
		public static frmPostParamAssignmentHook postParamAssignmentForm;
		public static frmPreUDCsHook preUDCsForm;
		public static frmUDC udcForm;
		public static frmScenarioTeardownHook scenarioTeardownForm;
		public static frmScenarioGroupTeardownHook scenarioGroupTeardownForm;
		#endregion

		// Project
		public static project currProject;
		
		// Global field change tracker
		public static RT.fieldChangeTracker fieldTracker = new RT.fieldChangeTracker();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args) {
			Boolean singleAppInstance = true; // Assume that we only want one instance of the program running.
						
			int returnCode = 0;
			bool launchGUI = true; // Whether or not to launch the GUI (certain command line arguments don't need the GUI)

			updateUserSettings();

			// Open up the default project...
			if (Properties.Settings.Default.repos_exportPath != "" && Directory.Exists(path: Properties.Settings.Default.repos_exportPath)) {
				currProject = new project(projectFilename: Properties.Settings.Default.repos_exportPath + "project.xml", excludedSchemas: Properties.Settings.Default.schemas_excludedSchemas);
			}

			if (args.Length > 0) {
				// Running in command line mode...
				string unrecognizedArgs = String.Empty;
				bool showArgHelp = false;
				string argHelp = String.Empty;
				
				// Give the user any unrecognized arguments and show him the valid arguments...
				if (unrecognizedArgs != String.Empty) {
					argHelp = "Unrecognized Arguments: " + unrecognizedArgs.TrimEnd(' ') + "\n\n";

					returnCode = 1;
				} else {
					try {
						for (int i = 0; i < args.Length; i++) {
							string argumentKeyValue = args[i];

							string argumentKey = argumentKeyValue.Trim('/').Split('=')[0].ToLower();
							string argumentValue = string.Empty;

							if (argumentKeyValue.Trim('/').Split('=').Count() == 2) {
								argumentValue = argumentKeyValue.Trim('/').Split('=')[1];
							}

							switch (argumentKey) {
								case MULTIPLE_INSTANCES:
									singleAppInstance = false;
									break;

								case RUN_DB_ARG:
									Program.openDBConnections();

									Program.currProject.repository.runAllTests(currProject: Program.currProject);

									Program.closeDBConnections();

									launchGUI = false;

									break;

								case HELP_ARG:
									showArgHelp = true;
									launchGUI = false;
									break;

								default:
									showArgHelp = true;

									break;
							}
						}
					} catch (Exception err) {
						MessageBox.Show(err.Message);
					}
				}

				if (showArgHelp) {
					string argsString = String.Empty;

					for (int i = 0; i < args.Count(); i++) {
						argsString += "\"" + args[i] + "\" ";
					}

					argHelp +=
						"Arguments:\n" +
						"/" + MULTIPLE_INSTANCES + " - Allows multiple instances of the program\n" +
						"/" + RUN_DB_ARG + " - Runs all tests against the target DB\n" +
						"/" + HELP_ARG + " - This help list\n\n" +
						"Actually Passed: " + argsString;

					MessageBox.Show(argHelp);
				}
			}

			if (launchGUI) {
				// Running in interactive mode....
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				if (singleAppInstance)
					myCustomApplication.Run(mainForm = new frmMain());
				else
					Application.Run(mainForm = new frmMain());
				
				// Close open connections
				closeDBConnections();
			}

			return returnCode;
		}

		private static void updateUserSettings() {
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
			Version appVersion = a.GetName().Version;
			string appVersionString = appVersion.ToString();

			if (Properties.Settings.Default.app_version != appVersion.ToString()) {
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.app_version = appVersionString;
			}
		}
				
		public static OracleConnection getCurrentTargetDBConnection() {
			return ((databaseTreeNode) Program.navigationForm.projectTreeView.selectedNode.getParentNode(schemaNodeType.database)).targetDatabase.conTargetDB;
		}

		// Re-opens any already-open connections
		public static void reopenDBConnections() {
			if (currProject != null) {
				for (int i = 0; i < currProject.targetDBs.Count; i++) {
					if (currProject.targetDBs[i].conTargetDB.State == ConnectionState.Open) {
						currProject.targetDBs[i].conTargetDB.Close();
						currProject.targetDBs[i].conTargetDB.Open();
					}
				}
			}
		}

		// Opens the application-wide connections to the target database and repository database.
		public static void openDBConnections(bool showPreferencesDialog = true) {
			closeDBConnections();

			if (Properties.Settings.Default.target_dbUsername != "" && Properties.Settings.Default.target_dbPassword != String.Empty) {
				for (int i = 0; i < currProject.targetDBs.Count; i++) {
					currProject.targetDBs[i].open(username: Properties.Settings.Default.target_dbUsername, password: RT.security.DecryptString(Properties.Settings.Default.target_dbPassword));

					if (currProject.targetDBs[i].conTargetDB.State != ConnectionState.Open) {
						if (showPreferencesDialog) {
							frmPreferences myForm = new frmPreferences();

							myForm.ShowDialog();
						} else {
							throw new Exception("Couldn't open connections.");
						}
					}
				}
			} else {
				MessageBox.Show("Database credentials not set!");
			}
		}

		// Closes the application-wide connections, if necessary
		public static void closeDBConnections() {
			if (currProject != null) {
				for (int i = 0; i < currProject.targetDBs.Count; i++) {
					if (currProject.targetDBs[i].conTargetDB.State == ConnectionState.Open) {
						currProject.targetDBs[i].conTargetDB.Close();
					}
				}
			}
		}
	}
}
