using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmPopupMatrix : Form {
		public frmPopupMatrix() {
			InitializeComponent();

			// Register our matrix control's gridview with the scenario group info tracker, to detect changes.
			Program.fieldTracker.registerField(matrix.gridView);
		}

		private void btnOkay_Click(object sender, EventArgs e) {
			matrix.saveNestedParams();

			this.Hide();
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.Hide();
		}
	}
}
