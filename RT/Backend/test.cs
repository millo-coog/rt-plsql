using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using RT;

namespace RT {
	public class test {
		// Events
		public event runStatusChangedHandler runStatusChanged;
		public event scenarioRunCompletedHandler scenarioRunCompleted;

		#region Private variables
		private testArgumentCollection prvLstTestArguments; // The test's formal arguments
		private scenarioGroupCollection prvLstScenarioGroups; // The test's child scenario groups
		private String prvXMLFilename;

		private DateTime prvLastModifiedXMLFile;

		private string prvGuid;
		private string prvName;
		private string prvDescription;
		private DateTime prvCreationDate;

		private string prvUnitSchema;
		private string prvUnitName;
		private string prvUnitMethod;
		private int prvOverload;
		private string prvUnitType;

		private bool prvIsPipelinedFunction;

		private string prvPLSQLDeclare;
		private string prvPLSQLBlock;
		#endregion

		#region Get/Set methods
		public testArgumentCollection testArguments {
			get { return prvLstTestArguments; }
			set { prvLstTestArguments = value; }
		}
		public scenarioGroupCollection scenarioGroups {
			get { return prvLstScenarioGroups; }
			set { prvLstScenarioGroups = value; }
		}
		public String xmlFilename {
			get { return prvXMLFilename; }
			set { prvXMLFilename = value; }
		}
		public DateTime xmlFileLastModified {
			get { return prvLastModifiedXMLFile; }
		}
		
		public string databaseName { get; set; }
		public string version { get; set; }

		public string guid {
			get { return prvGuid; }
			set { prvGuid = value; }
		}
		public String name {
			get { return prvName; }
			set { prvName = value; }
		}
		public String description {
			get { return prvDescription; }
			set { prvDescription = value; }
		}
		public DateTime creationDate {
			get { return prvCreationDate; }
			set { prvCreationDate = value; }
		}

		public String unitSchema {
			get { return prvUnitSchema; }
			set { prvUnitSchema = value; }
		}
		public String unitName {
			get { return prvUnitName; }
			set { prvUnitName = value; }
		}
		public String unitMethod {
			get { return prvUnitMethod; }
			set { prvUnitMethod = value; }
		}
		public int overload {
			get { return prvOverload; }
			set { prvOverload = value; }
		}
		public String unitType {
			get { return prvUnitType; }
			set { prvUnitType = value; }
		}

		public bool isPipelinedFunction {
			get { return prvIsPipelinedFunction; }
			set { prvIsPipelinedFunction = value; }
		}

		public String plSQLDeclare {
			get { return prvPLSQLDeclare; }
			set { prvPLSQLDeclare = value; }
		}
		public String plSQLBlock {
			get { return prvPLSQLBlock; }
			set { prvPLSQLBlock = value; }
		}
		#endregion

		#region Constructors
		public test() {
			init();
		}

		public test(project associatedProject, string xmlFilename) {
			Debug.Assert(xmlFilename != String.Empty);

			init();

			loadFromXML(associatedProject: associatedProject, filename: xmlFilename);
		}
		#endregion

		#region Methods
		private void init() {
			prvCreationDate = DateTime.Now;
			prvDescription = String.Empty;
			prvGuid = Guid.NewGuid().ToString();

			prvIsPipelinedFunction = false;
			prvLstScenarioGroups = new scenarioGroupCollection();
			
			prvLstTestArguments = new testArgumentCollection();

			prvName = "New Test";
			prvOverload = 0;
			prvPLSQLBlock = String.Empty;
			prvPLSQLDeclare = String.Empty;

			prvUnitMethod = String.Empty;
			prvUnitName = String.Empty;
			prvUnitSchema = String.Empty;
			prvUnitType = "PROCEDURE";

			prvLastModifiedXMLFile = DateTime.Now;

			prvXMLFilename = String.Empty;

			version = repos.WC_FORMAT_VERSION.ToString();
		}

		private test clone() {
			test newTest = new test();

			newTest.databaseName = this.databaseName;

			newTest.prvCreationDate = DateTime.Now;
			newTest.prvGuid = Guid.NewGuid().ToString();

			newTest.description = this.prvDescription;
			newTest.isPipelinedFunction = this.prvIsPipelinedFunction;
						
			newTest.name = "Clone of " + this.prvName;
			newTest.overload = this.prvOverload;
			newTest.plSQLBlock = this.prvPLSQLBlock;
			newTest.plSQLDeclare = this.prvPLSQLDeclare;
			
			newTest.unitMethod = this.prvUnitMethod;
			newTest.unitName = this.prvUnitName;
			newTest.unitSchema = this.prvUnitSchema;
			newTest.unitType = this.prvUnitType;

			newTest.xmlFilename = String.Empty;

			newTest.scenarioGroups = this.prvLstScenarioGroups.clone(newTest);
			newTest.testArguments = this.prvLstTestArguments.clone();
			
			return newTest;
		}

