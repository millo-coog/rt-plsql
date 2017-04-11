using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using Oracle.DataAccess.Client;

namespace RT.Controls {
	public partial class matrix : UserControl {
		private DataTable prvDTMatrix;
		private List<scenarioParameterCollection> prvNestedParams;
		public test myTest;
		public testArgument testArg = new testArgument();
		public bool isINParameter;

		public matrix() {
			InitializeComponent();

			grdMatrix.DataSource = prvDTMatrix;
			isINParameter = false;

			toggleComparisonTypeColumns();
		}

		public DataGridView gridView {
			get { return grdMatrix; }
		}

		public List<scenarioParameterCollection> dataSourceNestedParams {
			get {
				return prvNestedParams;
			}
			set {
				prvNestedParams = value;

				if (prvNestedParams != null) {
					prvDTMatrix = new DataTable();

					foreach (scenarioParameter scnParam in prvNestedParams[0]) {
						prvDTMatrix.Columns.Add(scnParam.testArg.argumentName);
						prvDTMatrix.Columns.Add(scenarioParameter.getMatrixColumnComparisonTypeColName(scnParam.testArg.argumentName));
					}

					foreach (scenarioParameterCollection row in prvNestedParams) {
						object[] arrParams = new object[row.Count * 2];

						int i = 0;
						foreach (scenarioParameter param in row) {
							if (isINParameter) {
								arrParams[i++] = param.value;
								arrParams[i++] = param.valueComparisonType;
							} else {
								arrParams[i++] = param.expectedOutValue;
								arrParams[i++] = param.expectedOutComparisonType;
							}
						}

						prvDTMatrix.Rows.Add(arrParams);
					}

					// Create the gridview
					grdMatrix.AutoGenerateColumns = false;
					grdMatrix.Columns.Clear();

					foreach (testArgument arg in testArg.childArguments) {
						// Add the column containing the actual plsqlValue expression...
						grdMatrix.Columns.Add(createValueColumn(arg.argumentName));

						// Add a dropdown box column for the comparison type...
						grdMatrix.Columns.Add(createComparisonTypeColumn(arg.argumentName));
					}
					
					grdMatrix.DataSource = prvDTMatrix;

					// Disable unapplicable buttons
					btnAddColumn.Enabled =
					btnDropColumn.Enabled =
					btnRenameColumn.Enabled = false;

					toggleComparisonTypeColumns();
				}
			}
		}

		public DataTable DataSourceDataTable {
			get { return prvDTMatrix; }
			set {
				grdMatrix.AutoGenerateColumns = false;
				grdMatrix.Columns.Clear();

				prvDTMatrix = value;

				if (prvDTMatrix != null) {
					// Create default columns in the datatable, if we were given an empty matrix...
					if (prvDTMatrix.Columns.Count == 0) {
						createDefaultColumns();
					}

					// Create columns in the gridview...
					switch (testArg.dataType) {
						case "TABLE":
							if (testArg.childArguments.Count == 1 && testArg.childArguments[0].dataType == "PL/SQL RECORD") {
								foreach (testArgument arg in testArg.childArguments[0].childArguments) {
									// Add the column containing the actual plsqlValue expression...
									grdMatrix.Columns.Add(createValueColumn(arg.argumentName));

									// Add a dropdown box column for the comparison type...
									grdMatrix.Columns.Add(createComparisonTypeColumn(arg.argumentName));
								}
							} else {
								foreach (testArgument arg in testArg.childArguments) {
									// Add the column containing the actual plsqlValue expression...
									grdMatrix.Columns.Add(createValueColumn(arg.argumentName));

									// Add a dropdown box column for the comparison type...
									grdMatrix.Columns.Add(createComparisonTypeColumn(arg.argumentName));
								}
							}

							break;

						case "PL/SQL RECORD":
							foreach (testArgument arg in testArg.childArguments) {
								// Add the column containing the actual plsqlValue expression...
								grdMatrix.Columns.Add(createValueColumn(arg.argumentName));

								// Add a dropdown box column for the comparison type...
								grdMatrix.Columns.Add(createComparisonTypeColumn(arg.argumentName));
							}
							
							break;

						default:
							for (int i = 0; i < prvDTMatrix.Columns.Count; i += 2) {
								// Add the column containing the actual plsqlValue expression...
								grdMatrix.Columns.Add(createValueColumn(prvDTMatrix.Columns[i].ColumnName));

								// Add a dropdown box column for the comparison type...
								grdMatrix.Columns.Add(createComparisonTypeColumn(prvDTMatrix.Columns[i].ColumnName));
							}

							break;
					}
				}
				
				grdMatrix.DataSource = prvDTMatrix;

				// Enable unapplicable buttons
				btnAddColumn.Enabled =
				btnDropColumn.Enabled =
				btnRenameColumn.Enabled = true;

				toggleComparisonTypeColumns();
			}
		}
		public XmlNode DataSourceXmlNode {
			get { return (XmlNode)grdMatrix.DataSource; }
			set { grdMatrix.DataSource = value; }
		}

