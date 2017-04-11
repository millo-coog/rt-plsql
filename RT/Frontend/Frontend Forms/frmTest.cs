using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmTest : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmTest() {
			InitializeComponent();
		}
		#endregion

		#region Methods
		// Loads the info from the given test into the interface
		public void loadFromTestObject(test objTest) {
			lblTestXMLFilename.Text = objTest.xmlFilename;
			lblTestGuid.Text = objTest.guid;

			txtTestName.Text = objTest.name;
			txtTestDescription.Text = objTest.description;

			txtUnitName.Text = String.Empty;
			if (objTest.unitSchema != String.Empty)
				txtUnitName.Text += objTest.unitSchema;

			if (objTest.unitName != String.Empty)
				txtUnitName.Text += "." + objTest.unitName;

			if (objTest.unitMethod != String.Empty)
				txtUnitName.Text += "." + objTest.unitMethod;

			txtUnitName.Text = txtUnitName.Text.Trim(new char[] { '.' });

			txtOverload.Text = objTest.overload.ToString();
			chkIsPipelinedFunction.Checked = objTest.isPipelinedFunction;

			cmbUnitType.SelectedItem = objTest.unitType;

			rtfTestDeclares.Text = objTest.plSQLDeclare;
			if (rtfTestDeclares.Text != String.Empty)
				plsql.highlight(rtfTestDeclares);

			rtfPLSQLBLock.Text = objTest.plSQLBlock;
			if (rtfPLSQLBLock.Text != String.Empty)
				plsql.highlight(rtfPLSQLBLock);

			lblCreationDate.Text = objTest.creationDate.ToShortDateString();

			grdTestArguments.DataSource = objTest.testArguments;
			
			Program.mainForm.loadAppTitle();
			
			Program.fieldTracker.needsSaving = false;

			// Warn the user if the test's arguments don't match what's in the database...
			if (objTest.hasArgumentMismatch(conTargetDB: Program.getCurrentTargetDBConnection())) {
				MessageBox.Show(text: "This test's old arguments don't match what's currently in the database. Hit the Refresh button to update them.", caption: "Parameter Mismatch", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
			}
		}

		// Copies the test information on the form back into the given test object...
		public void saveToTest(test objTest) {
			Program.fieldTracker.enabled = false;

			if (objTest != null) {				
				objTest.name = txtTestName.Text;

				// Update the treenode associated with this test.
				Program.navigationForm.projectTreeView.findNodeByGUID(nodeType: schemaNodeType.test, guid: objTest.guid).Text = objTest.name;

				objTest.description = txtTestDescription.Text;

				objTest.unitType = cmbUnitType.SelectedItem.ToString();

				if (objTest.plSQLBlock != rtfPLSQLBLock.Text)
					plsql.highlight(rtfPLSQLBLock);
				objTest.plSQLBlock = rtfPLSQLBLock.Text.Trim();

				string[] arrUnitParts = txtUnitName.Text.Split('.');
				objTest.unitSchema = arrUnitParts.Count() >= 1 ? arrUnitParts[0] : String.Empty;

				if (arrUnitParts.Count() == 3) {
					objTest.unitName = arrUnitParts[1];
					objTest.unitMethod = arrUnitParts[2];
				} else if (arrUnitParts.Count() == 2) {
					objTest.unitName = arrUnitParts[1];
					objTest.unitMethod = String.Empty;
				}
				objTest.overload = Int32.Parse(txtOverload.Text);

				objTest.isPipelinedFunction = chkIsPipelinedFunction.Checked;

				if (objTest.plSQLDeclare != rtfTestDeclares.Text)
					plsql.highlight(rtfTestDeclares);
				objTest.plSQLDeclare = rtfTestDeclares.Text;
			}
			
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}

		// Called manually when the test arguments could have changed out
		// from under what's shown on the form.
		public void refreshArguments(testArgumentCollection testArgs) {
			grdTestArguments.DataSource = testArgs;
		}
		#endregion

		#region Events
		private void frmTest_Load(object sender, EventArgs e) {
			grdTestArguments.AutoGenerateColumns = false;

			rtfTestDeclares.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };

			rtfPLSQLBLock.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };

			// Watch fields for changes...
			Program.fieldTracker.registerField(txtTestName);
			Program.fieldTracker.registerField(txtTestDescription);
			Program.fieldTracker.registerField(txtUnitName);
			Program.fieldTracker.registerField(txtOverload);
			Program.fieldTracker.registerField(chkIsPipelinedFunction);
			Program.fieldTracker.registerField(cmbUnitType);
			Program.fieldTracker.registerField(grdTestArguments);

			Program.fieldTracker.registerField(rtfTestDeclares);

			Program.fieldTracker.registerField(rtfPLSQLBLock);
		}
		private void grdTestArguments_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
			// Color the argument rows according to their IN/OUT direction...
			for (int i = 0; i < grdTestArguments.Rows.Count; i++) {
				if (!grdTestArguments.Rows[i].IsNewRow) {
					if (grdTestArguments.Rows[i].Cells["inOut"].Value.ToString() == "RETURN") {
						grdTestArguments.Rows[i].DefaultCellStyle.BackColor = Program.RETURN_VALUE_COLOR;
					} else {
						grdTestArguments.Rows[i].DefaultCellStyle.BackColor = Program.PARAMETER_COLOR;
					}
				}
			}
		}

		private void txtUnitName_Validating(object sender, CancelEventArgs e) {
			if (txtUnitName.Text.Trim() == String.Empty) {
				Program.mainForm.erpMain.SetError((Control) sender, "The unit name is required.");
				e.Cancel = true;
			} else {
				Program.mainForm.erpMain.Clear();
			}
		}
		
		private void frmTest_Shown(object sender, EventArgs e) {
			txtTestName.Focus();
		}

		private void btnUpdateArguments_Click(object sender, EventArgs e) {
			launchArgumentMatcher();
		}

		private void launchArgumentMatcher() {
			if (Program.fieldTracker.needsSaving == false) {
				frmArgumentUpdater frmArgUpd = new frmArgumentUpdater(currTest: Program.mainForm.currTest, conTargetDB: Program.getCurrentTargetDBConnection());

				frmArgUpd.Show();
			} else {
				MessageBox.Show(text: "You must save your test before editing the arguments list.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
			}
		}
		#endregion
	}
}
