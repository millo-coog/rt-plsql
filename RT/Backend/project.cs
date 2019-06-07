using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using RT;
using System.Diagnostics;
using Oracle.DataAccess.Client;

namespace RT {
	public class project {
		#region Properties
		public string description { get; set; }
		public string filename { get; set; }
		public string name { get; set; }

		public string guid { get; set; }
		public repos repository { get; set; }
		public string version { get; set; }

		private List<targetDB> prvLstTargetDBs;
		public List<targetDB> targetDBs {
			get { return prvLstTargetDBs; }
		}
		#endregion

		#region Constructors
		public project() {
			guid = Guid.NewGuid().ToString();
			version = "3.0";
			description = "";
			filename = "";
			name = "";

			repository = new repos(associatedProject: this);
			prvLstTargetDBs = new List<targetDB>();
		}

		public project(string projectFilename, string excludedSchemas) {
			filename = projectFilename;
			
			// Read the project file
			XmlDocument doc = new XmlDocument();
			xmlHelper xmlTestHelper = new xmlHelper(xmlFilename: filename);

			try {
				doc.Load(filename);
			} catch (Exception e) {
				throw new Exception("Could not parse '" + filename + "'.\n\n" + e.Message);
			}

			XmlElement rt_plsql_project = doc.DocumentElement;
			Debug.Assert(rt_plsql_project.Name == "rt_plsql_project", "The root element in '" + filename + "' must be named 'rt_plsql_project'.");

			description = xmlTestHelper.getAttribute(node: rt_plsql_project, name: "description", required: true);
			guid = xmlTestHelper.getAttribute(node: rt_plsql_project, name: "guid", required: true);
			name = xmlTestHelper.getAttribute(node: rt_plsql_project, name: "name", required: true);
			version = xmlTestHelper.getAttribute(node: rt_plsql_project, name: "version", required: true); // Although stored, the version is not currently consulted, but it is required.

			prvLstTargetDBs = new List<targetDB>();

			repository = new repos(associatedProject: this);
			
			List<String> reposDBNames = repository.getDatabaseNames();
			for (int i = 0; i < reposDBNames.Count; i++) {
				targetDB newTarget = new targetDB();

				newTarget.name = reposDBNames[i];
				newTarget.excludedSchemas = excludedSchemas;

				prvLstTargetDBs.Add(newTarget);
			}
		}
		#endregion

		#region Methods
		// Saves the project to its configuration file
		public void save() {
			using (XmlTextWriter writer = new XmlTextWriter(filename: filename, encoding: null)) {
				writer.IndentChar = '\t';
				writer.Indentation = 1;
				writer.Formatting = Formatting.Indented;

				writer.WriteStartDocument();

				writer.WriteComment("This is a RT PL/SQL project.");

				writer.WriteStartElement("rt_plsql_project");
				writer.WriteAttributeString("version", version);
				writer.WriteAttributeString("name", name);
				writer.WriteAttributeString("description", description);
				writer.WriteAttributeString("guid", guid);
				writer.WriteEndElement(); // rt_plsql_project

				writer.WriteEndDocument();
			}
		}