		public void saveNestedParams() {
			// Put what's in the gridview back into the nested params...
			if (prvNestedParams != null) {
				foreach (DataGridViewRow row in grdMatrix.Rows) {
				
					if (row.IsNewRow == false) {
						int fieldIndex = 0;

						for (int i = 0; i < row.Cells.Count; i += 2) {
							if (row.Index > prvNestedParams.Count - 1)
								prvNestedParams.Add(prvNestedParams[row.Index - 1].clone());

							if (isINParameter) {
								prvNestedParams[row.Index][fieldIndex].value = (row.Cells[i].Value == null ? "" : row.Cells[i].Value.ToString());
								prvNestedParams[row.Index][fieldIndex].valueComparisonType = row.Cells[i + 1].Value.ToString();
							} else {
								prvNestedParams[row.Index][fieldIndex].expectedOutValue = (row.Cells[i].Value == null ? "" : row.Cells[i].Value.ToString());
								prvNestedParams[row.Index][fieldIndex].expectedOutComparisonType = row.Cells[i + 1].Value.ToString();
							}

							fieldIndex++;
						}
					}
				}

				if (prvNestedParams.Count > grdMatrix.Rows.Count - 1) {
					prvNestedParams.RemoveRange(index: grdMatrix.Rows.Count - 1, count: prvNestedParams.Count - grdMatrix.Rows.Count + 1);
				}
			}
		}

		// Creates default columns in the matrix for the given test's parameter
		public void createDefaultColumns() {
			switch (testArg.dataType) {
			case "OBJECT":
				// This parameter's underlying output type is an SQL object.
				OracleCommand cmdGetObjectsFields = new OracleCommand(@"
					SELECT dba_type_attrs.attr_name
						FROM dba_type_attrs
						WHERE dba_type_attrs.owner || '.' || dba_type_attrs.type_name = :p_TypeName
						ORDER BY dba_type_attrs.attr_no",
				Program.getCurrentTargetDBConnection());

				cmdGetObjectsFields.BindByName = true;
				cmdGetObjectsFields.Parameters.Add("p_TypeName", OracleDbType.Varchar2, testArg.plsType, ParameterDirection.Input);

				OracleDataReader drObjectsFields = cmdGetObjectsFields.ExecuteReader();

				while (drObjectsFields.Read()) {
					prvDTMatrix.Columns.Add(drObjectsFields["attr_name"].ToString());
					prvDTMatrix.Columns.Add(scenarioParameter.getMatrixColumnComparisonTypeColName(drObjectsFields["attr_name"].ToString()));
				}

				drObjectsFields.Close();
				drObjectsFields.Dispose();

				cmdGetObjectsFields.Dispose();

				break;

			case "TABLE":
				if (testArg.childArguments.Count == 1 && testArg.childArguments[0].dataType == "PL/SQL RECORD") {
					foreach (testArgument arg in testArg.childArguments[0].childArguments) {
						prvDTMatrix.Columns.Add(arg.argumentName, typeof(DataTable));
						prvDTMatrix.Columns.Add(scenarioParameter.getMatrixColumnComparisonTypeColName(arg.argumentName));
					}
				} else {
					foreach (testArgument arg in testArg.childArguments) {
						prvDTMatrix.Columns.Add(arg.argumentName);
						prvDTMatrix.Columns.Add(scenarioParameter.getMatrixColumnComparisonTypeColName(arg.argumentName));
					}
				}

				break;

			case "PL/SQL RECORD":
				foreach (testArgument arg in testArg.childArguments) {
					prvDTMatrix.Columns.Add(arg.argumentName);
					prvDTMatrix.Columns.Add(scenarioParameter.getMatrixColumnComparisonTypeColName(arg.argumentName));
				}

				break;

			default:
				// The underlying type is a simple scalar (NUMBER, VARCHAR2)...
				prvDTMatrix.Columns.Add("column_value");
				prvDTMatrix.Columns.Add(scenarioParameter.getMatrixColumnComparisonTypeColName("column_value"));

				break;
			}
		}

