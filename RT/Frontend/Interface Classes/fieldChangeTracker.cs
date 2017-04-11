/*
 * A generic class for tracking GUI fields and keeping track that they've been changed.
 * Basically, you register every field you want to track, and the class can tell you
 * whether or not they need saving, due to changes.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public class fieldChangeTracker {

		#region Delegates
		// Fired when the "needs saving" status changes.
		public delegate void needsSavingFlagChanged(Boolean needsSaving);
		#endregion

		#region Private Variables
		private bool prvEnabled = true;
		private bool prvNeedsSaving = false;
		#endregion

		#region Events
		// Fired when the "needs saving" status changes.
		public needsSavingFlagChanged onNeedsSavingFlagChanged;
		#endregion

		#region Properties
		// Controls whether or not we are currently tracking the fields for changes.
		public bool enabled {
			get { return prvEnabled; }
			set { prvEnabled = value; }
		}

		// Indicates whether or not the tracked fields have any changes to save.
		public bool needsSaving {
			get { return prvNeedsSaving; }
			set {
				if (prvEnabled) {
					if (prvNeedsSaving != value && onNeedsSavingFlagChanged != null) {
						prvNeedsSaving = value;

						onNeedsSavingFlagChanged(value);
					}

					prvNeedsSaving = value;
				}
			}
		}
		#endregion

		#region Public Methods for Registering Fields
		public void registerField(TextBox txtField) {
			txtField.TextChanged += new EventHandler(txtField_TextChanged);
		}

		public void registerField(CheckBox chkField) {
			chkField.CheckedChanged += new EventHandler(chkField_CheckedChanged);
		}

		public void registerField(ComboBox cmbField) {
			cmbField.TextChanged += new EventHandler(cmbField_TextChanged);
		}

		public void registerField(DataGridView grdGrid) {
			grdGrid.CellValueChanged += new DataGridViewCellEventHandler(grdGrid_CellValueChanged);
		}

		public void registerField(RichTextBox rtfField) {
			rtfField.TextChanged += new EventHandler(rtfField_TextChanged);
		}
		#endregion

		#region Private Methods for Handling Field Changed Events
		void grdGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			needsSaving = true;
		}

		void cmbField_TextChanged(object sender, EventArgs e) {
			needsSaving = true;
		}
		
		void chkField_CheckedChanged(object sender, EventArgs e) {
			needsSaving = true;
		}

		void txtField_TextChanged(object sender, EventArgs e) {
			needsSaving = true;
		}

		void rtfField_TextChanged(object sender, EventArgs e) {
			needsSaving = true;
		}
		#endregion
	}
}
