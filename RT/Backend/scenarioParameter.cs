using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using RT;
using System.Diagnostics;

namespace RT {
	public class scenarioParameter {
		// Remember to add any new properties to the clone!
		private testArgument prvTestArg; // The test argument the parameter corresponds with...

		private string prvValue;
		private string prvValueComparisonType;
		private DataTable prvDataTable;

		private string prvExpectedOutValue; // Value expected for the OUT portion of an IN/OUT parameter.
		private string prvExpectedOutComparisonType; // Parameter type expected for the OUT portion of an IN/OUT parameter.
		private DataTable prvExpectedOutDataTable; // Holds any matrix that is expected for the OUT portion of an IN/OUT parameter.

		public List<scenarioParameterCollection> nestedParameters { get; set; } // A list of scenario parameter collections, to hold TABLE and PL/SQL nested parameters
		public List<scenarioParameterCollection> expectedOutNestedParameters { get; set; } // A list of scenario parameter collections, to hold out expected TABLE and PL/SQL nested parameters

		#region Get/set methods
		public testArgument testArg {
			get { return prvTestArg; }
			set { prvTestArg = value; }
		}

		public string value {
			get { return prvValue; }
			set { prvValue = value; }
		}
		public string plsqlValue { // Calculated, massaged form of the value that's useable in the generated PL/SQL code
			get {	return getPLSQLValue(value); }
		}
		public string plsqlRawValue { // Calculated, less massaged form of the value that's useable in the generated PL/SQL code
			get { return getPLSQLRawValue(value); }
		}
		public string valueComparisonType {
			get { return prvValueComparisonType; }
			set { prvValueComparisonType = value; }
		}
		public DataTable DataTable {
			get { return prvDataTable; }
			set { prvDataTable = value; }
		}

		public string expectedOutValue {
			get { return prvExpectedOutValue; }
			set { prvExpectedOutValue = value; }
		}
		public string expectedOutPLSQLRawValue { // Calculated, less massaged form of the value that's useable in the generated PL/SQL code
			get { return getPLSQLRawValue(expectedOutValue); }
		}
		public string expectedOutComparisonType {
			get { return prvExpectedOutComparisonType; }
			set { prvExpectedOutComparisonType = value; }
		}
		public DataTable ExpectedOutDataTable {
			get { return prvExpectedOutDataTable; }
			set { prvExpectedOutDataTable = value; }
		}

		public bool isDefaulted {
			get { return prvValueComparisonType == "defaulted"; }
		}
		#endregion

		#region Constructors
		public scenarioParameter() {
			prvTestArg = null;

			prvValue = String.Empty;
			prvValueComparisonType = String.Empty;
			prvDataTable = new DataTable();

			prvExpectedOutValue = String.Empty;
			prvExpectedOutComparisonType = String.Empty;
			prvExpectedOutDataTable = new DataTable();

			nestedParameters = new List<scenarioParameterCollection>();
			expectedOutNestedParameters = new List<scenarioParameterCollection>();
		}

		public scenarioParameter(testArgument arg) {
			prvTestArg = arg;
			
			prvValue = String.Empty;
			prvExpectedOutValue = String.Empty;
			
			if (arg.plsType == "BOOLEAN" || arg.plsType == "DATE" || arg.plsType == "TIMESTAMP" || arg.dataType == "REF CURSOR") {
				valueComparisonType = "exp";
				expectedOutComparisonType = "exp";
			} else {
				valueComparisonType = "value";
				expectedOutComparisonType = "value";
			}

			if (arg.dataType == "OBJECT") {
				valueComparisonType = "matrix";
				expectedOutComparisonType = "matrix";
			}

			if (arg.dataType == "TABLE" || arg.dataType == "PL/SQL RECORD") {
				valueComparisonType = "nested";
				expectedOutComparisonType = "nested";
			}

			prvDataTable = new DataTable();
			prvExpectedOutDataTable = new DataTable();

			nestedParameters = new List<scenarioParameterCollection>();
			if (arg.childArguments.Count > 0) {
				nestedParameters.Add(new scenarioParameterCollection(args: arg.childArguments));
			}
			
			expectedOutNestedParameters = new List<scenarioParameterCollection>();
			if (arg.childArguments.Count > 0) {
				expectedOutNestedParameters.Add(new scenarioParameterCollection(args: arg.childArguments));
			}

			//if (childParameters.Count > 0) {
			//	Debug.Print(arg.argumentName + ":");
			//	foreach (scenarioParameterCollection scnParamCollection in childParameters) {
			//		string strParams = "";

			//		foreach (scenarioParameter scnParam in scnParamCollection) {
			//			strParams += scnParam.testArg.argumentName + ", ";
			//		}

			//		Debug.Print(strParams);
			//	}
			//}
		}
		#endregion