		// Recursively returns all arguments and child arguments
		public testArgumentCollection getAllArgs(testArgumentCollection args = null) {
			testArgumentCollection argsList = (args == null ? this.testArguments : args);
			testArgumentCollection allArgs = new testArgumentCollection();

			foreach (testArgument arg in argsList) {
				allArgs.Add(arg);

				if (arg.childArguments.Count > 0) {
					testArgumentCollection childArgs = getAllArgs(arg.childArguments);

					foreach (testArgument childArg in childArgs) {
						allArgs.Add(childArg);
					}
				}
			}

			return allArgs;
		}
		#endregion

		#region Save/Delete methods
		// Deletes the UDC from disk
		public void delete() {
			Debug.Assert(prvXMLFilename != null);

			System.IO.File.Delete(prvXMLFilename);
		}

		// Saves the test, et. al., to disk.
		public void save(project associatedProject) {
			if (xmlFilename == String.Empty) {
				xmlFilename = associatedProject.repository.getTestFolderPath(this) + "\\" + associatedProject.repository.getTestFilename(this);
			}
			
			// If I don't have a scenario group, then add one...
			if (scenarioGroups.Count == 0) {
				scenarioGroup newScenarioGroup = new scenarioGroup(this);
				prvLstScenarioGroups.Add(newScenarioGroup);
			}

			System.IO.Directory.CreateDirectory(Path.GetDirectoryName(prvXMLFilename));

			writeXMLFile();
		}
		#endregion

