using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Data;
using Oracle.DataAccess.Client;

namespace RT {
	public class runBlock {
		private scenarioGroup prvScenarioGroup = null;
		private test prvTest = null;
		private int prvScenarioIndex = -1;

		private int v_IndentionLvl = 0;

		private String prvPLSQLBlock = String.Empty;

		private bool prvGenerateAsDebugBlock = false;

		// Used for getting appropriate bind variable names into the anonymous block, based on whether the caller will be binding or not...
		private string c_ARR_STATUSES_VAR_NAME;
		private string c_ARR_ERROR_NUMBERS_VAR_NAME;
		private string c_ARR_ERROR_MSGS_VAR_NAME;

		// Out variables for getting back actual and expected results from in/out/return parameters
		private string c_ARR_OUT_PRM_EXP_VAL_VAR_NAME;
		private string c_ARR_OUT_PRM_ACT_VAL_VAR_NAME;

		// Out variables for getting back actual and expected results from Cursor vs. Matrix UDC comparisons
		private string c_ARR_OUT_UDC_EXP_VAL_VAR_NAME;
		private string c_ARR_OUT_UDC_ACT_VAL_VAR_NAME;

		private bool v_MethodIsFunction;
		private int v_ReturnValueParamIndex;
		private int v_MaxParamNameLen;

		private testArgumentCollection prvAllTestArgs;

		bool checkMatchDeclared = false;

		#region Constructors
		public runBlock()	{ }

		public runBlock(test currTest) {
			prvTest = currTest;
			
			getParamInfo();
		}
		#endregion

		private string indent(int p_LvlAdjustment = 0) {
			return new String('\t', v_IndentionLvl + p_LvlAdjustment);
		}

		#region Header: Variables and Constant Declarations
		private void getParamInfo() {
			for (int i = 0; i < prvTest.testArguments.Count; i++) {
				// Determine the index of the return plsqlValue argument
				if (prvTest.testArguments[i].inOut == "RETURN") {
					v_MethodIsFunction = true;
					v_ReturnValueParamIndex = i;
				}

				// Determine the longest argument name, for formatting purposes
				if (prvTest.testArguments[i].argumentName.Length > v_MaxParamNameLen) {
					v_MaxParamNameLen = prvTest.testArguments[i].argumentName.Length;
				}
			}
		}

		public string getParameterVariableDeclaration() {
			bool v_DeclaredVariable = false;
			string paramDeclarationPLSQL = String.Empty;

			if (prvTest.testArguments.Count > 0) {
				paramDeclarationPLSQL += indent() + "-- Parameter variables" + Environment.NewLine;
        
				for (int i = 0; i < prvTest.testArguments.Count; i++) {
					if (prvTest.testArguments[i].inOut != "RETURN") {
						// Declare a record type for any "anonymous" record type...
						if (prvTest.testArguments[i].dataType == "PL/SQL RECORD" && prvTest.testArguments[i].plsType == "") {
							paramDeclarationPLSQL +=
								indent() + "TYPE " + prvTest.testArguments[i].getPLSQLTypeDeclaration() + " IS RECORD (";

							foreach (testArgument fieldArg in prvTest.testArguments[i].childArguments) {
								paramDeclarationPLSQL +=
									Environment.NewLine +
									indent(1) + fieldArg.argumentName + " " + (fieldArg.plsType == "VARCHAR2" ? "VARCHAR2(32767)" : fieldArg.plsType) + ",";
							}

							paramDeclarationPLSQL = paramDeclarationPLSQL.TrimEnd(',');

							paramDeclarationPLSQL +=
								Environment.NewLine +
								indent() + ");" + Environment.NewLine;
						}

						paramDeclarationPLSQL += 
							indent() + 
							prvTest.testArguments[i].argumentName.PadRight(v_MaxParamNameLen) + " " +
							prvTest.testArguments[i].getPLSQLTypeDeclaration() +
							"; -- " + prvTest.testArguments[i].inOut +
							Environment.NewLine;
              
						v_DeclaredVariable = true;
					}
				}
          
				if (v_DeclaredVariable) {
					paramDeclarationPLSQL += Environment.NewLine;
				}
			}

			if (prvTest.plSQLDeclare != string.Empty) {
				paramDeclarationPLSQL +=
					indent() + "-- User-DECLARE section..." + Environment.NewLine;

				paramDeclarationPLSQL += prvTest.plSQLDeclare;

				paramDeclarationPLSQL += Environment.NewLine + Environment.NewLine;
			}

			return paramDeclarationPLSQL;
		}
		
		public string getReturnValueVariableDeclaration(OracleConnection conTargetDB) {
			string returnVarPLSQL = String.Empty;
			testArgument returnArg;

			if (v_MethodIsFunction) {
				returnArg = prvTest.testArguments[v_ReturnValueParamIndex];

				if (prvTest.isPipelinedFunction) {
					// Declare a cursor to hold the pipelined result set call
					returnVarPLSQL +=
						indent() + returnArg.argumentName + " SYS_REFCURSOR; " + Environment.NewLine;
				} else {
					if (returnArg.plsType == "") {
						// When a function returns a %ROWTYPE, the datatype is PL/SQL RECORD, but there's no pls_type...
						Debug.Assert(condition: returnArg.dataType == "PL/SQL RECORD", message: "Can't handle this scenario: dataType of '" + returnArg.dataType + "', but need PL/SQL RECORD when pls_type is empty!");

						returnVarPLSQL +=
							indent() + "TYPE " + returnArg.getPLSQLTypeDeclaration() + " IS RECORD (";

						foreach (testArgument fieldArg in returnArg.childArguments) {
							returnVarPLSQL +=
								Environment.NewLine +
								indent(1) + fieldArg.argumentName + " " + (fieldArg.plsType == "VARCHAR2" ? "VARCHAR2(32767)" : fieldArg.plsType) + ",";
						}

						returnVarPLSQL = returnVarPLSQL.TrimEnd(',');

						returnVarPLSQL +=
							Environment.NewLine +
							indent() + ");" + Environment.NewLine +
							indent() + returnArg.argumentName + " " + returnArg.getPLSQLTypeDeclaration() + ";" + Environment.NewLine
							+ Environment.NewLine;
					} else {
						returnVarPLSQL +=
							indent() + returnArg.argumentName + " " + returnArg.getPLSQLTypeDeclaration() + ";" + Environment.NewLine;
					}
				}
			}

			return returnVarPLSQL;
		}

		private void declareOtherVarsAndConstants(OracleConnection conTargetDB) {
			// If generating a stand alone block, use declared variables, rather than
			// bind variables in the returned block.
			string linePrefix = prvGenerateAsDebugBlock ? String.Empty : "-- ";

			prvPLSQLBlock +=
				indent() + "-- Bind variables" + Environment.NewLine +
				indent() + linePrefix + "TYPE t_arrStatuses IS TABLE OF VARCHAR2(30) INDEX BY PLS_INTEGER;" + Environment.NewLine +
				indent() + linePrefix + c_ARR_STATUSES_VAR_NAME + " t_arrStatuses;" + Environment.NewLine +
				Environment.NewLine +
				indent() + linePrefix + "TYPE t_arrErrorNumbers IS TABLE OF NUMBER(5) INDEX BY PLS_INTEGER;" + Environment.NewLine +
				indent() + linePrefix + c_ARR_ERROR_NUMBERS_VAR_NAME + " t_arrErrorNumbers;" + Environment.NewLine +
				Environment.NewLine +
				indent() + linePrefix + "TYPE t_arrErrorMessages IS TABLE OF VARCHAR2(32767) INDEX BY PLS_INTEGER;" + Environment.NewLine +
				indent() + linePrefix + c_ARR_ERROR_MSGS_VAR_NAME + " t_arrErrorMessages;" + Environment.NewLine +
				Environment.NewLine;

			for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
				for (int j = 0; j < prvAllTestArgs.Count; j++) {
					if (prvAllTestArgs[j].inOut == "IN/OUT" || prvAllTestArgs[j].inOut == "OUT" || prvAllTestArgs[j].inOut == "RETURN") {
						prvPLSQLBlock +=
							indent() + linePrefix + c_ARR_OUT_PRM_EXP_VAL_VAR_NAME + i.ToString() + "_" + prvAllTestArgs[j].sequence.ToString() + " CLOB; -- " + prvAllTestArgs[j].argumentName + Environment.NewLine +
							indent() + linePrefix + c_ARR_OUT_PRM_ACT_VAL_VAR_NAME + i.ToString() + "_" + prvAllTestArgs[j].sequence.ToString() + " CLOB; -- " + prvAllTestArgs[j].argumentName + Environment.NewLine;
					}
				}

				for (int udcIndex = 0; udcIndex < prvScenarioGroup.udcCollection.Count; udcIndex++) {
					if (prvScenarioGroup.udcCollection[udcIndex].checkType == udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX) {					
						prvPLSQLBlock +=
							indent() + linePrefix + c_ARR_OUT_UDC_EXP_VAL_VAR_NAME + udcIndex.ToString() + "_" + i.ToString() + " CLOB; -- " + prvScenarioGroup.udcCollection[udcIndex].name + Environment.NewLine +
							indent() + linePrefix + c_ARR_OUT_UDC_ACT_VAL_VAR_NAME + udcIndex.ToString() + "_" + i.ToString() + " CLOB; -- " + prvScenarioGroup.udcCollection[udcIndex].name + Environment.NewLine;
					}
				}
			}


			prvPLSQLBlock +=
				Environment.NewLine;
      
			prvPLSQLBlock +=
				indent() + "-- Internal constants" + Environment.NewLine +
				indent() + "c_CRLF CONSTANT CHAR(2) := CHR(13) || CHR(10);" + Environment.NewLine +
				indent() + "c_LF   CONSTANT CHAR(1) := CHR(10);" + Environment.NewLine +
				indent() + "c_TAB  CONSTANT CHAR(1) := CHR(9);" + Environment.NewLine +
				Environment.NewLine;
      
			prvPLSQLBlock += 
				indent() + "-- Internal variables" + Environment.NewLine;

			prvPLSQLBlock +=
				getReturnValueVariableDeclaration(conTargetDB: conTargetDB);
        
			prvPLSQLBlock += 
				indent() + "v_CurrScenarioIndex$ PLS_INTEGER;" + Environment.NewLine +
				Environment.NewLine +
				indent() + "TYPE t_arrGUIDs IS TABLE OF VARCHAR2(36) INDEX BY PLS_INTEGER;" + Environment.NewLine +
				indent() + "v_arrGUIDs$ t_arrGUIDS;" + Environment.NewLine;
        
			// If we have parameters, then declare an array of booleans so the
			// user can tell whether a parameter was passed or not in their UDC's.
			if (prvTest.testArguments.Count > 0) {
				prvPLSQLBlock +=
					Environment.NewLine +
					indent() + "SUBTYPE t_ParameterName IS VARCHAR2(30);" + Environment.NewLine +
					indent() + "TYPE t_arrDefaulted IS TABLE OF BOOLEAN INDEX BY t_ParameterName;" + Environment.NewLine +
					indent() + "v_arrDefaulted t_arrDefaulted;" + Environment.NewLine;
			}
        
			prvPLSQLBlock += Environment.NewLine;
		}
		#endregion
		
		#region Header: Nested RT Subprocedures
		private void declareBoolToChar() {
			prvPLSQLBlock +=
				indent() + "FUNCTION boolToChar(p_Bool BOOLEAN) RETURN VARCHAR2 AS" + Environment.NewLine +
				indent() + "BEGIN" + Environment.NewLine +
				indent(1) + "RETURN" + Environment.NewLine +
				indent(2) +  "CASE" + Environment.NewLine +
				indent(2) +  "WHEN p_Bool = TRUE THEN 'TRUE'" + Environment.NewLine +
				indent(2) +  "WHEN p_Bool = FALSE THEN 'FALSE'" + Environment.NewLine +
				indent(2) +  "WHEN p_Bool IS NULL THEN 'NULL'" + Environment.NewLine +
				indent(1) +  "END;" + Environment.NewLine +
				indent() + "END;" + Environment.NewLine +
				Environment.NewLine;
		}