		public string getStats() {
			string report = "";

			foreach (targetDB db in prvLstTargetDBs) {
				int targetableUnits = 0;
				int numTestedUnits = 0;
				int numScenarios = 0;
				int numLibraryItems = 0;

				int numUDCCompareCursors = 0;
				int numUDCCompareCursorToMatrix = 0;
				int numUDCCursorReturningNoRows = 0;
				int numUDCCursorReturningRows = 0;
				int numUDCPLSQLBlock = 0;
				int numUDCRowValidatorChecks = 0;

				// Count the number of targetable units in this db...
				report += db.name + " Stats\n";

				OracleCommand cmdGetTestableUnits = new OracleCommand();

				string sql = @"
					SELECT all_procedures.owner, all_procedures.object_name, all_procedures.procedure_name, all_procedures.object_type, NVL(all_procedures.overload, 0) AS overload
					  FROM sys.all_procedures
					 WHERE all_procedures.subprogram_id != 0
					";

				if (db.excludedSchemas != String.Empty) {
					String[] arrSchemas = db.excludedSchemas.Split(',');

					sql += " AND all_procedures.owner NOT IN (";

					for (int i = 0; i < arrSchemas.Count(); i++) {
						sql += ":schema" + i.ToString() + ",";

						cmdGetTestableUnits.Parameters.Add("schema" + i.ToString(), arrSchemas[i].Trim().ToUpper());
					}

					sql = sql.TrimEnd(',') + ")";
				}

				sql += " ORDER BY all_procedures.owner, all_procedures.object_name, all_procedures.procedure_name, TO_NUMBER(all_procedures.overload) NULLS FIRST";

				cmdGetTestableUnits.CommandText = sql;
				cmdGetTestableUnits.Connection = db.conTargetDB;

				OracleDataReader drTestableUnits = cmdGetTestableUnits.ExecuteReader();

				while (drTestableUnits.Read()) {
					targetableUnits++;

					List<test> lstTests = new List<test>();

					repository.getTestList(
						lstTests: ref lstTests,
						databaseName: db.name,
						objectType: drTestableUnits["object_type"].ToString(),
						schema: drTestableUnits["owner"].ToString(),
						name: drTestableUnits["object_name"].ToString(),
						method: drTestableUnits["procedure_name"].ToString(),
						overload: Int32.Parse(drTestableUnits["overload"].ToString())
					);

					if (lstTests.Count > 0) {
						numTestedUnits++;

						foreach (test unitTest in lstTests) {
							foreach (scenarioGroup scnGroup in unitTest.scenarioGroups) {
								numScenarios += scnGroup.scenarios.Count;

								numLibraryItems += scnGroup.libraryItems.Count;

								foreach (udc currUDC in scnGroup.udcCollection) {
									switch (currUDC.checkType) {
										case udc.enumCheckTypes.COMPARE_CURSORS:
											numUDCCompareCursors++;
											break;
										case udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX:
											numUDCCompareCursorToMatrix++;
											break;
										case udc.enumCheckTypes.CURSOR_RETURNING_NO_ROWS:
											numUDCCursorReturningNoRows++;
											break;
										case udc.enumCheckTypes.CURSOR_RETURNING_ROWS:
											numUDCCursorReturningRows++;
											break;
										case udc.enumCheckTypes.PLSQL_BLOCK:
											numUDCPLSQLBlock++;
											break;
										case udc.enumCheckTypes.ROW_VALIDATION:
											numUDCRowValidatorChecks++;
											break;
									}
								}
							}
						}
					}
				}

				drTestableUnits.Close();
				drTestableUnits.Dispose();

				cmdGetTestableUnits.Dispose();

				GC.Collect();

				report +=
					"Targetable Units: " + targetableUnits.ToString() + "\n" +
					"Tested Units: " + numTestedUnits.ToString() + "\n" +
					"Untested Units: " + (targetableUnits - numTestedUnits) + "\n" +
					"Percent of Units Tested: " + (((float) numTestedUnits / targetableUnits)*100) + "%\n" +
					"\n" +
					"Number of Scenarios: " + numScenarios + "\n" +
					"Number of Library Items: " + numLibraryItems + "\n" +
					"\n" +
					"UDCs:\n" +
					"   Compare Cursors: " + numUDCCompareCursors.ToString() + "\n" +
					"   Compare Cursor to Matrix: " + numUDCCompareCursorToMatrix.ToString() + "\n" +
					"   Cursor Returning No Rows: " + numUDCCursorReturningNoRows.ToString() + "\n" +
					"   Cursor Returning Rows: " + numUDCCursorReturningRows.ToString() + "\n" +
					"   PL/SQL Block: " + numUDCPLSQLBlock.ToString() + "\n" +
					"   Row Validator: " + numUDCRowValidatorChecks.ToString() + "\n" +
					"\n";
			}

			return report;
		}
		#endregion
	}
}