		#region Methods
		private string getPLSQLValue (string valueToTransform) {
			string retValue = String.Empty;

			if (valueComparisonType == "exp") {
				if (valueToTransform == String.Empty) {
					retValue = "NULL";
				} else {
					retValue = valueToTransform;
				}
			} else {
				if (valueToTransform == String.Empty) {
					retValue = "NULL";
				} else {
					string type = testArg.plsType;

					switch (type) {
						case "VARCHAR2":
						case "CLOB":
						case "CHAR":
						case "VARCHAR":
						case "NCHAR":
						case "NVARCHAR2":
						case "LONG":
						case "NCLOB":
							retValue = "lfToCRLF('" + valueToTransform.Replace("'", "''") + "')";
							break;
						case "RAW":
							retValue = "TO_CHAR(lfToCRLF('" + valueToTransform.Replace("'", "''") + "'))";
							break;
						default:
							retValue = valueToTransform;
							break;
					}
				}
			}

			return retValue;
		}
		private string getPLSQLRawValue(string valueToTransform) {
			string retValue;

			if (valueComparisonType == "exp") {
				retValue = getPLSQLValue(valueToTransform);
			} else {
				if (valueToTransform == String.Empty) {
					retValue = getPLSQLValue(valueToTransform);
				} else {
					string type = testArg.plsType;

					if (type == "VARCHAR2" || type == "CLOB" || type == "CHAR" || type == "VARCHAR" || type == "NCHAR" || type == "NVARCHAR2" || type == "LONG" || type == "RAW" || type == "LONG RAW" || type == "NCLOB") {
						retValue = "'" + valueToTransform.Replace("'", "''") + "'";
					} else {
						retValue = valueToTransform;
					}
				}
			}

			return retValue;
		}

		// Utility method to return the name of the given plsqlValue column's corresponding comparison type column.
		public static string getMatrixColumnComparisonTypeColName(string columnName) {
			return "comparison_type_" + columnName + "$";
		}

		// Reads the scenario parameter from xml
		public void readFromXml(test parentTest, XmlNode parameterNode) {
			xmlHelper xmlTestHelper = new xmlHelper(xmlFilename: parentTest.xmlFilename);

			// Load the regular value...
			XmlNode valueNode = xmlTestHelper.getSingleNode(parameterNode, "value");

			if (valueNode != null) {
				this.valueComparisonType = valueNode.Attributes["parameter_type"].Value;

				switch (this.valueComparisonType) {
					case "matrix":
						this.DataTable = readMatrix(parameterNode: valueNode);
						break;

					case "nested":
						// Nested parameters
						this.nestedParameters = scenarioParameterCollection.readFromXML(xmlTestHelper: xmlTestHelper, parentTest: parentTest, args: this.testArg.childArguments, parentNode: valueNode);

						break;
					default:
						// No nested parameters
						this.value = valueNode.InnerText;
						break;
				}
			}

			// Load any expected OUT value associated with an IN/OUT parameter...
			XmlNode expectedOutValueNode = xmlTestHelper.getSingleNode(parameterNode, "expected_out_value");

			if (expectedOutValueNode != null) {
				this.expectedOutComparisonType = expectedOutValueNode.Attributes["comparison_type"].Value;

				switch (this.expectedOutComparisonType) {
				case "matrix":
					this.ExpectedOutDataTable = readMatrix(parameterNode: expectedOutValueNode);
					break;

				case "nested":
					this.expectedOutNestedParameters = scenarioParameterCollection.readFromXML(xmlTestHelper: xmlTestHelper, parentTest: parentTest, args: this.testArg.childArguments, parentNode: expectedOutValueNode);
					break;

				default:
					this.expectedOutValue = expectedOutValueNode.InnerText;
					break;
				}
			}
		}

