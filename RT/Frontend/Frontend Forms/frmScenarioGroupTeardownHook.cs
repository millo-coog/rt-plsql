using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmScenarioGroupTeardownHook : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmScenarioGroupTeardownHook() {
			InitializeComponent();
		}
		#endregion

		#region Methods
		public void clear() {
			Program.fieldTracker.enabled = false;

			rtfScenarioGroupTeardownBlock.Text = "";

			Program.fieldTracker.enabled = true;
		}

		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {
			SuspendLayout();

			Program.fieldTracker.enabled = false;
			
			if (objSCNGroup == null) {
				rtfScenarioGroupTeardownBlock.Text = String.Empty;
			} else {
				rtfScenarioGroupTeardownBlock.Text = objSCNGroup.scenarioGroupTeardown;
				if (rtfScenarioGroupTeardownBlock.Text != String.Empty)
					plsql.highlight(rtfScenarioGroupTeardownBlock);
			}

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
			
			ResumeLayout();
		}
		
		// Copies the information on the form into the given scenario group object...
		public void saveFormToSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup != null) {				
				if (objSCNGroup.scenarioGroupTeardown != rtfScenarioGroupTeardownBlock.Text)
					plsql.highlight(rtfScenarioGroupTeardownBlock);
				objSCNGroup.scenarioGroupTeardown = rtfScenarioGroupTeardownBlock.Text;
			}
			
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}
		#endregion
		
		#region Events		
		private void frmScenarioGroupTeardownHook_Load(object sender, EventArgs e) {
			rtfScenarioGroupTeardownBlock.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };
		
			Program.fieldTracker.registerField(rtfScenarioGroupTeardownBlock);	
		}
		#endregion
	}
}