		#region XML generation/parsing
		// Loads the test from the given xml file.
		public void loadFromXML(project associatedProject, string filename) {
			Debug.Assert(filename != String.Empty);

			prvXMLFilename = filename;
			prvLastModifiedXMLFile = File.GetLastWriteTime(prvXMLFilename);

			string tempPath = filename.Replace(associatedProject.repository.workingCopyPath, "").TrimStart('\\');
			databaseName = tempPath.Substring(0, tempPath.IndexOf("\\"));

			XmlDocument doc = new XmlDocument();
			xmlHelper xmlTestHelper = new xmlHelper(xmlFilename: prvXMLFilename);

			try {
				doc.Load(filename);
			} catch (Exception e) {
				throw new Exception("Could not parse '" + prvXMLFilename + "'.\n\n" + e.Message);
			}

			XmlElement exportTag = doc.DocumentElement;
			Debug.Assert(exportTag.Name == "export", "The root element in '" + prvXMLFilename + "' must be named 'export'.");

			this.version = xmlTestHelper.getAttribute(node: exportTag, name: "version", required: true);

			XmlNodeList testTags = doc.SelectNodes("//export//test");
			Debug.Assert(testTags.Count == 1, "'" + prvXMLFilename + "' must contain one and only one 'test' node.");

			foreach (XmlNode testTag in testTags) {
				// Load the test attributes...
				prvName = xmlTestHelper.getAttribute(node: testTag, name: "name", required: true);

				prvGuid = xmlTestHelper.getAttribute(node: testTag, name: "guid", defaultValue: prvGuid);

				bool validCreationDate = DateTime.TryParseExact(xmlTestHelper.getAttribute(node: testTag, name: "creation_date", required: true), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out prvCreationDate);
				Debug.Assert(validCreationDate, "The 'test' tag's creation_date attribute value must be in a valid 'MM/DD/YYYY HH:MI:SS' format in " + prvXMLFilename + ".");

				prvUnitSchema = xmlTestHelper.getAttribute(node: testTag, name: "unit_schema", required: true);
				prvUnitName = xmlTestHelper.getAttribute(node: testTag, name: "unit_name", required: true);
				prvUnitMethod = xmlTestHelper.getAttribute(node: testTag, name: "unit_method", required: false);
				prvUnitType = xmlTestHelper.getAttribute(node: testTag, name: "unit_type", required: true);
				prvOverload = Int32.Parse(xmlTestHelper.getAttribute(node: testTag, name: "overload", required: false, defaultValue: prvOverload.ToString()));

				String isPipelinedValue = xmlTestHelper.getAttribute(node: testTag, name: "is_pipelined_function", required: false, defaultValue: "N");
				Debug.Assert(isPipelinedValue == "N" || isPipelinedValue == "Y", "'" + isPipelinedValue + "' is not a valid value for the is_pipelined_function attribute of the test in '" + prvXMLFilename + "'. It must be Y/N.");
				prvIsPipelinedFunction = isPipelinedValue == "Y";

				prvDescription = xmlTestHelper.getSingleNodesText(node: testTag, name: "description");
				prvPLSQLDeclare = xmlTestHelper.getSingleNodesText(node: testTag, name: "plsql_declare");
				prvPLSQLBlock = xmlTestHelper.getSingleNodesText(node: testTag, name: "plsql_block");

				// Load the test arguments...
				prvLstTestArguments = getTestArgs(xmlTestHelper: xmlTestHelper, startingNode: testTag);

				// Load any scenario groups...
				XmlNode scenarioGroups = xmlTestHelper.getSingleNode(node: testTag, name: "scenario_groups");

				if (scenarioGroups != null) {
					foreach (XmlNode scenarioGroupNode in scenarioGroups.ChildNodes) {
						scenarioGroup newScnGroup = new scenarioGroup(this);

						newScnGroup.name = xmlTestHelper.getAttribute(node: scenarioGroupNode, name: "name", required: true);

						String reopenConnectionBeforeEveryScenario = xmlTestHelper.getAttribute(node: scenarioGroupNode, name: "reopen_connection_before_every_scenario", defaultValue: "N");
						Debug.Assert(reopenConnectionBeforeEveryScenario == "N" || reopenConnectionBeforeEveryScenario == "Y", "'" + reopenConnectionBeforeEveryScenario + "' is not a valid reopen_connection_before_every_scenario value in '" + prvXMLFilename + "'. Must be Y/N.");
						newScnGroup.reopenConnectionBeforeEveryScenario = reopenConnectionBeforeEveryScenario == "Y";

						String rollbackValue = xmlTestHelper.getAttribute(node: scenarioGroupNode, name: "rollback_afterwards", defaultValue: "Y");
						Debug.Assert(rollbackValue == "N" || rollbackValue == "Y", "'" + rollbackValue + "' is not a valid rollback_afterwards value in '" + prvXMLFilename + "'. Must be Y/N.");
						newScnGroup.rollbackAfterEveryScenario = rollbackValue == "Y";

						// Guids are optional in the xml file...
						newScnGroup.guid = xmlTestHelper.getAttribute(node: scenarioGroupNode, name: "guid", defaultValue: newScnGroup.guid);

						string maxRunTimeValue = xmlTestHelper.getAttribute(node: scenarioGroupNode, name: "max_allowed_run_time_in_seconds", defaultValue: newScnGroup.maxAllowedRunTimeInSeconds.ToString());
						int maxRunTime;
						bool maxRunTimeValid = Int32.TryParse(maxRunTimeValue, out maxRunTime);
						Debug.Assert(maxRunTimeValid, "In '" + prvXMLFilename + "', scenario group '" + newScnGroup.name + "' has an invalid 'max_allowed_run_time_in_seconds' attribute value of '" + maxRunTimeValue + "'.");
						Debug.Assert(maxRunTime > 0, "In '" + prvXMLFilename + "', scenario group '" + newScnGroup.name + "' must have a value greater than zero in its 'max_allowed_run_time_in_seconds' attribute.");
						newScnGroup.maxAllowedRunTimeInSeconds = maxRunTime;

						newScnGroup.description = xmlTestHelper.getSingleNodesText(node: scenarioGroupNode, name: "description", required: false);

						newScnGroup.scenarioGroupDeclare = xmlTestHelper.getSingleNodesText(node: scenarioGroupNode, name: "scenario_group_declare", required: false);
						newScnGroup.scenarioGroupStartup = xmlTestHelper.getSingleNodesText(node: scenarioGroupNode, name: "scenario_group_startup", required: false);
						newScnGroup.scenarioStartup = xmlTestHelper.getSingleNodesText(node: scenarioGroupNode, name: "scenario_startup", required: false);
						newScnGroup.postParamAssignment = xmlTestHelper.getSingleNodesText(node: scenarioGroupNode, name: "post_param_assignment", required: false);
						newScnGroup.preUDC = xmlTestHelper.getSingleNodesText(node: scenarioGroupNode, name: "pre_user_defined_checks", required: false);
						newScnGroup.scenarioTeardown = xmlTestHelper.getSingleNodesText(node: scenarioGroupNode, name: "scenario_teardown", required: false);
						newScnGroup.scenarioGroupTeardown = xmlTestHelper.getSingleNodesText(node: scenarioGroupNode, name: "scenario_group_teardown", required: false);

						// Load this scenario group's scenarios, if any...
						if (scenarioGroupNode.SelectSingleNode("scenarios") != null) {
							foreach (XmlNode scenarioNode in scenarioGroupNode.SelectSingleNode("scenarios").ChildNodes) {
								scenario newScenario = new scenario(this);

								newScenario.expectedException = xmlTestHelper.getAttribute(node: scenarioNode, name: "expected_exception");
								newScenario.expectedExceptionMessage = xmlTestHelper.getAttribute(node: scenarioNode, name: "expected_exception_message");

								newScenario.guid = xmlTestHelper.getAttribute(node: scenarioNode, name: "guid", defaultValue: newScenario.guid);

								newScenario.comments = xmlTestHelper.getSingleNodesText(node: scenarioNode, name: "comments");

								// Copy this scenario's parameter values into the new scenario object
								newScenario.parameters = scenarioParameterCollection.readFromXML(xmlTestHelper: xmlTestHelper, parentTest: this, args: this.testArguments, parentNode: scenarioNode)[0];

								newScnGroup.scenarios.Add(newScenario);
							}
						}

						// Load this scenario group's UDC's
						if (scenarioGroupNode.SelectSingleNode("user_defined_checks") != null) {
							foreach (XmlNode udcNode in scenarioGroupNode.SelectSingleNode("user_defined_checks").ChildNodes) {
								// Start by determining the UDC's check type...
								string UDCCheckTypeValue = xmlTestHelper.getAttribute(node: udcNode, name: "check_type", required: true);
								udc.enumCheckTypes udcType;

								if (Enum.TryParse(UDCCheckTypeValue, out udcType) == false) {
									throw new Exception("Invalid UDC check_type value of '" + UDCCheckTypeValue + "' in '" + prvXMLFilename + "'.");
								}

								udc newUDC = new udc(parentScenarioGroup: newScnGroup, newCheckType: udcType);

								newUDC.guid = xmlTestHelper.getAttribute(node: udcNode, name: "guid", required: false, defaultValue: newUDC.guid);

								string sortOrderValue = xmlTestHelper.getAttribute(node: udcNode, name: "sort_order", defaultValue: "0");
								int sortOrder;

								if (Int32.TryParse(sortOrderValue, out sortOrder) == false) {
									throw new Exception("Invalid UDC sort_order value of '" + sortOrderValue + "' in '" + prvXMLFilename + "'.");
								} else {
									newUDC.sortOrder = sortOrder;
								}

								newUDC.name = xmlTestHelper.getAttribute(node: udcNode, name: "name", required: true);

								newUDC.csvExcludedColumns = xmlTestHelper.getAttribute(node: udcNode, name: "csv_excluded_columns");

								newUDC.description = xmlTestHelper.getSingleNodesText(node: udcNode, name: "description");
								newUDC.plsqlCondition = xmlTestHelper.getSingleNodesText(node: udcNode, name: "plsql_condition");
								newUDC.plsqlBlock = xmlTestHelper.getSingleNodesText(node: udcNode, name: "plsql_block");
								newUDC.expectedCursor = xmlTestHelper.getSingleNodesText(node: udcNode, name: "expected_cursor");
								newUDC.actualCursor = xmlTestHelper.getSingleNodesText(node: udcNode, name: "actual_cursor");
								newUDC.rowExistenceCursor = xmlTestHelper.getSingleNodesText(node: udcNode, name: "row_existence_cursor");
								newUDC.rowValidationCursor = xmlTestHelper.getSingleNodesText(node: udcNode, name: "row_validation_cursor");

								// Load any row validation checks...
								if (newUDC.checkType == udc.enumCheckTypes.ROW_VALIDATION) {
									foreach (XmlNode rowValidationCheckNode in udcNode.SelectSingleNode("row_validation_checks")) {
										rowValidatorCheck newCheck = new rowValidatorCheck();

										newCheck.fieldName = xmlTestHelper.getAttribute(node: rowValidationCheckNode, name: "field_name", required: true);

										newCheck.fieldValue = xmlTestHelper.getAttribute(node: rowValidationCheckNode, name: "field_value");
										newCheck.comparisonType = xmlTestHelper.getAttribute(node: rowValidationCheckNode, name: "comparison_type", required: true);

										newUDC.fieldValidations.Add(newCheck);
									}
								}

								// Load any expected matrix...
								if (newUDC.checkType == udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX) {
									newUDC.expectedMatrix = new DataTable();

									// Use the first child row's columns to know what columns are in the matrix...
									foreach (XmlNode rowNode in udcNode.SelectSingleNode("expected_matrix")) {
										foreach (XmlNode columnNode in rowNode.ChildNodes) {
											newUDC.expectedMatrix.Columns.Add(xmlTestHelper.getAttribute(node: columnNode, name: "name", required: true));
										}

										break;
									}

									// Now, put the values into the datatable...
									foreach (XmlNode rowNode in udcNode.SelectSingleNode("expected_matrix")) {
										object[] arrParams = new object[newUDC.expectedMatrix.Columns.Count];

										foreach (XmlNode columnNode in rowNode.ChildNodes) {
											arrParams[newUDC.expectedMatrix.Columns[xmlTestHelper.getAttribute(node: columnNode, name: "name", required: true)].Ordinal] = columnNode.FirstChild.Value;
										}

										newUDC.expectedMatrix.Rows.Add(arrParams);
									}
								}

								newScnGroup.udcCollection.Add(newUDC);
							}
						}

						// Load this scenario group's library items...
						if (scenarioGroupNode.SelectSingleNode("library_items") != null) {
							foreach (XmlNode libraryItemNode in scenarioGroupNode.SelectSingleNode("library_items").ChildNodes) {
								libraryItem newLibraryItem = new libraryItem();

								newLibraryItem.name = xmlTestHelper.getAttribute(node: libraryItemNode, name: "name", required: true);
								newLibraryItem.description = xmlTestHelper.getAttribute(node: libraryItemNode, name: "description");
								newLibraryItem.value = xmlTestHelper.getSingleNodesText(node: libraryItemNode, name: "value", required: true);

								newScnGroup.libraryItems.Add(newLibraryItem);
							}
						}

						prvLstScenarioGroups.Add(newScnGroup);
					}
				}
			}
		}

