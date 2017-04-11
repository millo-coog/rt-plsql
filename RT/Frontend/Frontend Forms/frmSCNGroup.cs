using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmSCNGroup : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Constructor
		public frmSCNGroup() {
			InitializeComponent();
		}
		#endregion

		#region Methods
		// Loads the interface fields with the values in the given scenario group object
		public void loadFromSCNGroupObject(scenarioGroup objSCNGroup) {
			SuspendLayout();

			Program.fieldTracker.enabled = false;
			
			if (objSCNGroup == null) {
				lblScenarioGroupGuid.Text = String.Empty;
				txtScenarioName.Text = String.Empty;

				txtScenarioDescription.Text = String.Empty;
				txtMaxAllowedRunTime.Text = "300";

				rtfScenarioGroupDeclareBlock.Text = String.Empty;
				
				chkRollbackAfterEachScenario.Checked = true;
			} else {
				lblScenarioGroupGuid.Text = objSCNGroup.guid;
				txtScenarioName.Text = objSCNGroup.name;

				txtScenarioDescription.Text = objSCNGroup.description;
				txtMaxAllowedRunTime.Text = objSCNGroup.maxAllowedRunTimeInSeconds.ToString();

				rtfScenarioGroupDeclareBlock.Text = objSCNGroup.scenarioGroupDeclare;
				if (rtfScenarioGroupDeclareBlock.Text != String.Empty)
					plsql.highlight(rtfScenarioGroupDeclareBlock);
			
				chkReopenConnectionBeforeEveryScenario.Checked = objSCNGroup.reopenConnectionBeforeEveryScenario;
				chkRollbackAfterEachScenario.Checked = objSCNGroup.rollbackAfterEveryScenario;
			}

			Program.mainForm.loadAppTitle();

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
			
			ResumeLayout();
		}
		
		// Copies the information on the form into the given scenario group object...
		public void saveFormToSCNGroupObject(scenarioGroup objSCNGroup) {
			Program.fieldTracker.enabled = false;

			if (objSCNGroup != null) {
				objSCNGroup.name = txtScenarioName.Text;
				// Update the treenode associated with this scenario group.
				Program.navigationForm.projectTreeView.findNodeByGUID(schemaNodeType.scenarioGroup, objSCNGroup.guid).Text = objSCNGroup.name;

				objSCNGroup.description = txtScenarioDescription.Text;

				objSCNGroup.maxAllowedRunTimeInSeconds = Int32.Parse(txtMaxAllowedRunTime.Text);

				if (objSCNGroup.scenarioGroupDeclare != rtfScenarioGroupDeclareBlock.Text)
					plsql.highlight(rtfScenarioGroupDeclareBlock);
				objSCNGroup.scenarioGroupDeclare = rtfScenarioGroupDeclareBlock.Text;
								
				
				objSCNGroup.reopenConnectionBeforeEveryScenario = chkReopenConnectionBeforeEveryScenario.Checked;
				objSCNGroup.rollbackAfterEveryScenario = chkRollbackAfterEachScenario.Checked;
			}
			
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}
		#endregion

		#region Events
		private void frmSCNGroup_Load(object sender, EventArgs e) {
			// Give the RTF's tabs
			rtfScenarioGroupDeclareBlock.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };
			
			// Track form fields for changes...
			Program.fieldTracker.registerField(txtScenarioName);
			Program.fieldTracker.registerField(chkReopenConnectionBeforeEveryScenario);
			Program.fieldTracker.registerField(chkRollbackAfterEachScenario);
			Program.fieldTracker.registerField(txtScenarioDescription);
			Program.fieldTracker.registerField(txtMaxAllowedRunTime);

			Program.fieldTracker.registerField(rtfScenarioGroupDeclareBlock);
			Program.fieldTracker.registerField(chkRollbackAfterEachScenario);
		}
		#endregion
	}
}