		private static DataTable readMatrix(XmlNode parameterNode) {
			DataTable dtReadMatrix = new DataTable();

			// Use the first child row's columns to know what columns are in the matrix...
			foreach (XmlNode rowNode in parameterNode.SelectSingleNode("matrix")) {
				foreach (XmlNode columnNode in rowNode.ChildNodes) {
					string valueColumnName = columnNode.Attributes["name"].Value;
					string comparisonTypeColumnName = getMatrixColumnComparisonTypeColName(columnName: columnNode.Attributes["name"].Value);

					dtReadMatrix.Columns.Add(columnNode.Attributes["name"].Value);
					dtReadMatrix.Columns.Add(comparisonTypeColumnName);

					dtReadMatrix.Columns[comparisonTypeColumnName].DefaultValue = "value";
				}

				break;
			}

			// Now, put the values into the datatable...
			foreach (XmlNode rowNode in parameterNode.SelectSingleNode("matrix")) {
				object[] arrParams = new object[dtReadMatrix.Columns.Count];
				int columnIndex = 0;

				foreach (XmlNode columnNode in rowNode.ChildNodes) {
					arrParams[columnIndex++] = columnNode.FirstChild.Value;
					arrParams[columnIndex++] = columnNode.Attributes["comparison_type"].Value;
				}

				dtReadMatrix.Rows.Add(arrParams);
			}

			return dtReadMatrix;
		}

		// Saves the scenario parameter to xml
		public void writeToXml(XmlTextWriter writer, string nestedValueDirection = "") {
			writer.WriteStartElement("parameter");
			writer.WriteAttributeString("argument_name", prvTestArg.argumentName);

			// Write the usual value, which is passed in or is expected out/return, etc.
			if (nestedValueDirection == "" || nestedValueDirection == "IN") {
				writer.WriteStartElement("value");
				writer.WriteAttributeString("parameter_type", this.valueComparisonType);

				if (this.valueComparisonType == "matrix") {
					writeMatrix(dataTableToWrite: this.DataTable, writer: writer);
				} else {
					if (this.valueComparisonType == "nested" && nestedParameters.Count > 0) {
						foreach (scenarioParameterCollection scnParams in nestedParameters) {
							scnParams.writeToXML(writer: writer, nestedValueDirection: "IN");
						}
					} else {
						writer.WriteCData(this.value);
					}
				}

				writer.WriteEndElement(); // value
			}

			// Write the expected OUT portion value of an IN/OUT parameter
			if (nestedValueDirection == "" || nestedValueDirection == "OUT") {
				if (this.testArg.inOut == "IN/OUT") {
					writer.WriteStartElement("expected_out_value");
					writer.WriteAttributeString("comparison_type", this.expectedOutComparisonType);

					if (this.expectedOutComparisonType == "matrix") {
						writeMatrix(dataTableToWrite: this.ExpectedOutDataTable, writer: writer);
					} else {
						if (this.expectedOutComparisonType == "nested" && expectedOutNestedParameters.Count > 0) {
							foreach (scenarioParameterCollection scnParams in expectedOutNestedParameters) {
								scnParams.writeToXML(writer: writer, nestedValueDirection: "OUT");
							}
						} else {
							writer.WriteCData(this.expectedOutValue);
						}
					}

					writer.WriteEndElement(); // expected_out_value
				}
			}

			writer.WriteEndElement(); // parameter
		}

		private static void writeMatrix(DataTable dataTableToWrite, XmlTextWriter writer) {
			writer.WriteStartElement("matrix");

			// Write every row...
			for (int rowIndex = 0; rowIndex < dataTableToWrite.Rows.Count; rowIndex++) {
				writer.WriteStartElement("row");

				// Write every column within this row...
				for (int columnIndex = 0; columnIndex < dataTableToWrite.Columns.Count; columnIndex += 2) {
					writer.WriteStartElement("column");
					writer.WriteAttributeString("name", dataTableToWrite.Columns[columnIndex].ColumnName);
					writer.WriteAttributeString("comparison_type", dataTableToWrite.Rows[rowIndex][columnIndex + 1].ToString());

					writer.WriteCData(dataTableToWrite.Rows[rowIndex][columnIndex].ToString());

					writer.WriteEndElement(); // column
				}

				writer.WriteEndElement(); // row
			}

			writer.WriteEndElement();
		}
		#endregion
	}

