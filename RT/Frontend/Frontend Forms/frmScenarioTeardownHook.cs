using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmScenarioTeardownHook : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmScenarioTeardownHook() {
			InitializeComponent();
		}
		#endregion

		#region Methods
		public void clear() {
			Program.fieldTracker.enabled = false;

			rtfScenarioTeardownBlock.Text = "";
			
			Program.fieldTracker.enabled = true;
		}

		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {
			SuspendLayout();

			Program.fieldTracker.enabled = false;
			
			if (objSCNGroup == null) {				
				rtfScenarioTeardownBlock.Text = String.Empty;
			} else {
				rtfScenarioTeardownBlock.Text = objSCNGroup.scenarioTeardown;
				if (rtfScenarioTeardownBlock.Text != String.Empty)
					plsql.highlight(rtfScenarioTeardownBlock);
			}

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
			
			ResumeLayout();
		}
		
		// Copies the information on the form into the given scenario group object...
		public void saveFormToSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup != null) {
				if (objSCNGroup.scenarioTeardown != rtfScenarioTeardownBlock.Text)
					plsql.highlight(rtfScenarioTeardownBlock);
				objSCNGroup.scenarioTeardown = rtfScenarioTeardownBlock.Text;
			}
			
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}
		#endregion
		
		#region Events
		private void frmScenarioTeardownHook_Load(object sender, EventArgs e) {
			rtfScenarioTeardownBlock.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };
			
			Program.fieldTracker.registerField(rtfScenarioTeardownBlock);
		}
		#endregion
	}
}
