using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmPreUDCsHook : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmPreUDCsHook() {
			InitializeComponent();
		} 
		#endregion

		#region Methods
		public void clear() {
			Program.fieldTracker.enabled = false;

			rtfPreUDCPLSQL.Text = "";

			Program.fieldTracker.enabled = true;
		}

		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {
			SuspendLayout();

			Program.fieldTracker.enabled = false;
			
			if (objSCNGroup == null) {
				rtfPreUDCPLSQL.Text = String.Empty;
			} else {
				rtfPreUDCPLSQL.Text = objSCNGroup.preUDC;
				if (rtfPreUDCPLSQL.Text != String.Empty)
					plsql.highlight(rtfPreUDCPLSQL);
			}

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
			
			ResumeLayout();
		}
		
		// Copies the information on the form into the given scenario group object...
		public void saveFormToSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup != null) {				
				if (objSCNGroup.preUDC != rtfPreUDCPLSQL.Text)
					plsql.highlight(rtfPreUDCPLSQL);
				objSCNGroup.preUDC = rtfPreUDCPLSQL.Text;
			}
			
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}
		#endregion
		
		#region Events
		private void frmPreUDCsHook_Load(object sender, EventArgs e) {
			rtfPreUDCPLSQL.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };

			Program.fieldTracker.registerField(rtfPreUDCPLSQL);
		}
		#endregion
	}
}
