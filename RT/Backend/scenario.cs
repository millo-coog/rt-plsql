using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RT {
	public class scenario {
		// Remember to add new properties to the clone...
		private string prvGuid = String.Empty;
		private string prvExpectedException = String.Empty;
		private string prvComments = String.Empty;
		private string prvExpectedExceptionMessage = String.Empty;
		private scenarioParameterCollection prvParameters;
		
		#region Get/set methods
		public string guid {
			get { return prvGuid; }
			set { prvGuid = value; }
		}
		public string comments {
			get { return prvComments; }
			set { prvComments = value; }
		}
		public string expectedException {
			get { return prvExpectedException; }
			set { prvExpectedException = value; }
		}
		public string expectedExceptionMessage {
			get { return prvExpectedExceptionMessage; }
			set { prvExpectedExceptionMessage = value; }
		}
		public scenarioParameterCollection parameters {
			get { return prvParameters; }
			set { prvParameters = value; }
		}
		#endregion

		// Constructor
		public scenario(test associatedTest) {
			prvGuid = Guid.NewGuid().ToString();

			prvParameters = new scenarioParameterCollection(args: associatedTest.testArguments);
		}

		public targetStatus getRunStatus(scenarioRunResults runResults) {
			if (runResults.ContainsKey(this.prvGuid)) {
				return runResults[this.prvGuid].result == "OK" ? targetStatus.testsOk : targetStatus.testsFailed;
			} else {
				return targetStatus.noTests;
			}
		}

		// Clones me into a new scenario...
		public scenario clone(test associatedTest) {
			scenario newScenario = new scenario(associatedTest);

			newScenario.expectedException = this.prvExpectedException;
			newScenario.expectedExceptionMessage = this.prvExpectedExceptionMessage;
			newScenario.comments = this.prvComments;

			newScenario.parameters = this.prvParameters.clone();

			return newScenario;
		}

		public string getScenarioParamHash() {
			string hash = String.Empty;

			for (int i = 0; i < prvParameters.Count; i++) {
				hash += prvParameters[i].testArg.argumentName + "*" + (prvParameters[i].valueComparisonType == "defaulted" ? "defaulted" : "given") + ", ";
			}

			return hash.TrimEnd(new char[] {',', ' '});
		}
	}

	// Declare a collection class for our scenario class
	public class scenarioCollection : System.Collections.Generic.List<scenario> {
		// Adds another scenario to the collection...
		public new void Add(scenario newScenario) {
			// Make sure the new scenario's guid is unique to the scenario collection...
			for (int i = 0; i < this.Count; i++) {
				if (this[i].guid == newScenario.guid) {
					throw new Exception("Scenario guid #" + newScenario.guid + " is a duplicate of another scenario's guid in this scenario group.");
				}
			}

			base.Add(newScenario);
		}

		// Clones the current collection
		public scenarioCollection clone(test associatedTest) {
			scenarioCollection newCollection = new scenarioCollection();

			foreach (scenario currScenario in this) {
				newCollection.Add(currScenario.clone(associatedTest: associatedTest));
			}

			return newCollection;
		}
	}
}