		private testArgumentCollection getTestArgs(xmlHelper xmlTestHelper, XmlNode startingNode) {
			testArgumentCollection argsFound = new testArgumentCollection();

			XmlNode argumentsNode = xmlTestHelper.getSingleNode(node: startingNode, name: "arguments");

			if (argumentsNode != null) {
				foreach (XmlNode argumentNode in argumentsNode.ChildNodes) {
					testArgument newTestArgument = new testArgument();

					newTestArgument.argumentName = xmlTestHelper.getAttribute(node: argumentNode, name: "argument_name", required: false).ToLower();
					newTestArgument.inOut = xmlTestHelper.getAttribute(node: argumentNode, name: "in_out", required: false, defaultValue: "IN");

					newTestArgument.sequence = Int32.Parse(xmlTestHelper.getAttribute(node: argumentNode, name: "sequence", required: false, defaultValue: "-1"));
					newTestArgument.dataLevel = Int32.Parse(xmlTestHelper.getAttribute(node: argumentNode, name: "data_level", required: false, defaultValue: "-1"));
					newTestArgument.position = Int32.Parse(xmlTestHelper.getAttribute(node: argumentNode, name: "position", required: true));

					newTestArgument.dataType = xmlTestHelper.getAttribute(node: argumentNode, name: "data_type", required: false);
					newTestArgument.plsType = xmlTestHelper.getAttribute(node: argumentNode, name: "pls_type", required: false);

					String canDefaultValue = xmlTestHelper.getAttribute(node: argumentNode, name: "can_default", required: false, defaultValue: "N");
					Debug.Assert(canDefaultValue == "N" || canDefaultValue == "Y", "'" + canDefaultValue + "' is not a valid can_default value for the '" + newTestArgument.argumentName + "' argument in '" + prvXMLFilename + "'.");
					newTestArgument.canDefault = canDefaultValue == "Y";

					XmlNode childArgsNode = xmlTestHelper.getSingleNode(node: argumentNode, name: "arguments");
					if (childArgsNode != null)
						newTestArgument.childArguments = getTestArgs(xmlTestHelper, argumentNode);

					argsFound.Add(newTestArgument);
				}
			}

			return argsFound;
		}

