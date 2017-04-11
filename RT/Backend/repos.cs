﻿/*
 * This class manages the interaction with the file-based repository where the tests are stored.
 */
using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Oracle.DataAccess.Client;
using System.Threading.Tasks;
using System.Threading;

namespace RT {
	#region Delegates
	// Fires when the regression test's run status changes (i.e. we start/stop running a test).
	public delegate void runStatusChangedHandler(string msg, bool isError);

	// Fires when an individual scenario is done running.
	public delegate void scenarioRunCompletedHandler(int folderKey, string testName, string scenarioGroupGuid, string scenarioGroupName, int scenarioIndex, string scenarioGUID, string status, int errorNumber, string errorMessage);
	#endregion

	#region Enumerations
	// An enumeration of possible target statuses.
	public enum targetStatus {
		noTests = 0,
		testsNotRun = 1,
		testsOk = 2,
		testsFailed = 4
	}
	#endregion

	public class repos {
		#region Constants
		public const double WC_FORMAT_VERSION = 3; // The format version the regression tester will save tests in.
		#endregion

		#region Private variables
		private scenarioRunResults prvRunResults; // Cached copy of all the run results.
		private project myProject; // Project that owns this repository
		#endregion

		#region Properties
		// Returns all run results. Note: uses caching.
		public scenarioRunResults runResults {
			get {
				if (prvRunResults == null) {
					prvRunResults = new RT.scenarioRunResults(associatedProject: this.myProject);

					prvRunResults.loadResults();
				}

				return prvRunResults;
			}
		}

		public string workingCopyPath { get; set; }
		#endregion

		#region Constructor
		public repos(project associatedProject) {
			myProject = associatedProject;

			if (associatedProject.filename != "")
				workingCopyPath = Path.GetDirectoryName(associatedProject.filename) + "\\";
			else {
				workingCopyPath = "";
			}
		}
		#endregion

		#region Methods
		public void addDatabaseConnection(string databaseName) {
			if (System.IO.Directory.Exists(workingCopyPath + "\\" + databaseName.Trim())) {
				throw(new Exception(message: "The database '" + databaseName.Trim() + "' already exists in this project."));
			} else {
				Directory.CreateDirectory(workingCopyPath + "\\" + databaseName.Trim());
			}
		}

		// Calculates the working copy path that will hold the given target object.
		// Returns "<working copy>\<db>\<schema>\<objectType>\<name>\<method>\".
		private string getTestFolderPath(string databaseName, string schema, string objectType, string name, string method = "") {
			string destFolder = workingCopyPath + databaseName + "\\" + schema + "\\";

			switch (objectType) {
				case "TRIGGER":
					destFolder += "Triggers";
					break;
				case "PACKAGE":
					destFolder += "Packages";
					break;
				case "DATABASE":
					destFolder += "Database-Level";
					break;
				case "PROCEDURE":
					destFolder += "Procedures";
					break;
				case "FUNCTION":
					destFolder += "Functions";
					break;
				case "TYPE":
					destFolder += "Types";
					break;
				case "VIEW":
					destFolder += "Views";
					break;
				default:
					destFolder += objectType;
					break;
			}

			destFolder += "\\" + name;

			if (method != String.Empty && method != null) {
				destFolder += "\\" + method;
			}

			return destFolder;
		}

		// Returns the fully-qualified folder path that holds the given test's .xml file.
		public string getTestFolderPath(test objTest) {
		   return getTestFolderPath(databaseName: objTest.databaseName, schema: objTest.unitSchema, objectType: objTest.unitType, name: objTest.unitName, method: objTest.unitMethod);
		}
		
		private string getTestFilename(string testName) {
			return testName.Replace("\\", "_").Replace("/", "_") + ".xml";
		}
		
		public string getTestFilename(test objTest) {
			return getTestFilename(objTest.name);
		}

		// Deletes the given test object's corresponding file in the working copy...
		public void deleteTestFromWC(test testToDelete) {
			if (workingCopyPath != String.Empty) {
				// Remove the physical file...
				System.IO.File.Delete(testToDelete.xmlFilename);
				
				// Remove this test's scenario's guid's from the run results...
				for (int i = 0; i < testToDelete.scenarioGroups.Count; i++) {
					for (int j = 0; j < testToDelete.scenarioGroups[i].scenarios.Count; j++) {
						if (prvRunResults.ContainsKey(testToDelete.scenarioGroups[i].scenarios[j].guid)) {
							prvRunResults.Remove(testToDelete.scenarioGroups[i].scenarios[j].guid);
						}
					}
				}
			}
		}

		public List<String> getDatabaseNames() {
			List<String> lstDBNames = new List<String>();
			System.IO.DirectoryInfo[] subDirs = new DirectoryInfo(workingCopyPath).GetDirectories();

			for (int i = 0; i < subDirs.Count(); i++) {
				if (subDirs[i].Attributes.HasFlag(FileAttributes.Hidden) == false && subDirs[i].Name != ".svn" && subDirs[i].Name != ".hg") {
					lstDBNames.Add(subDirs[i].Name);
				}
			}

			return lstDBNames;
		}
		