		private void declareCompareCursors() {
			bool v_NeedMethod = false;

			// Testing pipelined result sets would need the method...
			v_NeedMethod = prvTest.isPipelinedFunction;

			// RETURN or OUT parameters would need the method...
			if (v_NeedMethod == false) {
				for (int i = 0; i < prvTest.testArguments.Count; i++) {
					if (prvTest.testArguments[i].plsType == "SYS_REFCURSOR"
						&& (prvTest.testArguments[i].inOut == "RETURN" || prvTest.testArguments[i].inOut == "IN/OUT" || prvTest.testArguments[i].inOut == "OUT")) {
							v_NeedMethod = true;
							break;
					}
				}
			}
          
			// Comparing cursors in UDC's would need the method or custom PL/SQL blocks that reference compareCursors$...
			if (v_NeedMethod == false) {
				for (int i = 0; i < prvScenarioGroup.udcCollection.Count; i++) {
					if (prvScenarioGroup.udcCollection[i].checkType == udc.enumCheckTypes.COMPARE_CURSORS
						|| prvScenarioGroup.udcCollection[i].plsqlBlock.ToUpper().Contains("COMPARECURSORS$")) {
						v_NeedMethod = true;
						break;
					}
				}
			}
          
			if (v_NeedMethod) {
				prvPLSQLBlock +=
					indent() + "PROCEDURE compareCursors$(p_ExpectedResults IN OUT NOCOPY SYS_REFCURSOR, p_ActualResults IN OUT NOCOPY SYS_REFCURSOR, p_CSVExcludedColumns VARCHAR2 := NULL) AS" + Environment.NewLine +
					indent(1) + "v_ExpectedCurID          NUMBER;" + Environment.NewLine +
					indent(1) + "v_ExpectedDescTab        dbms_sql.desc_tab3;" + Environment.NewLine +
					indent(1) + "v_ExpectedColumnCount    NUMBER;" + Environment.NewLine +
					indent(1) + "v_ExpectedNumRowsFetched PLS_INTEGER;" + Environment.NewLine +
					indent(1) + "v_ActualCurID            NUMBER;" + Environment.NewLine +
					indent(1) + "v_ActualDescTab          dbms_sql.desc_tab3;" + Environment.NewLine +
					indent(1) + "v_ActualColumnCount      NUMBER;" + Environment.NewLine +
					indent(1) + "v_ActualNumRowsFetched   PLS_INTEGER;" + Environment.NewLine +
					indent(1) + "v_CurrRowNum             PLS_INTEGER := 0;" + Environment.NewLine +
					Environment.NewLine +
					indent(1) + "-- Variables for holding retrieved column values " + Environment.NewLine +
					indent(1) + "v_ExpectedCharacterVar    VARCHAR2(4000); " + Environment.NewLine +
					indent(1) + "v_ExpectedClob            CLOB;" + Environment.NewLine +
					indent(1) + "v_ExpectedNumericVar      NUMBER;" + Environment.NewLine +
					indent(1) + "v_ExpectedDateVar         DATE;" + Environment.NewLine +
					indent(1) + "v_ExpectedTimestamp       TIMESTAMP;" + Environment.NewLine +
					indent(1) + "v_ExpectedTimestampWithTZ TIMESTAMP WITH TIME ZONE;" + Environment.NewLine +
					Environment.NewLine +
					indent(1) + "v_ActualCharacterVar    VARCHAR2(4000);" + Environment.NewLine +
					indent(1) + "v_ActualClob            CLOB;" + Environment.NewLine +
					indent(1) + "v_ActualNumericVar      NUMBER;" + Environment.NewLine +
					indent(1) + "v_ActualDateVar         DATE;" + Environment.NewLine +
					indent(1) + "v_ActualTimestamp       TIMESTAMP;" + Environment.NewLine +
					indent(1) + "v_ActualTimestampWithTZ TIMESTAMP WITH TIME ZONE;" + Environment.NewLine +
					Environment.NewLine +
					indent(1) + "PROCEDURE defineColumns(" + Environment.NewLine +
					indent(1) + "  p_CursorID      NUMBER," + Environment.NewLine +
					indent(1) + "  p_DescribeTable dbms_sql.desc_tab3)" + Environment.NewLine +
					indent(1) + "AS " + Environment.NewLine +
					indent(1) + "BEGIN " + Environment.NewLine +
					indent(2) +		"FOR v_i IN p_DescribeTable.FIRST .. p_DescribeTable.LAST LOOP " + Environment.NewLine +
					indent(3) +			"CASE p_DescribeTable(v_i).col_type " + Environment.NewLine +
					indent(3) +			"WHEN DBMS_TYPES.TYPECODE_CLOB THEN " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedClob); " + Environment.NewLine +
					indent(3) +			"WHEN DBMS_TYPES.TYPECODE_NUMBER THEN " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedNumericVar); " + Environment.NewLine +
					indent(3) +			"WHEN DBMS_TYPES.TYPECODE_DATE THEN " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedDateVar); " + Environment.NewLine +
					indent(3) +			"WHEN DBMS_TYPES.TYPECODE_TIMESTAMP THEN " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedTimestamp); " + Environment.NewLine +
					indent(3) +			"WHEN 180 THEN -- Timestamp" + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedTimestamp); " + Environment.NewLine +
					indent(3) +			"WHEN 181 THEN -- Timestamp with Time Zone " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedTimestampWithTZ); " + Environment.NewLine +
					indent(3) +			"WHEN DBMS_TYPES.TYPECODE_VARCHAR2 THEN " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedCharacterVar, 4000); " + Environment.NewLine +
					indent(3) +			"WHEN DBMS_TYPES.TYPECODE_CHAR THEN " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedCharacterVar, 4000); " + Environment.NewLine +
					indent(3) +			"WHEN DBMS_TYPES.TYPECODE_VARCHAR THEN " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedCharacterVar, 4000); " + Environment.NewLine +
					indent(3) +			"WHEN DBMS_TYPES.TYPECODE_VARCHAR2 THEN " + Environment.NewLine +
					indent(3) +			"  dbms_sql.define_column(p_CursorID, v_i, v_ExpectedCharacterVar, 4000); " + Environment.NewLine +
					indent(3) +			"ELSE " + Environment.NewLine +
					indent(3) +			"  RAISE_APPLICATION_ERROR(-20000, p_DescribeTable(v_i).col_type || ' is not a supported data type for a column!'); " + Environment.NewLine +
					indent(3) +			"END CASE; " + Environment.NewLine +
					indent(2) +		"END LOOP; " + Environment.NewLine +
					indent(1) + "END; " + Environment.NewLine +
					indent() + "BEGIN " + Environment.NewLine +
					indent() + "  v_ExpectedCurID := dbms_sql.to_cursor_number(p_ExpectedResults); " + Environment.NewLine +
					indent() + "  v_ActualCurID   := dbms_sql.to_cursor_number(p_ActualResults); " + Environment.NewLine +
					Environment.NewLine +
					indent() + "  dbms_sql.describe_columns3(v_ExpectedCurID, v_ExpectedColumnCount, v_ExpectedDescTab); " + Environment.NewLine +
					indent() + "  dbms_sql.describe_columns3(v_ActualCurID, v_ActualColumnCount, v_ActualDescTab); " + Environment.NewLine +
					Environment.NewLine +
					indent() + "  IF v_ExpectedDescTab.COUNT != v_ActualDescTab.COUNT THEN" + Environment.NewLine +
					indent() + "    RAISE_APPLICATION_ERROR(-20000, 'Column Number Mismatch: The expected result set has ' || v_ExpectedDescTab.COUNT || ' columns, but the actual result set has ' || v_ActualDescTab.COUNT || '!');" + Environment.NewLine +
					indent() + "  END IF;" + Environment.NewLine +
					Environment.NewLine +
					indent() + "  -- Define columns in the expected cursor ID " + Environment.NewLine +
					indent() + "  defineColumns(p_CursorID => v_ExpectedCurID, p_DescribeTable => v_ExpectedDescTab); " + Environment.NewLine +
					indent() + "  defineColumns(p_CursorID => v_ActualCurID, p_DescribeTable => v_ActualDescTab); " + Environment.NewLine +
					Environment.NewLine +              
					indent() + "  -- Fetch Rows and compare columns " + Environment.NewLine +
					indent() + "  LOOP " + Environment.NewLine +
					indent() + "    v_ExpectedNumRowsFetched := dbms_sql.fetch_rows(v_ExpectedCurID); " + Environment.NewLine +
					indent() + "    v_ActualNumRowsFetched   := dbms_sql.fetch_rows(v_ActualCurID); " + Environment.NewLine +
					Environment.NewLine +
					indent() + "    IF v_ExpectedNumRowsFetched < v_ActualNumRowsFetched THEN " + Environment.NewLine +
					indent() + "      RAISE_APPLICATION_ERROR(-20000, 'Got more rows back than expected.'); " + Environment.NewLine +
					indent() + "    ELSIF v_ExpectedNumRowsFetched > v_ActualNumRowsFetched THEN " + Environment.NewLine +
					indent() + "      RAISE_APPLICATION_ERROR(-20000, 'Got fewer rows back than expected.'); " + Environment.NewLine +
					indent() + "    ELSIF v_ExpectedNumRowsFetched = v_ActualNumRowsFetched AND v_ExpectedNumRowsFetched = 0 THEN " + Environment.NewLine +
					indent() + "      EXIT; " + Environment.NewLine +
					indent() + "    END IF; " + Environment.NewLine +
					Environment.NewLine +
					indent() + "    v_CurrRowNum := v_CurrRowNum + 1; " + Environment.NewLine +
					Environment.NewLine +
					indent() + "    FOR v_i IN 1 .. v_ExpectedColumnCount LOOP" + Environment.NewLine +
					indent() + "      IF INSTR(',' || p_CSVExcludedColumns || ',', ',' || v_ExpectedDescTab(v_i).col_name || ',') = 0 THEN" + Environment.NewLine +
					indent() + "        IF v_ExpectedDescTab(v_i).col_type IN (DBMS_TYPES.TYPECODE_VARCHAR2, DBMS_TYPES.TYPECODE_VARCHAR, DBMS_TYPES.TYPECODE_CHAR) THEN " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ExpectedCurID, v_i, v_ExpectedCharacterVar); " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ActualCurID, v_i, v_ActualCharacterVar); " + Environment.NewLine +
					Environment.NewLine +
					indent() + "          dbms_output.put_line('Exp: ' || v_ExpectedCharacterVar || ' vs. Actual: ' || v_ActualCharacterVar); " + Environment.NewLine +
					indent() + "          IF (v_ExpectedCharacterVar IS NULL AND v_ActualCharacterVar IS NOT NULL) OR (v_ExpectedCharacterVar IS NOT NULL AND v_ActualCharacterVar IS NULL) OR (v_ExpectedCharacterVar != v_ActualCharacterVar) THEN " + Environment.NewLine +
					indent() + "            RAISE_APPLICATION_ERROR(-20000, 'Row: ' || v_CurrRowNum || ', Column ' || v_ExpectedDescTab(v_i).col_name || ': exp: ' || v_ExpectedCharacterVar || ' vs. actual: ' || v_ActualCharacterVar || '.');" + Environment.NewLine +
					indent() + "          END IF; " + Environment.NewLine +
					indent() + "        ELSIF v_ExpectedDescTab(v_i).col_type = DBMS_TYPES.TYPECODE_CLOB THEN " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ExpectedCurID, v_i, v_ExpectedClob); " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ActualCurID, v_i, v_ActualClob); " + Environment.NewLine +
					indent() + "          dbms_output.put_line('Exp: ' || v_ExpectedClob || ' vs. Actual: ' || v_ActualClob); " + Environment.NewLine +
					Environment.NewLine +
					indent() + "          IF (v_ExpectedClob IS NULL AND v_ActualClob IS NOT NULL) OR (v_ExpectedClob IS NOT NULL AND v_ActualClob IS NULL) OR (v_ExpectedClob != v_ActualClob) THEN " + Environment.NewLine +
					indent() + "            RAISE_APPLICATION_ERROR(-20000, 'Row: ' || v_CurrRowNum || ', Column ' || v_ExpectedDescTab(v_i).col_name || ': exp: ' || v_ExpectedClob || ' vs. actual: ' || v_ActualClob || '.'); " + Environment.NewLine +
					indent() + "          END IF; " + Environment.NewLine +
					indent() + "        ELSIF v_ExpectedDescTab(v_i).col_type = DBMS_TYPES.TYPECODE_NUMBER THEN " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ExpectedCurID, v_i, v_ExpectedNumericVar); " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ActualCurID, v_i, v_ActualNumericVar); " + Environment.NewLine +
					indent() + "          dbms_output.put_line('Exp: ' || v_ExpectedNumericVar || ' vs. Actual: ' || v_ActualNumericVar); " + Environment.NewLine +
					indent() + "          IF (v_ExpectedNumericVar IS NULL AND v_ActualNumericVar IS NOT NULL) OR (v_ExpectedNumericVar IS NOT NULL AND v_ActualNumericVar IS NULL) OR (v_ExpectedNumericVar != v_ActualNumericVar) THEN " + Environment.NewLine +
					indent() + "            RAISE_APPLICATION_ERROR(-20000, 'Row: ' || v_CurrRowNum || ', Column ' || v_ExpectedDescTab(v_i).col_name || ' exp: ' || v_ExpectedNumericVar || ' vs. actual: ' || v_ActualNumericVar || '.');" + Environment.NewLine +
					indent() + "          END IF; " + Environment.NewLine +
					indent() + "        ELSIF v_ExpectedDescTab(v_i).col_type = DBMS_TYPES.TYPECODE_DATE THEN " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ExpectedCurID, v_i, v_ExpectedDateVar); " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ActualCurID, v_i, v_ActualDateVar); " + Environment.NewLine +
					indent() + "          dbms_output.put_line('Exp: ' || v_ExpectedDateVar || ' vs. Actual: ' || v_ActualDateVar); " + Environment.NewLine +
					Environment.NewLine +
					indent() + "          IF (v_ExpectedDateVar IS NULL AND v_ActualDateVar IS NOT NULL) OR (v_ExpectedDateVar IS NOT NULL AND v_ActualDateVar IS NULL) OR (v_ExpectedDateVar != v_ActualDateVar) THEN " + Environment.NewLine +
					indent() + "            RAISE_APPLICATION_ERROR(-20000, 'Row: ' || v_CurrRowNum || ', Column ' || v_ExpectedDescTab(v_i).col_name || ': exp: ' || v_ExpectedDateVar || ' vs. actual: ' || v_ActualDateVar || '.'); " + Environment.NewLine +
					indent() + "          END IF; " + Environment.NewLine +
					indent() + "        ELSIF v_ExpectedDescTab(v_i).col_type IN (180, DBMS_TYPES.TYPECODE_TIMESTAMP) THEN " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ExpectedCurID, v_i, v_ExpectedTimestamp); " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ActualCurID, v_i, v_ActualTimestamp); " + Environment.NewLine +
					indent() + "          dbms_output.put_line('Exp: ' || v_ExpectedTimestamp || ' vs. Actual: ' || v_ActualTimestamp); " + Environment.NewLine +
					indent() + "          IF (v_ExpectedTimestamp IS NULL AND v_ActualTimestamp IS NOT NULL) OR (v_ExpectedTimestamp IS NOT NULL AND v_ActualTimestamp IS NULL) OR (v_ExpectedTimestamp != v_ActualTimestamp) THEN " + Environment.NewLine +
					indent() + "            RAISE_APPLICATION_ERROR(-20000, 'Row: ' || v_CurrRowNum || ', Column ' || v_ExpectedDescTab(v_i).col_name || ': exp: ' || v_ExpectedTimestamp || ' vs. actual: ' || v_ActualTimestamp || '.'); " + Environment.NewLine +
					indent() + "          END IF; " + Environment.NewLine +
					indent() + "        ELSIF v_ExpectedDescTab(v_i).col_type = 181 THEN -- Timestamp with Time Zone " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ExpectedCurID, v_i, v_ExpectedTimestampWithTZ); " + Environment.NewLine +
					indent() + "          dbms_sql.column_value(v_ActualCurID, v_i, v_ActualTimestampWithTZ); " + Environment.NewLine +
					indent() + "          dbms_output.put_line('Exp: ' || v_ExpectedTimestampWithTZ || ' vs. Actual: ' || v_ActualTimestampWithTZ); " + Environment.NewLine +
					indent() + "          IF (v_ExpectedTimestampWithTZ IS NULL AND v_ActualTimestampWithTZ IS NOT NULL) OR (v_ExpectedTimestampWithTZ IS NOT NULL AND v_ActualTimestampWithTZ IS NULL) OR (v_ExpectedTimestampWithTZ != v_ActualTimestampWithTZ) THEN " + Environment.NewLine +
					indent() + "            RAISE_APPLICATION_ERROR(-20000, 'Row: ' || v_CurrRowNum || ', Column ' || v_ExpectedDescTab(v_i).col_name || ': exp: ' || v_ExpectedTimestampWithTZ || ' vs. actual: ' || v_ActualTimestampWithTZ || '.'); " + Environment.NewLine +
					indent() + "          END IF; " + Environment.NewLine +
					indent() + "        ELSE " + Environment.NewLine +
					indent() + "          RAISE_APPLICATION_ERROR(-20001, v_ExpectedDescTab(v_i).col_type || ' is not a supported data type!'); " + Environment.NewLine +
					indent() + "        END IF; " + Environment.NewLine +
					indent() + "      END IF; " + Environment.NewLine +
					indent() + "    END LOOP; " + Environment.NewLine +
					indent() + "  END LOOP; " + Environment.NewLine +
					Environment.NewLine +
					indent() + "  dbms_sql.close_cursor(v_ExpectedCurID); " + Environment.NewLine +
					indent() + "  dbms_sql.close_cursor(v_ActualCurID); " + Environment.NewLine +
					indent() + "EXCEPTION " + Environment.NewLine +
					indent() + "  WHEN OTHERS THEN " + Environment.NewLine +
					indent() + "    IF dbms_sql.is_open(v_ExpectedCurID) THEN " + Environment.NewLine +
					indent() + "      dbms_sql.close_cursor(v_ExpectedCurID); " + Environment.NewLine +
					indent() + "    END IF; " + Environment.NewLine +
					Environment.NewLine +
					indent() + "    IF dbms_sql.is_open(v_ActualCurID) THEN " + Environment.NewLine +
					indent() + "      dbms_sql.close_cursor(v_ActualCurID); " + Environment.NewLine +
					indent() + "    END IF; " + Environment.NewLine +
					Environment.NewLine +
					indent() + "    RAISE; " + Environment.NewLine +
					indent() + "END;" + Environment.NewLine +
					Environment.NewLine;
			}
		}

		private void declareLFToCRLF() {
			prvPLSQLBlock +=
				indent() + "FUNCTION lfToCRLF(p_String CLOB) RETURN CLOB AS" + Environment.NewLine +
				indent() + "BEGIN" + Environment.NewLine +
				indent(1) + "RETURN REPLACE(p_String, c_LF, c_CRLF);" + Environment.NewLine +
				indent() + "END;" + Environment.NewLine +
				Environment.NewLine;
		}

		private void declareLogStatus() {
			prvPLSQLBlock +=
				indent() + "PROCEDURE logStatus$(p_ScenarioIndex PLS_INTEGER, p_Status VARCHAR2, p_ErrorNumber NUMBER, p_ErrorMessage CLOB) AS" + Environment.NewLine +
				indent() + "BEGIN" + Environment.NewLine;
        
			if (prvGenerateAsDebugBlock) {
				prvPLSQLBlock +=
					indent(1) + "IF p_Status != 'OK' THEN" + Environment.NewLine +
					indent(2) +  "RAISE_APPLICATION_ERROR(-20911, p_ErrorNumber || '-' || p_ErrorMessage);" + Environment.NewLine +
					indent(1) + "END IF;" + Environment.NewLine +
					Environment.NewLine + // Pad the debug out to the same number of lines as the executable block,
					Environment.NewLine + // so error line numbers match between the two block styles.
					Environment.NewLine +
					Environment.NewLine +
					Environment.NewLine;
			} else {
				prvPLSQLBlock +=
					indent(1) + c_ARR_STATUSES_VAR_NAME + "(p_ScenarioIndex) := p_Status;" + Environment.NewLine +
					indent(1) + c_ARR_ERROR_NUMBERS_VAR_NAME + "(p_ScenarioIndex) := p_ErrorNumber;" + Environment.NewLine +
					Environment.NewLine +
					indent(1) + "IF " + c_ARR_ERROR_MSGS_VAR_NAME + "(p_ScenarioIndex) IS NULL THEN" + Environment.NewLine +
					indent(2) +   c_ARR_ERROR_MSGS_VAR_NAME + "(p_ScenarioIndex) := SUBSTR(RTRIM(p_ErrorMessage, c_CRLF), 1, 32767);" + Environment.NewLine +
					indent(1) + "ELSE" + Environment.NewLine +
					indent(2) +		c_ARR_ERROR_MSGS_VAR_NAME + "(p_ScenarioIndex) := SUBSTR(" + c_ARR_ERROR_MSGS_VAR_NAME + "(p_ScenarioIndex) || c_CRLF || RTRIM(p_ErrorMessage, c_CRLF), 1, 32767);" + Environment.NewLine +
					indent(1) + "END IF;" + Environment.NewLine;
			}
          
			prvPLSQLBlock +=
				indent() + "END;" + Environment.NewLine +
				Environment.NewLine;
		}

		private void declareResetParamVariables() {
			bool v_FoundParam = false;

			prvPLSQLBlock +=
				indent() + "PROCEDURE resetParamVariables AS" + Environment.NewLine +
				indent() + "BEGIN" + Environment.NewLine;
            
			for (int i = 0; i < prvTest.testArguments.Count; i++) {
				// For default or return plsqlValue parameters, reset them to NULL's so that they don't
				// retain their plsqlValue from a previous scenario.
				if (prvTest.testArguments[i].plsType == "VARCHAR2"
					|| prvTest.testArguments[i].plsType == "CLOB"
					|| prvTest.testArguments[i].plsType == "CHAR"
					|| prvTest.testArguments[i].plsType == "VARCHAR"
					|| prvTest.testArguments[i].plsType == "NCHAR"
					|| prvTest.testArguments[i].plsType == "NVARCHAR2"
					|| prvTest.testArguments[i].plsType == "LONG"
					|| prvTest.testArguments[i].plsType == "RAW"
					|| prvTest.testArguments[i].plsType == "LONG RAW"
					|| prvTest.testArguments[i].plsType == "NCLOB"
					|| prvTest.testArguments[i].plsType == "DATE"
					|| prvTest.testArguments[i].plsType == "PLS_INTEGER"
					|| prvTest.testArguments[i].plsType == "NUMBER"
					|| prvTest.testArguments[i].plsType == "INTEGER") {
					prvPLSQLBlock +=
						indent(1) + prvTest.testArguments[i].argumentName + " := NULL;" + Environment.NewLine;
              
					v_FoundParam = true;
				}
			}
          
			if (v_FoundParam == false) {
				prvPLSQLBlock +=
					indent(1) + "NULL;" + Environment.NewLine;
			}
          
			prvPLSQLBlock += 
				indent() + "END;" + Environment.NewLine +
				Environment.NewLine;
		}

		private void declareSetDefaultedParams() {
			prvPLSQLBlock +=
				indent() + "PROCEDURE setDefaultedParams(p_CSVParamNames VARCHAR2) AS" + Environment.NewLine +
				indent(1) + "c_CSV_DELIMITER CONSTANT VARCHAR2(10) := ',';" + Environment.NewLine +
				indent(1) + "v_Start PLS_INTEGER := 1;" + Environment.NewLine +
				indent(1) + "v_End   PLS_INTEGER;" + Environment.NewLine +
				indent() + "BEGIN" + Environment.NewLine +
				indent(1) + "IF p_CSVParamNames IS NOT NULL THEN" + Environment.NewLine +
				indent(2) +   "LOOP" + Environment.NewLine +
				indent(3) +      "v_End := INSTR(p_CSVParamNames || c_CSV_DELIMITER, c_CSV_DELIMITER, v_Start);" + Environment.NewLine +
				indent(3) +      "EXIT WHEN v_End = 0;" + Environment.NewLine +
				Environment.NewLine +
				indent(3) +      "v_arrDefaulted(SUBSTR(p_CSVParamNames, v_Start, v_End - v_Start)) := TRUE;" + Environment.NewLine +
				indent(3) +      "v_Start := v_End + LENGTH(c_CSV_DELIMITER);" + Environment.NewLine +
				indent(2) +   "END LOOP;" + Environment.NewLine +
				indent(1) + "END IF;" + Environment.NewLine +
				indent() + "END;" + Environment.NewLine +
				Environment.NewLine;
		}
		
		private void declareTestLevelSubProcedures() {		
			// Utility methods
			declareBoolToChar();
        
			declareCompareCursors();
        
			declareLFToCRLF();
        
			declareLogStatus();
                
			if (prvTest.testArguments.Count > 0) {
				declareResetParamVariables();
				          
				declareSetDefaultedParams();
			}
		}
		#endregion

		#region Header: Nested Scenario Group Subprocedures
		private void declareScenarioGroupDeclare() {
			if (prvScenarioGroup.scenarioGroupDeclare != String.Empty) {
				prvPLSQLBlock +=
					indent() + "-- User-defined scenario group declare..." + Environment.NewLine +
					indent() + prvScenarioGroup.scenarioGroupDeclare + Environment.NewLine +
					Environment.NewLine;
			}
		}
        
		// Internal scenario group startup
		private void declareScenarioGroupStartup() {
			testArgumentCollection allArgs = prvTest.getAllArgs();

			prvPLSQLBlock +=
				indent() + "PROCEDURE scenarioGroupStartup$ AS" + Environment.NewLine +
				indent() + "BEGIN" + Environment.NewLine +
				indent(1) + "NULL;" + Environment.NewLine;

			// List the out-bound variables that contain the expected and actual values of any out/return parameters.
			// They must be enumerated; otherwise, when c# binds them in, they might not be referenced anywhere else
			// in the block.
			for (int i = 0; i < prvScenarioGroup.scenarios.Count(); i++) {
				for (int j = 0; j < allArgs.Count; j++) {
					if (allArgs[j].inOut == "IN/OUT" || allArgs[j].inOut == "OUT" || allArgs[j].inOut == "RETURN") {
						prvPLSQLBlock +=
							indent(1) + c_ARR_OUT_PRM_EXP_VAL_VAR_NAME + i.ToString() + "_" + allArgs[j].sequence.ToString() + " := NULL; -- " + allArgs[j].argumentName + Environment.NewLine +
							indent(1) + c_ARR_OUT_PRM_ACT_VAL_VAR_NAME + i.ToString() + "_" + allArgs[j].sequence.ToString() + " := NULL; -- " + allArgs[j].argumentName + Environment.NewLine;
					}
				}

				for (int j = 0; j < prvScenarioGroup.udcCollection.Count; j++) {
					if (prvScenarioGroup.udcCollection[j].checkType == udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX) {
						prvPLSQLBlock +=
							indent(1) + c_ARR_OUT_UDC_EXP_VAL_VAR_NAME + j.ToString() + "_" + i.ToString() + " := NULL; -- " + prvScenarioGroup.udcCollection[j].name + Environment.NewLine +
							indent(1) + c_ARR_OUT_UDC_ACT_VAL_VAR_NAME + j.ToString() + "_" + i.ToString() + " := NULL; -- " + prvScenarioGroup.udcCollection[j].name + Environment.NewLine;
					}
				}
			}
           
			// Initialize the array of scenario guids
			for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
				prvPLSQLBlock +=
					indent(1) + "v_arrGUIDs$(" + (i+1).ToString() + ") := '" + prvScenarioGroup.scenarios[i].guid + "';" + Environment.NewLine;
			}

			if (prvTest.testArguments.Count > 0) {
				prvPLSQLBlock += 
					Environment.NewLine +
					indent(1) + "-- Initialize the array of default parameter boolean indicators" + Environment.NewLine;
              
				for (int v_Param = 0; v_Param < prvTest.testArguments.Count; v_Param++) {
					if (v_Param != v_ReturnValueParamIndex) {
						prvPLSQLBlock += 
							indent(1) + "v_arrDefaulted('" + prvTest.testArguments[v_Param].argumentName + "') := FALSE;" + Environment.NewLine;
					}
				}
			}
  
			prvPLSQLBlock += 
				indent() + "END;" + Environment.NewLine +
				Environment.NewLine;
		}

		private string getRowValidationPLSQL(udc c_CHECK_REC) {
			string v_PLSQL = String.Empty;
			 
			foreach (rowValidatorCheck v_Field in c_CHECK_REC.fieldValidations) {
				switch (v_Field.comparisonType) {
					case "Don't Test":
						break;
					case "Input Parameter":
					case "Exp":
						v_PLSQL +=
							indent(3) + "IF NOT NVL((v_ActualUDCRow." + v_Field.fieldName + " IS NULL AND v_ExpectedUDCRow." + v_Field.fieldName + " IS NULL) OR (v_ActualUDCRow." + v_Field.fieldName + " = v_ExpectedUDCRow." + v_Field.fieldName + "), FALSE) THEN" + Environment.NewLine +
							indent(4) +		"logStatus$(p_ScenarioIndex$, 'Row Validator Failed', 0, 'UDC \"" + c_CHECK_REC.name.Replace("'", "''") + "\", Scenario #' || p_ScenarioIndex$ || ': Column " + v_Field.fieldName + " failed: Expected ' || v_ExpectedUDCRow." + v_Field.fieldName + " || ', but got: \"' || v_ActualUDCRow." + v_Field.fieldName + " || '\" instead.');" + Environment.NewLine +
							indent(3) + "END IF;" + Environment.NewLine;
						break;
					case "IS NULL":
						v_PLSQL +=
							indent(3) + "IF v_ActualUDCRow." + v_Field.fieldName + " IS NOT NULL THEN" + Environment.NewLine +
							indent(4) + "logStatus$(p_ScenarioIndex$, 'Row Validator Failed', 0, 'UDC \"" + c_CHECK_REC.name.Replace("'", "''") + "\", Scenario #' || p_ScenarioIndex$ || ': Column " + v_Field.fieldName + " is supposed to be NULL, but it''s \"' || v_ActualUDCRow." + v_Field.fieldName + " || '\", instead.');" + Environment.NewLine +
							indent(3) + "END IF;" + Environment.NewLine;
						break;
					case "NOT NULL":
						v_PLSQL +=
							indent(3) + "IF v_ActualUDCRow." + v_Field.fieldName + " IS NULL THEN" + Environment.NewLine +
							indent(4) + "logStatus$(p_ScenarioIndex$, 'Row Validator Failed', 0, 'UDC \"" + c_CHECK_REC.name.Replace("'", "''") + "\", Scenario #' || p_ScenarioIndex$ || ': Column " + v_Field.fieldName + " is supposed to be not NULL, but it''s NULL, instead.');" + Environment.NewLine +
							indent(3) + "END IF;" + Environment.NewLine;
						break;
					case "Value":
						v_PLSQL +=
							indent(3) + "IF NOT NVL((v_ActualUDCRow." + v_Field.fieldName + " IS NULL AND v_ExpectedUDCRow." + v_Field.fieldName + " IS NULL) OR (v_ActualUDCRow." + v_Field.fieldName + " = v_ExpectedUDCRow." + v_Field.fieldName + "), FALSE) THEN" + Environment.NewLine +
							indent(4) + "logStatus$(p_ScenarioIndex$, 'Row Validator Failed', 0, 'UDC \"" + c_CHECK_REC.name.Replace("'", "''") + "\", Scenario  #' || p_ScenarioIndex$ || ': Column " + v_Field.fieldName + " failed: Expected ' || v_ExpectedUDCRow." + v_Field.fieldName + " || ', but got: ' || v_ActualUDCRow." + v_Field.fieldName + ");" + Environment.NewLine +
							indent(3) + "END IF;" + Environment.NewLine;
						break;
					case "PL/SQL Block":
						v_PLSQL +=
							indent(3) + v_Field.fieldValue + Environment.NewLine;
						break;
				}
			}
          
			return v_PLSQL;
		}

		private void buildCursorResultTest(int udcIndex, udc udc, bool p_ShouldReturnRow) {
			if (p_ShouldReturnRow) {
				prvPLSQLBlock +=
					indent() + "PROCEDURE cursorReturningRowsUDC" + udcIndex.ToString() + "$(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) IS" + Environment.NewLine;
			} else {
				prvPLSQLBlock +=
					indent() + "PROCEDURE cursorReturningNoRowsUDC" + udcIndex.ToString() + "$(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) IS" + Environment.NewLine;
			}
        
			prvPLSQLBlock +=
				indent() + "BEGIN" + Environment.NewLine +                
				indent(1) + "-- Cursor row existence check...." + Environment.NewLine +
				//indent(1) + "<<rt_row_existence_check_block>>" + Environment.NewLine +
				indent(1) + "DECLARE" + Environment.NewLine +
				indent(2) +   "CURSOR c_CursorRowTest IS" + Environment.NewLine +
				udc.rowExistenceCursor + Environment.NewLine +
				indent(2) +   "v_CursorRow c_CursorRowTest%ROWTYPE;" + Environment.NewLine +
				indent(2) +   "v_RowFound BOOLEAN;" + Environment.NewLine +
				indent(1) + "BEGIN" + Environment.NewLine +
				indent(2) +   "IF " + (udc.plsqlCondition == String.Empty ? "TRUE" : udc.plsqlCondition) + Environment.NewLine +
				indent(2) +		"THEN" + Environment.NewLine +
				indent(3) +     "OPEN c_CursorRowTest;" + Environment.NewLine +
				indent(3) +     "FETCH c_CursorRowTest INTO v_CursorRow;" + Environment.NewLine +
				indent(3) +     "v_RowFound := c_CursorRowTest%FOUND;" + Environment.NewLine +
				indent(3) +     "CLOSE c_CursorRowTest;" + Environment.NewLine;
          
			if (p_ShouldReturnRow) {
				prvPLSQLBlock += 
					indent(3) +   "IF NOT v_RowFound THEN" + Environment.NewLine +              
					indent(4) +     "logStatus$(p_ScenarioIndex$, 'UDC Failed', 0, 'UDC \"" + udc.name.Replace("'", "''") + "\" did not return a row but was supposed to.');" + Environment.NewLine +
					indent(3) +   "END IF;" + Environment.NewLine;
			} else {
				prvPLSQLBlock += 
					indent(3) +   "IF v_RowFound THEN" + Environment.NewLine +
					indent(4) +			"logStatus$(p_ScenarioIndex$, 'UDC Failed', 0, 'UDC \"" + udc.name.Replace("'", "''") + "\" returned at least one row but was not supposed to return any.');" + Environment.NewLine +
					indent(3) +   "END IF;" + Environment.NewLine;
			}
          
			prvPLSQLBlock += 
				indent(2) +     "END IF;" + Environment.NewLine +
				indent(1) +   "END;" + Environment.NewLine +
				indent() + "END;" + Environment.NewLine + Environment.NewLine;
		}

		private void declareCheckMatchSubProc() {
			// Declare the checkMatch procedure only once....
			if (checkMatchDeclared == false) {
				prvPLSQLBlock +=
					indent() + "PROCEDURE checkMatch(p_ScenarioIndex$ PLS_INTEGER, p_CallIdentifier VARCHAR2, p_ActualValue VARCHAR2, p_ComparisonType VARCHAR2, p_ExpectedValue VARCHAR2) AS" + Environment.NewLine +
					indent(1) +	"v_Failed BOOLEAN := FALSE;" + Environment.NewLine +
					indent() + "BEGIN" + Environment.NewLine +
					indent(1) +    "IF p_ComparisonType = 'is null' AND p_ActualValue IS NOT NULL THEN" + Environment.NewLine +
					indent(3) +			"v_Failed := TRUE;" + Environment.NewLine +
					indent(1) +    "ELSIF p_ComparisonType = 'not null' AND p_ActualValue IS NULL THEN" + Environment.NewLine +
					indent(3) +			"v_Failed := TRUE;" + Environment.NewLine +
					indent(1) +    "ELSIF p_ComparisonType IN ('value', 'exp') THEN" + Environment.NewLine +
					indent(2) +      "IF p_ActualValue != p_ExpectedValue OR (p_ActualValue IS NULL AND p_ExpectedValue IS NOT NULL) OR (p_ExpectedValue IS NULL AND p_ActualValue IS NOT NULL) THEN" + Environment.NewLine +
					indent(3) +				"v_Failed := TRUE;" + Environment.NewLine +
					indent(2) +      "END IF;" + Environment.NewLine +
					indent(1) +   "END IF;" + Environment.NewLine +
					Environment.NewLine +
					indent(1) +	"IF v_Failed THEN" + Environment.NewLine +
					indent(2) +		"logStatus$(p_ScenarioIndex$, 'Ret Val Prob', 0, " +
											"'Expected vs. actual difference: www.diffme.com?guid=' || v_arrGUIDs$(p_ScenarioIndex$) || '_' || p_CallIdentifier);" + Environment.NewLine +
					indent(1) + "END IF;" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine +
					Environment.NewLine;

				checkMatchDeclared = true;
			}
		}

		private void declareMatrixComparisonSubProc(string callIdentifier, DataTable expectedDataTable, string runCondition = "") {			
			int expectedNumRows = expectedDataTable.Rows.Count;

			declareCheckMatchSubProc();

			prvPLSQLBlock +=
				indent() + "-- Comparing an actual sys ref cursor against an expected matrix..." + Environment.NewLine +
				indent() + "PROCEDURE cmpVsMtx" + callIdentifier + "$(p_ScenarioIndex$ PLS_INTEGER, p_ActualResults SYS_REFCURSOR, p_OutBuiltExpResults OUT NOCOPY CLOB, p_OutBuiltActResults OUT NOCOPY CLOB) AS" + Environment.NewLine;

			if (expectedDataTable.Columns.Count > 0) {
				// Declare a row type to fetch the actual cursor into...
				prvPLSQLBlock +=
					indent(1) + "TYPE t_ActualResult$ IS RECORD (" + Environment.NewLine;

				for (int cursorColumnIndex = 0; cursorColumnIndex < expectedDataTable.Columns.Count; cursorColumnIndex += 2) {
					prvPLSQLBlock +=
						indent(2) + expectedDataTable.Columns[cursorColumnIndex].ColumnName + " VARCHAR2(32767)";

					if (cursorColumnIndex < expectedDataTable.Columns.Count - 3) {
						prvPLSQLBlock += "," + Environment.NewLine;
					}
				}

				prvPLSQLBlock +=
					Environment.NewLine +
					indent(1) + ");" + Environment.NewLine +
					Environment.NewLine;

				prvPLSQLBlock +=
					indent(1) + "TYPE t_arrActualResultRows$ IS TABLE OF t_ActualResult$ INDEX BY PLS_INTEGER;" + Environment.NewLine +
					indent(1) + "v_arrActualResultRows$   t_arrActualResultRows$;" + Environment.NewLine +
					indent(1) + "v_arrExpectedResultRows$ t_arrActualResultRows$;" + Environment.NewLine +
					Environment.NewLine +
					indent(1) + "TYPE t_arrColumnComparisonTypes$ IS TABLE OF VARCHAR2(20) INDEX BY PLS_INTEGER;" + Environment.NewLine +
					indent(1) + "TYPE t_arrComparisonTypes$ IS TABLE OF t_arrColumnComparisonTypes$ INDEX BY PLS_INTEGER;" + Environment.NewLine +
					indent(1) + "v_arrComparisonTypes$ t_arrComparisonTypes$;" + Environment.NewLine +
					Environment.NewLine;

				prvPLSQLBlock +=
					indent(1) + "PROCEDURE concatRowToCLOB(p_ResultRow t_ActualResult$, p_arrColumnComparisonTypes t_arrColumnComparisonTypes$, p_CLOB IN OUT NOCOPY CLOB) AS" + Environment.NewLine +
					indent(1) + "BEGIN" + Environment.NewLine;						

				int paramIndex = 0;
				for (int currColumnIndex = 0; currColumnIndex < expectedDataTable.Columns.Count; currColumnIndex += 2) {
					string columnName = expectedDataTable.Columns[currColumnIndex].ColumnName;

					prvPLSQLBlock +=
						indent(2) + "IF p_arrColumnComparisonTypes.COUNT = 0 OR p_arrColumnComparisonTypes(" + (paramIndex++) + ") != 'don''t test' THEN" + Environment.NewLine +
						indent(3) +		"p_CLOB := p_CLOB || p_ResultRow." + columnName + ";" + Environment.NewLine +
						indent(2) + "END IF;" + Environment.NewLine;

					if (currColumnIndex < expectedDataTable.Columns.Count - 2) {
						prvPLSQLBlock +=
							indent(2) + "p_CLOB := p_CLOB || c_TAB;" + Environment.NewLine;
					}
				}

				prvPLSQLBlock +=
					indent(2) +		"p_CLOB := p_CLOB || c_CRLF;" + Environment.NewLine +
					indent(1) + "END;" + Environment.NewLine +
					Environment.NewLine +
					indent(1) + "PROCEDURE concatRowToCLOB(p_ResultRow t_ActualResult$, p_CLOB IN OUT NOCOPY CLOB) AS" + Environment.NewLine +
					indent(2) +		"v_arrColumnComparisonTypes t_arrColumnComparisonTypes$;" + Environment.NewLine +
					indent(1) + "BEGIN" + Environment.NewLine +
					indent(2) +		"concatRowToCLOB(p_ResultRow => p_ResultRow, p_arrColumnComparisonTypes => v_arrColumnComparisonTypes, p_CLOB => p_CLOB);" + Environment.NewLine +
					indent(1) + "END;" + Environment.NewLine +
					Environment.NewLine;

				prvPLSQLBlock +=
					indent() + "BEGIN" + Environment.NewLine;

				if (runCondition != "") {
					prvPLSQLBlock +=
						indent(1) + "IF " + runCondition + Environment.NewLine +
						indent(1) + "THEN" + Environment.NewLine; // This is on the next line, in case the PL/SQL condition ends in a comment					

					v_IndentionLvl++;
				}		

				prvPLSQLBlock +=
					indent(1) +		"FETCH p_ActualResults BULK COLLECT INTO v_arrActualResultRows$;" + Environment.NewLine +
					indent(1) +		"CLOSE p_ActualResults;" + Environment.NewLine +
					Environment.NewLine +
					indent(1) + "-- Default comparison types to 'value' to reduce the number of generated lines..." + Environment.NewLine +
					indent(1) + "FOR v_i IN 1 .. " + expectedNumRows + " LOOP" + Environment.NewLine +
					indent(2) +		"FOR v_j IN 0 .. " + (expectedDataTable.Columns.Count/2) + " LOOP" + Environment.NewLine +
					indent(3) +			"v_arrComparisonTypes$(v_i)(v_j) := 'value';" + Environment.NewLine +
					indent(2) +		"END LOOP;" + Environment.NewLine +
					indent(1) +	"END LOOP;" + Environment.NewLine +
					Environment.NewLine;

				StringBuilder sbExpectedMatrixPLSQL = new StringBuilder();

				// Enumerate the column names into the actual and expected OUT CLOBs...
				sbExpectedMatrixPLSQL
					.AppendLine()
					.Append(indent(1))
					.AppendLine("-- Concatenate the column headers into the actual and expected OUT CLOBs...");

				sbExpectedMatrixPLSQL
					.Append(indent(1))
					.Append("p_OutBuiltExpResults := ")							
					.AppendLine();

				for (int cursorColumnIndex = 0; cursorColumnIndex < expectedDataTable.Columns.Count; cursorColumnIndex += 2) {
					sbExpectedMatrixPLSQL
						.Append(indent(2))
						.Append("'")
						.Append(expectedDataTable.Columns[cursorColumnIndex].ColumnName)
						.Append("'");

					if (cursorColumnIndex < expectedDataTable.Columns.Count - 3) {
						sbExpectedMatrixPLSQL
							.AppendLine(" || c_TAB ||");
					} else {
						sbExpectedMatrixPLSQL
							.AppendLine(" || c_CRLF;");
					}
				}

				sbExpectedMatrixPLSQL
					.Append(indent(1))
					.AppendLine("p_OutBuiltActResults := p_OutBuiltExpResults;");

				// Put the expected matrix into the expected array...
				for (int currRowIndex = 0; currRowIndex < expectedNumRows; currRowIndex++) {
					int paramNumber = 0;

					sbExpectedMatrixPLSQL
						.Append(indent(1))
						.Append("-- Row #")
						.Append(currRowIndex + 1)
						.AppendLine();

					for (int currColumnIndex = 0; currColumnIndex < expectedDataTable.Columns.Count; currColumnIndex += 2) {
						string columnName = expectedDataTable.Columns[currColumnIndex].ColumnName;
						string comparisonType = expectedDataTable.Rows[currRowIndex][currColumnIndex + 1].ToString();

						if (comparisonType != "don't test") {
							sbExpectedMatrixPLSQL
								.Append(indent(1))
								.Append("v_arrExpectedResultRows$(")
								.Append((currRowIndex + 1))
								.Append(").")
								.Append(columnName)
								.Append(" := ");

							switch (comparisonType) {
								case "exp":
									sbExpectedMatrixPLSQL.Append(expectedDataTable.Rows[currRowIndex][currColumnIndex].ToString());
									break;
								case "value":
									sbExpectedMatrixPLSQL
										.Append("'")
										.Append(expectedDataTable.Rows[currRowIndex][currColumnIndex].ToString().Replace("'", "''"))
										.Append("'");
									break;
								case "is null":
								case "not null":
									sbExpectedMatrixPLSQL
										.Append(indent(1))
										.Append("NULL");
									break;
							}

							sbExpectedMatrixPLSQL.Append(";").AppendLine();
						}

						// For comparison types that aren't the default (plsqlValue), overlay the default with the non-default comparison type.
						if (comparisonType != "value") {
							sbExpectedMatrixPLSQL.Append(
								indent(1))
								.Append("v_arrComparisonTypes$(")
								.Append((currRowIndex + 1))
								.Append(")(")
								.Append(paramNumber)
								.Append(") := '")
								.Append(comparisonType.Replace("'", "''"))
								.Append("';")
								.AppendLine();
						}
								
						paramNumber++;
					}							
				}
						
				prvPLSQLBlock += sbExpectedMatrixPLSQL.ToString();

				// Compare the actual vs. expected plsqlValue arrays...
				prvPLSQLBlock +=
					indent(1) + "IF v_arrActualResultRows$.COUNT = v_arrExpectedResultRows$.COUNT THEN" + Environment.NewLine +
					indent(2) +		"FOR v_i IN v_arrActualResultRows$.FIRST .. v_arrActualResultRows$.LAST LOOP" + Environment.NewLine;

				if (expectedNumRows == 0) {
					prvPLSQLBlock +=
						indent(3) + "NULL;" + Environment.NewLine;
				} else {
					int columnIndex = 0;
					for (int currColumnIndex = 0; currColumnIndex < expectedDataTable.Columns.Count; currColumnIndex += 2) {
						string columnName = expectedDataTable.Columns[currColumnIndex].ColumnName;

						prvPLSQLBlock +=
							indent(3) + "checkMatch(" + Environment.NewLine +
							indent(4) +    "p_ScenarioIndex$ => p_ScenarioIndex$," + Environment.NewLine +
							indent(4) +		"p_CallIdentifier => '" + callIdentifier + "'," + Environment.NewLine +
							indent(4) +    "p_ActualValue => v_arrActualResultRows$(v_i)." + columnName + "," + Environment.NewLine +
							indent(4) +    "p_ComparisonType => v_arrComparisonTypes$(v_i)(" + (columnIndex++) +")," + Environment.NewLine +
							indent(4) +		"p_ExpectedValue => v_arrExpectedResultRows$(v_i)." + columnName + Environment.NewLine +
							indent(3) + ");" + Environment.NewLine +
							Environment.NewLine +
							indent(3) + "EXIT WHEN " + c_ARR_STATUSES_VAR_NAME + "(p_ScenarioIndex$) != 'OK';" + Environment.NewLine +
							Environment.NewLine;
					}
				}

				prvPLSQLBlock +=
					indent(2) +		"END LOOP;" + Environment.NewLine +
					indent(1) + "ELSE" + Environment.NewLine +
					indent(2) +		"logStatus$(p_ScenarioIndex$, 'Ret Val Prob', 0, " +
												"'Expected vs. actual difference: www.diffme.com?guid=' || v_arrGUIDs$(p_ScenarioIndex$) || '_" + callIdentifier + "');" + Environment.NewLine +								
					indent(1) + "END IF;" + Environment.NewLine +
					Environment.NewLine;

				// If the comparison failed, concatenate the actual and expected result arrays into the actual and expected OUT CLOB variables...
				prvPLSQLBlock +=
					indent(1) + "-- If there was a problem, dump the actual and expected result arrays into OUT CLOB variables..." + Environment.NewLine +
					indent(1) + "IF " + c_ARR_STATUSES_VAR_NAME + "(p_ScenarioIndex$) != 'OK' THEN" + Environment.NewLine +
					indent(2) +		"-- Concatenate expected results..." + Environment.NewLine +
					indent(2) +		"IF v_arrExpectedResultRows$.COUNT > 0 THEN" + Environment.NewLine +
					indent(3) +			"FOR v_i IN v_arrExpectedResultRows$.FIRST .. v_arrExpectedResultRows$.LAST LOOP" + Environment.NewLine +
					indent(4) +				"concatRowToCLOB(p_ResultRow => v_arrExpectedResultRows$(v_i), p_arrColumnComparisonTypes => v_arrComparisonTypes$(v_i), p_CLOB => p_OutBuiltExpResults);" + Environment.NewLine +
					indent(3) +			"END LOOP;" + Environment.NewLine +
					indent(2) +		"END IF;" + Environment.NewLine +
					Environment.NewLine +
					indent(2) +		"-- Concatenate actual results..." + Environment.NewLine +
					indent(2) +		"IF v_arrActualResultRows$.COUNT > 0 THEN" + Environment.NewLine +
					indent(3) +			"FOR v_i IN v_arrActualResultRows$.FIRST .. v_arrActualResultRows$.LAST LOOP" + Environment.NewLine +
					indent(4) +				"IF v_arrActualResultRows$.COUNT = v_arrExpectedResultRows$.COUNT THEN" + Environment.NewLine +
					indent(5) +					"-- If the arrays are the same size, use the comparison types from the expected values array, to get any don't-test-style comparison types..." + Environment.NewLine +
					indent(5) +					"concatRowToCLOB(p_ResultRow => v_arrActualResultRows$(v_i), p_arrColumnComparisonTypes => v_arrComparisonTypes$(v_i), p_CLOB => p_OutBuiltActResults);" + Environment.NewLine +
					indent(4) +				"ELSE" + Environment.NewLine +
					indent(5) +					"concatRowToCLOB(p_ResultRow => v_arrActualResultRows$(v_i), p_CLOB => p_OutBuiltActResults);" + Environment.NewLine +
					indent(4) +				"END IF;" + Environment.NewLine +
					indent(3) +			"END LOOP;" + Environment.NewLine +
					indent(2) +		"END IF;" + Environment.NewLine +
					indent(1) + "END IF;" + Environment.NewLine;

				if (runCondition != "") {
					v_IndentionLvl--;

					prvPLSQLBlock +=
						indent(1) + "END IF;" + Environment.NewLine;
				}
			} else {
				// The expected matrix has no columns, so we don't know what the actual matrix will look like - log it as an error.
				prvPLSQLBlock +=
					indent() + "BEGIN" + Environment.NewLine +
					indent(1) + "logStatus$(p_ScenarioIndex$, 'Ret Val Prob', 0, 'Expected matrix is not set up!');" + Environment.NewLine;
			}

			// End the procedure declaration
			prvPLSQLBlock +=
				indent() + "EXCEPTION" + Environment.NewLine +
				indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
				indent(2) +		"logStatus$(p_ScenarioIndex$, 'Matrix Comp Failed', sqlcode, " +
										"'Run-time error in cmpParamVsMatrix for call identifier " + callIdentifier + ": ' || c_LF || dbms_utility.format_error_stack || c_LF || dbms_utility.format_error_backtrace);" + Environment.NewLine +
				indent() + "END;" + Environment.NewLine +
				Environment.NewLine;
		}

		private void declareMatrixComparisonSubProcs() {
			for (int currScenarioIndex = 0; currScenarioIndex < prvScenarioGroup.scenarios.Count; currScenarioIndex++) {
				for (int currColumn = 0; currColumn < prvTest.testArguments.Count; currColumn++) {
					if (prvTest.testArguments[currColumn].inOut != "IN") {
						// Declare a procedure to compare a OUT/RETURN cursor against a user-defined matrix...
						if (prvTest.testArguments[currColumn].inOut == "OUT" || prvTest.testArguments[currColumn].inOut == "RETURN") {
							switch (prvScenarioGroup.scenarios[currScenarioIndex].parameters[currColumn].valueComparisonType) {
								case "matrix":
									declareMatrixComparisonSubProc(
										callIdentifier: "SCN_" + currScenarioIndex + "_P_" + prvTest.testArguments[currColumn].sequence.ToString(),
										expectedDataTable: prvScenarioGroup.scenarios[currScenarioIndex].parameters[currColumn].DataTable
									);
									break;

								case "nested":
									if (prvTest.testArguments[currColumn].dataType == "REF CURSOR"
										|| (prvTest.testArguments[currColumn].dataType == "TABLE" && prvTest.isPipelinedFunction)) {
										declareMatrixComparisonSubProc(
											callIdentifier: "SCN_" + currScenarioIndex + "_P_" + prvTest.testArguments[currColumn].sequence.ToString(),
											expectedDataTable: scenarioParameterCollection.toDataTable(nestedParameters: prvScenarioGroup.scenarios[currScenarioIndex].parameters[currColumn].nestedParameters, useValueAttributes: true)
										);
									}
									
									break;
							}
						}

						// Declare a procedure to compare the OUT side of an IN/OUT cursor against a user-defined matrix...
						if (prvTest.testArguments[currColumn].inOut == "IN/OUT") {
							switch (prvScenarioGroup.scenarios[currScenarioIndex].parameters[currColumn].expectedOutComparisonType) {
								case "matrix":
									declareMatrixComparisonSubProc(
										callIdentifier: "SCN_" + currScenarioIndex + "_P_" + prvTest.testArguments[currColumn].sequence.ToString() + "_EXP_OUT",
										expectedDataTable: prvScenarioGroup.scenarios[currScenarioIndex].parameters[currColumn].ExpectedOutDataTable
									);
									break;

								case "nested":
									if (prvTest.testArguments[currColumn].dataType == "REF CURSOR"
										|| (prvTest.testArguments[currColumn].dataType == "TABLE" && prvTest.isPipelinedFunction)) {
										declareMatrixComparisonSubProc(
											callIdentifier: "SCN_" + currScenarioIndex + "_P_" + prvTest.testArguments[currColumn].sequence.ToString() + "_EXP_OUT",
											expectedDataTable: scenarioParameterCollection.toDataTable(nestedParameters: prvScenarioGroup.scenarios[currScenarioIndex].parameters[currColumn].expectedOutNestedParameters, useValueAttributes: false)
										);
									}

									break;
							}
						}
					}
				}
			}
		}

		private void declareScenarioGroupSubProcs(OracleConnection conTargetDB) {
			declareScenarioGroupDeclare();
      
			declareScenarioGroupStartup();
      
			// User scenario group startup code
			if (prvScenarioGroup.scenarioGroupStartup != String.Empty) {
				prvPLSQLBlock += 
					Environment.NewLine +
					indent() + "PROCEDURE userScenarioGroupStartup$ AS" + Environment.NewLine +
					indent() + "BEGIN" + Environment.NewLine +
					indent(1) + "-- User-Defined Scenario Group Startup" + Environment.NewLine +
					prvScenarioGroup.scenarioGroupStartup +
					Environment.NewLine +
					indent() + "EXCEPTION" + Environment.NewLine +
					indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
					indent(2) +   "RAISE_APPLICATION_ERROR(-20998, 'Run-time error in scenario group \"" + prvScenarioGroup.name.Replace("'", "''") + "\"''s startup: ' || c_LF || dbms_utility.format_error_stack);" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine;
			}
      
			// Scenario Group Startup
			prvPLSQLBlock += 
				Environment.NewLine +
				indent() + "PROCEDURE scenarioStartup$(p_ScenarioIndex$ PLS_INTEGER) AS" + Environment.NewLine;
        
			if (prvTest.testArguments.Count > 0) {
				prvPLSQLBlock += 
					indent(1) + "v_Param t_ParameterName;" + Environment.NewLine;
			}
        
			prvPLSQLBlock += 
				indent() + "BEGIN" + Environment.NewLine;
        
			v_IndentionLvl++;
        
			// @@@ Need to use ILO preference
			//if (false) { // util.getPreference('repos.use_ilo') = 'Y' THEN
			//   prvPLSQLBlock += 
			//      indent() + "-- Initialize ilo's tasks..." + Environment.NewLine +
			//      indent() + "ilo_task.end_all_tasks;" + Environment.NewLine +
			//      Environment.NewLine;
			//}

			prvPLSQLBlock +=
				indent() + "-- Initialize the framework variables..." + Environment.NewLine +
				indent() + c_ARR_STATUSES_VAR_NAME + "(p_ScenarioIndex$) := 'OK';" + Environment.NewLine +
				indent() + c_ARR_ERROR_NUMBERS_VAR_NAME + "(p_ScenarioIndex$) := 0;" + Environment.NewLine +
				indent() + c_ARR_ERROR_MSGS_VAR_NAME + "(p_ScenarioIndex$) := NULL;" + Environment.NewLine;
			        
			if (prvTest.testArguments.Count > 0) {
				// If this unit has parameters, then we need to reinitialize the
				// array of booleans that indicate whether or not a parameter was
				// passed this time around before every scenario....
				prvPLSQLBlock += 
					Environment.NewLine +
					indent() + "-- Re-initialize the array of booleans that indicates whether the parameters were defaulted or not for this particular scenario..." + Environment.NewLine +
					indent() + "v_Param := v_arrDefaulted.FIRST;" + Environment.NewLine +
					indent() + "LOOP" + Environment.NewLine +
					indent(1) + "EXIT WHEN v_Param IS NULL;" + Environment.NewLine +
					indent(1) + "v_arrDefaulted(v_Param) := FALSE;" + Environment.NewLine +
					indent(1) + "v_Param := v_arrDefaulted.NEXT(v_Param);" + Environment.NewLine +
					indent() + "END LOOP;" + Environment.NewLine +
					Environment.NewLine +
					indent() + "resetParamVariables;" + Environment.NewLine;
            
				v_IndentionLvl--;
          
				prvPLSQLBlock += 
					indent() + "EXCEPTION" + Environment.NewLine +
					indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
					indent(2) +   "RAISE_APPLICATION_ERROR(-20000, 'p_ScenarioIndex$:' || p_ScenarioIndex$);" + Environment.NewLine;
          
				v_IndentionLvl++;
			}
        
			v_IndentionLvl--;
        
			prvPLSQLBlock += 
				indent() + "END;" + Environment.NewLine;
        
			// User scenario startup code
			if (prvScenarioGroup.scenarioStartup != String.Empty) {
				prvPLSQLBlock += 
					Environment.NewLine + Environment.NewLine +
					indent() + "PROCEDURE userScenarioStartup$(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) AS" + Environment.NewLine +
					indent() + "BEGIN" + Environment.NewLine +
					indent(1) + "-- User-Defined Scenario Startup" + Environment.NewLine +          
					prvScenarioGroup.scenarioStartup +
					Environment.NewLine +
					indent() + "EXCEPTION" + Environment.NewLine +
					indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
					indent(2) + "RAISE_APPLICATION_ERROR(-20997, 'Run-time error in scenario #' || p_ScenarioIndex$ || '''s startup: ' || c_LF || dbms_utility.format_error_stack);" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine;
			}
        
			// User post-parameter assignment code
			if (prvScenarioGroup.postParamAssignment != String.Empty) {
				prvPLSQLBlock += 
					Environment.NewLine +
					indent() + "PROCEDURE userPostParamAssignmentHook(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) AS" + Environment.NewLine +
					indent() + "BEGIN" + Environment.NewLine +            
					indent(1) + "-- User-defined post parameter assignment code" + Environment.NewLine +
					prvScenarioGroup.postParamAssignment +
					Environment.NewLine +
					indent() + "EXCEPTION" + Environment.NewLine +
					indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
					indent(2) + "RAISE_APPLICATION_ERROR(-20996, 'Run-time error in scenario #' || p_ScenarioIndex$ || '''s post parameter assignment: ' || c_LF || dbms_utility.format_error_stack);" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine +
					Environment.NewLine;
			}
        
			// Matrix comparison (expected vs. actual return/out parameter) methods
			declareMatrixComparisonSubProcs();
			
			// User-scenario teardown
			if (prvScenarioGroup.scenarioTeardown != String.Empty) {
				prvPLSQLBlock +=
					indent() + "PROCEDURE userScenarioTeardown$(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) AS" + Environment.NewLine +
					indent() + "BEGIN" + Environment.NewLine +
					indent(1) + "-- User-Defined Scenario Teardown" + Environment.NewLine +
					prvScenarioGroup.scenarioTeardown +
					Environment.NewLine +
					indent() + "EXCEPTION" + Environment.NewLine +
					indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
					indent(2) +		"RAISE_APPLICATION_ERROR(-20995, 'Run-time error in scenario #' || p_ScenarioIndex$ || '''s teardown: ' || c_LF || dbms_utility.format_error_stack);" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine;
			}
        
			// Pre-UDC hook
			if (prvScenarioGroup.preUDC != String.Empty) {
				prvPLSQLBlock += 
					Environment.NewLine + Environment.NewLine +
					indent() + "PROCEDURE userPreUDCs(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) AS" + Environment.NewLine +
					indent() + "BEGIN" + Environment.NewLine +
					indent(1) + "-- User-Defined Pre-UDC code" + Environment.NewLine +
					prvScenarioGroup.preUDC +
					Environment.NewLine +
					indent() + "EXCEPTION" + Environment.NewLine +
					indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
					indent(2) + "RAISE_APPLICATION_ERROR(-20994, 'Run-time error in scenario #' || p_ScenarioIndex$ || '''s pre-UDC hook: ' || c_LF || dbms_utility.format_error_stack);" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine;
			}
        
			// UDC's
			if (prvScenarioGroup.udcCollection.Count > 0) {
				prvPLSQLBlock += 
					Environment.NewLine +
					indent() + "-- UDC methods..." + Environment.NewLine;
          
				for (int i = 0; i < prvScenarioGroup.udcCollection.Count; i++) {
					switch (prvScenarioGroup.udcCollection[i].checkType) {
						case udc.enumCheckTypes.PLSQL_BLOCK:
							prvPLSQLBlock +=
								indent() + "PROCEDURE userprvPLSQLBlockUDC" + i.ToString() + "$(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) IS" + Environment.NewLine +
								indent() + "BEGIN" + Environment.NewLine +
								indent(1) + "-- User-defined PL/SQL block...." + Environment.NewLine +
								indent(1) + "IF " + (prvScenarioGroup.udcCollection[i].plsqlCondition == String.Empty ? "TRUE" : prvScenarioGroup.udcCollection[i].plsqlCondition) + Environment.NewLine +
								indent(1) + "THEN" + Environment.NewLine + // This is on the next line, in case the PL/SQL condition ends in a comment
								indent(2) + "BEGIN" + Environment.NewLine +
								prvScenarioGroup.udcCollection[i].plsqlBlock + Environment.NewLine +
								indent(2) + "EXCEPTION" + Environment.NewLine +
								indent(3) + "WHEN OTHERS THEN" + Environment.NewLine +
								indent(4) + "logStatus$(p_ScenarioIndex$, 'UDC Failed', sqlcode, 'UDC \"" + prvScenarioGroup.udcCollection[i].name.Replace("'", "''") + "\"" +
									" returned the following error: ' || c_CRLF || dbms_utility.format_error_stack || c_CRLF || dbms_utility.format_error_backtrace); -- Handle exception in user-defined check" + Environment.NewLine +
								indent(2) + "END;" + Environment.NewLine +
								indent(1) + "END IF;" + Environment.NewLine +
								indent() + "EXCEPTION" + Environment.NewLine +
								indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
								indent(2) + "RAISE_APPLICATION_ERROR(-20993, 'Run-time error in scenario #' || p_ScenarioIndex$ || '''s PL/SQL Block UDC #" + i.ToString() + ": ' || c_LF || dbms_utility.format_error_stack);" + Environment.NewLine +
								indent() + "END;" + Environment.NewLine +
								Environment.NewLine;
							break;

						case udc.enumCheckTypes.CURSOR_RETURNING_ROWS:
							buildCursorResultTest(i, prvScenarioGroup.udcCollection[i], p_ShouldReturnRow: true);
							break;

						case udc.enumCheckTypes.CURSOR_RETURNING_NO_ROWS:
							buildCursorResultTest(i, prvScenarioGroup.udcCollection[i], p_ShouldReturnRow: false);
							break;

						case udc.enumCheckTypes.ROW_VALIDATION:
							prvPLSQLBlock +=
								indent() + "PROCEDURE validateRowUDC" + i.ToString() + "$(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) IS" + Environment.NewLine +
								indent(1) + "CURSOR c_UDCRow IS" + Environment.NewLine +
								prvScenarioGroup.udcCollection[i].rowValidationCursor + Environment.NewLine +
								indent(1) + "v_ExpectedUDCRow c_UDCRow%ROWTYPE;" + Environment.NewLine +
								indent(1) + "v_ActualUDCRow   c_UDCRow%ROWTYPE;" + Environment.NewLine +
								indent() + "BEGIN" + Environment.NewLine +
								indent(1) + "IF " + (prvScenarioGroup.udcCollection[i].plsqlCondition == String.Empty ? "TRUE" : prvScenarioGroup.udcCollection[i].plsqlCondition) + Environment.NewLine +
								indent(1) + "THEN" + Environment.NewLine + // This is on the next line, in case the PL/SQL condition ends in a comment
								indent(2) + "OPEN c_UDCRow;" + Environment.NewLine +
								indent(2) + "FETCH c_UDCRow INTO v_ActualUDCRow;" + Environment.NewLine +
								indent(2) + "IF c_UDCRow%NOTFOUND THEN" + Environment.NewLine +
								indent(3) + "logStatus$(p_ScenarioIndex$, 'UDC Missing Row', 0, 'UDC #" + (i+1).ToString() + " cannot find the row to validate in scenario #' || p_ScenarioIndex$ || '!');" + Environment.NewLine +
								indent(2) + "END IF;" + Environment.NewLine +
								Environment.NewLine +
								indent(2) + "IF " + c_ARR_STATUSES_VAR_NAME + "(p_ScenarioIndex$) = 'OK' THEN" + Environment.NewLine;

							foreach (rowValidatorCheck v_Field in prvScenarioGroup.udcCollection[i].fieldValidations) {
								switch (v_Field.comparisonType) {
									case "Don't Test":
										break;
									case "Input Parameter":
									case "Exp":
									case "Value":
										prvPLSQLBlock +=
											indent(3) + "v_ExpectedUDCRow." + v_Field.fieldName + " := " + v_Field.fieldValue + ";" + Environment.NewLine;
										break;
									default:
										break; // Do nothing
								}
							}

							prvPLSQLBlock +=
								Environment.NewLine +
								getRowValidationPLSQL(prvScenarioGroup.udcCollection[i]) + Environment.NewLine +
								Environment.NewLine +
								indent(3) + "-- Make sure there aren't any more rows in the cursor..." + Environment.NewLine +
								indent(3) + "FETCH c_UDCRow INTO v_ActualUDCRow;" + Environment.NewLine +
								indent(3) + "IF c_UDCRow%FOUND THEN" + Environment.NewLine +
								indent(4) + "logStatus$(p_ScenarioIndex$, 'UDC Too Many Rows', 0, 'UDC #" + (i+1).ToString() + " found more than one row to validate in scenario #' || p_ScenarioIndex$ || '!');" + Environment.NewLine +
								indent(3) + "END IF;" + Environment.NewLine +
								indent(2) + "END IF;" + Environment.NewLine +
								Environment.NewLine +
								indent(2) + "CLOSE c_UDCRow;" + Environment.NewLine +
								indent(1) + "END IF;" + Environment.NewLine +
								indent() + "END;" +
								Environment.NewLine;

							break;

						case udc.enumCheckTypes.COMPARE_CURSORS:
							prvPLSQLBlock +=
								indent() + "PROCEDURE compareCursorsUDC" + i.ToString() + "$(p_ScenarioIndex$ PLS_INTEGER, p_ScenarioGUID$ VARCHAR2) AS" + Environment.NewLine +
								indent(1) + "v_ExpectedResults SYS_REFCURSOR;" + Environment.NewLine +
								indent(1) + "v_ActualResults   SYS_REFCURSOR;" + Environment.NewLine +
								indent() + "BEGIN" + Environment.NewLine +
								indent(1) + "IF " + (prvScenarioGroup.udcCollection[i].plsqlCondition == String.Empty ? "TRUE" : prvScenarioGroup.udcCollection[i].plsqlCondition) + Environment.NewLine +
								indent(1) + "THEN" + Environment.NewLine + // This is on the next line, in case the PL/SQL condition ends in a comment								
								indent(2) + "OPEN v_ExpectedResults FOR" + Environment.NewLine +
								prvScenarioGroup.udcCollection[i].expectedCursor.TrimEnd(';') + ";" + Environment.NewLine +
								Environment.NewLine +
								indent(2) + "OPEN v_ActualResults FOR" + Environment.NewLine +
								prvScenarioGroup.udcCollection[i].actualCursor.TrimEnd(';') + ";" + Environment.NewLine +
								Environment.NewLine +
								indent(2) + "compareCursors$(p_ExpectedResults => v_ExpectedResults, p_ActualResults => v_ActualResults, p_CSVExcludedColumns => '" + prvScenarioGroup.udcCollection[i].csvExcludedColumns + "');" + Environment.NewLine +
								indent(1) + "END IF;" + Environment.NewLine +
								indent() + "END;" + Environment.NewLine +
								Environment.NewLine;
							break;

						case udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX:
							declareMatrixComparisonSubProc(
								callIdentifier: "UDC_" + i.ToString(),
								expectedDataTable: prvScenarioGroup.udcCollection[i].expectedMatrix,
								runCondition: prvScenarioGroup.udcCollection[i].plsqlCondition);

							break;
					}
				}
          
				// Declare a single procedure that calls all of the UDC's
				prvPLSQLBlock +=
					indent() + "PROCEDURE callUDCs$(p_ScenarioIndex$ PLS_INTEGER) IS" + Environment.NewLine +
					indent() + "BEGIN" + Environment.NewLine;
            
				v_IndentionLvl++;
          
				prvPLSQLBlock +=
					indent() + "-- Call user-defined checks, if we didn't have an unexpected exception..." + Environment.NewLine +
					indent() + "IF " + c_ARR_STATUSES_VAR_NAME + "(p_ScenarioIndex$) = 'OK' THEN" + Environment.NewLine;
          
				if (prvScenarioGroup.preUDC != String.Empty) {
					prvPLSQLBlock +=
						indent(1) + "userPreUDCs(p_ScenarioIndex$ => p_ScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(p_ScenarioIndex$));" + Environment.NewLine +
						Environment.NewLine;
				}
        
				for (int i = 0; i < prvScenarioGroup.udcCollection.Count; i++) {
					switch (prvScenarioGroup.udcCollection[i].checkType) {
						case udc.enumCheckTypes.PLSQL_BLOCK:
							prvPLSQLBlock +=
								indent(1) + "userprvPLSQLBlockUDC" + i.ToString() + "$(p_ScenarioIndex$ => p_ScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(p_ScenarioIndex$));" + Environment.NewLine;
							break;

						case udc.enumCheckTypes.CURSOR_RETURNING_ROWS:
							prvPLSQLBlock +=
							indent(1) + "cursorReturningRowsUDC" + i.ToString() + "$(p_ScenarioIndex$ => p_ScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(p_ScenarioIndex$));" + Environment.NewLine;
							break;

						case udc.enumCheckTypes.CURSOR_RETURNING_NO_ROWS:
							prvPLSQLBlock +=
								indent(1) + "cursorReturningNoRowsUDC" + i.ToString() + "$(p_ScenarioIndex$ => p_ScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(p_ScenarioIndex$));" + Environment.NewLine;
							break;

						case udc.enumCheckTypes.ROW_VALIDATION:
							prvPLSQLBlock +=
								indent(1) + "validateRowUDC" + i.ToString() + "$(p_ScenarioIndex$ => p_ScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(p_ScenarioIndex$));" + Environment.NewLine;
							break;

						case udc.enumCheckTypes.COMPARE_CURSORS:
							prvPLSQLBlock +=
								indent(1) + "compareCursorsUDC" + i.ToString() + "$(p_ScenarioIndex$ => p_ScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(p_ScenarioIndex$));" + Environment.NewLine;
							break;

						case udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX:
							string cursorName = "c_ActualResults_UDC_" + i.ToString();

							prvPLSQLBlock +=
								indent(1) + "DECLARE" + Environment.NewLine +
								indent(2) +		cursorName + " SYS_REFCURSOR;" + Environment.NewLine +
								indent(1) + "BEGIN" + Environment.NewLine +
								indent(2) +		"OPEN " + cursorName + " FOR" + Environment.NewLine +
								prvScenarioGroup.udcCollection[i].actualCursor.TrimEnd(';') + ";" + Environment.NewLine +
								Environment.NewLine +
								indent(2) +		"CASE p_ScenarioIndex$" + Environment.NewLine;

							for (int currScenarioIndex = 0; currScenarioIndex < prvScenarioGroup.scenarios.Count; currScenarioIndex++) {
								prvPLSQLBlock +=
									indent(2) + "WHEN " + (currScenarioIndex+1).ToString() + " THEN" + Environment.NewLine +
									indent(3) +		"cmpVsMtxUDC_" + i.ToString() + "$(" +
										"p_ScenarioIndex$ => p_ScenarioIndex$, " + 
										"p_ActualResults => " + cursorName + ", " +
										"p_OutBuiltExpResults => " + c_ARR_OUT_UDC_EXP_VAL_VAR_NAME + i.ToString() + "_" + currScenarioIndex.ToString() + ", " + 
										"p_OutBuiltActResults => " + c_ARR_OUT_UDC_ACT_VAL_VAR_NAME + i.ToString() + "_" + currScenarioIndex.ToString() + ");" + Environment.NewLine;
							}
						
							prvPLSQLBlock +=
								indent(2) +		"END CASE;" + Environment.NewLine +
								indent(1) + "END;" + Environment.NewLine;

							break;
					}
				}

				v_IndentionLvl--;
          
				prvPLSQLBlock += 
					indent(1) + "END IF;" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine +
					Environment.NewLine;
			}
        
			// Automatic scenario teardown
			prvPLSQLBlock += 
				Environment.NewLine +
				indent() + "PROCEDURE scenarioTeardown$(p_ScenarioIndex$ PLS_INTEGER) AS" + Environment.NewLine +
				indent() + "BEGIN" + Environment.NewLine;

			// Close any cursors that might be open, after finishing iterating through them...
			for (int i = 0; i < prvTest.testArguments.Count; i++) {
				if ((prvTest.testArguments[i].inOut == "RETURN" && prvTest.isPipelinedFunction)
					|| (prvTest.testArguments[i].inOut == "IN" && prvTest.testArguments[i].plsType == "SYS_REFCURSOR")
					|| ((prvTest.testArguments[i].inOut == "IN/OUT" || prvTest.testArguments[i].inOut == "OUT" || prvTest.testArguments[i].inOut == "RETURN") && prvTest.testArguments[i].plsType == "SYS_REFCURSOR")) {

					prvPLSQLBlock +=
						indent(1) + "-- Close " + prvTest.testArguments[i].argumentName + " if still open, after iterating thru all records..." + Environment.NewLine +
						indent(1) + "DECLARE" + Environment.NewLine +
						indent(2) +		"v_CursorNumber INTEGER;" + Environment.NewLine +
						indent(1) + "BEGIN" + Environment.NewLine +
						indent(2) +		"IF " + prvTest.testArguments[i].argumentName + "%ISOPEN THEN" + Environment.NewLine +
						indent(3) +			"IF " + c_ARR_STATUSES_VAR_NAME + "(p_ScenarioIndex$) = 'OK' THEN" + Environment.NewLine +
						indent(4) +				"-- Fetch every record to make sure the cursor doesn't raise an exception half-way through fetching its records..." + Environment.NewLine +
						indent(4) +				"v_CursorNumber:= DBMS_SQL.TO_CURSOR_NUMBER(" + prvTest.testArguments[i].argumentName + "); " + Environment.NewLine +
						Environment.NewLine +
						indent(4) +				"WHILE DBMS_SQL.FETCH_ROWS(v_CursorNumber) > 0 LOOP" + Environment.NewLine +
						indent(5) +					"NULL;" + Environment.NewLine +
						indent(4) +				"END LOOP;" + Environment.NewLine +
						Environment.NewLine +
						indent(4) +				"DBMS_SQL.CLOSE_CURSOR(v_CursorNumber);" + Environment.NewLine +
						indent(3) +			"ELSE" + Environment.NewLine +
						indent(4) +				"CLOSE " + prvTest.testArguments[i].argumentName + ";" + Environment.NewLine +
						indent(3) +			"END IF;" + Environment.NewLine +
						indent(2) +		"END IF;" + Environment.NewLine +
						indent(1) + "EXCEPTION" + Environment.NewLine +
						indent(2) +		"WHEN OTHERS THEN" + Environment.NewLine +
						indent(3) +			"IF " + c_ARR_STATUSES_VAR_NAME + "(p_ScenarioIndex$) = 'OK' THEN" + Environment.NewLine +
						indent(4) +				"logStatus$(p_ScenarioIndex$, 'Cursor Iteration Exception', sqlcode, dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
						indent(3) +			"END IF;" + Environment.NewLine +
						indent(1) +	"END;" + Environment.NewLine +
						Environment.NewLine;
				}
			}
  
			if (prvScenarioGroup.rollbackAfterEveryScenario) {
				prvPLSQLBlock += 
					indent(1) + "-- Scenario-defined rollback" + Environment.NewLine +
					indent(1) + "BEGIN" + Environment.NewLine +
					indent(2) +   "ROLLBACK TO spPostScenarioGroupStartup;" + Environment.NewLine +
					indent(1) + "EXCEPTION" + Environment.NewLine +
					indent(2) +   "WHEN OTHERS THEN" + Environment.NewLine +
					indent(3) +     "IF sqlcode = -1086 THEN" + Environment.NewLine +
					indent(4) +       "RAISE_APPLICATION_ERROR(-20911, 'Unable to rollback to the savepoint before the scenario! Did the scenario COMMIT or do a DDL operation?');" + Environment.NewLine +
					indent(3) +     "ELSE" + Environment.NewLine +
					indent(4) +       "RAISE;" + Environment.NewLine +
					indent(3) +     "END IF;" + Environment.NewLine +
					indent(1) + "END;" + Environment.NewLine;
			} else {
				prvPLSQLBlock += 
					indent(1) + "NULL;" + Environment.NewLine;
			}
        
			prvPLSQLBlock += 
				indent() + "END;" + Environment.NewLine;
        
			if (prvScenarioGroup.scenarioGroupTeardown != String.Empty) {
				prvPLSQLBlock += 
					Environment.NewLine + Environment.NewLine +
					indent() + "PROCEDURE userScenarioGroupTeardown$ AS" + Environment.NewLine +
					indent() + "BEGIN" + Environment.NewLine +
					indent(1) + "-- User-Defined Scenario Group Teardown" + Environment.NewLine +
					prvScenarioGroup.scenarioGroupTeardown +
					Environment.NewLine +
					indent() + "EXCEPTION" + Environment.NewLine +
					indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
					indent(2) +   "RAISE_APPLICATION_ERROR(-20992, 'Run-time error in scenario group \"" + prvScenarioGroup.name + "\"''s group teardown: ' || c_LF || dbms_utility.format_error_stack);" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine;
			}
		}
		#endregion

		private void getScenarioGroupHeader(OracleConnection conTargetDB) {
			prvPLSQLBlock +=
				//indent() + "<<rt_test_block>>" + Environment.NewLine +
				indent() + "DECLARE" + Environment.NewLine;
			v_IndentionLvl++;

			prvPLSQLBlock +=
				indent() + "-- This is a debug block for:" + Environment.NewLine +
				indent() + "--    Scenario Group: " + prvScenarioGroup.name + Environment.NewLine +
				((prvScenarioIndex != -1) ? indent() + "--    Scenario: " + (prvScenarioIndex + 1) + Environment.NewLine : String.Empty) +
				indent() + "--" + Environment.NewLine +
				indent() + "-- You may need to comment out calls to UDC's, resetting package state, etc." + Environment.NewLine +
				indent() + "-- WATCH OUT for committing your test run; be sure to ROLLBACK when you're done." + Environment.NewLine +
				Environment.NewLine;
			
			getParamInfo();

			declareOtherVarsAndConstants(conTargetDB: conTargetDB);

			prvPLSQLBlock +=
				getParameterVariableDeclaration();

			declareTestLevelSubProcedures();

			v_IndentionLvl--;
			prvPLSQLBlock +=
				indent() + "BEGIN" + Environment.NewLine +
				//indent(1) + "<<rt_scn_group_block>>" + Environment.NewLine +
				indent(1) + "DECLARE" + Environment.NewLine;

			v_IndentionLvl += 2;
			declareScenarioGroupSubProcs(conTargetDB: conTargetDB);
			v_IndentionLvl -= 2;

			prvPLSQLBlock +=
				indent(1) + "BEGIN" + Environment.NewLine;
			v_IndentionLvl++;

			prvPLSQLBlock +=
				indent(1) + "SAVEPOINT spPreScenarioGroupStartup;" + Environment.NewLine + Environment.NewLine;

			prvPLSQLBlock +=
				indent(1) + "scenarioGroupStartup$;" + Environment.NewLine +
				Environment.NewLine;

			// Because the user startup won't run for every scenario in the scenario group,
			// we need to run it once here at the beginning.
			if (prvScenarioGroup.scenarioGroupStartup != String.Empty) {
				prvPLSQLBlock +=
				indent(1) + "userScenarioGroupStartup$;" + Environment.NewLine +
				Environment.NewLine;
			}

			prvPLSQLBlock +=
				indent(1) + "SAVEPOINT spPostScenarioGroupStartup;" + Environment.NewLine + Environment.NewLine;
		}

		#region Main Body
		private void callStartupProcedures() {
			prvPLSQLBlock +=
				indent() + "scenarioStartup$(p_ScenarioIndex$ => v_CurrScenarioIndex$);" + Environment.NewLine +
				Environment.NewLine;
        
			if (prvScenarioGroup.scenarioStartup != String.Empty) {
				prvPLSQLBlock +=
					indent() + "userScenarioStartup$(p_ScenarioIndex$ => v_CurrScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(v_CurrScenarioIndex$));" + Environment.NewLine +
				Environment.NewLine;
			}
		}
            
		private void assignInputVariables() {

			if ((v_MethodIsFunction && prvTest.testArguments.Count > 1) // Don't count the return plsqlValue as a input parameter for functions
				|| (v_MethodIsFunction == false && prvTest.testArguments.Count > 0)) {
					prvPLSQLBlock +=
						indent() + "-- Assign parameter values" + Environment.NewLine +
						indent() + "BEGIN" + Environment.NewLine;
          
				v_IndentionLvl++;
        
				prvPLSQLBlock += 
					indent() + "CASE v_CurrScenarioIndex$" + Environment.NewLine;

				int i = 0;
				foreach (scenario scn in prvScenarioGroup.scenarios) {
					prvPLSQLBlock += 
						indent() + "WHEN " + (i+1).ToString() + " THEN" + Environment.NewLine;
            
					v_IndentionLvl++;

					recursivelyAssignInputVariables(nestedParams: scn.parameters);
					
					v_IndentionLvl--;
					i++;
				}
                
				prvPLSQLBlock += 
					indent() + "END CASE;" + Environment.NewLine;
          
				v_IndentionLvl--;
      
				prvPLSQLBlock += 
					indent() + "EXCEPTION" + Environment.NewLine +
					indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
					indent(2) +   "logStatus$(v_CurrScenarioIndex$, 'Assignment Failed', sqlcode, dbms_utility.format_error_stack);" + Environment.NewLine +
					indent() + "END;" + Environment.NewLine +
					Environment.NewLine;
			}
		}

		private void recursivelyAssignInputVariables(scenarioParameterCollection nestedParams, string parentVariableName = "", scenarioParameter parentParam = null) {
			bool isCaseEmpty;
			string v_CSVDefaultedParams;
			string variableName = "";
			string childVariableName = "";

			isCaseEmpty = true;
			v_CSVDefaultedParams = String.Empty;

			foreach (scenarioParameter param in nestedParams) {
				// If the parameter is passed, then set it to what the user wants
				if (param.testArg.isReturnArgument() == false
					&& param.isDefaulted == false
					&& param.testArg.inOut != "OUT"
					) {

					if (parentParam == null) {
						variableName = (parentVariableName == "" ? "" : parentVariableName + ".") + param.testArg.argumentName;
						childVariableName = (parentVariableName == "" ? "" : parentVariableName + ".") + param.testArg.argumentName;
					} else {
						variableName = ((parentVariableName == "" ? "" : parentVariableName + ".") + (parentParam.testArg.dataType == "TABLE" ? "" : param.testArg.argumentName)).TrimEnd('.');
						childVariableName = ((parentVariableName == "" ? "" : parentVariableName + ".") + (parentParam.testArg.dataType == "TABLE" ? "" : param.testArg.argumentName)).TrimEnd('.');
					}

					switch (param.valueComparisonType) {
						case "exp":
							prvPLSQLBlock +=
								indent() + variableName +
								" := " + param.plsqlValue + ";" + Environment.NewLine;
							break;

						case "select":
							prvPLSQLBlock +=
								indent() + "OPEN " + variableName + " FOR" + Environment.NewLine +
								indent(1) + param.plsqlValue.TrimEnd(';') + ";" + Environment.NewLine +
								Environment.NewLine;
							break;

						case "matrix":
							prvPLSQLBlock +=
								indent() + "OPEN " + variableName + " FOR" + Environment.NewLine;

							DataTable dtMatrix = param.DataTable;

							for (int currRow = 0; currRow < dtMatrix.Rows.Count; currRow++) {
								prvPLSQLBlock +=
									indent(1) + "SELECT ";

								for (int currCol = 0; currCol < dtMatrix.Columns.Count; currCol += 2) {
									string comparisonType = dtMatrix.Rows[currRow][currCol + 1].ToString();

									switch (comparisonType) {
										case "exp":
											prvPLSQLBlock += dtMatrix.Rows[currRow][currCol].ToString();
											break;
										case "value":
											prvPLSQLBlock += "'" + dtMatrix.Rows[currRow][currCol].ToString().Replace("'", "''") + "'";
											break;
									}

									prvPLSQLBlock += " AS " + dtMatrix.Columns[currCol].ColumnName;

									if (currCol < dtMatrix.Columns.Count-2) {
										prvPLSQLBlock += ", ";
									}
								}

								prvPLSQLBlock += " FROM dual";

								if (currRow < dtMatrix.Rows.Count - 1) {
									prvPLSQLBlock += Environment.NewLine +
										indent(1) + "UNION ALL" + Environment.NewLine;
								}
							}

							prvPLSQLBlock += ";" + Environment.NewLine +
								Environment.NewLine;

							break;

						default:
							if (param.testArg.dataType == "REF CURSOR" && param.valueComparisonType == "nested") {
								// Strongly-typed cursor, populated through nested parameter values...
								prvPLSQLBlock +=
									indent() + "OPEN " + variableName + " FOR" + Environment.NewLine;

								for (int currRow = 0; currRow < param.nestedParameters.Count; currRow++) {
									prvPLSQLBlock +=
										indent(1) + "SELECT ";

									scenarioParameterCollection fieldParams = param.nestedParameters[currRow][0].nestedParameters[0];

									for (int currCol = 0; currCol < fieldParams.Count; currCol++) {
										string comparisonType = fieldParams[currCol].valueComparisonType;

										prvPLSQLBlock += fieldParams[currCol].plsqlRawValue;										

										prvPLSQLBlock += " AS " + fieldParams[currCol].testArg.argumentName;

										if (currCol < fieldParams.Count - 1) {
											prvPLSQLBlock += ", ";
										}
									}

									prvPLSQLBlock += " FROM dual";

									if (currRow < param.nestedParameters.Count - 1) {
										prvPLSQLBlock += Environment.NewLine +
											indent(1) + "UNION ALL" + Environment.NewLine;
									}
								}

								prvPLSQLBlock += ";" + Environment.NewLine +
									Environment.NewLine;
							} else {
								switch (param.testArg.dataType) {
									case "TABLE":
										// Initialize a parameter with the right type...
										prvPLSQLBlock +=
											indent() + variableName +
												" := " + param.testArg.plsType + "();" + Environment.NewLine +
											indent() + variableName + ".EXTEND(" + param.nestedParameters.Count + ");" + Environment.NewLine +
											Environment.NewLine;

										int row = 0;
										foreach (scenarioParameterCollection childParams in param.nestedParameters) {
											recursivelyAssignInputVariables(nestedParams: childParams, parentVariableName: childVariableName + "(" + (row + 1).ToString() + ")", parentParam: param);

											row++;
										}

										prvPLSQLBlock +=
											Environment.NewLine;

										break;

									case "PL/SQL RECORD":
										foreach (scenarioParameterCollection childParams in param.nestedParameters) {
											recursivelyAssignInputVariables(
												nestedParams: childParams,
												parentVariableName: childVariableName,
												parentParam: param);
										}

										break;

									default:
										prvPLSQLBlock +=
											indent() + variableName +
											" := " + param.plsqlValue + ";" + Environment.NewLine;
										break;
								}
							}
							break;
					}

					isCaseEmpty = false;
				}
			}

			// For defaulted parameters, indicate that they were defaulted....
			if (isCaseEmpty == false) {
				prvPLSQLBlock +=
					Environment.NewLine;
			}

			foreach (scenarioParameter param in nestedParams) {
				if (param.isDefaulted) {
					v_CSVDefaultedParams += param.testArg.argumentName + ",";
				}
			}

			v_CSVDefaultedParams = v_CSVDefaultedParams.TrimEnd(',');

			if (v_CSVDefaultedParams != String.Empty) {
				prvPLSQLBlock +=
					indent() + "setDefaultedParams('" + v_CSVDefaultedParams + "');" + Environment.NewLine;
				isCaseEmpty = false;
			}

			// Prevent an empty CASE WHEN block...
			if (isCaseEmpty) {
				prvPLSQLBlock +=
					indent() + "NULL;" + Environment.NewLine;
			}
		}

		private void callUserPostParamAssign() {
			if (prvScenarioGroup.postParamAssignment != String.Empty) {
				prvPLSQLBlock +=
					indent() + "userPostParamAssignmentHook(p_ScenarioIndex$ => v_CurrScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(v_CurrScenarioIndex$));" + Environment.NewLine;
			}
		}

		private void callUnit(scenario scn) {
			bool v_FirstParam = true;

			if (prvTest.plSQLBlock != String.Empty || prvTest.unitType == "TRIGGER" || prvTest.unitType == "VIEW") {
				prvPLSQLBlock +=
					indent() + (prvTest.plSQLBlock == String.Empty ? "NULL;" : prvTest.plSQLBlock) + Environment.NewLine;
			} else {
				if (prvTest.isPipelinedFunction) {
					prvPLSQLBlock +=
						indent() +
						"OPEN " + prvTest.testArguments[v_ReturnValueParamIndex].argumentName + " FOR" + Environment.NewLine +
						indent(1) + "SELECT * FROM TABLE(";
				} else {
					if (v_MethodIsFunction) {
						prvPLSQLBlock +=
							indent() +
							prvTest.testArguments[v_ReturnValueParamIndex].argumentName + " := ";
					} else {
						prvPLSQLBlock += indent();
					}
				}

				prvPLSQLBlock +=
					"\"" + prvTest.unitSchema + "\"" +
					(prvTest.unitName != String.Empty ? ".\"" + prvTest.unitName + "\"" : String.Empty) + 
					(prvTest.unitMethod != String.Empty ? ".\"" + prvTest.unitMethod + "\"" : String.Empty);
          
				// Stick in parameters here
				for (int i = 0; i < prvTest.testArguments.Count; i++) {
					if (prvTest.testArguments[i].inOut != "RETURN"
						&& scn.parameters[i].isDefaulted == false)
					{
						if (v_FirstParam) {
							prvPLSQLBlock += "(" + Environment.NewLine;
							v_FirstParam = false;
						} else {
							prvPLSQLBlock += "," + Environment.NewLine;
						}
                  
						prvPLSQLBlock +=
							indent(2) + prvTest.testArguments[i].argumentName.PadRight(v_MaxParamNameLen) + " => ";
                  
						prvPLSQLBlock +=
							prvTest.testArguments[i].argumentName;
					}
				}
          
				if (!v_FirstParam) {
					prvPLSQLBlock += Environment.NewLine +
					indent() + ")";
				}
            
				if (prvTest.isPipelinedFunction) {
					prvPLSQLBlock += ')';
				}
          
				prvPLSQLBlock +=
					";" + Environment.NewLine;
			}
		}

		private void callTargetUnits() {
			prvPLSQLBlock += 
				indent() + "-- Call the unit" + Environment.NewLine +
				indent() + "IF " + c_ARR_STATUSES_VAR_NAME + "(v_CurrScenarioIndex$) = 'OK' THEN" + Environment.NewLine +
				//indent(1) + "<<rt_call_block>>" + Environment.NewLine +
				indent(1) + "DECLARE" + Environment.NewLine +
				indent(2) +   "v_SQLCode$ PLS_INTEGER;" + Environment.NewLine +
				indent(2) +   "v_SQLErrm$ VARCHAR2(32767);" + Environment.NewLine +
				indent(1) + "BEGIN" + Environment.NewLine;
        
			v_IndentionLvl += 2;
        
			if (prvTest.testArguments.Count == 0
				|| (v_MethodIsFunction && prvTest.testArguments.Count == 1))
			{
				// Target has no parameters
				callUnit(scn: null);
			} else {
				Dictionary<String, String> scenarioHashIndexCSV = new Dictionary<String, String>();

				for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
					if (prvScenarioIndex == -1 || prvScenarioIndex == i) {
						String hash = prvScenarioGroup.scenarios[i].getScenarioParamHash();

						if (scenarioHashIndexCSV.ContainsKey(hash)) {
							scenarioHashIndexCSV[hash] += "," + (i+1).ToString(); 
						} else {
							scenarioHashIndexCSV.Add(hash, (i+1).ToString());
						}
					}
				}

				prvPLSQLBlock += 
					indent() +   "CASE" + Environment.NewLine;

				foreach (KeyValuePair<string, string> pair in scenarioHashIndexCSV) {
					prvPLSQLBlock +=
						indent() + "WHEN v_CurrScenarioIndex$ IN (" + pair.Value + ") THEN" + Environment.NewLine;
              
					v_IndentionLvl++;
            
					// Find the first scenario with this param hash, and use its parameter layout...
					callUnit(scn: prvScenarioGroup.scenarios[Int32.Parse(pair.Value.Split(',')[0]) - 1]);
              
					v_IndentionLvl--;
				}
        
				prvPLSQLBlock += 
					indent() + "END CASE;" + Environment.NewLine +
					Environment.NewLine;
			}
		}
      
		private void catchAnyRunException() {
			string v_CSVUnexpectedExceptions = String.Empty;

			Regex numberedException = new Regex("-?[0-9]+");

			// First, generate any exception handlers for named exceptions - @@@ These could be ordered by 
			// expected named exception, like they used to be, to minimize generated code.
			for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
				if (prvScenarioIndex == -1 || prvScenarioIndex == i) {
					if (prvScenarioGroup.scenarios[i].expectedException != String.Empty
						&& prvScenarioGroup.scenarios[i].expectedException != "ANY"
						&& !numberedException.IsMatch(prvScenarioGroup.scenarios[i].expectedException))
					{
						prvPLSQLBlock +=
							indent() + "WHEN " + prvScenarioGroup.scenarios[i].expectedException + " THEN" + Environment.NewLine +
							indent(1) + "IF v_CurrScenarioIndex$ IN (" + (i+1).ToString() + ") THEN" + Environment.NewLine +
							indent(2) +   "logStatus$(v_CurrScenarioIndex$, 'OK', 0, dbms_utility.format_error_stack);" + Environment.NewLine +
							indent(1) + "ELSE" + Environment.NewLine +
							indent(2) +   "logStatus$(v_CurrScenarioIndex$, 'Got Wrong Exception', sqlcode, 'Got ' || sqlcode || ': ' || dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
							indent(1) + "END IF;" + Environment.NewLine +
							Environment.NewLine;
					}
				}
			}
        
			// Next handle the scenarios that aren't expecting an exception at all exception...
			for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
				if (prvScenarioGroup.scenarios[i].expectedException == String.Empty) {
					v_CSVUnexpectedExceptions += (i+1).ToString() + ", ";
				}
			}

			v_CSVUnexpectedExceptions = v_CSVUnexpectedExceptions.TrimEnd(new char[] {',', ' '});
			v_CSVUnexpectedExceptions = v_CSVUnexpectedExceptions == String.Empty ? "NULL" : v_CSVUnexpectedExceptions;

			prvPLSQLBlock +=
				indent() + "WHEN OTHERS THEN" + Environment.NewLine +
				indent(1) + "IF v_CurrScenarioIndex$ IN (" + v_CSVUnexpectedExceptions + ") THEN" + Environment.NewLine +
				indent(2) +   "-- We weren't expecting an exception, so it's a problem that we have one..." + Environment.NewLine +
				indent(2) +   "logStatus$(v_CurrScenarioIndex$, 'Got Exception', sqlcode, dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
				indent(1) + "ELSE" + Environment.NewLine +
				indent(2) +   "v_SQLCode$ := sqlcode;" + Environment.NewLine +
				indent(2) +   "v_SQLErrm$ := NVL(sqlerrm, ' ');" + Environment.NewLine +
				Environment.NewLine +
				indent(2) +    "IF FALSE OR" + Environment.NewLine;
        
			v_IndentionLvl += 2;
        
			// Handle numeric or "ANY" exceptions... @@@ Need to hash here, like we used to, to cut down on the amount of generated code
			for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
				if (prvScenarioIndex == -1 || prvScenarioIndex == i) {
					if (prvScenarioGroup.scenarios[i].expectedException == "ANY"
						|| numberedException.IsMatch(prvScenarioGroup.scenarios[i].expectedException)) {
						if (prvScenarioGroup.scenarios[i].expectedException == "ANY") {
							prvPLSQLBlock +=
								indent() + "   (v_CurrScenarioIndex$ IN (" + (i+1).ToString() + ")";

							if (prvScenarioGroup.scenarios[i].expectedExceptionMessage != String.Empty) {
								prvPLSQLBlock +=
									" AND v_SQLErrm$ LIKE '%" + prvScenarioGroup.scenarios[i].expectedExceptionMessage + "%'";
							}

							prvPLSQLBlock +=
								") OR" + Environment.NewLine;
						} else {
							prvPLSQLBlock +=
								indent() + "   (v_SQLCode$ = " + prvScenarioGroup.scenarios[i].expectedException + " AND v_CurrScenarioIndex$ IN (" + (i+1).ToString() + ")";

							if (prvScenarioGroup.scenarios[i].expectedExceptionMessage != String.Empty) {
								prvPLSQLBlock +=
									" AND v_SQLErrm$ LIKE '%" + prvScenarioGroup.scenarios[i].expectedExceptionMessage + "%'";
							}

							prvPLSQLBlock +=
								") OR" + Environment.NewLine;
						}
					}
				}
			}

			if (prvPLSQLBlock.Substring(prvPLSQLBlock.Length-4) == "OR" + Environment.NewLine)
				prvPLSQLBlock = prvPLSQLBlock.Substring(0, prvPLSQLBlock.Length - 4);
        
			prvPLSQLBlock +=
				Environment.NewLine +
				indent() + "THEN" + Environment.NewLine +
				indent(1) + "logStatus$(v_CurrScenarioIndex$, 'OK', v_SQLCode$, dbms_utility.format_error_stack);" + Environment.NewLine +
				indent() + "ELSE" + Environment.NewLine +
				indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Got Wrong Exception', v_SQLCode$, 'Got ' || sqlcode || ': ' || dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
				indent() + "END IF;" + Environment.NewLine;
        
			v_IndentionLvl--;
        
			prvPLSQLBlock +=
				indent() + "END IF;" + Environment.NewLine;
          
			v_IndentionLvl--;
		}
      
		private void logIfExpectedExceptionMissing() {
			string v_CSVScenariosWithExpExcptn = String.Empty;

			for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
				if (prvScenarioGroup.scenarios[i].expectedException != String.Empty) {
					v_CSVScenariosWithExpExcptn += (i+1).ToString() + ", ";
				}
			}

			v_CSVScenariosWithExpExcptn = v_CSVScenariosWithExpExcptn.TrimEnd(new char[] {',', ' '});

			if (v_CSVScenariosWithExpExcptn != String.Empty) {
				prvPLSQLBlock += 
				indent() + "IF v_CurrScenarioIndex$ IN (" + v_CSVScenariosWithExpExcptn + ") THEN" + Environment.NewLine;

				// Pipelined result sets have to fetch all their rows to know that an exception isn't raised half-way through processing.
				// Thus, fetch all rows to see if an exception happens.
				// Requires 11g or higher, due to the special use of dbms_sql to convert a sys_refcursor to a cursor number...
				if (prvTest.isPipelinedFunction) {
					prvPLSQLBlock +=
						indent(1) +		"DECLARE" + Environment.NewLine +
						indent(2) +			"v_CursorNumber INTEGER;" + Environment.NewLine +
						indent(1) +		"BEGIN" + Environment.NewLine +
						indent(2) +			"IF " + prvTest.testArguments[v_ReturnValueParamIndex].argumentName  + "%ISOPEN THEN" + Environment.NewLine +
						indent(3) +				"-- Fetch every record to make sure the cursor doesn't raise an exception half-way through fetching its records..." + Environment.NewLine +
						indent(3) +				"v_CursorNumber:= DBMS_SQL.TO_CURSOR_NUMBER(" + prvTest.testArguments[v_ReturnValueParamIndex].argumentName + "); " + Environment.NewLine +
						Environment.NewLine +
						indent(3) +				"WHILE DBMS_SQL.FETCH_ROWS(v_CursorNumber) > 0 LOOP" + Environment.NewLine +
						indent(4) +					"NULL;" + Environment.NewLine +
						indent(3) +				"END LOOP;" + Environment.NewLine +
						Environment.NewLine +
						indent(3) +				"DBMS_SQL.CLOSE_CURSOR(v_CursorNumber);" + Environment.NewLine +
						indent(2) +			"END IF;" + Environment.NewLine +
						indent(1) +		"END;" + Environment.NewLine +
						Environment.NewLine;
				}
          
				prvPLSQLBlock +=
					indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Exception not Raised!', 0, 'This should have thrown an exception.');" + Environment.NewLine +
					indent() + "END IF;" + Environment.NewLine +
					Environment.NewLine;
			}
		}
  
		private bool hasOutputParameters() {
			bool v_HasOutParams = false;

			for (int i = 0; i < prvTest.testArguments.Count; i++) {
				if (prvTest.testArguments[i].inOut == "IN/OUT" || prvTest.testArguments[i].inOut == "OUT" || prvTest.testArguments[i].inOut == "RETURN") {
					v_HasOutParams = true;
					break;
				}
			}

			return v_HasOutParams;
		}

		private void compareActualVsExpectedResults(OracleConnection conTargetDB, scenarioParameterCollection nestedParams, int scenarioIndex, string parentVariableName = "", int parentArrayIndex = -1, scenarioParameter parentParam = null, scenarioParameter rootParam = null) {
			bool v_FoundTestableParam = false;
			string variableName = "";
			string childVariableName = "";
			string outVarExpValueVarName = "";

			for (int i = 0; i < nestedParams.Count; i++) {
				if (nestedParams[i].testArg.inOut == "RETURN" || nestedParams[i].testArg.inOut == "IN/OUT" || nestedParams[i].testArg.inOut == "OUT") {
					string comparisonType;
					string plsqlRawValue;

					if (parentParam == null) {
						variableName = (parentVariableName == "" ? "" : parentVariableName + ".") + nestedParams[i].testArg.argumentName;
						childVariableName = (parentVariableName == "" ? "" : parentVariableName + ".") + nestedParams[i].testArg.argumentName;

						//if (nestedParams[i].testArg.dataType == "TABLE") {
						//	outVarExpValueVarName = "v_OutVarExpValue_" + nestedParams[i].testArg.position.ToString() + "$";
						//} else {
							outVarExpValueVarName = "v_OutVarExpValue_" + nestedParams[i].testArg.sequence.ToString() + "$";
						//}
					} else {
						variableName = ((parentVariableName == "" ? "" : parentVariableName + ".") + (parentParam.testArg.dataType == "TABLE" ? "" : nestedParams[i].testArg.argumentName)).TrimEnd('.');
						childVariableName = ((parentVariableName == "" ? "" : parentVariableName + ".") + (parentParam.testArg.dataType == "TABLE" ? "" : nestedParams[i].testArg.argumentName)).TrimEnd('.');

						if (variableName.IndexOf('.') >= 0) {
							int dotPosition = variableName.IndexOf('.');
							int openParenPosition = variableName.IndexOf('(');

							if (openParenPosition != -1 && openParenPosition < dotPosition) {
								outVarExpValueVarName = "v_OutVarExpValue_" + rootParam.testArg.sequence.ToString() + "$" + variableName.Substring(variableName.IndexOf('('));
							} else {
								outVarExpValueVarName = "v_OutVarExpValue_" + rootParam.testArg.sequence.ToString() + "$" + "." + variableName.Substring(variableName.IndexOf('.') + 1);
							}
						} else {
							outVarExpValueVarName = "v_OutVarExpValue_" + rootParam.testArg.sequence.ToString() + "$";
						}

						if (parentParam.testArg.dataType == "TABLE") {
							variableName += "(" + parentArrayIndex.ToString() + ")";
							childVariableName += "(" + parentArrayIndex.ToString() + ")";
							outVarExpValueVarName += "(" + parentArrayIndex.ToString() + ")";
						}
					}

					if (nestedParams[i].testArg.inOut == "IN/OUT") {
						plsqlRawValue = nestedParams[i].expectedOutPLSQLRawValue;
						comparisonType = nestedParams[i].expectedOutComparisonType;
					} else {
						plsqlRawValue = nestedParams[i].plsqlRawValue;
						comparisonType = nestedParams[i].valueComparisonType;
					}

					if ((nestedParams[i].testArg.dataType == "TABLE" || nestedParams[i].testArg.dataType == "PL/SQL RECORD")
						&& !(nestedParams[i].testArg.inOut == "RETURN" && prvTest.isPipelinedFunction)
						&& (comparisonType != "is null" && comparisonType != "not null")
					)
					{
						if (nestedParams[i].testArg.dataType == "TABLE") {
							// Initialize and allocate the expected out table we'll be comparing with...
							if (comparisonType != "don't test") {
								int arraySize;

								if (nestedParams[i].testArg.inOut == "RETURN" || nestedParams[i].testArg.inOut == "OUT") {
									arraySize = nestedParams[i].nestedParameters.Count;
								} else {
									arraySize = nestedParams[i].expectedOutNestedParameters.Count;
								}

								prvPLSQLBlock +=
									indent() + outVarExpValueVarName +
										" := " + nestedParams[i].testArg.plsType + "();" + Environment.NewLine +
									indent() + outVarExpValueVarName + ".EXTEND(" + arraySize.ToString() + ");" + Environment.NewLine +
									Environment.NewLine;

								// Make sure the actual table has something in it...
								prvPLSQLBlock +=
									indent() + "IF " + variableName + " IS NULL THEN" + Environment.NewLine +
									indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Null Actual Array', 0, 'Actual array " + variableName + " we got back is null.'); " + Environment.NewLine +
									indent() + "ELSIF " + variableName + ".COUNT != " + outVarExpValueVarName + ".COUNT THEN" + Environment.NewLine +
									indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Array Size Mismatch', 0, 'Actual array " + variableName + " has ' || " + variableName + ".COUNT || ' elements; Exp array has ' || " + outVarExpValueVarName + ".COUNT || ' elements');" + Environment.NewLine +
									indent() + "ELSE" + Environment.NewLine;

								v_IndentionLvl++;
							}
						} else {
							if (comparisonType == "matrix") {
								prvPLSQLBlock +=
									indent() + "logStatus$(v_CurrScenarioIndex$, 'Test Mismatch', 0, 'Cannot test parameter " + nestedParams[i].testArg.argumentName + " with a Comparison Type of matrix; use nested instead.');" + Environment.NewLine +
									Environment.NewLine;
							}
						}
					} else {
						prvPLSQLBlock +=
							indent() + "-- Check the " + variableName + " actual return value, using a comparison type of *" + comparisonType + "*..." + Environment.NewLine;

						// See if this is a pipelined result set's return plsqlValue or a parameter that is a sys_refcursor...
						if ((nestedParams[i].testArg.inOut == "RETURN" && prvTest.isPipelinedFunction)
							|| ((nestedParams[i].testArg.inOut == "IN/OUT" || nestedParams[i].testArg.inOut == "OUT" || nestedParams[i].testArg.inOut == "RETURN") && nestedParams[i].testArg.plsType == "SYS_REFCURSOR")) {

							if (comparisonType == "don't test") {
								// Even if the user tells us to ignore the ref cursor, certain
								// Oracle errors only happen if you call %ISOPEN, so do so...
								prvPLSQLBlock +=
									indent() + "BEGIN" + Environment.NewLine +
									indent(1) + "IF " + variableName + "%ISOPEN THEN" + Environment.NewLine +
									indent(2) + "NULL; -- Do nothing" + Environment.NewLine +
									indent(1) + "END IF;" + Environment.NewLine +

									indent() + "EXCEPTION" + Environment.NewLine +
									indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
									Environment.NewLine +
									indent(2) + "logStatus$(v_CurrScenarioIndex$, 'Cursor Not Open', sqlcode, dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
									indent() + "END;" + Environment.NewLine +
									Environment.NewLine;
							} else {
								v_FoundTestableParam = true;

								switch (comparisonType) {
									case "matrix":
										// Compare the expected results vs. the actual results.
										prvPLSQLBlock +=
											indent() + "cmpVsMtxSCN_" + scenarioIndex.ToString() + "_P_" + nestedParams[i].testArg.sequence.ToString() + (nestedParams[i].testArg.inOut == "IN/OUT" ? "_EXP_OUT" : String.Empty) + "$(" +
											"p_ScenarioIndex$ => v_CurrScenarioIndex$, " +
											"p_ActualResults => " + variableName + ", " +
											"p_OutBuiltExpResults => " + c_ARR_OUT_PRM_EXP_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + ", " +
											"p_OutBuiltActResults => " + c_ARR_OUT_PRM_ACT_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + ");" + Environment.NewLine;
										break;

									case "nested":
										prvPLSQLBlock +=
											indent() + "cmpVsMtxSCN_" + scenarioIndex.ToString() + "_P_" + nestedParams[i].testArg.sequence.ToString() + (nestedParams[i].testArg.inOut == "IN/OUT" ? "_EXP_OUT" : String.Empty) + "$(" +
											"p_ScenarioIndex$ => v_CurrScenarioIndex$, " +
											"p_ActualResults => " + variableName + ", " +
											"p_OutBuiltExpResults => " + c_ARR_OUT_PRM_EXP_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + ", " +
											"p_OutBuiltActResults => " + c_ARR_OUT_PRM_ACT_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + ");" + Environment.NewLine;

										break;

									case "no rows":
										// Make sure the actual result doesn't have any rows in it.

										// Requires 11g or higher, due to the special use of dbms_sql to convert a sys_refcursor to a cursor number...
										prvPLSQLBlock +=
											indent() + "DECLARE" + Environment.NewLine +
											indent(1) + "v_CursorNumber INTEGER;" + Environment.NewLine +
											indent() + "BEGIN" + Environment.NewLine +
											indent(1) + "IF " + variableName + "%ISOPEN THEN" + Environment.NewLine +
											indent(2) + "v_CursorNumber:= DBMS_SQL.TO_CURSOR_NUMBER(" + variableName + "); " + Environment.NewLine +
											Environment.NewLine +
											indent(2) + "IF DBMS_SQL.FETCH_ROWS(v_CursorNumber) > 0 THEN" + Environment.NewLine +
											indent(3) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', 0, '" + variableName + " cursor had rows when it shouldn''t have had any.');" + Environment.NewLine +
											indent(2) + "END IF;" + Environment.NewLine +
											Environment.NewLine +
											indent(2) + "DBMS_SQL.CLOSE_CURSOR(v_CursorNumber);" + Environment.NewLine +
											indent(1) + "END IF;" + Environment.NewLine +
											indent() + "EXCEPTION" + Environment.NewLine +
											indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
											indent(2) + "IF " + variableName + "%ISOPEN THEN" + Environment.NewLine +
											indent(3) + "CLOSE " + variableName + ";" + Environment.NewLine +
											indent(2) + "END IF;" + Environment.NewLine +
											Environment.NewLine +
											indent(2) + "logStatus$(v_CurrScenarioIndex$, 'Got Exception', sqlcode, dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
											indent() + "END;" + Environment.NewLine +
											Environment.NewLine;

										break;

									case "some rows":
										// Make sure the actual results actually have some rows in it....

										// Requires 11g or higher, due to the special use of dbms_sql to convert a sys_refcursor to a cursor number...
										prvPLSQLBlock +=
											indent() + "DECLARE" + Environment.NewLine +
											indent(1) + "v_HasRows      BOOLEAN := FALSE;" + Environment.NewLine +
											indent(1) + "v_CursorNumber INTEGER;" + Environment.NewLine +
											indent() + "BEGIN" + Environment.NewLine +
											indent(1) + "IF " + variableName + "%ISOPEN THEN" + Environment.NewLine +
											indent(2) + "-- Fetch every record to make sure the cursor doesn't crash:" + Environment.NewLine +
											indent(2) + "v_CursorNumber:= DBMS_SQL.TO_CURSOR_NUMBER(" + variableName + "); " + Environment.NewLine +
											Environment.NewLine +
											indent(2) + "WHILE DBMS_SQL.FETCH_ROWS(v_CursorNumber) > 0 LOOP" + Environment.NewLine +
											indent(3) + "v_HasRows := TRUE;" + Environment.NewLine +
											indent(2) + "END LOOP;" + Environment.NewLine +
											Environment.NewLine +
											indent(2) + "DBMS_SQL.CLOSE_CURSOR(v_CursorNumber);" + Environment.NewLine +
											indent(1) + "END IF;" + Environment.NewLine +
											Environment.NewLine +
											indent(1) + "IF v_HasRows = FALSE THEN" + Environment.NewLine +
											indent(2) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', 0, '" + variableName + " cursor didn''t have any rows in it, but should have.');" + Environment.NewLine +
											indent(1) + "END IF;" + Environment.NewLine +
											indent() + "EXCEPTION" + Environment.NewLine +
											indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
											indent(2) + "IF " + variableName + "%ISOPEN THEN" + Environment.NewLine +
											indent(3) + "CLOSE " + variableName + ";" + Environment.NewLine +
											indent(2) + "END IF;" + Environment.NewLine +
											Environment.NewLine +
											indent(2) + "logStatus$(v_CurrScenarioIndex$, 'Got Exception', sqlcode, dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
											indent() + "END;" + Environment.NewLine +
											Environment.NewLine;

										break;

									case "exp":
										prvPLSQLBlock +=
											indent() + "BEGIN" + Environment.NewLine +
											indent(1) + "compareCursors$(p_ActualResults => " + variableName + ", p_ExpectedResults => " + plsqlRawValue + ");" + Environment.NewLine +
											indent() + "EXCEPTION" + Environment.NewLine +
											indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
											indent(2) + "logStatus$(v_CurrScenarioIndex$, 'Got Exception', sqlcode, 'Parameter " + variableName + " Failed: ' || dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
											indent() + "END;" + Environment.NewLine;
										break;

									case "select":
										// Compare the given SELECT statement's expected results with the actual results via cursor comparison.
										prvPLSQLBlock +=
											indent() + "OPEN " + outVarExpValueVarName + " FOR" + Environment.NewLine +
												plsqlRawValue.TrimEnd(';') + ";" + Environment.NewLine +
											Environment.NewLine +
											indent() + "-- Compare expected ref cursor with the actual ref cursor..." + Environment.NewLine +
											indent() + "BEGIN" + Environment.NewLine +
											indent(1) + "compareCursors$(p_ExpectedResults => " + outVarExpValueVarName + ", p_ActualResults => " + variableName + ");" + Environment.NewLine +
											indent() + "EXCEPTION" + Environment.NewLine +
											indent(1) + "WHEN OTHERS THEN" + Environment.NewLine +
											indent(2) + "logStatus$(v_CurrScenarioIndex$, 'Got Exception', sqlcode, 'Parameter " + variableName + " Failed: ' || dbms_utility.format_error_stack || ' ' || dbms_utility.format_error_backtrace);" + Environment.NewLine +
											indent() + "END;" +
											Environment.NewLine;
										break;

									case "is null":
										prvPLSQLBlock +=
											indent() + "IF " + variableName + " IS NOT NULL THEN" + Environment.NewLine +
											indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', sqlcode, 'Parameter " + variableName + " Failed: Expected NULL, but got non-NULL.');" + Environment.NewLine +
											indent() + "END IF;" + Environment.NewLine;
										break;

									case "not null":
										prvPLSQLBlock +=
											indent() + "IF " + variableName + " IS NULL THEN" + Environment.NewLine +
											indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', sqlcode, 'Parameter " + variableName + " Failed: Expected non-NULL, but got NULL.');" + Environment.NewLine +
											indent() + "END IF;" + Environment.NewLine;
										break;

									default:
										throw new Exception("In scenario #" + scenarioIndex + ", " + comparisonType + " is not a valid comparison type in column " + variableName + "!");
								}
							}
						} else {
							if (comparisonType != "don't test") {
								v_FoundTestableParam = true;

								if ((comparisonType == "exp" || comparisonType == "value")
									&& (",VARCHAR2,CLOB,CHAR,VARCHAR,NCHAR,NVARCHAR2,LONG,RAW,LONG RAW,NCLOB,".IndexOf("," + nestedParams[i].testArg.plsType + ",") >= 0)) {
									if (comparisonType == "value") {
										prvPLSQLBlock +=
											indent() + outVarExpValueVarName + " := REPLACE(" + plsqlRawValue + ", CHR(10), CHR(13) || CHR(10));" + Environment.NewLine +
											Environment.NewLine;
									} else {
										prvPLSQLBlock +=
											indent() + outVarExpValueVarName + " := " + plsqlRawValue + ";" + Environment.NewLine +
											Environment.NewLine;
									}

									prvPLSQLBlock +=
										indent() + c_ARR_OUT_PRM_EXP_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + " := TO_CLOB(" + outVarExpValueVarName + ");" + Environment.NewLine;
								} else {
									prvPLSQLBlock +=
										indent() + outVarExpValueVarName + " := " + plsqlRawValue + ";" + Environment.NewLine +
										Environment.NewLine;

									if (comparisonType == "exp") {
										if (nestedParams[i].testArg.plsType == "BOOLEAN") {
											prvPLSQLBlock +=
												indent() + c_ARR_OUT_PRM_EXP_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + " := boolToChar(" +
												plsqlRawValue +
												");" + Environment.NewLine;
										} else {
											prvPLSQLBlock +=
												indent() + c_ARR_OUT_PRM_EXP_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + " := TO_CLOB(" +
												plsqlRawValue +
												");" + Environment.NewLine;
										}
									} else {
										prvPLSQLBlock +=
											indent() + c_ARR_OUT_PRM_EXP_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + " := '" +
											plsqlRawValue.Replace("'", "''") +
											"';" + Environment.NewLine;
									}
								}

								switch (comparisonType) {
									case "is null":
										prvPLSQLBlock +=
											indent() + c_ARR_OUT_PRM_ACT_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + " := '<is null>';" + Environment.NewLine +
											Environment.NewLine +
											indent() + "IF " + variableName + " IS NOT NULL THEN" + Environment.NewLine +
											indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', 0, '" + variableName + ": Expected a NULL return value, but got a non-NULL instead.');" + Environment.NewLine +
											indent() + "END IF;" + Environment.NewLine;
										break;
									case "not null":
										prvPLSQLBlock +=
											indent() + c_ARR_OUT_PRM_ACT_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + " := '<isn''t null>';" + Environment.NewLine +
											Environment.NewLine +
											indent() + "IF " + variableName + " IS NULL THEN" + Environment.NewLine +
											indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', 0, '" + variableName + ": Expected a non-NULL return value, but got a NULL instead.');" + Environment.NewLine +
											indent() + "END IF;" + Environment.NewLine;
										break;

									default:
										// Log the actual plsqlValue....
										if (nestedParams[i].testArg.plsType == "BOOLEAN") {
											prvPLSQLBlock +=
												indent() + c_ARR_OUT_PRM_ACT_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + " := boolToChar(" + variableName + ");" + Environment.NewLine +
												Environment.NewLine;
										} else {
											prvPLSQLBlock +=
												indent() + c_ARR_OUT_PRM_ACT_VAL_VAR_NAME + scenarioIndex.ToString() + "_" + nestedParams[i].testArg.sequence.ToString() + " := TO_CLOB(" + variableName + ");" + Environment.NewLine +
												Environment.NewLine;
										}

										prvPLSQLBlock +=
											indent() + "IF " + variableName + " != " + outVarExpValueVarName + " OR (" + outVarExpValueVarName + " IS NULL AND " + variableName + " IS NOT NULL) OR (" + variableName + " IS NULL AND " + outVarExpValueVarName + " IS NOT NULL) THEN" + Environment.NewLine;

										if (nestedParams[i].testArg.plsType == "BOOLEAN") {
											prvPLSQLBlock +=
												indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', 0, " +
												"'" + variableName + ": Expected ' || boolToChar(" + outVarExpValueVarName + ") || ', but got ' || boolToChar(" + variableName + ") || ', instead.');" + Environment.NewLine;
										} else {
											string diffmeFilename = "SCN_" + scenarioIndex + "_P_" + nestedParams[i].testArg.sequence.ToString();

											if (comparisonType == "exp") {
												prvPLSQLBlock +=
													indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', 0, " +
													"'" + variableName + ": Expected vs. actual difference: www.diffme.com?guid=' || v_arrGUIDs$(v_CurrScenarioIndex$) || '_" + diffmeFilename + "');" + Environment.NewLine;
											} else {
												prvPLSQLBlock +=
													indent(1) + "logStatus$(v_CurrScenarioIndex$, 'Ret Val Prob', 0, ";

												if (plsqlRawValue == String.Empty) {
													prvPLSQLBlock +=
														"'" + variableName + ": Expected NULL, but got \"' || " + variableName + " || '\", instead.');";
												} else {
													// If the expected plsqlValue is short, show it inline; otherwise, use a file differ.
													if (plsqlRawValue.Length <= 20 && plsqlRawValue.IndexOf((char)10) < 0) {
														prvPLSQLBlock +=
															"'" + variableName + ": Expected ' || " + outVarExpValueVarName + " || ', but got \"' || " + variableName + " || '\" instead.');";
													} else {
														prvPLSQLBlock +=
															"'" + variableName + ": Expected vs. actual difference: www.diffme.com?guid=' || v_arrGUIDs$(v_CurrScenarioIndex$) || '_" + diffmeFilename + "');";
													}

													prvPLSQLBlock +=
														Environment.NewLine;
												}
											}
										}

										prvPLSQLBlock +=
											indent() + "END IF;" + Environment.NewLine;
										break;
								}
							}
						}

						prvPLSQLBlock +=
							Environment.NewLine;
					}

					if (comparisonType == "nested"
						&& nestedParams[i].testArg.dataType != "REF CURSOR"
						&& !(nestedParams[i].testArg.inOut == "RETURN" && prvTest.isPipelinedFunction)) {
						int j = 0;
						List<scenarioParameterCollection> lstNestedParams = (nestedParams[i].testArg.inOut == "IN/OUT") ? nestedParams[i].expectedOutNestedParameters : nestedParams[i].nestedParameters;

						foreach (scenarioParameterCollection childParams in lstNestedParams) {
							compareActualVsExpectedResults(conTargetDB: conTargetDB,
								nestedParams: childParams,
								scenarioIndex: scenarioIndex,
								parentVariableName: childVariableName,
								parentParam: nestedParams[i],
								parentArrayIndex: j++ + 1,
								rootParam: (rootParam == null ? nestedParams[i] : rootParam)
							);
						}

						if (nestedParams[i].testArg.dataType == "TABLE") {
							v_IndentionLvl--;

							prvPLSQLBlock +=
								indent() + "END IF;" + Environment.NewLine +
								Environment.NewLine;
						}
					}
				}
			}

			if (v_FoundTestableParam == false) {
				prvPLSQLBlock +=
					indent() + "NULL; -- No testable param" + Environment.NewLine;
			}
		}

		private void testReturnValues(OracleConnection conTargetDB) {
			bool v_FoundScenarioToTest = false;

			// Check function return values
			if (v_MethodIsFunction || hasOutputParameters()) {
				prvPLSQLBlock +=
					//indent() + "<<rt_return_value_block>>" + Environment.NewLine +
					indent() + "DECLARE" + Environment.NewLine +
					indent(1) + "-- Declare variables to cache the expected values..." + Environment.NewLine;

				for (int i = 0; i < prvTest.testArguments.Count; i++) {
					if (prvTest.isPipelinedFunction && prvTest.testArguments[i].inOut == "RETURN") {
						prvPLSQLBlock +=
							indent(1) + "v_OutVarExpValue_" + prvTest.testArguments[i].sequence.ToString() + "$ SYS_REFCURSOR;" + Environment.NewLine;
					} else {
						if (prvTest.testArguments[i].inOut == "RETURN"
							|| prvTest.testArguments[i].inOut == "OUT"
							|| prvTest.testArguments[i].inOut == "IN/OUT") {
							prvPLSQLBlock +=
								indent(1) +
								"v_OutVarExpValue_" + prvTest.testArguments[i].sequence.ToString() + "$ " +
								prvTest.testArguments[i].getPLSQLTypeDeclaration() +
								"; -- " + prvTest.testArguments[i].argumentName + Environment.NewLine;
						}
					}
				}

				prvPLSQLBlock +=
					indent() + "BEGIN" + Environment.NewLine;

				v_IndentionLvl++;

				for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
					if (hasOutputParameters()) {
						if (v_FoundScenarioToTest == false) {
							prvPLSQLBlock +=
								indent() + "-- Compare actual vs. expected return values, if we didn't have an unexpected exception..." + Environment.NewLine +
								indent() + "IF " + c_ARR_STATUSES_VAR_NAME + "(v_CurrScenarioIndex$) = 'OK' THEN" + Environment.NewLine +
								indent(1) + "-- Test output parameters/return values..." + Environment.NewLine +
								indent(1) + "CASE v_CurrScenarioIndex$" + Environment.NewLine;

							v_FoundScenarioToTest = true;
						}

						prvPLSQLBlock +=
							indent(1) + "WHEN " + (i+1).ToString() + " THEN" + Environment.NewLine;

						v_IndentionLvl += 2;
						
						compareActualVsExpectedResults(conTargetDB: conTargetDB, nestedParams: prvScenarioGroup.scenarios[i].parameters, scenarioIndex: i);

						v_IndentionLvl -= 2;
					}
				}

				if (v_FoundScenarioToTest) {
					prvPLSQLBlock +=
						indent(1) + "ELSE NULL;" + Environment.NewLine +
						indent(1) + "END CASE;" + Environment.NewLine;
				}

				prvPLSQLBlock +=
					indent() + "END IF;" + Environment.NewLine;

				v_IndentionLvl--;

				prvPLSQLBlock +=
					indent() + "END;" + Environment.NewLine;
			}
		}
      
		private void callAnyUDCs() {
			string csvScenariosWithUDCs = String.Empty;

			if (prvScenarioGroup.udcCollection.Count > 0) {
				for (int i = 0; i < prvScenarioGroup.scenarios.Count; i++) {
					// Generate the code for the UDC's if we weren't expecting an exception
					if (prvScenarioGroup.scenarios[i].expectedException == String.Empty) {
						csvScenariosWithUDCs += (i + 1).ToString() + ",";
					}
				}

				csvScenariosWithUDCs = csvScenariosWithUDCs.TrimEnd(new char[] { ',' });

				if (csvScenariosWithUDCs != String.Empty) {
					prvPLSQLBlock +=
						Environment.NewLine +
						indent() + "IF v_CurrScenarioIndex$ IN (" + csvScenariosWithUDCs + ") THEN" + Environment.NewLine +
						indent(1) + "callUDCs$(p_ScenarioIndex$ => v_CurrScenarioIndex$);" + Environment.NewLine +
						indent() + "END IF;" + Environment.NewLine;
				}
			}
		}
  
		private void callTeardownProcedures() {
			if (prvScenarioGroup.scenarioTeardown != String.Empty) {
				prvPLSQLBlock +=
					Environment.NewLine +
					indent() + "userScenarioTeardown$(p_ScenarioIndex$ => v_CurrScenarioIndex$, p_ScenarioGUID$ => v_arrGUIDs$(v_CurrScenarioIndex$));" + Environment.NewLine;
			}
        
			prvPLSQLBlock +=
				Environment.NewLine +
				indent() + "scenarioTeardown$(p_ScenarioIndex$ => v_CurrScenarioIndex$);" + Environment.NewLine +
				Environment.NewLine;
		}

		private void getScenarioGroupBlock(OracleConnection conTargetDB) {      
			v_IndentionLvl++;
      
			prvPLSQLBlock +=
				indent() + "FOR v_CurrScenarioIndex$ IN 1 .. " + prvScenarioGroup.scenarios.Count.ToString() + " LOOP" + Environment.NewLine;

			v_IndentionLvl++;
      
			callStartupProcedures();

			if (prvScenarioIndex != -1) {
				prvPLSQLBlock +=
					indent(1) + "IF v_CurrScenarioIndex$ != " + prvScenarioIndex + " AND v_CurrScenarioIndex$ = " + prvScenarioIndex + " THEN" + Environment.NewLine +
					indent(2) +		"CONTINUE;" + Environment.NewLine +
					indent(1) + "END IF;" + Environment.NewLine +
					Environment.NewLine;
			}
      
			// Build CASE statement to Assign parameter values by scenario index
			assignInputVariables();
      
			callUserPostParamAssign();
          
			// Build CASE statement to run the hashed unit calls
			callTargetUnits();
            
			// Log an error if it should have raised an exception, but didn't...
			logIfExpectedExceptionMissing();
      
			v_IndentionLvl--;
      
			prvPLSQLBlock += 
				indent() + "EXCEPTION" + Environment.NewLine;
      
			v_IndentionLvl++;
      
			catchAnyRunException();
      
			v_IndentionLvl -= 2;
      
			prvPLSQLBlock += 
				indent(1) + "END;" + Environment.NewLine +
				indent() + "END IF;" + Environment.NewLine +
				Environment.NewLine;
      
			testReturnValues(conTargetDB: conTargetDB);
      
			callAnyUDCs();
            
			callTeardownProcedures();
      
			v_IndentionLvl--;
      
			prvPLSQLBlock += 
				indent() + "END LOOP;" + Environment.NewLine +
				Environment.NewLine;
      
			v_IndentionLvl--;
		}
		#endregion

		#region Footer
		private void getScenarioGroupFooter() {
			if (prvScenarioGroup.scenarioGroupTeardown != String.Empty) {
				prvPLSQLBlock += 
					Environment.NewLine +
					indent(1) + "userScenarioGroupTeardown$;" + Environment.NewLine +
					Environment.NewLine;
			}

			prvPLSQLBlock +=
				Environment.NewLine +
				indent(1) + (prvGenerateAsDebugBlock ? "-- " : String.Empty) + "ROLLBACK;" + Environment.NewLine;
      
			prvPLSQLBlock +=
				indent(1) + "sys.dbms_session.modify_package_state(sys.dbms_session.FREE_ALL_RESOURCES);" + Environment.NewLine;
      
			prvPLSQLBlock +=
				indent() + "END;" + Environment.NewLine;
	  
			v_IndentionLvl = v_IndentionLvl - 1;
			prvPLSQLBlock +=
				indent() + "END;" + Environment.NewLine;
        
			if (prvGenerateAsDebugBlock) {
				prvPLSQLBlock +=
					indent() + "/" + Environment.NewLine +
					Environment.NewLine +
					"ROLLBACK;";
			}
		}
		#endregion

		public string getPLSQLRunBlock(OracleConnection conTargetDB, scenarioGroup scnGroup, string targetDBName, Boolean generateAsDebugBlock = false, int scenarioIndex = -1) {
			prvScenarioGroup = scnGroup;
			prvTest = scnGroup.test;
			prvAllTestArgs = prvTest.getAllArgs();

			prvGenerateAsDebugBlock = generateAsDebugBlock;
			prvScenarioIndex = scenarioIndex;

			prvPLSQLBlock = String.Empty;
			
			// Use appropriate variable names, based on whether the caller will be binding or not...
			c_ARR_STATUSES_VAR_NAME        = (prvGenerateAsDebugBlock ? "" : ":") + "v_arrStatuses";
			c_ARR_ERROR_NUMBERS_VAR_NAME   = (prvGenerateAsDebugBlock ? "" : ":") + "v_arrErrorNumbers";
			c_ARR_ERROR_MSGS_VAR_NAME      = (prvGenerateAsDebugBlock ? "" : ":") + "v_arrErrorMessages";
			c_ARR_OUT_PRM_EXP_VAL_VAR_NAME = (prvGenerateAsDebugBlock ? "" : ":") + "v_arrOutPrmsExpValue_";
			c_ARR_OUT_PRM_ACT_VAL_VAR_NAME = (prvGenerateAsDebugBlock ? "" : ":") + "v_arrOutPrmsActValue_";
			c_ARR_OUT_UDC_EXP_VAL_VAR_NAME = (prvGenerateAsDebugBlock ? "" : ":") + "v_arrOutUDCExpValue_";
			c_ARR_OUT_UDC_ACT_VAL_VAR_NAME = (prvGenerateAsDebugBlock ? "" : ":") + "v_arrOutUDCActValue_";

			v_MethodIsFunction = false;
			v_ReturnValueParamIndex = -1;
			v_MaxParamNameLen = 0;

			v_IndentionLvl = 0;
  
			getScenarioGroupHeader(conTargetDB: conTargetDB);
    
			getScenarioGroupBlock(conTargetDB: conTargetDB);
    
			getScenarioGroupFooter();
  
			return prvPLSQLBlock;
		}
	}
}