		public void writeXMLFile() {
			using (XmlTextWriter writer = new XmlTextWriter(prvXMLFilename, Encoding.Unicode)) {
				writer.IndentChar = '\t';
				writer.Indentation = 1;
				writer.Formatting = Formatting.Indented;

				writer.WriteStartDocument();

				writer.WriteComment("This is a Regression Tester export.");

				writer.WriteStartElement("export");
				writer.WriteAttributeString("version", repos.WC_FORMAT_VERSION.ToString());

				writer.WriteStartElement("test");
				writer.WriteAttributeString("name", prvName);
				writer.WriteAttributeString("creation_date", prvCreationDate.ToString("MM/dd/yyyy HH:mm:ss"));
				writer.WriteAttributeString("guid", prvGuid);
				writer.WriteAttributeString("unit_schema", prvUnitSchema);
				writer.WriteAttributeString("unit_name", prvUnitName);
				writer.WriteAttributeString("unit_method", prvUnitMethod);
				writer.WriteAttributeString("unit_type", prvUnitType);
				writer.WriteAttributeString("overload", prvOverload.ToString());
				writer.WriteAttributeString("is_pipelined_function", prvIsPipelinedFunction ? "Y" : String.Empty);

				writer.WriteElementString("description", prvDescription);
				writer.WriteElementString("plsql_declare", prvPLSQLDeclare);
				writer.WriteElementString("plsql_block", prvPLSQLBlock);

				// Write out the sorted test arguments
				writeTestArguments(writer, prvLstTestArguments);

				// Wrtie scenario groups and their scenarios and UDC's...
				writer.WriteStartElement("scenario_groups");

				for (int i = 0; i < prvLstScenarioGroups.Count; i++) {
					writer.WriteStartElement("scenario_group");
					writer.WriteAttributeString("name", prvLstScenarioGroups[i].name);

					writer.WriteAttributeString("reopen_connection_before_every_scenario", prvLstScenarioGroups[i].reopenConnectionBeforeEveryScenario ? "Y" : "N");
					writer.WriteAttributeString("rollback_afterwards", prvLstScenarioGroups[i].rollbackAfterEveryScenario ? "Y" : "N");

					writer.WriteAttributeString("max_allowed_run_time_in_seconds", prvLstScenarioGroups[i].maxAllowedRunTimeInSeconds.ToString());
					writer.WriteAttributeString("guid", prvLstScenarioGroups[i].guid);

					writer.WriteElementString("description", prvLstScenarioGroups[i].description);
					writer.WriteElementString("scenario_group_declare", prvLstScenarioGroups[i].scenarioGroupDeclare);
					writer.WriteElementString("scenario_group_startup", prvLstScenarioGroups[i].scenarioGroupStartup);
					writer.WriteElementString("scenario_startup", prvLstScenarioGroups[i].scenarioStartup);
					writer.WriteElementString("post_param_assignment", prvLstScenarioGroups[i].postParamAssignment);
					writer.WriteElementString("pre_user_defined_checks", prvLstScenarioGroups[i].preUDC);
					writer.WriteElementString("scenario_teardown", prvLstScenarioGroups[i].scenarioTeardown);
					writer.WriteElementString("scenario_group_teardown", prvLstScenarioGroups[i].scenarioGroupTeardown);

					// Scenarios
					scenarioCollection lstScenarios = prvLstScenarioGroups[i].scenarios;

					writer.WriteStartElement("scenarios");

					for (int j = 0; j < lstScenarios.Count; j++) {
						writer.WriteStartElement("scenario");
						writer.WriteAttributeString("expected_exception", lstScenarios[j].expectedException);
						writer.WriteAttributeString("expected_exception_message", lstScenarios[j].expectedExceptionMessage);
						writer.WriteAttributeString("guid", lstScenarios[j].guid);

						writer.WriteElementString("comments", lstScenarios[j].comments);
						
						// Write out the scenario's parameters...
						lstScenarios[j].parameters.writeToXML(writer: writer);
						
						writer.WriteEndElement(); // scenario
					}

					writer.WriteEndElement(); // scenarios


					// UDC's
					writer.WriteStartElement("user_defined_checks");

					List<udc> lstUDCs = prvLstScenarioGroups[i].udcCollection;

					for (int j = 0; j < lstUDCs.Count; j++) {
						writer.WriteStartElement("user_defined_check");
						writer.WriteAttributeString("name", lstUDCs[j].name);
						writer.WriteAttributeString("check_type", lstUDCs[j].checkType.ToString());
						writer.WriteAttributeString("sort_order", lstUDCs[j].sortOrder.ToString());
						writer.WriteAttributeString("guid", lstUDCs[j].guid);

						writer.WriteAttributeString("csv_excluded_columns", lstUDCs[j].csvExcludedColumns);

						writer.WriteElementString("description", lstUDCs[j].description);
						writer.WriteElementString("plsql_condition", lstUDCs[j].plsqlCondition);
						writer.WriteElementString("plsql_block", lstUDCs[j].plsqlBlock);
						writer.WriteElementString("expected_cursor", lstUDCs[j].expectedCursor);
						writer.WriteElementString("actual_cursor", lstUDCs[j].actualCursor);
						writer.WriteElementString("row_existence_cursor", lstUDCs[j].rowExistenceCursor);
						writer.WriteElementString("row_validation_cursor", lstUDCs[j].rowValidationCursor);

						// Enumerate any row validations within this UDC...
						if (lstUDCs[j].checkType == udc.enumCheckTypes.ROW_VALIDATION) {
							System.ComponentModel.BindingList<rowValidatorCheck> lstRowChecks = lstUDCs[j].fieldValidations;

							writer.WriteStartElement("row_validation_checks");

							for (int k = 0; k < lstRowChecks.Count; k++) {
								writer.WriteStartElement("row_validation_check");
								writer.WriteAttributeString("field_name", lstRowChecks[k].fieldName);
								writer.WriteAttributeString("comparison_type", lstRowChecks[k].comparisonType);
								writer.WriteAttributeString("field_value", lstRowChecks[k].fieldValue);
								writer.WriteEndElement(); // row_validation_check
							}

							writer.WriteEndElement(); // row_validation_checks
						}

						// Write out any stored matrix...
						if (lstUDCs[j].checkType == udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX) {
							writer.WriteStartElement("expected_matrix");

							// Write every row...
							for (int rowIndex = 0; rowIndex < lstUDCs[j].expectedMatrix.Rows.Count; rowIndex++) {
								writer.WriteStartElement("row");

								// Write every column within this row...
								for (int columnIndex = 0; columnIndex < lstUDCs[j].expectedMatrix.Columns.Count; columnIndex++) {
									writer.WriteStartElement("column");
									writer.WriteAttributeString("name", lstUDCs[j].expectedMatrix.Columns[columnIndex].ColumnName);

									writer.WriteCData(lstUDCs[j].expectedMatrix.Rows[rowIndex][columnIndex].ToString());

									writer.WriteEndElement(); // column
								}

								writer.WriteEndElement(); // row
							}

							writer.WriteEndElement(); // expected_matrix
						}

						writer.WriteEndElement(); // user_defined_check
					}

					writer.WriteEndElement(); // user_defined_checks

					// Library items
					writer.WriteStartElement("library_items");

					for (int j = 0; j < prvLstScenarioGroups[i].libraryItems.Count; j++) {
						writer.WriteStartElement("item");
						writer.WriteAttributeString("name", prvLstScenarioGroups[i].libraryItems[j].name);
						writer.WriteAttributeString("description", prvLstScenarioGroups[i].libraryItems[j].description);

						writer.WriteElementString("value", prvLstScenarioGroups[i].libraryItems[j].value);
						writer.WriteEndElement(); // item
					}

					writer.WriteEndElement(); // library_items

					writer.WriteEndElement(); // scenario_group
				}

				writer.WriteEndElement(); // scenario_groups

				writer.WriteEndElement(); // test
				writer.WriteEndElement(); // export

				writer.WriteEndDocument();
			}
		}