		// Returns a list of test objects that correspond with the given database target and run type.
		public List<test> getTestList(
			string databaseName = "",
			string objectType = "",
			string schema = "",
			string name = "",
			string method = "",
			int overload = -1,
			System.IO.DirectoryInfo root = null) // Used to recurse into subfolders
		{
			List<test> lstTests = new List<test>();
			string wcDBLocation = String.Empty;
			string wcSchemaLocation = String.Empty;
			string wcObjectTypeLocation = String.Empty;
			string wcNameLocation = String.Empty;
			string wcMethodLocation = String.Empty;
			
			System.IO.FileInfo[] files = null;
			System.IO.DirectoryInfo[] subDirs = null;

			if (root == null) {
				root = new DirectoryInfo(workingCopyPath + "\\" + databaseName);
			} else {
				// Make sure we were given a valid starting place
				if (!Directory.Exists(root.FullName))
					return lstTests;

				// Figure out where we are in the directory structure
				String[] arrFolders = root.FullName.Replace(workingCopyPath, String.Empty).Split('\\');

				wcDBLocation = arrFolders[0];

				if (arrFolders.Count() >= 2) {
					wcSchemaLocation = arrFolders[1];

					// See if we can skip this schema folder...
					if (schema != String.Empty && wcSchemaLocation != schema)
						return lstTests;
					
					if (arrFolders.Count() >= 3) {
						wcObjectTypeLocation = arrFolders[2].ToUpper().TrimEnd('S');

						// See if we can skip this object type folder...
						if (objectType != String.Empty && wcObjectTypeLocation != objectType)
							return lstTests;

						if (arrFolders.Count() >= 4) {
							wcNameLocation = arrFolders[3];

							// See if we can skip this name folder...
							if (name != String.Empty && wcNameLocation != name)
								return lstTests;

							if (arrFolders.Count() >= 5) {
								wcMethodLocation = arrFolders[4];

								// See if we can skip this method folder...
								if (method != String.Empty && wcMethodLocation != method)
									return lstTests;
							}
						}
					}
				}
			}
			
			files = root.GetFiles("*.*");

			if (files != null) {
				foreach (System.IO.FileInfo fileInfo in files) {
					bool add = false;

					//Debug.WriteLine("Checking " + fileInfo.FullName);

					if ((databaseName == "" || wcDBLocation.ToUpper() == databaseName.ToUpper())
						&& (objectType == "" || wcObjectTypeLocation.ToUpper() == objectType.ToUpper())
						&& (schema == "" || wcSchemaLocation.ToUpper() == schema.ToUpper())
						&& (name == "" || wcNameLocation.ToUpper() == name.ToUpper())
						&& (method == "" || wcMethodLocation.ToUpper() == method.ToUpper()))
					{
						if (overload == -1) {
							add = true;
						} else {
							Debug.WriteLine("Constructing test for overloaded target: " + fileInfo.FullName);
							test tempTest = new test(associatedProject: this.myProject, xmlFilename: fileInfo.FullName);

							if (tempTest.overload == overload)
								add = true;

							tempTest = null;
						}
					}

					if (add && fileInfo.Name != Path.GetFileName(myProject.filename) && fileInfo.Name != Path.GetFileName(runResults.getRunResultsFilename())) {
						Debug.WriteLine("Constructing test for: ["  + databaseName + ", " + objectType + ", " + schema + ", " + name + ", " + method + "] " + ((char) 9) + fileInfo.FullName);
						lstTests.Add(new test(associatedProject: this.myProject, xmlFilename: fileInfo.FullName));
					}
				}

				subDirs = root.GetDirectories();

				foreach (System.IO.DirectoryInfo dirInfo in subDirs) {
					if (dirInfo.Name != ".svn" && dirInfo.Name != ".hg") {
						// Recursive call for each subdirectory.						
						List<test> lstChildTests =
							getTestList(
								databaseName: databaseName,
								objectType: objectType,
								schema: schema,
								name: name,
								method: method,
								overload: overload,
								root: dirInfo);

						foreach (test currTest in lstChildTests) {
							lstTests.Add(currTest);
						}

						lstChildTests = null;
					}
				}
			}

			return lstTests;
		}
		
		// Upgrades all tests in the current working copy to the current repository format by
		// opening each test file and saving it again in the new format.
		public void upgrade() {
			List<test> lstAllTests = getTestList();

			for (int i = 0; i < lstAllTests.Count; i++) {
				lstAllTests[i].save(associatedProject: myProject);
			}
		}

		// Returns the status of the given object, based on its run results...
		public targetStatus getTargetStatusCode(
			string databaseName,
			string objectType = "",
			string schema = "",
			string name = "",
			string method = "",
			int overload = -1)
		{
			targetStatus status = targetStatus.noTests;

			// Find all the tests associated with the given object...
			List<test> lstTests =
				getTestList(
					databaseName: databaseName,
					objectType: objectType,
					schema: schema,
					name: name,
					method: method,
					overload: overload);

			// Aggregate the scenarios' results into the status to return...
			foreach (test currTest in lstTests) {
				status |= currTest.getStatus(runResults: runResults);
			}

			return status;
		}

