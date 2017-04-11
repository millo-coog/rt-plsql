using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmUDC : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmUDC() {
			InitializeComponent();
		}
		#endregion

		#region Methods
		public void loadFromUDCObject(udc objUDC) {
			SuspendLayout();
			Program.fieldTracker.enabled = false;

			if (objUDC == null) {				
				cmbUDCType.SelectedIndex = 1;
				showAppropriateUDCTab();

				rtfUDCPLSQLBlock.Text = String.Empty;
				rtfUDCRowExistenceCursor.Text = String.Empty;
				rtfUDCRowValidationCursor.Text = String.Empty;
				rtfUDCPLSQLCondition.Text = String.Empty;
				rtfExpectedCursor.Text = String.Empty;
				rtfActualCursor.Text = String.Empty;
				txtCSVExcludedColumns.Text = String.Empty;
				rtfMatrixComparisonActualCursor.Text = String.Empty;
				txtUDCSortOrder.Text = "0";
				txtUDCName.Text = String.Empty;
				txtUDCDescription.Text = String.Empty;
				lblUDCGuid.Text = String.Empty;

				grdRowValidation.DataSource = null;

				mtxExpectedResults.DataSourceDataTable = null;
			} else {
				cmbUDCType.SelectedIndex = ((int)objUDC.checkType) - 1;

				rtfUDCPLSQLBlock.Text = objUDC.plsqlBlock;
				if (rtfUDCPLSQLBlock.Text != String.Empty)
					plsql.highlight(rtfUDCPLSQLBlock);

				rtfUDCRowExistenceCursor.Text = objUDC.rowExistenceCursor;
				if (rtfUDCRowExistenceCursor.Text != String.Empty)
					plsql.highlight(rtfUDCRowExistenceCursor);

				rtfUDCRowValidationCursor.Text = objUDC.rowValidationCursor;
				if (rtfUDCRowValidationCursor.Text != String.Empty)
					plsql.highlight(rtfUDCRowValidationCursor);

				rtfUDCPLSQLCondition.Text = objUDC.plsqlCondition;
				if (rtfUDCPLSQLCondition.Text != String.Empty)
					plsql.highlight(rtfUDCPLSQLCondition);

				rtfExpectedCursor.Text = objUDC.expectedCursor;
				if (rtfExpectedCursor.Text != String.Empty)
					plsql.highlight(rtfExpectedCursor);

				rtfActualCursor.Text = objUDC.actualCursor;
				if (rtfActualCursor.Text != String.Empty)
					plsql.highlight(rtfActualCursor);

				txtCSVExcludedColumns.Text = objUDC.csvExcludedColumns;

				rtfMatrixComparisonActualCursor.Text = objUDC.actualCursor;
				if (rtfMatrixComparisonActualCursor.Text != String.Empty)
					plsql.highlight(rtfMatrixComparisonActualCursor);

				txtUDCSortOrder.Text = objUDC.sortOrder.ToString();
				txtUDCName.Text = objUDC.name;
				txtUDCDescription.Text = objUDC.description;
				lblUDCGuid.Text = objUDC.guid;
				  
				grdRowValidation.DataSource = null;

				if (objUDC.expectedMatrix == null)
					objUDC.expectedMatrix = new DataTable();

				mtxExpectedResults.DataSourceDataTable = objUDC.expectedMatrix;

				showAppropriateUDCTab();

				if (objUDC.checkType == udc.enumCheckTypes.ROW_VALIDATION) {
					grdRowValidation.DataSource = null;
					grdRowValidation.Columns.Clear();
					grdRowValidation.AutoGenerateColumns = false;

					grdRowValidation.Columns.Add("fieldName", "Fieldname");
					grdRowValidation.Columns["fieldName"].DataPropertyName = "fieldName";
					grdRowValidation.Columns["fieldName"].DefaultCellStyle.BackColor = Color.FromArgb(223, 223, 223);
					grdRowValidation.Columns["fieldName"].ReadOnly = true;

					DataGridViewComboBoxColumn comparisonType = new DataGridViewComboBoxColumn();
					comparisonType.DataPropertyName = "comparisonType";
					comparisonType.HeaderText = "Comparison";
					comparisonType.Items.AddRange(new object[] {
						"Don't Test",
						"Exp",
						"Input Parameter",
						"IS NULL",
						"NOT NULL",
						"PL/SQL Block",
						"Value"});
					comparisonType.Name = "comparisonType";
					comparisonType.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 192);
					grdRowValidation.Columns.Add(comparisonType);

					grdRowValidation.Columns.Add("fieldValue", "Expression");
					grdRowValidation.Columns["fieldValue"].DataPropertyName = "fieldValue";
					grdRowValidation.Columns["fieldValue"].DefaultCellStyle.BackColor = Program.PARAMETER_COLOR;
					
					grdRowValidation.DataSource = objUDC.fieldValidations;
				}
			}

			ResumeLayout();
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}

		public void saveFormToUDC(udc objUDC) {
			if (objUDC != null) {
				// Error checking
				if (cmbUDCType.SelectedValue == null) {
					Show();

					cmbUDCType.Focus();
					MessageBox.Show("You need to choose the type of UDC.", "Error", MessageBoxButtons.OK);
					return;
				}

				if ((udc.enumCheckTypes)Int32.Parse(cmbUDCType.SelectedValue.ToString()) == udc.enumCheckTypes.ROW_VALIDATION &&
					rtfUDCRowValidationCursor.Text.Trim() == String.Empty) {

					Show();

					rtfUDCRowValidationCursor.Focus();
					MessageBox.Show("You must have an SQL statement for row validators.");
					return;
				}

				if ((udc.enumCheckTypes)Int32.Parse(cmbUDCType.SelectedValue.ToString()) == udc.enumCheckTypes.COMPARE_CURSORS) {
					if (rtfExpectedCursor.Text.Trim() == String.Empty) {
						Show();

						rtfExpectedCursor.Focus();
						MessageBox.Show("You must provide a SELECT statement for the expected results cursor.");
						return;
					}

					if (rtfActualCursor.Text.Trim() == string.Empty) {
						Show();

						rtfActualCursor.Focus();
						MessageBox.Show("You must provide a SELECT statement for the actual results cursor.");
						return;
					}
				}
								
				// Copy what's in the form fields to the current UDC object, so that it can
				// be saved.
				objUDC.description = txtUDCDescription.Text;
				objUDC.name = txtUDCName.Text;
				objUDC.checkType = (udc.enumCheckTypes)Int32.Parse(cmbUDCType.SelectedValue.ToString());

				if (objUDC.plsqlBlock != rtfUDCPLSQLBlock.Text)
					plsql.highlight(rtfUDCPLSQLBlock);
				objUDC.plsqlBlock = rtfUDCPLSQLBlock.Text;

				if (objUDC.rowExistenceCursor != rtfUDCRowExistenceCursor.Text)
					plsql.highlight(rtfUDCRowExistenceCursor);
				objUDC.rowExistenceCursor = rtfUDCRowExistenceCursor.Text;

				if (objUDC.rowValidationCursor != rtfUDCRowValidationCursor.Text)
					plsql.highlight(rtfUDCRowValidationCursor);
				objUDC.rowValidationCursor = rtfUDCRowValidationCursor.Text;

				if (objUDC.plsqlCondition != rtfUDCPLSQLCondition.Text)
					plsql.highlight(rtfUDCPLSQLCondition);
				objUDC.plsqlCondition = rtfUDCPLSQLCondition.Text;

				objUDC.sortOrder = Int32.Parse(txtUDCSortOrder.Text);

				switch (objUDC.checkType) {
					case udc.enumCheckTypes.COMPARE_CURSORS:
						if (objUDC.actualCursor != rtfActualCursor.Text)
							plsql.highlight(rtfActualCursor);
						objUDC.actualCursor = rtfActualCursor.Text;

						if (objUDC.expectedCursor != rtfExpectedCursor.Text)
							plsql.highlight(rtfExpectedCursor);
						objUDC.expectedCursor = rtfExpectedCursor.Text;

						objUDC.csvExcludedColumns = txtCSVExcludedColumns.Text;

						break;

					case udc.enumCheckTypes.ROW_VALIDATION:
						if (objUDC.actualCursor != rtfActualCursor.Text)
							plsql.highlight(rtfActualCursor);
						objUDC.actualCursor = rtfActualCursor.Text;

						try {
							objUDC.parseRowValidatingCursor(Program.getCurrentTargetDBConnection());
						} catch (Exception e) {
							MessageBox.Show(text: e.Message, caption: "Parsing Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
						}

						break;

					case udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX:
						if (objUDC.actualCursor != rtfMatrixComparisonActualCursor.Text)
							plsql.highlight(rtfMatrixComparisonActualCursor);
						objUDC.actualCursor = rtfMatrixComparisonActualCursor.Text;

						break;
				}

				// Update the UDC tree node, associated with this udc.
				udcTreeNode tnUDC = (udcTreeNode)Program.navigationForm.projectTreeView.findNodeByGUID(schemaNodeType.udc, objUDC.guid);
				tnUDC.refresh();

				Program.fieldTracker.needsSaving = false;
			}
		}
		#endregion

		#region Events
		private void frmUDC_Load(object sender, EventArgs e) {
			// Setup tab stops
			rtfUDCPLSQLBlock.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };
			rtfUDCRowExistenceCursor.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };
			rtfUDCRowValidationCursor.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };

			cmbUDCType.DataSource = udc.getUDCTypes();

			// Watch interface fields for changes
			Program.fieldTracker.registerField(txtUDCName);
			Program.fieldTracker.registerField(cmbUDCType);
			Program.fieldTracker.registerField(txtUDCSortOrder);
			Program.fieldTracker.registerField(txtUDCDescription);
			Program.fieldTracker.registerField(rtfUDCPLSQLCondition);
			Program.fieldTracker.registerField(rtfUDCPLSQLBlock);
			Program.fieldTracker.registerField(rtfUDCRowExistenceCursor);
			Program.fieldTracker.registerField(rtfUDCRowValidationCursor);
			Program.fieldTracker.registerField(rtfExpectedCursor);
			Program.fieldTracker.registerField(rtfActualCursor);
			Program.fieldTracker.registerField(txtCSVExcludedColumns);
			Program.fieldTracker.registerField(grdRowValidation);
			Program.fieldTracker.registerField(rtfMatrixComparisonActualCursor);
		}

		private void cmbUDCType_SelectedIndexChanged(object sender, EventArgs e) {
			showAppropriateUDCTab();
		}

		public void showAppropriateUDCTab() {
			// Remove all possible UDC tabs...
			tabUDCOptions.TabPages.Remove(tabUDCCompareCursors);
			tabUDCOptions.TabPages.Remove(tabUDCPLSQLBlock);
			tabUDCOptions.TabPages.Remove(tabUDCRowExistenceCursor);
			tabUDCOptions.TabPages.Remove(tabUDCRowValidator);
			tabUDCOptions.TabPages.Remove(tabUDCompareCursorVsMatrix);

			// Add back the UDC tab we need, based on the UDC type
			switch ((udc.enumCheckTypes) Int32.Parse(((DataRowView) cmbUDCType.SelectedItem)["check_type"].ToString())) {
				case udc.enumCheckTypes.COMPARE_CURSORS:
					tabUDCOptions.TabPages.Add(tabUDCCompareCursors);
					break;

				case udc.enumCheckTypes.CURSOR_RETURNING_NO_ROWS:
					goto case udc.enumCheckTypes.CURSOR_RETURNING_ROWS; // Fall through
				case udc.enumCheckTypes.CURSOR_RETURNING_ROWS:
					tabUDCOptions.TabPages.Add(tabUDCRowExistenceCursor);
					break;

				case udc.enumCheckTypes.PLSQL_BLOCK:
					tabUDCOptions.TabPages.Add(tabUDCPLSQLBlock);
					break;

				case udc.enumCheckTypes.ROW_VALIDATION:
					tabUDCOptions.TabPages.Add(tabUDCRowValidator);
					break;

				case udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX:
					tabUDCOptions.TabPages.Add(tabUDCompareCursorVsMatrix);
					break;
			}
		}

		private void grdRowValidation_CellEnter(object sender, DataGridViewCellEventArgs e) {
			// We're working around a behavior of the gridview, in that, if it's edit mode
			// is EditOnEnter, clicking on the row header to select it for deletion, edits
			// the first column - see the rowheadermouseclick event.
			Program.udcForm.grdRowValidation.EditMode = DataGridViewEditMode.EditOnEnter;
		}

		private void grdRowValidation_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
			// We're working around a behavior of the gridview, in that, if it's edit mode
			// is EditOnEnter, clicking on the row header to select it for deletion, edits
			// the first column - see the CellEnter event for where we reset the edit mode.
			Program.udcForm.grdRowValidation.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

			Program.udcForm.grdRowValidation.EndEdit();
		}
		#endregion
	}
}
