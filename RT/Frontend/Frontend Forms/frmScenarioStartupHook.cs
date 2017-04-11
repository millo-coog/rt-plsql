using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmScenarioStartupHook : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmScenarioStartupHook() {
			InitializeComponent();
		} 
		#endregion

		#region Methods
		public void clear() {
			Program.fieldTracker.enabled = false;

			rtfScenarioStartupBlock.Text = "";
			
			Program.fieldTracker.enabled = true;
		}

		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {
			SuspendLayout();

			Program.fieldTracker.enabled = false;
			
			if (objSCNGroup == null) {
				rtfScenarioStartupBlock.Text = String.Empty;
			} else {
				rtfScenarioStartupBlock.Text = objSCNGroup.scenarioStartup;
				if (rtfScenarioStartupBlock.Text != String.Empty)
					plsql.highlight(rtfScenarioStartupBlock);				
			}

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
			
			ResumeLayout();
		}
		
		// Copies the information on the form into the given scenario group object...
		public void saveFormToSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup != null) {
				if (objSCNGroup.scenarioStartup != rtfScenarioStartupBlock.Text)
					plsql.highlight(rtfScenarioStartupBlock);
				objSCNGroup.scenarioStartup = rtfScenarioStartupBlock.Text;
			}
			
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}
		#endregion
		
		#region Events
		private void frmScenarioStartupHook_Load(object sender, EventArgs e) {
			rtfScenarioStartupBlock.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };
			
			Program.fieldTracker.registerField(rtfScenarioStartupBlock);
		}
		#endregion
	}
}