		// Creates and returns a new plsqlValue column...
		public DataGridViewTextBoxColumn createValueColumn(string columnName) {
			DataGridViewTextBoxColumn valueColumn = new DataGridViewTextBoxColumn();

			valueColumn.HeaderText = columnName;
			valueColumn.DataPropertyName = columnName;
			valueColumn.Name = columnName;

			if (testArg.inOut == "IN" || (testArg.inOut == "IN/OUT" && isINParameter)) {
				valueColumn.DefaultCellStyle.BackColor = Program.PARAMETER_COLOR;
			} else {
				valueColumn.DefaultCellStyle.BackColor = Program.RETURN_VALUE_COLOR;
			}

			return valueColumn;
		}

		// Creates and returns a new comparison type column...
		public DataGridViewComboBoxColumn createComparisonTypeColumn(string originalColumnName) {
			object[] arrComparisonTypes;
			string fieldName;

			if (isINParameter)
				arrComparisonTypes = new object[] { "value", "exp", "don't test", "is null", "not null", "matrix", "nested" };
			else
				arrComparisonTypes = new object[] { "value", "exp", "don't test", "is null", "not null", "matrix", "nested" };

			fieldName = scenarioParameter.getMatrixColumnComparisonTypeColName(columnName: originalColumnName);

			DataGridViewComboBoxColumn comparisonTypeColumn = new DataGridViewComboBoxColumn();
			comparisonTypeColumn.DataPropertyName = fieldName;
			comparisonTypeColumn.Name = fieldName;
			comparisonTypeColumn.HeaderText = "...";
			comparisonTypeColumn.Items.AddRange(arrComparisonTypes);
			comparisonTypeColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			comparisonTypeColumn.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 255, 224);

			comparisonTypeColumn.Visible = chkShowComparisonTypeColumns.Checked;

			return comparisonTypeColumn;
		}

		// Adds a new user column and companion comparison type column to the matrix.
		private void btnAddColumn_Click(object sender, EventArgs e) {
			if (DataSourceDataTable == null) {
				MessageBox.Show("It's null!"); // Should never happen.
			} else {
				String newColumnName = "Column_" + (DataSourceDataTable.Columns.Count / 2 + 1).ToString();
				DialogResult result;

				result = InputBox.Prompt("Add Column", "Please enter the name of the new column:", ref newColumnName);

				if (result == System.Windows.Forms.DialogResult.OK && newColumnName.Trim() != String.Empty) {
					string comparisonColumnName = scenarioParameter.getMatrixColumnComparisonTypeColName(columnName: newColumnName);

					DataSourceDataTable.Columns.Add(newColumnName);
					DataSourceDataTable.Columns.Add(comparisonColumnName);

					// Default the new comparison type column...
					for (int i = 0; i < DataSourceDataTable.Rows.Count; i++) {
						prvDTMatrix.Rows[i].SetField(comparisonColumnName, "value");
					}

					grdMatrix.Columns.Add(createValueColumn(newColumnName));
					grdMatrix.Columns.Add(createComparisonTypeColumn(newColumnName));

					// Need to create a row, if there isn't one - otherwise, we get a null-reference exception below on CurrentRow.
					if (DataSourceDataTable.Rows.Count == 0) {
						DataSourceDataTable.Rows.Add(new object[] { String.Empty, "value" });
					}

					// Set focus on the new column...
					grdMatrix.CurrentCell = grdMatrix.CurrentRow.Cells[newColumnName];
					grdMatrix.Focus();
				}
			}
		}

