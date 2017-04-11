using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmPostParamAssignmentHook : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmPostParamAssignmentHook() {
			InitializeComponent();
		} 
		#endregion

		#region Methods
		public void clear() {
			Program.fieldTracker.enabled = false;

			rtfPostParamAssignment.Text = "";

			Program.fieldTracker.enabled = true;
		}

		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {			
			SuspendLayout();

			Program.fieldTracker.enabled = false;
			
			if (objSCNGroup == null) {				
				rtfPostParamAssignment.Text = String.Empty;
			} else {
				rtfPostParamAssignment.Text = objSCNGroup.postParamAssignment;
				if (rtfPostParamAssignment.Text != String.Empty)
					plsql.highlight(rtfPostParamAssignment);
			}

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
			
			ResumeLayout();
		}
		
		// Copies the information on the form into the given scenario group object...
		public void saveFormToSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup != null) {				
				if (objSCNGroup.postParamAssignment != rtfPostParamAssignment.Text)
					plsql.highlight(rtfPostParamAssignment);
				objSCNGroup.postParamAssignment = rtfPostParamAssignment.Text;
			}
			
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}
		#endregion

		#region Events		
		private void frmPostParamAssignmentHook_Load(object sender, EventArgs e) {
			rtfPostParamAssignment.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };

			Program.fieldTracker.registerField(rtfPostParamAssignment);
		} 
		#endregion
	}
}
