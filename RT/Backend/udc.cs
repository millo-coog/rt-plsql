using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RT {
	public class udc {
		public enum enumCheckTypes {
			PLSQL_BLOCK = 1,
			CURSOR_RETURNING_ROWS = 2,
			CURSOR_RETURNING_NO_ROWS = 3,
			ROW_VALIDATION = 4,
			COMPARE_CURSORS = 5,
			COMPARE_CURSOR_TO_MATRIX = 6
		}

		#region Private variables
		// Remember to add any new private variables to the clone!

		private string prvGuid;

		private scenarioGroup prvScenarioGroup;

		//private schemaTreeNode prvAssociatedNode;

		private enumCheckTypes prvCheckType;
		private int prvSortOrder;
		private string prvName;
		private string prvDescription;
		private string prvPLSQLBlock;
		private string prvRowExistenceCursor;
		private string prvRowValidationCursor;
		private string prvPLSQLCondition;
		private string prvExpectedCursor;
		private DataTable prvExpectedMatrix;
		private string prvActualCursor;
		private string prvCSVExcludedColumns; // CSV of columns to exclude in a cursor comparison
		private rowValidatorCheckCollection prvFieldValidations;
		#endregion

		#region Get/Set methods
		public string guid {
			get { return prvGuid; }
			set { prvGuid = value; }
		}
		public enumCheckTypes checkType {
			get { return prvCheckType; }
			set { prvCheckType = value; }
		}
		public scenarioGroup scenarioGroup {
			get { return prvScenarioGroup; }
			set {
				Debug.Assert(value != null);
				prvScenarioGroup = value;
			}
		}
		public string plsqlBlock {
			get { return prvPLSQLBlock; }
			set { prvPLSQLBlock = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string rowValidationCursor {
			get { return prvRowValidationCursor; }
			set { prvRowValidationCursor = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string rowExistenceCursor {
			get { return prvRowExistenceCursor; }
			set { prvRowExistenceCursor = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string expectedCursor {
			get { return prvExpectedCursor; }
			set { prvExpectedCursor = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public DataTable expectedMatrix {
			get { return prvExpectedMatrix; }
			set { prvExpectedMatrix = value; }
		}
		public string actualCursor {
			get { return prvActualCursor; }
			set { prvActualCursor = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public string csvExcludedColumns {
			get { return prvCSVExcludedColumns; }
			set { prvCSVExcludedColumns = value.Replace(" ", "").Trim(); }
		}
		public string plsqlCondition {
			get { return prvPLSQLCondition; }
			set { prvPLSQLCondition = value.Trim().TrimEnd('\n').TrimEnd(); }
		}
		public int sortOrder {
			get { return prvSortOrder; }
			set { prvSortOrder = value; }
		}
		public string name {
			get { return prvName; }
			set {
				prvName = value;
			}
		}
		public string description {
			get { return prvDescription; }
			set { prvDescription = value; }
		}
		public rowValidatorCheckCollection fieldValidations {
			get { return prvFieldValidations; }
			set { prvFieldValidations = value; }
		}
		#endregion

		// Constructor
		public udc(scenarioGroup parentScenarioGroup, enumCheckTypes newCheckType) {
			prvScenarioGroup = parentScenarioGroup;
			
			prvGuid = Guid.NewGuid().ToString();
			
			prvCheckType = newCheckType;

			switch (newCheckType) {
				case enumCheckTypes.COMPARE_CURSOR_TO_MATRIX:
					prvName = "Compare Cursor vs. Matrix";
					break;
				case enumCheckTypes.COMPARE_CURSORS:
					prvName = "Compare Cursors";
					break;
				case enumCheckTypes.CURSOR_RETURNING_NO_ROWS:
					prvName = "Ensure Row Doesn't Exist";
					break;
				case enumCheckTypes.CURSOR_RETURNING_ROWS:
					prvName = "Ensure Row Exists";
					break;
				case enumCheckTypes.PLSQL_BLOCK:
					prvName = "PL/SQL Block";
					break;
				case enumCheckTypes.ROW_VALIDATION:
					prvName = "Validate Row";
					break;
			}
			
			prvSortOrder = prvScenarioGroup.udcCollection.Count;

			prvFieldValidations = new rowValidatorCheckCollection();
		}

		#region Methods
		public udc clone() {
			if (this.checkType == enumCheckTypes.COMPARE_CURSOR_TO_MATRIX) {
				throw new Exception("The code isn't written to clone a Cursor vs. Matrix UDC!");
			}

			udc newUDC = new udc(parentScenarioGroup: prvScenarioGroup, newCheckType: this.prvCheckType);

			newUDC.prvName = this.prvName;
			newUDC.prvDescription = this.prvDescription;
			newUDC.prvPLSQLBlock = this.prvPLSQLBlock;
			newUDC.prvRowExistenceCursor = this.prvRowExistenceCursor;
			newUDC.prvRowValidationCursor = this.prvRowValidationCursor;
			newUDC.prvPLSQLCondition = this.prvPLSQLCondition;
			newUDC.prvExpectedCursor = this.prvExpectedCursor;
			newUDC.prvActualCursor = this.prvActualCursor;
			newUDC.prvCSVExcludedColumns = this.prvCSVExcludedColumns;

			newUDC.prvFieldValidations = this.prvFieldValidations.clone();

			prvScenarioGroup.udcCollection.Add(newUDC);
			
			return newUDC;
		}

		public void delete() {
			prvScenarioGroup.udcCollection.Remove(this);
		}

		public static DataTable getUDCTypes() {
			DataTable dtUDCTypes = new DataTable();

			dtUDCTypes.Columns.Add(new DataColumn("check_type"));
			dtUDCTypes.Columns.Add(new DataColumn("name"));

			dtUDCTypes.Rows.Add(new object[] {1, "PL/SQL Block"});
			dtUDCTypes.Rows.Add(new object[] {2, "Cursor Returning Rows"});
			dtUDCTypes.Rows.Add(new object[] {3, "Cursor Returning No Rows"});
			dtUDCTypes.Rows.Add(new object[] {4, "Row Validator"});
			dtUDCTypes.Rows.Add(new object[] {5, "Compare Cursors"});
			dtUDCTypes.Rows.Add(new object[] {6, "Compare Cursors to Matrix" });

			return dtUDCTypes;
		}

		public void parseRowValidatingCursor(OracleConnection conTarget) {
			string csvColumnNamesForRowValidators = String.Empty;

			runBlock runBlockForDeclarations = new runBlock(currTest: this.scenarioGroup.test);
			
			// Parse the query that fetches the row to validate on the target database, 
			// and then feed its list of columns that it returns to the repository for the UDC.
			OracleCommand cmdParseRowQuery = new OracleCommand(
				"DECLARE\n" +
				"	v_CursorToParse SYS_REFCURSOR;\n" +
				"\n" +
				"	v_CursorID    NUMBER;\n" +
				"	v_colCnt      INTEGER := 0;\n" +
				"	v_arrColDescs sys.dbms_sql.desc_tab3;\n" +
				"\n" +
					this.scenarioGroup.test.plSQLDeclare + "\n\n" +
					this.scenarioGroup.scenarioGroupDeclare + "\n\n" +
					runBlockForDeclarations.getParameterVariableDeclaration() + "\n\n" +
					runBlockForDeclarations.getReturnValueVariableDeclaration(conTargetDB: conTarget) +
				"\n" +
				"BEGIN\n" +
				"	OPEN v_CursorToParse FOR" + "\n" +
				"		" + prvRowValidationCursor + "\n" +

				"	v_CursorID := dbms_sql.to_cursor_number(v_CursorToParse);\n" +
				"	sys.dbms_sql.describe_columns3(v_CursorID, v_colCnt, v_arrColDescs);\n" +
				"\n" +
				"	FOR v_i IN 1 .. v_colCnt LOOP\n" +
				"		:v_ColumnNames := :v_ColumnNames || v_arrColDescs(v_i).col_name || ', ';\n" +
				"	END LOOP;\n" +
				"\n" +
				"	:v_ColumnNames := RTRIM(:v_ColumnNames, ', ');\n" +
 				"\n" +
				"	sys.dbms_sql.close_cursor(v_CursorID);\n" +
				"END;",
				conTarget);
			
			cmdParseRowQuery.Parameters.Add("v_ColumnNames", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

			try {
				cmdParseRowQuery.ExecuteNonQuery();

				csvColumnNamesForRowValidators = cmdParseRowQuery.Parameters["v_ColumnNames"].Value.ToString();
			} catch (Exception err) {
				throw new Exception(
					"I wasn't able to parse this row validation query:\n" +
					prvRowValidationCursor + "\n" +
					"Within this context:\n" +
					cmdParseRowQuery.CommandText + "\n\n" +
					"Due to this error:\n" +
					err.Message);
			} finally {
				cmdParseRowQuery.Dispose();
			}

			// Merge the columns from this new query, into any existing column comparisons
			// so the user doesn't have to respecify all comparison types and values if he
			// slightly changed his query
			String[] arrColumnNames = csvColumnNamesForRowValidators.Split(',');
			rowValidatorCheckCollection newFieldValidations = new rowValidatorCheckCollection();

			for (int i = 0; i < arrColumnNames.Count(); i++) {
				// If the previous set of field validations had this column, then
				// preserve any comparison types and values...
				bool previouslyExisted = false;
				foreach (rowValidatorCheck currCheck in prvFieldValidations) {
					if (currCheck.fieldName == arrColumnNames[i].Trim()) {
						previouslyExisted = true;

						newFieldValidations.Add(currCheck);
						break;
					}
				}

				// If he didn't, then add him as a new column...
				if (previouslyExisted == false) {
					rowValidatorCheck fieldCheck = new rowValidatorCheck();

					fieldCheck.fieldName = arrColumnNames[i].Trim();
					fieldCheck.comparisonType = "Don't Test";

					newFieldValidations.Add(fieldCheck);
				}
			}

			prvFieldValidations = newFieldValidations;
		}
		#endregion
	}


	// Declare a collection iterator for our udc class
	public class udcCollection : System.Collections.Generic.List<udc> {
		// Adds another UDC to the collection...
		public new void Add(udc newUDC) {
			// Make sure the new UDC's guid is unique to the UDC collection...
			for (int i = 0; i < this.Count; i++) {
				if (this[i].guid == newUDC.guid) {
					throw new Exception("UDC guid #" + newUDC.guid + " is a duplicate of another UDC's guid in this scenario group.");
				}
			}

			base.Add(newUDC);
		}

		public udcCollection clone() {
			udcCollection newCollection = new udcCollection();
			int numItems = this.Count;

			for (int i = 0; i < numItems; i++) {
				newCollection.Add(this[i].clone());
			}

			return newCollection;
		}

		public udc this[String name] {
			get {
				foreach (udc currUDC in this) {
					if (currUDC.name == name) {
						return currUDC;
					}
				}
				
				return null;
			}
		}

		public void Remote(String name) {
			foreach (udc currUDC in this) {
				if (currUDC.name == name) {
					this.Remove(currUDC);
					break;
				}
			}
		}
	}
}