		// Drops the current column from the matrix.
		private void btnDropColumn_Click(object sender, EventArgs e) {
			string columnNameToDrop = grdMatrix.CurrentCell.OwningColumn.Name;

			if (MessageBox.Show("Are you sure you want to drop the '" + columnNameToDrop + "' column from your expected results?", "Drop Column?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes) {
				string comparisonTypeColumnName = scenarioParameter.getMatrixColumnComparisonTypeColName(columnName: columnNameToDrop);

				prvDTMatrix.Columns.Remove(columnNameToDrop);
				prvDTMatrix.Columns.Remove(comparisonTypeColumnName);

				grdMatrix.Columns.Remove(comparisonTypeColumnName);
				grdMatrix.Columns.Remove(columnNameToDrop);
			}
		}

		// Renames the current column in the matrix.
		private void btnRenameColumn_Click(object sender, EventArgs e) {
			renameColumn();
		}

		// Renames the current column in the matrix.
		private void renameColumn() {
			if (grdMatrix.CurrentCell == null) {
				MessageBox.Show(caption: "Error", text: "Please add a row, first.", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
			} else {
				string oldColumnName = grdMatrix.CurrentCell.OwningColumn.Name;
				String newColumnName = oldColumnName;
				DialogResult result;

				result = InputBox.Prompt("Rename Column", "Please enter the new name of the '" + oldColumnName + "' column:", ref newColumnName);

				if (result == System.Windows.Forms.DialogResult.OK && newColumnName.Trim() != String.Empty) {
					prvDTMatrix.Columns[oldColumnName].ColumnName = newColumnName;

					grdMatrix.Columns[oldColumnName].Name = newColumnName;
					grdMatrix.Columns[newColumnName].DataPropertyName = newColumnName;
					grdMatrix.Columns[newColumnName].HeaderText = newColumnName;

					// Set focus on new (old) column
					grdMatrix.CurrentCell = grdMatrix.Rows[grdMatrix.CurrentRow.Index].Cells[newColumnName];
				}
			}
		}

		// Pastes a matrix from the clipboard.
		private void btnPasteFromClipboard_Click(object sender, EventArgs e) {
			string csvData = Clipboard.GetText();

			// See if the first row is column headers...
			bool hasColumnHeaders = (MessageBox.Show("Does the data contain column headers?", "Column Headers?", MessageBoxButtons.YesNo) == DialogResult.Yes);

			Program.fieldTracker.needsSaving = true;

			prvDTMatrix.Clear();
			prvDTMatrix.Columns.Clear();

			grdMatrix.Columns.Clear();

			string[] rows = Regex.Split(csvData.Trim(new char[] { '\r', '\n' }), "\r\n");

			for (int i = 0; i < rows.Length; i++) {
				string[] columns = rows[i].Split('\t');
				object[] arrValues = new object[columns.Length*2];
				int arrValuesIndex = 0;

				// Create the columns in the datatable, if we haven't already...
				if (prvDTMatrix.Columns.Count == 0) {
					for (int k = 0; k < columns.Length; k++) {
						prvDTMatrix.Columns.Add("Column_" + (k + 1).ToString());
						prvDTMatrix.Columns.Add(scenarioParameter.getMatrixColumnComparisonTypeColName(columnName: "Column_" + (k + 1).ToString()));
					}
				}

				if (hasColumnHeaders && i == 0) {
					string newColumnName;

					// Populate the column names using the first row from the clipboard...
					for (int j = 0; j < columns.Length; j++) {
						newColumnName = columns[j].Trim(new char[] { '\"' });

						prvDTMatrix.Columns[arrValuesIndex++].ColumnName = newColumnName;
						prvDTMatrix.Columns[arrValuesIndex++].ColumnName = scenarioParameter.getMatrixColumnComparisonTypeColName(columnName: newColumnName);
					}
				} else {
					// Copy the parsed column values into the object array
					for (int j = 0; j < columns.Length; j++) {
						arrValues[arrValuesIndex++] = columns[j].Trim(new char[] { '\"' });
						arrValues[arrValuesIndex++] = "value";
					}

					// Add the new row...
					prvDTMatrix.Rows.Add(arrValues);
				}
			}

			DataSourceDataTable = prvDTMatrix;
		}

		private void grdMatrix_KeyDown(object sender, KeyEventArgs e) {
			// Let the user rename a column by pressing F2 in the gridview.
			if (e.KeyCode == Keys.F2) {
				renameColumn();
			}
		}

		private void toggleComparisonTypeColumns() {
			for (int i = 1; i < grdMatrix.Columns.Count; i += 2) {
				grdMatrix.Columns[i].Visible = chkShowComparisonTypeColumns.Checked;
			}
		}

		private void chkShowComparisonTypeColumns_CheckedChanged(object sender, EventArgs e) {
			toggleComparisonTypeColumns();
		}

		private void grdMatrix_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
			// Open up the matrix popup window, if appropriate...
			if (prvNestedParams != null) {
				testArgument columnArg = testArg.childArguments[e.ColumnIndex / 2];

				if ((columnArg.dataType == "TABLE" || columnArg.dataType == "PL/SQL RECORD")
					&& grdMatrix.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString() == "nested") {
					frmPopupMatrix myMatrix = new frmPopupMatrix();

					myMatrix.matrix.myTest = Program.mainForm.currTest;
					myMatrix.matrix.testArg = columnArg;

					myMatrix.matrix.isINParameter = isINParameter;

					if (prvNestedParams.Count - 1 < e.RowIndex)
						prvNestedParams.Add(new scenarioParameterCollection(args: testArg.childArguments));

					if (isINParameter) {
						myMatrix.matrix.dataSourceNestedParams = prvNestedParams[e.RowIndex][e.ColumnIndex / 2].nestedParameters;
					} else {
						myMatrix.matrix.dataSourceNestedParams = prvNestedParams[e.RowIndex][e.ColumnIndex / 2].expectedOutNestedParameters;
					}

					myMatrix.ShowDialog();
				}
			}
		}
	}
}