		// Recursively writes the test arguments and any child test arguments the have...
		private void writeTestArguments(XmlTextWriter writer, testArgumentCollection testArgs) {
			writer.WriteStartElement("arguments");

			var sortedTestArguments =
				from arg in testArgs
				orderby (arg.position == 0 ? 999999 : arg.position) // Sort any return plsqlValue to the bottom of the list
				select arg;

			foreach (var testArg in sortedTestArguments) {
				writer.WriteStartElement("argument");
				writer.WriteAttributeString("argument_name", testArg.argumentName);
				writer.WriteAttributeString("in_out", testArg.inOut);
				writer.WriteAttributeString("sequence", testArg.sequence.ToString());
				writer.WriteAttributeString("data_level", testArg.dataLevel.ToString());
				writer.WriteAttributeString("position", testArg.position.ToString());
				writer.WriteAttributeString("data_type", testArg.dataType);				
				writer.WriteAttributeString("pls_type", testArg.plsType);
				writer.WriteAttributeString("can_default", testArg.canDefault ? "Y" : "N");

				if (testArg.childArguments.Count > 0) {
					writeTestArguments(writer, testArg.childArguments);
				}

				writer.WriteEndElement(); // argument
			}

			writer.WriteEndElement(); // arguments
		}
		#endregion

