using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Media;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace RT {
	public class scenarioGroup {
		// Events
		public event runStatusChangedHandler runStatusChanged;
		public event scenarioRunCompletedHandler scenarioRunCompleted; // Fires when we have the status of an individual scenario's run

		#region Private Variables
		// Remember to add new variables to the clone!
		private string prvGuid;
		private test prvTest; // The test I'm associated with.

		// Hooks
		private string prvScenarioGroupDeclare;
		private string prvScenarioGroupStartup;
		private string prvScenarioStartup;
		private string prvPostParamAssignment;
		private string prvPreUDC;
		private string prvScenarioTeardown;
		private string prvScenarioGroupTeardown;
		
		private Boolean prvRollbackAfterEveryScenario;
		private string prvName;
		private string prvDescription;
		private Boolean prvReopenConnectionBeforeEveryScenario;
		private int prvMaxAllowedRunTimeInSeconds;

		private scenarioCollection prvLstScenarios;

		private udcCollection prvUDCCollection;

		private libraryItemCollection prvLibraryItems;
		#endregion

		#region Get/Set property methods
		public string guid {
			get { return prvGuid; }
			set { prvGuid = value; }
		}
		public test test {
			get { return prvTest; }
			set { prvTest = value; }
		}

		public string scenarioGroupDeclare {
			get { return prvScenarioGroupDeclare; }
			set { prvScenarioGroupDeclare = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string scenarioGroupStartup {
			get { return prvScenarioGroupStartup; }
			set { prvScenarioGroupStartup = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string scenarioStartup {
			get { return prvScenarioStartup; }
			set { prvScenarioStartup = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string postParamAssignment {
			get { return prvPostParamAssignment; }
			set { prvPostParamAssignment = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string preUDC {
			get { return prvPreUDC; }
			set { prvPreUDC = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string scenarioTeardown {
			get { return prvScenarioTeardown; }
			set { prvScenarioTeardown = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string scenarioGroupTeardown {
			get { return prvScenarioGroupTeardown; }
			set { prvScenarioGroupTeardown = value.Trim().TrimEnd('\n').TrimEnd(); }
		}

		public bool reopenConnectionBeforeEveryScenario {
			get { return prvReopenConnectionBeforeEveryScenario; }
			set { prvReopenConnectionBeforeEveryScenario = value; }
		}

		public bool rollbackAfterEveryScenario {
			get { return prvRollbackAfterEveryScenario; }
			set { prvRollbackAfterEveryScenario = value; }
		}

		public scenarioCollection scenarios {
			get { return prvLstScenarios; }
			set { prvLstScenarios = value; }
		}

		public udcCollection udcCollection {
			get { return prvUDCCollection; }
			set { prvUDCCollection = value; }
		}

		public libraryItemCollection libraryItems {
			get { return prvLibraryItems; }
			set { prvLibraryItems = value; }
		}

		public string name {
			get { return prvName; }
			set { prvName = value; }
		}
		public string description {
			get { return prvDescription; }
			set { prvDescription = value; }
		}
		public int maxAllowedRunTimeInSeconds {
			get { return prvMaxAllowedRunTimeInSeconds; }
			set { prvMaxAllowedRunTimeInSeconds = value; }
		}
		#endregion

		#region Constructors
		public scenarioGroup(test test) {
			prvDescription = String.Empty;
			prvGuid = Guid.NewGuid().ToString();
			prvLstScenarios = new scenarioCollection();
			prvName = "General Tests";
			prvMaxAllowedRunTimeInSeconds = 60 * 5;

			prvPostParamAssignment = String.Empty;
			prvPreUDC = String.Empty;
			prvRollbackAfterEveryScenario = true;

			prvScenarioGroupDeclare = String.Empty;
			prvScenarioGroupStartup = String.Empty;
			prvScenarioGroupTeardown = String.Empty;
			prvScenarioStartup = String.Empty;
			prvScenarioTeardown = String.Empty;

			prvTest = test;

			prvUDCCollection = new udcCollection();

			prvLibraryItems = new libraryItemCollection();
		}
		#endregion

		// Returns the scenario's parameters in a single row, rather than multiple rows, for display purposes in a grid.
		public DataTable getScenarios(scenarioRunResults runResults) {
			Debug.Assert(prvTest != null);

			DataTable dtScenarios = new DataTable();

			// We have to get our scenario parameters in the same order they're in
			// in the list of test arguments.

			// Add all of the needed columns to the datatable
			dtScenarios.Columns.Add("Key");

			// Add the unit's arguments as parameters
			testArgumentCollection lstTestArguments = prvTest.testArguments;

			for (int j = 0; j < lstTestArguments.Count; j++) {
				dtScenarios.Columns.Add(lstTestArguments[j].argumentName); // Parameter plsqlValue
				dtScenarios.Columns.Add("pt" + j.ToString()); // Parameter type

				if (lstTestArguments[j].inOut == "IN/OUT") {
					dtScenarios.Columns.Add("exp_out" + j.ToString()); // OUT expected plsqlValue for an IN/OUT parameter
					dtScenarios.Columns.Add("exp_out_pt" + j.ToString()); // OUT parameter type of the expected plsqlValue for an IN/OUT parameter
				}
			}

			// Add more of the usual columns
			dtScenarios.Columns.Add("Expected Exception");
			dtScenarios.Columns.Add("Expected Exception Message");
			dtScenarios.Columns.Add("Comments");
			dtScenarios.Columns.Add("Result");
			dtScenarios.Columns.Add("Error_Number");
			dtScenarios.Columns.Add("Error_Message");
			dtScenarios.Columns.Add("GUID");

			// Add every row...
			int numColumns = dtScenarios.Columns.Count;
			int keyColumnIndex = dtScenarios.Columns["Key"].Ordinal;
			int expectedExceptionOrdinal = dtScenarios.Columns["Expected Exception"].Ordinal;
			int expectedExceptionMessageOrdinal = dtScenarios.Columns["Expected Exception Message"].Ordinal;
			int commentsOrdinal = dtScenarios.Columns["Comments"].Ordinal;
			int resultsOrdinal = dtScenarios.Columns["Result"].Ordinal;
			int errorNumberOrdinal = dtScenarios.Columns["Error_Number"].Ordinal;
			int errorMessageOrdinal = dtScenarios.Columns["Error_Message"].Ordinal;
			int guidOrdinal = dtScenarios.Columns["GUID"].Ordinal;

			for (int i = 0; i < prvLstScenarios.Count; i++) {
				// Populate an array with the scenario values
				object[] arrParams = new object[numColumns];
				List<scenarioParameter> lstScenarioParameters = prvLstScenarios[i].parameters;

				arrParams[keyColumnIndex] = (i + 1).ToString();

				for (int j = 0; j < lstScenarioParameters.Count; j++) {
					arrParams[dtScenarios.Columns[lstScenarioParameters[j].testArg.argumentName].Ordinal] = lstScenarioParameters[j].value;
					arrParams[dtScenarios.Columns["pt" + j.ToString()].Ordinal] = lstScenarioParameters[j].valueComparisonType;

					if (lstScenarioParameters[j].testArg.inOut == "IN/OUT") {
						arrParams[dtScenarios.Columns["exp_out" + j.ToString()].Ordinal] = lstScenarioParameters[j].expectedOutValue;
						arrParams[dtScenarios.Columns["exp_out_pt" + j.ToString()].Ordinal] = lstScenarioParameters[j].expectedOutComparisonType;
					}
				}

				arrParams[expectedExceptionOrdinal] = prvLstScenarios[i].expectedException;
				arrParams[expectedExceptionMessageOrdinal] = prvLstScenarios[i].expectedExceptionMessage;
				arrParams[commentsOrdinal] = prvLstScenarios[i].comments;

				if (runResults.ContainsKey(prvLstScenarios[i].guid)) {
					arrParams[resultsOrdinal] = runResults[prvLstScenarios[i].guid].result;
					arrParams[errorNumberOrdinal] = runResults[prvLstScenarios[i].guid].errorNumber.ToString();
					arrParams[errorMessageOrdinal] = runResults[prvLstScenarios[i].guid].errorMessage;
				}

				arrParams[guidOrdinal] = prvLstScenarios[i].guid;

				dtScenarios.Rows.Add(arrParams);
			}

			return dtScenarios;
		}

		public targetStatus getStatus(scenarioRunResults runResults) {
			targetStatus scnGroupStatus = targetStatus.unknown;

			for (int i = 0; i < prvLstScenarios.Count; i++) {
				targetStatus runStatus = prvLstScenarios[i].getRunStatus(runResults: runResults);

				if (scnGroupStatus == targetStatus.unknown)
					scnGroupStatus = runStatus;
				else
					scnGroupStatus |= runStatus;
			}

			if (scnGroupStatus == targetStatus.unknown)
				scnGroupStatus = targetStatus.testsNotRun;

			return scnGroupStatus;
		}

		public scenarioGroup clone(test newTest) {
			scenarioGroup newScenarioGroup = new scenarioGroup(test: newTest);

			newScenarioGroup.scenarioGroupDeclare = this.prvScenarioGroupDeclare;
			newScenarioGroup.prvScenarioGroupStartup = this.prvScenarioGroupStartup;
			newScenarioGroup.prvScenarioStartup = this.prvScenarioStartup;
			newScenarioGroup.prvPostParamAssignment = this.prvPostParamAssignment;
			newScenarioGroup.prvPreUDC = this.prvPreUDC;
			newScenarioGroup.prvScenarioTeardown = this.prvScenarioTeardown;
			newScenarioGroup.prvScenarioGroupTeardown = this.prvScenarioGroupTeardown;
	
			newScenarioGroup.prvRollbackAfterEveryScenario = this.prvRollbackAfterEveryScenario;
			newScenarioGroup.prvReopenConnectionBeforeEveryScenario = this.prvReopenConnectionBeforeEveryScenario;
			newScenarioGroup.prvName = this.prvName;
			newScenarioGroup.prvDescription = this.prvDescription;
			newScenarioGroup.prvMaxAllowedRunTimeInSeconds = this.prvMaxAllowedRunTimeInSeconds;

			newScenarioGroup.prvLstScenarios = this.prvLstScenarios.clone(associatedTest: newTest);

			newScenarioGroup.prvUDCCollection = this.prvUDCCollection.clone();

			newScenarioGroup.prvLibraryItems = this.prvLibraryItems.clone();

			newTest.scenarioGroups.Add(newScenarioGroup);

			return newScenarioGroup;
		}

		private void createMutatedScenario(int validScenarioIndex, int p_CurrParamIndex, string p_ParameterComparisonType, string p_ExpectedException, string p_ExpectedExceptionMessage, string p_ScenarioComment, string p_InvalidValue) {
			scenario srcScenario = scenarios[validScenarioIndex];
			scenario newScenario = new scenario(prvTest);

			newScenario.expectedException = p_ExpectedException;
			newScenario.expectedExceptionMessage = p_ExpectedExceptionMessage;
			newScenario.comments = p_ScenarioComment;

			for (int i = 0; i < srcScenario.parameters.Count; i++) {
				scenarioParameter newParam = newScenario.parameters[i];

				newParam.valueComparisonType = srcScenario.parameters[i].valueComparisonType;

				if (i == p_CurrParamIndex) {
					newParam.value = p_InvalidValue;
					newParam.valueComparisonType = p_ParameterComparisonType;
				} else {
					if (p_ExpectedException != "" && (prvTest.testArguments[i].inOut == "OUT" || prvTest.testArguments[i].inOut == "RETURN")) {
						newParam.value = String.Empty;
						newParam.valueComparisonType = "don't test";
					} else if (p_ExpectedException != "" && prvTest.testArguments[i].inOut == "IN/OUT") {
						newParam.expectedOutValue = String.Empty;
						newParam.expectedOutComparisonType = "don't test";
					} else {
						newParam.value = srcScenario.parameters[i].value;
					}
				}
			}

			scenarios.Add(newScenario);
		}

		public void createInvalidParamScenarios(OracleConnection conTarget, Int32 validScenarioIndex) {
			// For every parameter, pass NULL, an invalid plsqlValue, a valid plsqlValue, and if it can be defaulted, default it.
			
			// Create scenarios that let the parameters default...
			for (int i = 0; i < prvTest.testArguments.Count; i++) {
				if ((prvTest.testArguments[i].inOut == "IN" || prvTest.testArguments[i].inOut == "IN/OUT")
					&& prvTest.testArguments[i].canDefault) {

					// Don't permutate defaulted parameters...
					if (scenarios[validScenarioIndex].parameters[i].valueComparisonType == "defaulted")
						continue;

					createMutatedScenario(
						validScenarioIndex: validScenarioIndex,
						p_CurrParamIndex: i,
						p_ParameterComparisonType: "defaulted",
						p_ExpectedException: String.Empty,
						p_ExpectedExceptionMessage: String.Empty,
						p_ScenarioComment: "Defaulting " + prvTest.testArguments[i].argumentName,
						p_InvalidValue: "");
				}
			}

			// Create scenarios that pass NULL's in the parameters...
			for (int i = 0; i < prvTest.testArguments.Count; i++) {
				if (prvTest.testArguments[i].inOut == "IN" || prvTest.testArguments[i].inOut == "IN/OUT") {
					createMutatedScenario(
						validScenarioIndex: validScenarioIndex,
						p_CurrParamIndex: i,
						p_ParameterComparisonType: "exp",
						p_ExpectedException: "123.45",
						p_ExpectedExceptionMessage: String.Empty,
						p_ScenarioComment: prvTest.testArguments[i].argumentName + ": passing NULL",
						p_InvalidValue: "NULL");
				}
			}

			// Create scenarios pass invalid values in the parameters...
			for (int i = 0; i < prvTest.testArguments.Count; i++) {

				if (prvTest.testArguments[i].inOut == "IN" || prvTest.testArguments[i].inOut == "IN/OUT") {
					if (prvTest.testArguments[i].plsType == "BOOLEAN") {
						// Booleans really can't have an invalid plsqlValue, so just skip this permutation...
						continue;
					}
					
					createMutatedScenario(
						validScenarioIndex: validScenarioIndex,
						p_CurrParamIndex: i,
						p_ParameterComparisonType: "value",
						p_ExpectedException: "123.45",
						p_ExpectedExceptionMessage: String.Empty,
						p_ScenarioComment: prvTest.testArguments[i].argumentName + ": passing invalid value",
						p_InvalidValue: "-1");
				}
			}
		}

		public void delete(scenarioRunResults runResults) {
			for (int i = 0; i < this.scenarios.Count; i++) {
				this.scenarios.Remove(runResults, this.scenarios[i].guid);
			}

			prvTest.scenarioGroups.Remove(this);
		}

		public void run(OracleConnection conTarget, scenarioRunResults runResults) {
			int numScenarios = this.scenarios.Count;

			if (numScenarios == 0) {
				if (runStatusChanged != null)
					runStatusChanged("No child scenarios...", isError: false);
				return;
			}

			if (prvReopenConnectionBeforeEveryScenario) {
				runSerially(conTarget: conTarget, runResults: runResults);
			} else {
				runBlock(
					conTarget: conTarget,
					generatedPLSQL: (new runBlock()).getPLSQLRunBlock(conTargetDB: conTarget, scnGroup: this, targetDBName: conTarget.DatabaseName),
					runResults: runResults);
			}
		}

		// Runs the scenarios in the scenario group one at a time, closing and
		// reopening the db connection between each one. Useful for scenarios
		// that tend to corrupt or de-stabilize the session.
		private void runSerially(OracleConnection conTarget, scenarioRunResults runResults) {
			for (int i = 0; i < this.scenarios.Count; i++) {
				// Get a brand-new Oracle connection...
				if (runStatusChanged != null)
					runStatusChanged("   Re-opening Oracle connection...", isError: false);

				conTarget.Close();
				OracleConnection.ClearPool(conTarget);
				conTarget.Open();

				// Run just this particular scenario...
				runBlock(
					conTarget: conTarget,
					generatedPLSQL: (new runBlock()).getPLSQLRunBlock(conTargetDB: conTarget, scnGroup: this, targetDBName: conTarget.DatabaseName, scenarioIndex: i),
					runResults: runResults,
					scenarioIndex: i);
			}
		}

		private void runBlock(OracleConnection conTarget, string generatedPLSQL, scenarioRunResults runResults, int scenarioIndex = -1) {
			int numScenarios = this.scenarios.Count;
			Boolean scenarioFailed = false;
			
			if (runStatusChanged != null)
				runStatusChanged(prvTest.name + " - " + prvName + "...", isError: false);

			if (generatedPLSQL != String.Empty) {
				OracleCommand cmdRunScenarioGroup = new OracleCommand(generatedPLSQL.Replace(Environment.NewLine, "\n"), conTarget);
				OracleTimeStamp tsStartTime;
				OracleTimeStamp tsEndTime;
								
				cmdRunScenarioGroup.BindByName = true;

				// Array of statuses
				int[] arrStatusesLengths = new int[numScenarios];
				OracleParameterStatus[] arrStatusesOraParamStatuses = new OracleParameterStatus[numScenarios];

				for (int i = 0; i < numScenarios; i++) {
					arrStatusesLengths[i] = 30;
					arrStatusesOraParamStatuses[i] = OracleParameterStatus.Success;
				}

				cmdRunScenarioGroup.Parameters.Add("v_arrStatuses", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
				cmdRunScenarioGroup.Parameters["v_arrStatuses"].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
				cmdRunScenarioGroup.Parameters["v_arrStatuses"].ArrayBindSize = arrStatusesLengths;
				cmdRunScenarioGroup.Parameters["v_arrStatuses"].ArrayBindStatus = arrStatusesOraParamStatuses;
				cmdRunScenarioGroup.Parameters["v_arrStatuses"].Value = null;
				cmdRunScenarioGroup.Parameters["v_arrStatuses"].Size = numScenarios;

				// Array of error numbers
				OracleParameterStatus[] arrErrorNumbersOraParamStatuses = new OracleParameterStatus[numScenarios];

				for (int i = 0; i < numScenarios; i++) {
					arrErrorNumbersOraParamStatuses[i] = OracleParameterStatus.Success;
				}

				cmdRunScenarioGroup.Parameters.Add("v_arrErrorNumbers", OracleDbType.Int32).Direction = ParameterDirection.Output;
				cmdRunScenarioGroup.Parameters["v_arrErrorNumbers"].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
				cmdRunScenarioGroup.Parameters["v_arrErrorNumbers"].ArrayBindStatus = arrErrorNumbersOraParamStatuses;
				cmdRunScenarioGroup.Parameters["v_arrErrorNumbers"].Value = null;
				cmdRunScenarioGroup.Parameters["v_arrErrorNumbers"].Size = numScenarios;

				// Array of error messages
				int[] arrErrorMessageLengths = new int[numScenarios];
				OracleParameterStatus[] arrErrorMessagesOraParamStatuses = new OracleParameterStatus[numScenarios];
				
				for (int i = 0; i < numScenarios; i++) {
					arrErrorMessageLengths[i] = 32767;
					arrErrorMessagesOraParamStatuses[i] = OracleParameterStatus.Success;
				}

				cmdRunScenarioGroup.Parameters.Add("v_arrErrorMessages", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
				cmdRunScenarioGroup.Parameters["v_arrErrorMessages"].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
				cmdRunScenarioGroup.Parameters["v_arrErrorMessages"].ArrayBindSize = arrErrorMessageLengths;
				cmdRunScenarioGroup.Parameters["v_arrErrorMessages"].ArrayBindStatus = arrErrorMessagesOraParamStatuses;
				cmdRunScenarioGroup.Parameters["v_arrErrorMessages"].Value = null;
				cmdRunScenarioGroup.Parameters["v_arrErrorMessages"].Size = numScenarios;

				// Individual bound variables for any library items
				for (int i = 0; i < prvLibraryItems.Count; i++) {
					cmdRunScenarioGroup.Parameters.Add(prvLibraryItems[i].name, OracleDbType.Clob);
					cmdRunScenarioGroup.Parameters[prvLibraryItems[i].name].Value = prvLibraryItems[i].value;
				}

				// Bind arrays to receive back any data about expected vs. actual return and OUT parameter tests....
				bool hasOutParams = true;

				// See how many out parameters we need to bind to receive actual vs. expected return values....
				foreach (testArgument arg in prvTest.testArguments) {
					if (arg.inOut != "IN") {
						hasOutParams = true;
						break;
					}
				}

				// Array of out parameter names
				OracleParameterStatus[] arrParamsNamesOraParamStatuses = new OracleParameterStatus[numScenarios];

				for (int i = 0; i < numScenarios; i++) {
					arrParamsNamesOraParamStatuses[i] = OracleParameterStatus.Success;
				}

				// OUT bind variables for getting back out parameters' expected and actual values.
				testArgumentCollection allArgs = prvTest.getAllArgs();

				for (int i = 0; i < numScenarios; i++) {
					for (int j = 0; j < allArgs.Count; j++) {
						testArgument arg = allArgs[j];

						if (arg.inOut == "IN/OUT" || arg.inOut == "OUT" || arg.inOut == "RETURN") {
							cmdRunScenarioGroup.Parameters.Add("v_arrOutPrmsExpValue_" + i.ToString() + "_" + arg.sequence.ToString(), OracleDbType.Clob).Direction = ParameterDirection.Output;
							cmdRunScenarioGroup.Parameters.Add("v_arrOutPrmsActValue_" + i.ToString() + "_" + arg.sequence.ToString(), OracleDbType.Clob).Direction = ParameterDirection.Output;
						}
					}

					// OUT bind variables for getting back Cursor vs. Matrix UDC actual and expected values.
					for (int udcIndex = 0; udcIndex < udcCollection.Count; udcIndex++) {
						if (udcCollection[udcIndex].checkType == udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX) {
							cmdRunScenarioGroup.Parameters.Add("v_arrOutUDCExpValue_" + udcIndex.ToString() + "_" + i.ToString(), OracleDbType.Clob).Direction = ParameterDirection.Output;
							cmdRunScenarioGroup.Parameters.Add("v_arrOutUDCActValue_" + udcIndex.ToString() + "_" + i.ToString(), OracleDbType.Clob).Direction = ParameterDirection.Output;
						}
					}
				}

				cmdRunScenarioGroup.CommandTimeout = prvMaxAllowedRunTimeInSeconds;

				tsStartTime = new OracleTimeStamp(DateTime.Now);

				try {
					cmdRunScenarioGroup.ExecuteNonQuery();
					tsEndTime = new OracleTimeStamp(DateTime.Now);

					// Process the results of the runs...
					string status;
					int errorNumber;
					string errorMessage;

					for (int i = 0; i < numScenarios; i++) {
						// If we're only running on scenario, ignore any other scenarios, but it, 
						// because none of the other scenarios have valid run data in them.
						if (scenarioIndex != -1 && scenarioIndex != i)
							continue;

						// Get the results
						status = (cmdRunScenarioGroup.Parameters["v_arrStatuses"].Value as Array).GetValue(i).ToString();
						errorNumber = Int32.Parse((cmdRunScenarioGroup.Parameters["v_arrErrorNumbers"].Value as Array).GetValue(i).ToString());
						errorMessage = (cmdRunScenarioGroup.Parameters["v_arrErrorMessages"].Value as Array).GetValue(i).ToString();
						errorMessage = errorMessage == "null" ? String.Empty : errorMessage;
						
						runResults.mergeResult(
							databaseName: conTarget.DatabaseName,
							unitSchema: prvTest.unitSchema,
							unitName: prvTest.unitName,
							unitMethod: prvTest.unitMethod,
							unitOverload: prvTest.overload.ToString(),
							unitType: prvTest.unitType,
							scenarioIndex: i,
							guid: prvLstScenarios[i].guid,
							testName: this.test.name,
							scenarioGroupName: prvName,
							scenarioGroupGuid: prvGuid,
							result: status,
							errorMessage: errorMessage,
							errorNumber: errorNumber);
						
						// Give the interface the results...
						if (scenarioRunCompleted != null) {
							scenarioRunCompleted(
								folderKey: -1, // @@@ Folder key
								testName: prvTest.name,
								scenarioGroupGuid: prvGuid,
								scenarioGroupName: prvName,
								scenarioIndex: i+1,
								scenarioGUID: prvLstScenarios[i].guid,
								status: status,
								errorNumber: errorNumber,
								errorMessage: errorMessage
							);
						}
					
						// Write any OUT/RETURN parameter differences to disk...
						if (hasOutParams && status != "OK") {
							string expectedResult;
							string actualResult;
							string tempPath = Path.GetTempPath();

							for (int j = 0; j < allArgs.Count; j++) {
								if (allArgs[j].inOut == "IN/OUT" || allArgs[j].inOut == "OUT" || allArgs[j].inOut == "RETURN") {
									OracleClob expValue = (OracleClob)cmdRunScenarioGroup.Parameters["v_arrOutPrmsExpValue_" + i.ToString() + "_" + allArgs[j].sequence.ToString()].Value;

									if (expValue.IsNull || expValue.Value == String.Empty) {
										expectedResult = String.Empty;
									} else {
										expectedResult = expValue.Value;
										expectedResult = expectedResult == "null" ? String.Empty : expectedResult;
									}

									if (((OracleClob)cmdRunScenarioGroup.Parameters["v_arrOutPrmsActValue_" + i.ToString() + "_" + allArgs[j].sequence.ToString()].Value).IsNull) {
										actualResult = String.Empty;
									} else {
										actualResult = ((OracleClob)cmdRunScenarioGroup.Parameters["v_arrOutPrmsActValue_" + i.ToString() + "_" + allArgs[j].sequence.ToString()].Value).Value;
										actualResult = actualResult == "null" ? String.Empty : actualResult;
									}

									System.IO.File.WriteAllText(tempPath + "\\rt_" + scenarios[i].guid + "_SCN_" + i.ToString() + "_P_" + allArgs[j].sequence.ToString() + (allArgs[j].inOut == "IN/OUT" ? "_EXP_OUT" : "") + "_Expected.txt", expectedResult);
									System.IO.File.WriteAllText(tempPath + "\\rt_" + scenarios[i].guid + "_SCN_" + i.ToString() + "_P_" + allArgs[j].sequence.ToString() + (allArgs[j].inOut == "IN/OUT" ? "_EXP_OUT" : "") + "_Actual.txt", actualResult);
								}
							}
						}

						// Write any Cursor vs. Matrix UDC comparison differences to disk.
						for (int udcIndex = 0; udcIndex < udcCollection.Count; udcIndex++) {
							if (udcCollection[udcIndex].checkType == udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX) {
								string expectedResult;
								string actualResult;
								OracleClob expValue = (OracleClob)cmdRunScenarioGroup.Parameters["v_arrOutUDCExpValue_" + udcIndex.ToString() + "_" + i.ToString()].Value;

								if (expValue.IsNull || expValue.Value == String.Empty) {
									expectedResult = String.Empty;
								} else {
									expectedResult = expValue.Value;
									expectedResult = expectedResult == "null" ? String.Empty : expectedResult;
								}

								if (((OracleClob)cmdRunScenarioGroup.Parameters["v_arrOutUDCActValue_" + udcIndex.ToString() + "_" + i.ToString()].Value).IsNull) {
									actualResult = String.Empty;
								} else {
									actualResult = ((OracleClob)cmdRunScenarioGroup.Parameters["v_arrOutUDCActValue_" + udcIndex.ToString() + "_" + i.ToString()].Value).Value;
									actualResult = actualResult == "null" ? String.Empty : actualResult;
								}

								System.IO.File.WriteAllText(Path.GetTempPath() + "\\rt_" + scenarios[i].guid + "_UDC_" + udcIndex.ToString() + "_Expected.txt", expectedResult);
								System.IO.File.WriteAllText(Path.GetTempPath() + "\\rt_" + scenarios[i].guid + "_UDC_" + udcIndex.ToString() + "_Actual.txt", actualResult);
							}
						}
					}
				} catch (OracleException err) {
					bool isTimeoutError = false;
					string message;
					string status;

					if (-err.Number == -1013) {
						// ORA-01013: user requested cancel of current operation
						// The scenario group exceeded its timeout
						status = "Timeout Exceeded";
						message = "Allowed runtime of " + this.prvMaxAllowedRunTimeInSeconds.ToString() + " second(s) exceeded.";

						isTimeoutError = true;
					} else {
						status = "Run Block Error";
						message = err.Message;
					}

					for (int i = 0; i < prvLstScenarios.Count; i++) {
						// If we're only running on scenario, ignore any other scenarios, but it, 
						// because none of the other scenarios have valid run data in them.
						if (scenarioIndex != -1 && scenarioIndex != i)
							continue;

						// Store that we were unable to run this scenario group...
						runResults.mergeResult(
							databaseName: conTarget.DatabaseName,
							unitSchema: prvTest.unitSchema,
							unitName: prvTest.unitName,
							unitMethod: prvTest.unitMethod,
							unitOverload: prvTest.overload.ToString(),
							unitType: prvTest.unitType,
							scenarioIndex: i,
							guid: prvLstScenarios[i].guid,
							testName: this.test.name,
							scenarioGroupName: prvName,
							scenarioGroupGuid: prvGuid,
							result: status,
							errorMessage: message,
							errorNumber: -err.Number);

						// Tell the interface we were unable to run this scenario group
						if (scenarioRunCompleted != null) {
							scenarioRunCompleted(
								folderKey: -1, // @@@ Folder key
								testName: prvTest.name,
								scenarioGroupGuid: "",
								scenarioGroupName: prvName,
								scenarioIndex: -1,
								scenarioGUID: String.Empty,
								status: status,
								errorNumber: -err.Number,
								errorMessage: message
							);
						}
					}

					if (runStatusChanged != null)
						runStatusChanged(prvTest.name + " - " + prvName + " error:\n" + message, isError: true);

					// Attempt to recover from a problem, by closing and re-opening the db connection...
					if (isTimeoutError == false) {
						if (scenarioRunCompleted != null) {
							scenarioRunCompleted(
								folderKey: -1, // @@@ Folder key
								testName: prvTest.name,
								scenarioGroupGuid: "",
								scenarioGroupName: "",
								scenarioIndex: -1,
								scenarioGUID: String.Empty,
								status: "Run Block Error",
								errorNumber: -err.Number,
								errorMessage: "Flushing pool and reconnecting to target db..."
							);
						}

						conTarget.Close();
						OracleConnection.ClearPool(conTarget);
						conTarget.Open();
					}

					scenarioFailed = true;
				}

				cmdRunScenarioGroup.Dispose();

				if (scenarioFailed) {
					if (runStatusChanged != null)
						runStatusChanged(prvTest.name + " - " + prvName + " failed.\n", isError: true);
				} else {
					if (runStatusChanged != null)
						runStatusChanged(prvTest.name + " - " + prvName + " ran fine.\n", isError: false);
				}
			}
		}
	}

	// A collection of scenario groups...
	public class scenarioGroupCollection : System.Collections.Generic.List<scenarioGroup> {
		// Adds another scenario group to the collection...
		public new void Add(scenarioGroup newScenarioGroup) {
			// Make sure the new scenario group's guid is unique to the scenario group collection...
			for (int i = 0; i < this.Count; i++) {
				if (this[i].guid == newScenarioGroup.guid) {
					throw new Exception("Scenario group guid #" + newScenarioGroup.guid + " is a duplicate of another scenario group's guid in this test.");
				}
			}

			base.Add(newScenarioGroup);
		}

		// Clones a scenario group collection to a new test...
		public scenarioGroupCollection clone(test newTest) {
			scenarioGroupCollection newCollection = new scenarioGroupCollection();

			foreach (scenarioGroup currScnGroup in this) {
				newCollection.Add(currScnGroup.clone(newTest: newTest));
			}

			return newCollection;
		}

		public void addParameter(testArgument parentArgument, testArgument newArg) {
			foreach (scenarioGroup scnGroup in this) {
				foreach (scenario scn in scnGroup.scenarios) {
					scn.parameters.addParameter(parentArgument: parentArgument, newArg: newArg);
				}
			}
		}

		// Recursively renames parameters that are part of the argument we're renaming...
		public void renameParameter(testArgument oldArg, testArgument newArg) {
			foreach (scenarioGroup scnGroup in this) {
				foreach (scenario scn in scnGroup.scenarios) {
					scn.parameters.renameParameter(oldArg: oldArg, newArg: newArg);
				}
			}
		}

		// Recursively remove parameters that are part of the argument we're deleting....
		public void removeParameter(testArgument arg) {
			foreach (scenarioGroup scnGroup in this) {
				foreach (scenario scn in scnGroup.scenarios) {
					scn.parameters.removeParameter(arg: arg);
				}
			}
		}
	}
}
