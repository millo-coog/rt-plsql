using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmScenarios : WeifenLuo.WinFormsUI.Docking.DockContent {
		public frmScenarios() {
			InitializeComponent();
		}

		#region Methods
		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {
			SuspendLayout();
			grdScenarios.SuspendDrawing();
			grdScenarios.SuspendLayout();

			Program.fieldTracker.enabled = false;
			
			if (objSCNGroup == null) {
				grdScenarios.DataSource = null;
				grdScenarios.Columns.Clear();
			} else {
				grdScenarios.DataSource = null;
				grdScenarios.Columns.Clear();

				grdScenarios.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
				grdScenarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

				grdScenarios.Visible = false;

				grdScenarios.Columns.Add(new RT.keyScenarioColumn());
				
				// Find the names of the unit's parameters and add them as columns, along with their parameter types (plsqlValue, exp, etc.)
				testArgumentCollection lstTestArguments = objSCNGroup.test.testArguments;

				for (int j = 0; j < lstTestArguments.Count; j++) {
					parameterValueScenarioColumn newParamValueCol = new parameterValueScenarioColumn(testArg: lstTestArguments[j]);
					parameterValueTypeScenarioColumn newParamValueTypeCol = new parameterValueTypeScenarioColumn(testArg: lstTestArguments[j], paramIndex: j);
					
					grdScenarios.Columns.Add(newParamValueCol);
					grdScenarios.Columns.Add(newParamValueTypeCol);

					newParamValueCol.correspondingValueTypeColumn = newParamValueTypeCol;
					newParamValueTypeCol.correspondingValueColumn = newParamValueCol;

					if (lstTestArguments[j].inOut == "IN/OUT") {
						parameterValueScenarioColumn newOutParamValueCol = new parameterValueScenarioColumn(testArg: lstTestArguments[j], isExpectedValueForInOutColumn: true);
						parameterValueTypeScenarioColumn newOutParamValueTypeCol = new parameterValueTypeScenarioColumn(testArg: lstTestArguments[j], paramIndex: j, isExpectedValueForInOutColumn: true);

						grdScenarios.Columns.Add(newOutParamValueCol);
						grdScenarios.Columns.Add(newOutParamValueTypeCol);

						newOutParamValueCol.correspondingValueTypeColumn = newOutParamValueTypeCol;
						newOutParamValueTypeCol.correspondingValueColumn = newOutParamValueCol;
					}
				}

				grdScenarios.Columns.Add(new RT.expectedExceptionScenarioColumn());
				grdScenarios.Columns.Add(new RT.expectedExceptionMessageScenarioColumn());
				grdScenarios.Columns.Add(new RT.commentsScenarioColumn());
				grdScenarios.Columns.Add(new RT.lastRunResultsScenarioColumn());
				grdScenarios.Columns.Add(new RT.lastRunErrorNumberScenarioColumn());
				grdScenarios.Columns.Add(new RT.lastRunErrorMessageScenarioColumn());
				grdScenarios.Columns.Add(new RT.GUIDScenarioColumn());
					
				// If only the "Add" row is in the scenario gridview, then add one scenario
				// for the user....
				if (objSCNGroup.scenarios.Count == 0) {
					addRequiredScenarioToGroup(lstTestArguments);
				}
					
				DataTable dtScenarios = objSCNGroup.getScenarios(runResults: Program.currProject.repository.runResults);

				grdScenarios.AutoGenerateColumns = false;
				grdScenarios.DataSource = dtScenarios;
				
				sortScenarioColumns();
				
				hideParameterTypeColumns();
				
				setScenarioRowDefaults(lstTestArguments, grdScenarios.Rows[grdScenarios.NewRowIndex]);
				
				for (int i = 0; i < grdScenarios.Columns.Count; i++) {
					grdScenarios.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
				}

				grdScenarios.Visible = true;

				// Apply highlighting...
				for (int i = 0; i < grdScenarios.Rows.Count; i++) {
					// Highlight the rows based on their run status. Has to be done after the gridview is visible again, or
					// they won't color right.
					if (grdScenarios.Rows[i].IsNewRow == false) {
						highlightScenarioRow(grdScenarios.Rows[i]);
					}
				}
			}

			Program.mainForm.loadAppTitle();

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;

			grdScenarios.ResumeDrawing();
			grdScenarios.ResumeLayout();
			ResumeLayout();
		}

		// Copies the information on the form into the given scenario group object...
		public void saveFormToSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup != null) {
				// Validate data....
				if (scenarioGroupIsValid() == false)
					return;

				// Save the scenario group's scenarios

				// Walk through the scenarios in the gridview in reverse order, so we can delete
				// them easily....
				for (int i = grdScenarios.Rows.Count - 1; i >= 0; i--) {
					if (grdScenarios.Rows[i].IsNewRow == false) {
						if (grdScenarios.Rows[i].Tag != null && grdScenarios.Rows[i].Tag.ToString() == "Deleted" && grdScenarios.Rows[i].Cells["GUID"].Value != null && grdScenarios.Rows[i].Cells["GUID"].Value.ToString() != string.Empty) {
							// Find and remove the scenarios from memory...
							for (int j = 0; j < objSCNGroup.scenarios.Count; j++) {
								if (objSCNGroup.scenarios[j].guid == grdScenarios.Rows[i].Cells["GUID"].Value.ToString()) {
									objSCNGroup.scenarios.RemoveAt(j);
									break;
								}
							}

							// Remove the scenario row from the gridview.
							grdScenarios.Rows.RemoveAt(i);

							// If the user removed the last scenario, add back the default, required scenario
							if (grdScenarios.Rows.Count == 1) {
								grdScenarios.DataSource = null;
							}
						}
					}
				}

				// Now walk through in regular order, adding and changing scenarios
				bool isNewScenario;
				bool isChangedScenario;
				for (int i = 0; i < grdScenarios.Rows.Count; i++) {
					if (grdScenarios.Rows[i].IsNewRow == false) {
						isNewScenario = grdScenarios.Rows[i].Cells["GUID"].Value == null || grdScenarios.Rows[i].Cells["GUID"].Value.ToString() == String.Empty;
						isChangedScenario = grdScenarios.Rows[i].Tag != null && grdScenarios.Rows[i].Tag.ToString() == "Changed";

						// Any deletes would cause our keys to be off, so just reassign it to the index.
						grdScenarios.Rows[i].Cells["Key"].Value = (i+1).ToString();

						if (isNewScenario) {
							// New scenario row - set the scenario properties
							scenario newScenario = new scenario(objSCNGroup.test);

							newScenario.comments = grdScenarios.Rows[i].Cells["Comments"].Value.ToString();
							newScenario.expectedException = grdScenarios.Rows[i].Cells["Expected Exception"].Value.ToString();
							newScenario.expectedExceptionMessage = grdScenarios.Rows[i].Cells["Expected Exception Message"].Value.ToString();
								
							objSCNGroup.scenarios.Add(newScenario);

							grdScenarios.Rows[i].Cells["GUID"].Value = newScenario.guid;
						}

						if (isChangedScenario) {
							// Changed scenario row - change the existing scenario properties
							for (int j = 0; j < objSCNGroup.scenarios.Count; j++) {
								if (objSCNGroup.scenarios[j].guid == grdScenarios.Rows[i].Cells["GUID"].Value.ToString()) {
									objSCNGroup.scenarios[j].expectedException = grdScenarios.Rows[i].Cells["Expected Exception"].Value.ToString();
									objSCNGroup.scenarios[j].expectedExceptionMessage = grdScenarios.Rows[i].Cells["Expected Exception Message"].Value.ToString();
									objSCNGroup.scenarios[j].comments = grdScenarios.Rows[i].Cells["Comments"].Value.ToString();

									break;
								}
							}
								
							grdScenarios.Rows[i].Tag = String.Empty;
						}

						if (isNewScenario || isChangedScenario) {
							// Update the stored user parameters with the new values
							int paramIndex = 0;
							for (int j = 0; j < grdScenarios.Columns.Count; j++) {
								if (grdScenarios.Columns[j].GetType() == typeof(parameterValueScenarioColumn)) {
									parameterValueScenarioColumn paramValueCol = (parameterValueScenarioColumn) grdScenarios.Columns[j];

									if (paramValueCol.isOutColumnPortion == false) {
										// Find the corresponding scenario in the scenarios list and update it...
										for (int k = 0; k < objSCNGroup.scenarios.Count; k++) {
											if (objSCNGroup.scenarios[k].guid == grdScenarios.Rows[i].Cells["GUID"].Value.ToString()) {

												// We found the scenario - now find the right parameter...
												for (int m = 0; m < objSCNGroup.scenarios[k].parameters.Count(); m++) {
													if (objSCNGroup.scenarios[k].parameters[m].testArg == paramValueCol.testArg) {
														// Save the plsqlValue and comparison type...
														String value = grdScenarios.Rows[i].Cells[j].Value == null ? String.Empty : grdScenarios.Rows[i].Cells[j].Value.ToString();
														value = value.Replace("\r\n", "\n").Replace("\n", "\r\n");

														objSCNGroup.scenarios[k].parameters[m].value = value;
														objSCNGroup.scenarios[k].parameters[m].valueComparisonType = grdScenarios.Rows[i].Cells["pt" + paramIndex].Value.ToString();

														// Save any corresponding OUT column associated with this column in an IN/OUT pair.
														if (paramValueCol.testArg.inOut == "IN/OUT") {
															String outValue = grdScenarios.Rows[i].Cells["exp_out" + paramIndex].Value == null ? String.Empty : grdScenarios.Rows[i].Cells["exp_out" + paramIndex].Value.ToString();
															outValue = outValue.Replace("\r\n", "\n").Replace("\n", "\r\n");

															objSCNGroup.scenarios[k].parameters[m].expectedOutValue = outValue;
															objSCNGroup.scenarios[k].parameters[m].expectedOutComparisonType = grdScenarios.Rows[i].Cells["exp_out_pt" + paramIndex].Value.ToString();
														}

														break;
													}
												}

												break;
											}
										}

										paramIndex++;
									}
								}
							}
						}
					}
				}
			}

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}

		private bool scenarioGroupIsValid() {
			bool isValid = true;
			DataGridViewCell currCell = null;

			// Validate the data passed in the scenarios
			for (int i = 0; i < grdScenarios.Rows.Count; i++) {
				if (grdScenarios.Rows[i].IsNewRow == false
					 && (grdScenarios.Rows[i].Tag == null || grdScenarios.Rows[i].Tag.ToString() != "Deleted")) {

					for (int j = 0; j < grdScenarios.Columns.Count; j++) {
						currCell = grdScenarios.Rows[i].Cells[j];

						if (grdScenarios.Columns[j].GetType() == typeof(parameterValueScenarioColumn)) {
							parameterValueScenarioColumn paramValueCol = (parameterValueScenarioColumn) grdScenarios.Columns[j];

							DataGridViewCell paramValueTypeCell;

							if (paramValueCol.isOutColumnPortion) {
								paramValueTypeCell = grdScenarios.Rows[i].Cells[paramValueCol.correspondingValueTypeColumn.Index];
							} else {
								paramValueTypeCell = grdScenarios.Rows[i].Cells[paramValueCol.correspondingValueTypeColumn.Index];
							}

							// Examine empty cells
							if (currCell.Value == null || currCell.Value.ToString() == String.Empty) {
								// Expression parameter types must have something in their parameter cell...
								if (paramValueTypeCell.Value.ToString() == "exp") {
									if (paramValueCol.testArg.inOut == "RETURN") {
										MessageBox.Show("This scenario's return value needs an expression to test against.");

										isValid = false;
										break;
									}
								}
							} else {
								// Examine non-empty cells
								if (paramValueTypeCell.Value.ToString() == "defaulted") {
									// Defaulted parameters must not pass a plsqlValue
									if (paramValueCol.testArg.inOut == "RETURN")
										MessageBox.Show("This scenario's return value must be blank if you're defaulting the parameter.");
									else
										MessageBox.Show("This scenario's parameter value must be blank if you're defaulting the parameter.");

									isValid = false;
									break;
								}
							}
						}
					}
				}

				if (isValid == false)
					break;
			}

			if (isValid == false) {
				grdScenarios.Focus();
				grdScenarios.CurrentCell = currCell;
			}

			return isValid;
		}

		private void sortScenarioColumns() {
			// If the parameters type columns are all supposed to be immediately after
			// their corresponding parameter column (i.e. inline), then make sure they are.
			if (chkDisplayParamTypesInline.Checked) {
				int paramIndex = 0;

				for (int i = 0; i < grdScenarios.Columns.Count; i++) {
					if (grdScenarios.Columns[i].GetType() == typeof(parameterValueTypeScenarioColumn)) {
						grdScenarios.Columns[i].DisplayIndex = 1 + (2 * paramIndex) + 1;
						
						// Change the color of parameters (not return values)
						if (grdScenarios.Columns[i].DefaultCellStyle.BackColor == System.Drawing.Color.FromArgb(255, 255, 224))
							grdScenarios.Columns[i].DefaultCellStyle.BackColor = Program.PARAMETER_COLOR;

						paramIndex++;
					}
				}
			} else {
				int numArgs = Program.mainForm.currTest.testArguments.Count;

				// Because every IN/OUT argument generates another argument/column (for the OUT portion of the argument), count them as another column.
				for (int i = 0; i < Program.mainForm.currTest.testArguments.Count; i++) {
					if (Program.mainForm.currTest.testArguments[i].inOut == "IN/OUT")
						numArgs++;
				}

				for (int i = 0; i < grdScenarios.Columns.Count; i++) {
					DataGridViewColumn col = grdScenarios.Columns[i];

					if (col.GetType() == typeof(parameterValueTypeScenarioColumn)) {
						col.DisplayIndex = (numArgs * 2);
					}
				}
			}
		}

		private void addRequiredScenarioToGroup(testArgumentCollection lstTestArgs) {
			grdScenarios.Rows.Add();

			setScenarioRowDefaults(lstTestArgs, grdScenarios.Rows[0]);
		}
								
		private void hideParameterTypeColumns() {
			DataGridViewColumn col;

			for (int i = 0; i < grdScenarios.Columns.Count; i++) {
				col = grdScenarios.Columns[i];

				if (col.GetType() == typeof(parameterValueTypeScenarioColumn)) {
					col.Visible = !chkHideParameterTypes.Checked;
				}
			}
		}

		public void highlightScenarioRow(DataGridViewRow row) {
			DataGridViewCellStyle lastRunOKStyle = new DataGridViewCellStyle();
			lastRunOKStyle.BackColor = Color.PaleGreen;

			DataGridViewCellStyle lastRunFailedStyle = new DataGridViewCellStyle();
			lastRunFailedStyle.BackColor = Color.Pink;

			if (row.Cells["Last Run Results"].Value != null) {
				row.Cells["Last Run Error Message"].Value = row.Cells["Last Run Error Message"].Value.ToString().TrimEnd('\r', '\n');

				// Color the cells according to their last run results...
				if (row.Cells["Last Run Results"].Value.ToString() == "OK") {
					row.Cells["Last Run Results"].Style = lastRunOKStyle;
					row.Cells["Last Run Error #"].Style = lastRunOKStyle;
					row.Cells["Last Run Error Message"].Style = lastRunOKStyle;
				} else {
					row.Cells["Last Run Results"].Style = lastRunFailedStyle;
					row.Cells["Last Run Error #"].Style = lastRunFailedStyle;
					row.Cells["Last Run Error Message"].Style = lastRunFailedStyle;
				}

				// If the error message contains a new line, set the cell to multi-line format.
				if (row.Cells["Last Run Error Message"].Value.ToString() != String.Empty
					&& row.Cells["Last Run Error Message"].Value.ToString().IndexOf("\n") >= 0) {
					row.Cells["Last Run Error Message"].Style.WrapMode = DataGridViewTriState.True;
				} else {
					row.Cells["Last Run Error Message"].Style.WrapMode = DataGridViewTriState.False;
				}
			}
			
			// Highlight any special cells...
			for (int j = 0; j < grdScenarios.Columns.Count; j++) {
				highlightCell(row.Cells[j]);
			}
		}
		
		private void highlightCell(DataGridViewCell cell) {
			if (cell.OwningRow.IsNewRow == false &&
                grdScenarios.Columns[cell.ColumnIndex].GetType() == typeof(parameterValueTypeScenarioColumn))
            {
				parameterValueTypeScenarioColumn paramValueTypeCol = (parameterValueTypeScenarioColumn)grdScenarios.Columns[cell.ColumnIndex];
				DataGridViewCell valueCell = grdScenarios.Rows[cell.RowIndex].Cells[paramValueTypeCol.correspondingValueColumn.Index]; // My associated value cell

				String paramType = grdScenarios.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value.ToString();

				if (paramType == "matrix") {
					// "Color" the associated parameter value as a matrix...
					valueCell.Value = Program.MATRIX_CELL_TEXT_INDICATOR;

					valueCell.ReadOnly = true;
					valueCell.Style.ForeColor = Color.Blue;
				} else {
					if (paramType == "nested" &&
						(paramValueTypeCol.testArg.dataType == "TABLE" || paramValueTypeCol.testArg.dataType == "PL/SQL RECORD" || paramValueTypeCol.testArg.dataType == "REF CURSOR")) {
						// Color the associated parameter value as a nested parameter.
						if (paramValueTypeCol.testArg.dataType == "REF CURSOR")
							valueCell.Value = Program.STRONG_REF_CURSOR_CELL_TEXT_INDICATOR;
						else
							valueCell.Value = Program.TYPE_CELL_TEXT_INDICATOR;

						valueCell.ReadOnly = true;
						valueCell.Style.ForeColor = Color.Blue;
					} else {
						// Remove any unneeded "coloring"...
						if (valueCell.Value != null &&
							(valueCell.Value.ToString() == Program.MATRIX_CELL_TEXT_INDICATOR
							|| valueCell.Value.ToString() == Program.TYPE_CELL_TEXT_INDICATOR
							|| valueCell.Value.ToString() == Program.STRONG_REF_CURSOR_CELL_TEXT_INDICATOR)) {
							valueCell.Value = String.Empty;

							valueCell.ReadOnly = false;
							valueCell.Style.ForeColor = grdScenarios.Columns[cell.ColumnIndex].DefaultCellStyle.ForeColor;
						}
					}
				}
			}
		}

		// Takes the given scenario gridviewrow and sets its default values
		public void setScenarioRowDefaults(testArgumentCollection lstTestArgs, DataGridViewRow gvrScenario) {
			gvrScenario.Tag = null;

			grdScenarios.CellValueChanged -= grdScenarios_CellValueChanged;
			
			// Default the new row's cells to their default values...

			gvrScenario.Cells["Key"].Value = String.Empty;
			
			foreach (DataGridViewCell currCell in gvrScenario.Cells) {
				// Default the parameter type (expression, plsqlValue, etc.) based on the datatype of the parameter
				if (currCell.OwningColumn.GetType() == typeof(parameterValueTypeScenarioColumn)) {
					testArgument testArg = ((parameterValueTypeScenarioColumn) currCell.OwningColumn).testArg;

					if (testArg.plsType == "BOOLEAN" || testArg.plsType == "DATE" || testArg.plsType == "TIMESTAMP" || testArg.dataType == "REF CURSOR") {
						currCell.Value = "exp";
					} else {
					   currCell.Value = "value";
					}

					if (testArg.dataType == "OBJECT") {
						currCell.Value = "matrix";
					}

					if (testArg.dataType == "TABLE" || testArg.dataType == "PL/SQL RECORD") {
						currCell.Value = "nested";
					}
				}				
			}

			gvrScenario.Cells["Expected Exception"].Value = String.Empty;
			gvrScenario.Cells["Expected Exception Message"].Value = String.Empty;
			gvrScenario.Cells["Comments"].Value = String.Empty;
			
			grdScenarios.CellValueChanged += grdScenarios_CellValueChanged;
		}
				
		public void updateScenarioRunStatus(string scenarioGuid, string status, Int32 errorNumber, string errorMessage) {
			for (int i = 0; i < grdScenarios.Rows.Count; i++) {
				if (grdScenarios.Rows[i].IsNewRow == false && grdScenarios.Rows[i].Cells["GUID"].Value.ToString() == scenarioGuid) {
					grdScenarios.Rows[i].Cells["Last Run Results"].Value = status;
					grdScenarios.Rows[i].Cells["Last Run Error #"].Value = errorNumber == 0 ? String.Empty : errorNumber.ToString();
					grdScenarios.Rows[i].Cells["Last Run Error Message"].Value = errorMessage;

					highlightScenarioRow(grdScenarios.Rows[i]);
	
					break;
				}
			}
		}
		#endregion

		#region Events
		private void btnGenerateAutoScenarios_Click(object sender, EventArgs e) {			
			String message =
				"This feature will automatically generate variations of the given good scenario, " + 
				"defaulting parameters, and passing NULL or invalid parameters in place of the good ones. Have you selected the good " +
				"scenario you wish to mutate?";

			if (MessageBox.Show(text: message, caption: "Auto-Generate Scenarios", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes) {
				Program.mainForm.currScenarioGroup.createInvalidParamScenarios(Program.getCurrentTargetDBConnection(), grdScenarios.CurrentRow.Index);

				loadFromSCNGroupObject(Program.mainForm.currScenarioGroup);

				Program.fieldTracker.needsSaving = true;
			}
		}

		private void btnShrinkColumns_Click(object sender, EventArgs e) {
			for (int i = 0; i < grdScenarios.Columns.Count; i++) {
				if (grdScenarios.Columns[i].GetType() == typeof(parameterValueScenarioColumn)) {
					grdScenarios.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
					grdScenarios.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomLeft;
				}
			}
		}

		private void btnCloneScenario_Click(object sender, EventArgs e) {
			DataGridViewSelectedRowCollection arrSelectedRows;

			// If no rows (scenarios) are selected, select the row the cursor is in...
			if (grdScenarios.SelectedRows.Count == 0) {
				grdScenarios.CurrentRow.Selected = true;
			}

			arrSelectedRows = grdScenarios.SelectedRows;

			if ((DataTable)grdScenarios.DataSource == null) {
				MessageBox.Show("You must save the scenario group before cloning a scenario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} else {
				// Loop through every selected row and clone it...
				DataTable dtScenarios = (DataTable)grdScenarios.DataSource;
				bool aScenarioHadAMatrix = false;

				for (int rowIndex = arrSelectedRows.Count - 1; rowIndex >= 0; rowIndex--) {
					if (arrSelectedRows[rowIndex].IsNewRow == false) {
						// Add a new row...
						dtScenarios.Rows.Add();

						// Copy the values from the source row to the cloned row
						DataGridViewRow newRow = grdScenarios.Rows[grdScenarios.Rows.Count - 2];

						for (int i = 0; i < grdScenarios.Columns.Count; i++) {
							newRow.Cells[i].Value = arrSelectedRows[rowIndex].Cells[i].Value;

							// Note any matrices, so we can later warn the user they weren't copied...
							if (arrSelectedRows[rowIndex].Cells[i].OwningColumn.GetType() == typeof(parameterValueTypeScenarioColumn)) {
								if (newRow.Cells[i].Value.ToString() == "matrix") {
									aScenarioHadAMatrix = true;
								}
							}
						}

						// Clear any row-specific information from the source row
						newRow.Cells["Key"].Value = String.Empty;
						newRow.Cells["GUID"].Value = String.Empty;
						newRow.Tag = String.Empty;

						// After we clone the row, set focus to the cell that corresponds
						// with the column we were originally in.
						grdScenarios.CurrentCell = newRow.Cells[grdScenarios.Columns[grdScenarios.CurrentCell.ColumnIndex].Name];
					} else {
						MessageBox.Show(text: "You cannot clone the 'new' scenario row.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
					}
				}

				if (aScenarioHadAMatrix)
					MessageBox.Show(text: "Warning: Matrices are not yet cloned (not implemented) - you need to copy them yourself, for now.", caption: "Warning", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Warning);
			}
		}

		private void btnGetIndividualScenarioRunBlock_Click(object sender, EventArgs e) {
			Program.mainForm.promptToSaveAnyChanges();

			if (grdScenarios.SelectedRows.Count != 0) {
				MessageBox.Show("You need to choose a single scenario first!", "Error", MessageBoxButtons.OK);
			} else {
				SuspendLayout();

				Program.outputForm.clearOutput();
				Program.outputForm.debugWrite((new runBlock()).getPLSQLRunBlock(conTargetDB: Program.getCurrentTargetDBConnection(), scnGroup: Program.mainForm.currScenarioGroup, targetDBName: Program.getCurrentTargetDBConnection().DatabaseName, generateAsDebugBlock: true, scenarioIndex: Int32.Parse(grdScenarios.CurrentRow.Cells["Key"].Value.ToString())-1));

				Program.outputForm.highlightOutputAsPLSQL();

				//scnGroupForm.tabGroup.SelectedTab = tabDebug;

				ResumeLayout();
			}
		}

		private void chkHideParameterTypes_CheckedChanged(object sender, EventArgs e) {
			SuspendLayout();
			hideParameterTypeColumns();
			ResumeLayout();
		}
		
		private void chkDisplayParamTypesInline_CheckedChanged(object sender, EventArgs e) {
			sortScenarioColumns();
		}
		
		private void grdScenarios_CurrentCellDirtyStateChanged(object sender, EventArgs e) {
			if (grdScenarios.IsCurrentCellDirty) {
				// Force the CellValueChanged event to happen, without requiring the user to leave their current cell.
				grdScenarios.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}

		private void grdScenarios_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			// Note that the test now needs saving...
			if (e.RowIndex >= 0 &&

				 grdScenarios.Rows[e.RowIndex].IsNewRow == false &&

			    // Ignore run result columns....
			    e.ColumnIndex != grdScenarios.Columns["Last Run Results"].Index &&
			    e.ColumnIndex != grdScenarios.Columns["Last Run Error #"].Index &&
			    e.ColumnIndex != grdScenarios.Columns["Last Run Error Message"].Index &&
				 				 
				 e.ColumnIndex != grdScenarios.Columns["Key"].Index && // Don't mark the row as changed when we added a new scenario, and its key cell gets populated
				 grdScenarios.Rows[e.RowIndex].Cells["GUID"].Value != null &&
				 grdScenarios.Rows[e.RowIndex].Cells["GUID"].Value.ToString() != String.Empty) { // Only mark existing scenario records as being changed

				grdScenarios.Rows[e.RowIndex].Tag = "Changed";
				Program.fieldTracker.needsSaving = true;
			}
			
			highlightCell(grdScenarios.Rows[e.RowIndex].Cells[e.ColumnIndex]);
		}
		
		private void grdScenarios_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
			e.Row.Tag = "Deleted";
			e.Row.DefaultCellStyle.BackColor = Color.DarkGray;
			e.Cancel = true;

			Program.fieldTracker.needsSaving = true;
		}

		private void grdScenarios_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e) {
			setScenarioRowDefaults(Program.mainForm.currTest.testArguments, e.Row);
		}

		private void grdScenarios_CellEnter(object sender, DataGridViewCellEventArgs e) {
			// We're working around a behavior of the gridview, in that, if it's edit mode
			// is EditOnEnter, clicking on the row header to select it for deletion, edits
			// the first column - see the rowheadermouseclick event.
			grdScenarios.EditMode = DataGridViewEditMode.EditOnEnter;
		}

		private void grdScenarios_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
			// We're working around a behavior of the gridview, in that, if it's edit mode
			// is EditOnEnter, clicking on the row header to select it for deletion, edits
			// the first column - see the CellEnter event for where we reset the edit mode.
			grdScenarios.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

			grdScenarios.EndEdit();
		}
		
		private void grdScenarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
			// Open up child windows, if appropriate....
			if (e.ColumnIndex != -1 && e.RowIndex != -1 && grdScenarios.Columns[e.ColumnIndex].GetType() == typeof(parameterValueScenarioColumn)) {
				parameterValueScenarioColumn paramValueCol = (parameterValueScenarioColumn) grdScenarios.Columns[e.ColumnIndex];
				string comparisonType = grdScenarios.Rows[e.RowIndex].Cells[paramValueCol.correspondingValueTypeColumn.Index].Value.ToString();

				// Open up the matrix popup window, if appropriate...
				if (comparisonType == "matrix") {
					if (grdScenarios.Rows[e.RowIndex].Cells["GUID"].Value.ToString() == String.Empty) {
						// User must save row before accessing matrix...
						MessageBox.Show("You must save the scenario before you can edit its matrix.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					} else {
						frmPopupMatrix myMatrix = new frmPopupMatrix();

						myMatrix.matrix.myTest = Program.mainForm.currTest;
						myMatrix.matrix.testArg = paramValueCol.testArg;

						if (paramValueCol.testArg.inOut == "IN/OUT" && paramValueCol.isOutColumnPortion) {
							myMatrix.matrix.isINParameter = false;
							myMatrix.matrix.DataSourceDataTable = Program.mainForm.currScenarioGroup.scenarios[e.RowIndex].parameters[paramValueCol.testArg.argumentName].ExpectedOutDataTable;
						} else {
							myMatrix.matrix.isINParameter = true;
							myMatrix.matrix.DataSourceDataTable = Program.mainForm.currScenarioGroup.scenarios[e.RowIndex].parameters[paramValueCol.testArg.argumentName].DataTable;
						}

						myMatrix.ShowDialog();
					}
				} else {
					if (comparisonType == "nested" && paramValueCol.testArg.childArguments.Count > 0) {
						// This parameter has nested parameters...
						if (grdScenarios.Rows[e.RowIndex].Cells["GUID"].Value.ToString() == String.Empty) {
							// User must save row before accessing nested parameters...
							MessageBox.Show("You must save the scenario before you can edit nested parameters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						} else {
							frmPopupMatrix myMatrix = new frmPopupMatrix();

							myMatrix.matrix.myTest = Program.mainForm.currTest;
							myMatrix.matrix.testArg = paramValueCol.testArg;

							if (paramValueCol.testArg.inOut == "IN/OUT" && paramValueCol.isOutColumnPortion) {
								myMatrix.matrix.isINParameter = false;
								myMatrix.matrix.dataSourceNestedParams = Program.mainForm.currScenarioGroup.scenarios[e.RowIndex].parameters[paramValueCol.testArg.argumentName].expectedOutNestedParameters;
							} else {
								myMatrix.matrix.isINParameter = true;
								myMatrix.matrix.dataSourceNestedParams = Program.mainForm.currScenarioGroup.scenarios[e.RowIndex].parameters[paramValueCol.testArg.argumentName].nestedParameters;
							}
							
							myMatrix.ShowDialog();
						}
					}
				}
			}
		}

		private void grdScenarios_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e) {
			string tooltip = "";

			if (e.ColumnIndex > -1 && e.RowIndex >= 0)
			{
				// Calculate tooltip for a parameter value cell...
				if (grdScenarios.Columns[e.ColumnIndex].GetType() == typeof(parameterValueScenarioColumn)) {
					parameterValueScenarioColumn paramValueCol = (parameterValueScenarioColumn)grdScenarios.Columns[e.ColumnIndex];

					if (grdScenarios.Rows[e.RowIndex].Cells[paramValueCol.correspondingValueTypeColumn.Index].Value != null) {
						tooltip =
							"Type: " + paramValueCol.testArg.plsType + "\n" +
							"Value Type: " + grdScenarios.Rows[e.RowIndex].Cells[paramValueCol.correspondingValueTypeColumn.Index].Value.ToString() + "\n\n";
					}

					foreach (testArgument testArg in paramValueCol.testArg.childArguments) {
						tooltip += testArg.dataType + " " + testArg.argumentName + " " + testArg.plsType + "\n";
					}
				}

				// Calculate tooltip for a value type cell...
				if (grdScenarios.Columns[e.ColumnIndex].GetType() == typeof(parameterValueTypeScenarioColumn)) {
					parameterValueTypeScenarioColumn paramValueTypeCol = (parameterValueTypeScenarioColumn)grdScenarios.Columns[e.ColumnIndex];

					tooltip = "Name: " + paramValueTypeCol.testArg.argumentName;
				}
			}

			e.ToolTipText = tooltip.TrimEnd('\n');
		}

		private void grdScenarios_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
			// Set the cursor to a hand pointer if the mouse is over a clickable cell...
			if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
				DataGridViewCell cell = grdScenarios.Rows[e.RowIndex].Cells[e.ColumnIndex];

				if (cell.Value != null &&
					(cell.Value.ToString() == Program.MATRIX_CELL_TEXT_INDICATOR
					|| cell.Value.ToString() == Program.TYPE_CELL_TEXT_INDICATOR
					|| cell.Value.ToString() == Program.STRONG_REF_CURSOR_CELL_TEXT_INDICATOR)) {
					grdScenarios.Cursor = Cursors.Hand;
				} else {
					grdScenarios.Cursor = Cursors.Default;
				}
			}
		}

		DataGridViewCell lastParamValueType;
		private void grdScenarios_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e) {
			// Show a popup menu for parameter value cells...
			if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
				if (grdScenarios.Columns[e.ColumnIndex].GetType() == typeof(parameterValueScenarioColumn)) {
					parameterValueTypeScenarioColumn paramTypeCol = ((parameterValueScenarioColumn)grdScenarios.Columns[e.ColumnIndex]).correspondingValueTypeColumn;
					
					object[] arrItems = new object[paramTypeCol.Items.Count];

					lastParamValueType = grdScenarios.Rows[e.RowIndex].Cells[paramTypeCol.Index];

					paramTypeCol.Items.CopyTo(arrItems, 0);

					cmbValueComparisonType.Items.Clear();
					cmbValueComparisonType.Items.AddRange(arrItems);
					cmbValueComparisonType.SelectedItem = lastParamValueType.Value.ToString();
					
					e.ContextMenuStrip = cmsCellPopup;
				}
			}
		}

		private void cmbValueComparisonType_SelectedIndexChanged(object sender, EventArgs e) {
			lastParamValueType.Value = cmbValueComparisonType.SelectedItem;
		}

		private void frmScenarios_Load(object sender, EventArgs e) {			
			grdScenarios.DoubleBuffered(true);
			
			grdScenarios.Focus();

			if (grdScenarios.Rows.Count > 0) {
				grdScenarios.CurrentCell = grdScenarios.Rows[0].Cells[1];
			}
		}

		private void grdScenarios_DataError(object sender, DataGridViewDataErrorEventArgs e) {
			MessageBox.Show(caption: "Error in Scenarios", text: "Error in cell [" + e.RowIndex + ", " + e.ColumnIndex + "]: " + e.Exception.Message);
		}
		#endregion
	}
}
