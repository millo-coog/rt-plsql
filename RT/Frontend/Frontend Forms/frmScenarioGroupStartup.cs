using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmScenarioGroupStartupHook : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmScenarioGroupStartupHook() {
			InitializeComponent();
		}
		#endregion

		#region Methods
		public void clear() {
			Program.fieldTracker.enabled = false;

			rtfScenarioGroupStartupBlock.Text = "";
			
			Program.fieldTracker.enabled = true;
		}

		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup == null) {
				rtfScenarioGroupStartupBlock.Text = "";
			} else {
				rtfScenarioGroupStartupBlock.Text = objSCNGroup.scenarioGroupStartup;
				if (rtfScenarioGroupStartupBlock.Text != String.Empty)
					plsql.highlight(rtfScenarioGroupStartupBlock);
			}
			
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}

		public void saveFormToSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup.scenarioGroupStartup != rtfScenarioGroupStartupBlock.Text)
					plsql.highlight(rtfScenarioGroupStartupBlock);
				objSCNGroup.scenarioGroupStartup = rtfScenarioGroupStartupBlock.Text;

			Program.fieldTracker.enabled = true;
		}
		#endregion

		#region Events
		private void frmGroupStartup_Load(object sender, EventArgs e) {
			rtfScenarioGroupStartupBlock.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };
			
			Program.fieldTracker.registerField(rtfScenarioGroupStartupBlock);
		}
		#endregion
	}
}