		#region Other
		public bool hasArgumentMismatch(OracleConnection conTargetDB) {
			// Get the list of current arguments from the database...
			testArgumentCollection newArgs =
				targetDB.getMethodArguments(
						conTargetDB: conTargetDB,
						schema: prvUnitSchema,
						package: prvUnitMethod != String.Empty ? prvUnitName : String.Empty,
						method: prvUnitMethod == String.Empty ? prvUnitName : prvUnitMethod,
						overload: overload,
						returnedAsNested: false);

			// See if any differences are detected...
			testArgumentCollection allArgs = this.getAllArgs(this.testArguments);

			bool mismatch = (newArgs.Count != allArgs.Count);

			if (mismatch == false) {
				// Further checking...
				for (int i = 0; i < newArgs.Count; i++) {
					if (allArgs[i].argumentName.ToUpper() != newArgs[i].argumentName.ToUpper()
						|| allArgs[i].inOut != newArgs[i].inOut
						|| allArgs[i].sequence != newArgs[i].sequence
						|| allArgs[i].dataLevel != newArgs[i].dataLevel
						|| allArgs[i].position != newArgs[i].position
						|| allArgs[i].dataType != newArgs[i].dataType
						|| allArgs[i].plsType.ToUpper() != newArgs[i].plsType.ToUpper()
						|| allArgs[i].canDefault != newArgs[i].canDefault) {
						mismatch = true;
						break;
					}
				}
			}

			return mismatch;
		}