	// Declare a collection iterator for our row validator check class
	public class scenarioParameterCollection : System.Collections.Generic.List<scenarioParameter> {
		// Constructors
		public scenarioParameterCollection() {

		}

		public scenarioParameterCollection(testArgumentCollection args) {
			foreach (testArgument arg in args) {
				this.Add(item: new scenarioParameter(arg: arg));
			}
		}

		// Clones the current collection
		public scenarioParameterCollection clone() {
			scenarioParameterCollection newCollection = new scenarioParameterCollection();

			foreach (scenarioParameter currSCNParam in this) {
				scenarioParameter newParam = new scenarioParameter();

				newParam.testArg = currSCNParam.testArg;

				newParam.value = currSCNParam.value;
				newParam.valueComparisonType = currSCNParam.valueComparisonType;
				
				newParam.expectedOutValue = currSCNParam.expectedOutValue;
				newParam.expectedOutComparisonType = currSCNParam.expectedOutComparisonType;

				newParam.DataTable = currSCNParam.DataTable;
				newParam.ExpectedOutDataTable = currSCNParam.ExpectedOutDataTable;

				foreach (scenarioParameterCollection childScnParamCollection in currSCNParam.nestedParameters) {
					newParam.nestedParameters.Add(childScnParamCollection.clone());
				}

				foreach (scenarioParameterCollection childScnParamCollection in currSCNParam.expectedOutNestedParameters) {
					newParam.expectedOutNestedParameters.Add(childScnParamCollection.clone());
				}

				newCollection.Add(newParam);
			}

			return newCollection;
		}

		// Finds the given parameter in the collection by name
		public scenarioParameter this[String argumentName] {
			get {
				foreach (scenarioParameter currSCNParam in this) {
					if (currSCNParam.testArg.argumentName == argumentName) {
						return currSCNParam;
					}
				}

				return null;
			}
		}

		public void addParameter(testArgument parentArgument, testArgument newArg) {
			if (parentArgument == null || this.Count == 0) {
				scenarioParameter newScnParam = new scenarioParameter(arg: newArg);

				// Allow new parameters to default in tests, if possible; otherwise, they
				// just retain their default comparison type based on their argument's PLS_TYPE.
				if (newArg.canDefault) {
					newScnParam.valueComparisonType = "defaulted";
				}

				if (newArg.inOut == "IN/OUT")
					newScnParam.expectedOutComparisonType = "don't test";

				this.Add(newScnParam);
			} else {
				foreach (scenarioParameter param in this) {
					if (param.testArg == parentArgument) {
						foreach (scenarioParameterCollection scnParamCollection in param.nestedParameters) {
							scenarioParameter newScnParam = new scenarioParameter(arg: newArg);

							// Allow new parameters to default in tests, if possible; otherwise, they
							// just retain their default comparison type based on their argument's PLS_TYPE.
							if (newArg.canDefault) {
								newScnParam.valueComparisonType = "defaulted";
							}

							if (newArg.inOut == "IN/OUT")
								newScnParam.expectedOutComparisonType = "don't test";

							scnParamCollection.Add(newScnParam);
						}

						foreach (scenarioParameterCollection scnParamCollection in param.expectedOutNestedParameters) {
							scenarioParameter newScnParam = new scenarioParameter(arg: newArg);

							// Allow new parameters to default in tests, if possible; otherwise, they
							// just retain their default comparison type based on their argument's PLS_TYPE.
							if (newArg.canDefault) {
								newScnParam.valueComparisonType = "defaulted";
							}

							if (newArg.inOut == "IN/OUT")
								newScnParam.expectedOutComparisonType = "don't test";

							scnParamCollection.Add(newScnParam);
						}

						break;
					} else {
						// Recurse into any children until we find our parent argument...

						foreach (scenarioParameterCollection scnParamCollection in param.nestedParameters) {
							scnParamCollection.addParameter(parentArgument: parentArgument, newArg: newArg);
						}

						foreach (scenarioParameterCollection scnParamCollection in param.expectedOutNestedParameters) {
							scnParamCollection.addParameter(parentArgument: parentArgument, newArg: newArg);
						}
					}
				}
			}
		}

