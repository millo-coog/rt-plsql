using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmLibraryItems : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmLibraryItems() {
			InitializeComponent();
		}
		#endregion

		#region Methods
		// Loads the interface fields with the values in the given scenario group object
		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {
			if (objSCNGroup == null) {
				cmbLibraryItemName.Items.Clear();
				cmbLibraryItemName.Text = String.Empty;
				txtLibraryItemDescription.Text = String.Empty;
				txtLibraryItemValue.Text = String.Empty;
			} else {
				cmbLibraryItemName.Items.Clear();
				for (int i = 0; i < objSCNGroup.libraryItems.Count; i++) {
					cmbLibraryItemName.Items.Add(objSCNGroup.libraryItems[i].name);
				}
				if (objSCNGroup.libraryItems.Count > 0) {
					cmbLibraryItemName.SelectedIndex = 0;
				} else {
					cmbLibraryItemName.Text = String.Empty;
					txtLibraryItemDescription.Text = String.Empty;
					txtLibraryItemValue.Text = String.Empty;
				}
			}
		}
		#endregion

		#region Events
		private void frmLibraryItems_Load(object sender, EventArgs e) {
			Program.fieldTracker.registerField(txtLibraryItemDescription);
			Program.fieldTracker.registerField(txtLibraryItemValue);
		}

		private void cmbLibraryItemName_SelectedIndexChanged(object sender, EventArgs e) {
			if (cmbLibraryItemName.SelectedIndex >= 0) {
				txtLibraryItemDescription.Text = Program.mainForm.currScenarioGroup.libraryItems[cmbLibraryItemName.SelectedIndex].description;
				txtLibraryItemValue.Text = Program.mainForm.currScenarioGroup.libraryItems[cmbLibraryItemName.SelectedIndex].value;
			}
		}

		private void txtLibraryItemValue_TextChanged(object sender, EventArgs e) {
			if (cmbLibraryItemName.SelectedIndex >= 0) {
				Program.mainForm.currScenarioGroup.libraryItems[cmbLibraryItemName.SelectedIndex].value = txtLibraryItemValue.Text;
			}
		}
		
		private void btnAddLibraryItem_Click(object sender, EventArgs e) {
			libraryItem newLibraryItem = new libraryItem();

			newLibraryItem.name = cmbLibraryItemName.Text;

			cmbLibraryItemName.Items.Add(newLibraryItem.name);
			Program.mainForm.currScenarioGroup.libraryItems.Add(newLibraryItem);

			cmbLibraryItemName.SelectedIndex = cmbLibraryItemName.Items.Count - 1;

			txtLibraryItemDescription.Text = String.Empty;
			txtLibraryItemValue.Text = String.Empty;

			Program.fieldTracker.needsSaving = true;
		}

		private void btnDeleteLibraryItem_Click(object sender, EventArgs e) {
			if (cmbLibraryItemName.SelectedIndex >= 0) {
				Program.mainForm.currScenarioGroup.libraryItems.RemoveAt(cmbLibraryItemName.SelectedIndex);

				cmbLibraryItemName.Items.RemoveAt(cmbLibraryItemName.SelectedIndex);

				txtLibraryItemDescription.Text = String.Empty;
				txtLibraryItemValue.Text = String.Empty;

				Program.fieldTracker.needsSaving = true;
			}
		}

		private void txtLibraryItemDescription_TextChanged(object sender, EventArgs e) {
			if (cmbLibraryItemName.SelectedIndex >= 0) {
				Program.mainForm.currScenarioGroup.libraryItems[cmbLibraryItemName.SelectedIndex].description = txtLibraryItemDescription.Text;
			}
		}
		#endregion
	}
}