		public void addArgument(testArgument parentArgument, testArgument newArg) {
			// Adding new parameter...
			prvLstScenarioGroups.addParameter(parentArgument: parentArgument, newArg: newArg);

			if (parentArgument == null)
				this.prvLstTestArguments.Add(newArg);
			else
				parentArgument.childArguments.Add(newArg);
		}

		public void renameArgument(testArgument oldArg, testArgument newArg) {
			// Rename references to the argument in the scenario groups' parameters...
			prvLstScenarioGroups.renameParameter(oldArg: oldArg, newArg: newArg);

			// Rename the argument in the nested argument collection
			testArguments.renameArgument(oldArg: oldArg, newArg: newArg);
		}

		public void removeArgument(testArgument arg) {
			// Remove references to the argument in the scenario groups' parameters...
			prvLstScenarioGroups.removeParameter(arg: arg);
			
			// Remove the argument from the nested argument collection...
			testArguments.removeArgument(arg);
		}		

		public targetStatus getStatus(scenarioRunResults runResults) {
			targetStatus testStatus = targetStatus.noTests;

			for (int i = 0; i < prvLstScenarioGroups.Count; i++) {
				testStatus |= prvLstScenarioGroups[i].getStatus(runResults: runResults);
			}

			return testStatus == targetStatus.noTests ? targetStatus.testsNotRun : testStatus;
		}

		public void runTest(OracleConnection conTarget, scenarioRunResults runResults) {
			// Run each of this test's scenario groups...
			for (int i = 0; i < prvLstScenarioGroups.Count; i++) {
				if (runStatusChanged != null) {
					prvLstScenarioGroups[i].runStatusChanged -= runStatusChanged; // Remove it, just in case it was already there...
					prvLstScenarioGroups[i].runStatusChanged += runStatusChanged; // ... and re-add it.
				}
				if (scenarioRunCompleted != null) {
					prvLstScenarioGroups[i].scenarioRunCompleted -= scenarioRunCompleted; // Remove it, just in case it was already there...
					prvLstScenarioGroups[i].scenarioRunCompleted += scenarioRunCompleted; // ... and re-add it.
				}

				prvLstScenarioGroups[i].run(conTarget: conTarget, runResults: runResults);
			}
		}

		public void loadArgs(OracleConnection conTarget) {
			prvLstTestArguments =
				targetDB.getMethodArguments(
					conTargetDB: conTarget,
					schema: unitSchema,
					package: unitMethod != String.Empty ? unitName : String.Empty,
					method: unitMethod == String.Empty ? unitName : unitMethod,
					overload: overload);
		}
		#endregion
	}
}