		// Renames (overlays) the given old parameter with a new one...
		public void renameParameter(testArgument oldArg, testArgument newArg) {
			foreach (scenarioParameter param in this) {
				if (param.testArg == oldArg) {
					param.testArg = newArg;

					if (oldArg.inOut != newArg.inOut) {
						if (oldArg.inOut == "IN/OUT" && newArg.inOut != "IN/OUT") {
							param.expectedOutNestedParameters.Clear();
						}

						if (oldArg.inOut == "IN") {
							// Was only an IN parameter before, but now has out-style behavior
							param.valueComparisonType = "don't test";
						}
					}

					break;
				} else {
					if (param.nestedParameters.Count > 0) {
						foreach (scenarioParameterCollection nestedScnParams in param.nestedParameters) {
							nestedScnParams.renameParameter(oldArg: oldArg, newArg: newArg);
						}
					}

					if (param.expectedOutNestedParameters.Count > 0) {
						foreach (scenarioParameterCollection nestedOutScnParams in param.nestedParameters) {
							nestedOutScnParams.renameParameter(oldArg: oldArg, newArg: newArg);
						}
					}
				}
			}
		}

		// Removes the given parameter from the collection
		public void removeParameter(testArgument arg) {
			foreach (scenarioParameter param in this) {
				if (param.testArg == arg) {
					this.Remove(param);
					break;
				} else {
					if (param.nestedParameters.Count > 0) {
						foreach (scenarioParameterCollection nestedScnParams in param.nestedParameters) {
							nestedScnParams.removeParameter(arg: arg);
						}
					}

					if (param.expectedOutNestedParameters.Count > 0) {
						foreach (scenarioParameterCollection nestedOutScnParams in param.nestedParameters) {
							nestedOutScnParams.removeParameter(arg: arg);
						}
					}
				}
			}
		}

		public static List<scenarioParameterCollection> readFromXML(xmlHelper xmlTestHelper, test parentTest, testArgumentCollection args, XmlNode parentNode) {
			List<scenarioParameterCollection> lstNestedParams = new List<scenarioParameterCollection>();

			foreach (XmlNode parametersNode in parentNode.ChildNodes) {
				if (parametersNode.Name == "parameters") {
					scenarioParameterCollection paramCollection = new scenarioParameterCollection(args: args);

					foreach (XmlNode parameterNode in parametersNode.ChildNodes) {
						string argName = xmlTestHelper.getAttribute(node: parameterNode, name: "argument_name", required: true).ToLower();

						if (paramCollection[argName] == null) {
							throw new Exception(message: "When reading " + xmlTestHelper.filename + ", in the <parameters> section, a non-existent argument named '" + argName + " was referenced.");
						} else {
							paramCollection[argName].readFromXml(parentTest: parentTest, parameterNode: parameterNode);
						}
					}

					lstNestedParams.Add(paramCollection);
				}
			}

			return lstNestedParams;
		}

		public void writeToXML(XmlTextWriter writer, string nestedValueDirection = "") {
			var sortedParameters =
				from param in this
				orderby (param.testArg.position == 0 ? 999999 : param.testArg.position) // Sort any return plsqlValue to the bottom of the list
				select param;

			writer.WriteStartElement("parameters");
			
			foreach (var param in sortedParameters) {
				param.writeToXml(writer: writer, nestedValueDirection: nestedValueDirection);
			}

			writer.WriteEndElement(); // parameters
		}

		public static DataTable toDataTable(List<scenarioParameterCollection> nestedParameters, bool useValueAttributes) {
			DataTable dtMatrix = new DataTable();

			foreach (testArgument arg in nestedParameters[0][0].testArg.childArguments) {
				dtMatrix.Columns.Add(arg.argumentName);
				dtMatrix.Columns.Add(scenarioParameter.getMatrixColumnComparisonTypeColName(arg.argumentName));
			}

			foreach (scenarioParameterCollection row in nestedParameters) {
				scenarioParameterCollection spc;

				if (useValueAttributes) {
					spc = row[0].nestedParameters[0];
				} else {
					spc = row[0].expectedOutNestedParameters[0];
				}

				object[] arrParams = new object[spc.Count * 2];

				int i = 0;
				foreach (scenarioParameter param in spc) {
					if (useValueAttributes) {
						arrParams[i++] = param.value;
						arrParams[i++] = param.valueComparisonType;
					} else {
						arrParams[i++] = param.expectedOutValue;
						arrParams[i++] = param.expectedOutComparisonType;
					}
				}

				dtMatrix.Rows.Add(arrParams);
			}

			return dtMatrix;
		}
	}
}
