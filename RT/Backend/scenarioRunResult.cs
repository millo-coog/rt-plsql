using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace RT {
	public class scenarioRunResult {
		private string prvTestName;
		private string prvScenarioGroupName;
		private string prvScenarioGroupGuid;
		private string prvScenarioGuid;
		private int prvScenarioIndex; // Index of the scenario within its scenario group
		private string prvResult;
		private string prvErrorMessage;
		private int prvErrorNumber;

		public string databaseName { get; set; }

		public string testName {
			get { return prvTestName; }
			set { prvTestName = value; }
		}
		public string scenarioGroupName {
			get { return prvScenarioGroupName; }
			set { prvScenarioGroupName = value; }
		}
		public string scenarioGroupGuid {
			get { return prvScenarioGroupGuid; }
			set { prvScenarioGroupGuid = value; }
		}
		public string scenarioGuid {
			get { return prvScenarioGuid; }
			set { prvScenarioGuid = value; }
		}
		public int scenarioIndex {
			get { return prvScenarioIndex; }
			set { prvScenarioIndex = value; }
		}
		public string result {
			get { return prvResult; }
			set { prvResult = value; }
		}
		public string errorMessage {
			get { return prvErrorMessage; }
			set { prvErrorMessage = value; }
		}
		public int errorNumber{
			get { return prvErrorNumber; }
			set { prvErrorNumber = value; }
		}

		public scenarioRunResult(string databaseName, int scenarioIndex, string guid, string testName, string scenarioGroupName, string scenarioGroupGuid, string result, string errorMessage, int errorNumber) {
			this.databaseName = databaseName;
			this.prvTestName = testName;
			this.prvScenarioGroupName = scenarioGroupName;
			this.prvScenarioGroupGuid = scenarioGroupGuid;
			this.prvScenarioGuid = guid;
			this.prvScenarioIndex = scenarioIndex;
			this.prvResult = result;
			this.prvErrorMessage = errorMessage;
			this.prvErrorNumber = errorNumber;
		}
	}

	public class scenarioRunResults : Dictionary<string, scenarioRunResult> {
		public const double RUN_RESULTS_VERSION = 1.3;

		#region Properties
		public project myProject { get; set; }
		#endregion

		#region Constructor
		public scenarioRunResults(project associatedProject) {
			myProject = associatedProject;
		}
		#endregion

		public string getRunResultsFilename() {
			return myProject.repository.workingCopyPath + "runResults.xml";
		}

		// Saves the given run result...
		public void mergeResult(string databaseName, int scenarioIndex, string guid, string testName, string scenarioGroupName, string scenarioGroupGuid, string result, string errorMessage, int errorNumber) {
			if (this.ContainsKey(guid)) {
				// Update
				this[guid].databaseName = databaseName;
				this[guid].testName = testName;
				this[guid].scenarioGroupName = scenarioGroupName;
				this[guid].scenarioGroupGuid = scenarioGroupGuid;
				this[guid].scenarioIndex = scenarioIndex;
				this[guid].result = result;
				this[guid].errorMessage = errorMessage;
				this[guid].errorNumber = errorNumber;
			} else {
				// Add
				this[guid] = 
					new scenarioRunResult(
						databaseName: databaseName,
						scenarioIndex: scenarioIndex, 
						guid: guid, 
						testName: testName, 
						scenarioGroupName: scenarioGroupName, 
						scenarioGroupGuid: scenarioGroupGuid, 
						result: result, 
						errorMessage: errorMessage,
						errorNumber: errorNumber);
			}
		}

		public void clearResults() {
			this.Clear();

			saveResults();
		}

		// Loads saved run results from disk into memory...
		public void loadResults() {
			string runResultsFilename = getRunResultsFilename();

			if (File.Exists(runResultsFilename)) {
				string foundScenarioGroupGuid = String.Empty;

				XmlDocument doc = new XmlDocument();
				doc.Load(runResultsFilename);

				XmlElement exportTag = doc.DocumentElement;
				Double version = Double.Parse(exportTag.Attributes["version"].Value);
				int scenarioIndex = 0;
				string databaseName;

				XmlNodeList resultTags = doc.SelectNodes("//export//result");

				foreach (XmlNode resultTag in resultTags) {
					if (version >= 1.1) {
						// Version 1.1 added the scenario group guid...
						foundScenarioGroupGuid = resultTag.Attributes["scenarioGroupGuid"].Value;
					} else {
						foundScenarioGroupGuid = String.Empty;
					}

					if (version >= 1.2) {
						// Version 1.2 added the scenario index within its scenario group...
						scenarioIndex = Int32.Parse(resultTag.Attributes["scenarioIndex"].Value);
					} else {
						scenarioIndex = -1;
					}

					if (version >= 1.3) {
						// Version 1.3 added the database name...
						databaseName = resultTag.Attributes["databaseName"].Value;
					} else {
						databaseName = "";
					}

					// Load the run result's attributes...
					mergeResult(
						guid: resultTag.Attributes["guid"].Value,
						databaseName: databaseName,
						testName: resultTag.Attributes["testName"].Value,
						scenarioGroupName: resultTag.Attributes["scenarioGroupName"].Value,
						scenarioGroupGuid: foundScenarioGroupGuid,
						scenarioIndex: scenarioIndex,
						result: resultTag.Attributes["result"].Value,
						errorMessage: resultTag.Attributes["errorMessage"].Value,
						errorNumber: Int32.Parse(resultTag.Attributes["errorNumber"].Value));
				}
			}
		}

		// Saves the run results to disk...
		public void saveResults() {
			using (XmlTextWriter writer = new XmlTextWriter(getRunResultsFilename(), Encoding.Unicode)) {
				writer.IndentChar = '\t';
				writer.Indentation = 1;
				writer.Formatting = Formatting.Indented;

				writer.WriteStartDocument();

				writer.WriteComment("This is a Regression Tester run results export.");

				writer.WriteStartElement("export");
				writer.WriteAttributeString("version", RUN_RESULTS_VERSION.ToString());

				foreach (string guid in this.Keys) {
					writer.WriteStartElement("result");
					writer.WriteAttributeString("guid", guid);
					writer.WriteAttributeString("databaseName", this[guid].databaseName);
					writer.WriteAttributeString("scenarioIndex", this[guid].scenarioIndex.ToString());
					writer.WriteAttributeString("testName", this[guid].testName);
					writer.WriteAttributeString("scenarioGroupGuid", this[guid].scenarioGroupGuid);
					writer.WriteAttributeString("scenarioGroupName", this[guid].scenarioGroupName);
					writer.WriteAttributeString("result", this[guid].result);
					writer.WriteAttributeString("errorMessage", this[guid].errorMessage);
					writer.WriteAttributeString("errorNumber", this[guid].errorNumber.ToString());
					writer.WriteEndElement();
				}

				writer.WriteEndElement();

				writer.WriteEndDocument();
			}
		}
	}
}