		// Runs all of the tests associated with the given database target.
		public void runAllTestsInObject(
			OracleConnection conTarget,
			string schema = "",
			string objectType = "",
			string name = "",
			string method = "",
			int overload = -1,

			runStatusChangedHandler runStatusChanged = null,
			scenarioRunCompletedHandler scenarioRunCompleted = null)
		{			
			List<test> lstTests =
				getTestList(
					databaseName: conTarget.DataSource,
					objectType: objectType,
					schema: schema,
					name: name,
					method: method,
					overload: overload);

			for (int i = 0; i < lstTests.Count; i++) {
				if (runStatusChanged != null)
					lstTests[i].runStatusChanged += runStatusChanged;
				if (scenarioRunCompleted != null) {
					lstTests[i].scenarioRunCompleted += scenarioRunCompleted;
				}

				lstTests[i].runTest(conTarget: conTarget, runResults: runResults);
			}

			runResults.saveResults();
		}

		// Runs all of the tests in the repository and produces an HTML report.
		public void runAllTests(project currProject, string rtProtocolPrefix) {
			// Clear out any run results, just so that we don't have run results from old tests
			// whose targets are no longer actually still in the target database.
			runResults.clearResults();

			using (System.IO.StreamWriter runResultsFile = new System.IO.StreamWriter("runResults.html", false)) {
				int numScenarios = 0;
				int numFailingTests = 0;

				// Run all the tests in the target databases
				for (int i = 0; i < currProject.targetDBs.Count; i++) {
					try {
						runAllTestsInObject(conTarget: currProject.targetDBs[i].conTargetDB);
					} catch (Exception err) {
						numFailingTests++;

						runResultsFile.Write(
							"<p style=\"color: #AAAAAA;\">Fatal Run Error: " + err.Message + "<br /><br />" + err.StackTrace.Replace(Environment.NewLine, "<br />") + "</p>");
					}
				}

				// Count the number of dying tests.
				foreach (string guid in runResults.Keys) {
					numScenarios++;

					if (runResults[guid].result != "OK") {
						numFailingTests++;
					}
				}

				// Write results header...
				runResultsFile.Write(@"
					<html>
					<head>
						<style type='text/css'>
							p, td, th { font-family: Arial; font-size: 10pt; }

							th { vertical-align: bottom; text-align: center; }

							.scenarioNumber { text-align: center; }
							.errorRunResult { color: red; }
							.errorNumber { color: red; text-align: right; }
						</style>
					</head>

					<body>
						<p>There " + (numScenarios == 1 ? "is" : "are") + " " + numScenarios.ToString("N0") + " scenario" + (numScenarios == 1 ? String.Empty : "s") + " total.</p>");

				if (numFailingTests == 0) {
					runResultsFile.Write("<p>All tests passed!</p>");
				} else {
					runResultsFile.Write(@"
					<p>There are " + numFailingTests.ToString() + @" regression tests that are failing:</p>

					<table border='1' cellpadding='2' cellspacing='0'>
					<col style='' />
					<col style='' />
					<col style='color: red;' />
					<col style='text-align: right;' />
					<tr style='text-align: center !important;'>
						<th>DB</th>
						<th>Test Name</th>
						<th>Scenario Group Name</th>
						<th>Scenario #</th>
						<th class='errorRunResult'>Run Result</th>
						<th>Run<br />Error #</th>
					</tr>");

					// Write the results...
					foreach (string guid in runResults.Keys) {
						if (runResults[guid].result != "OK") {
							runResultsFile.Write(
								"<tr>\n" +
									"<td>" + runResults[guid].databaseName + "</td>\n" +
									"<td>" + runResults[guid].testName + "</td>\n" +
									"<td><a href=\"" + rtProtocolPrefix + "://scenarioGroupGuid=" + runResults[guid].scenarioGroupGuid + "/\">" + runResults[guid].scenarioGroupName + "</a></td>\n" +
									"<td class='scenarioNumber'>" + (runResults[guid].scenarioIndex + 1) + "</td>\n" +
									"<td class='errorRunResult'>" + runResults[guid].result + "</td>\n" +
									"<td class='errorNumber'>" + runResults[guid].errorNumber + "</td>\n" +
								"</tr>\n" +
								"<tr>\n" +
									"<td colspan=\"6\" style=\"color: #AAAAAA; padding: 6px 6px 18px 24px; text-align: left;\">" + runResults[guid].errorMessage.Replace("\n", "<br />") + "</td>\n" +
								"</tr>");
						}
					}
				}

				// Write report footer
				runResultsFile.Write("	</table>");
				runResultsFile.Write("</body>");
				runResultsFile.Write("</html>");
			}
		}
		#endregion
	}	
}
