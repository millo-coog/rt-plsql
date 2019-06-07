using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;

namespace RT {
	public partial class frmMain : Form {
		public test currTest;
		public scenarioGroup currScenarioGroup;
		private udc currUDC;

		// Constructor
		public frmMain() {
			InitializeComponent();
		}

		// Destructor
		~frmMain() {
			Program.closeDBConnections();
		}

		#region Events
		private void frmMain_Load(object sender, EventArgs e) {
			SuspendLayout();

			loadPanelLayout();

			tsRunProgress.Text = String.Empty;

			// Load the current tests...
			if (Program.currProject != null) {
				Program.navigationForm.projectTreeView.Populate(Program.currProject);
			}

			syncGUI();

			// ****************************
			// Now that the form is entirely loaded, begin allow changes to be tracked for save prompts.
			// ****************************

			// Register the test controls with the field tracker, so we can
			// prompt the user to save before accidently losing any changes.
			Program.fieldTracker.onNeedsSavingFlagChanged += indicateWeNeedToSave;

			ResumeLayout();
		}

		void ctlProjectTreeView_afterSelect(genericTreeNode selectedNode) {
			syncGUI();
		}

		void ctlProjectTreeView_beforeSelect(TreeViewCancelEventArgs e) {
			promptToSaveAnyChanges(e);
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
			Program.closeDBConnections();
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
			promptToSaveAnyChanges();

			savePanelLayout();
		}

		// Toolbar button to refresh the schema treeview.
		private void tsRefreshTreeView_Click(object sender, EventArgs e) {
			loadProject();
		}
		#endregion

		#region Create Test/SCN Group/UDC
		private void createTest() {
			Program.fieldTracker.enabled = false;
			SuspendLayout();

			test newTest = new test();

			newTest.databaseName = Program.getCurrentTargetDBConnection().DatabaseName;

			// Populate the new test with information about its target...
			genericTreeNode tnTargetNode =
				((genericTreeNode)Program.navigationForm.projectTreeView.selectedNode).getParentNode(
					new schemaNodeType[] { schemaNodeType.schemaLevelFunction, schemaNodeType.schemaLevelProcedure, schemaNodeType.package, schemaNodeType.trigger, schemaNodeType.type, schemaNodeType.method });

			newTest.unitSchema = tnTargetNode.schema;
			newTest.unitName = tnTargetNode.name;
			newTest.unitMethod = tnTargetNode.method;

			if (((genericTreeNode)tnTargetNode).nodeType == schemaNodeType.method) {
				newTest.overload = ((methodTreeNode)tnTargetNode).overload;
			} else {
				newTest.overload = 0;
			}

			newTest.unitType = tnTargetNode.getObjectType();

			newTest.name = newTest.unitSchema + "." + tnTargetNode.name;
			newTest.name += newTest.unitMethod != String.Empty ? "." + tnTargetNode.method : String.Empty;
			newTest.name += newTest.overload == 0 ? String.Empty : "-" + newTest.overload.ToString();

			// If the target already has tests on it, we need to take on an incrementing number to the default name of the new test...
			if (tnTargetNode.Nodes.Count > 1) // Ignore the arguments subfolder
				newTest.name += "_" + (tnTargetNode.Nodes.Count - 1).ToString();

			newTest.name = newTest.name.ToLower();

			newTest.loadArgs(Program.getCurrentTargetDBConnection());

			// Create a treeview node for the new test...
			testTreeNode tnTest = new testTreeNode(associatedTest: newTest);

			tnTargetNode.Nodes.Add(tnTest);

			tnTest.EnsureVisible();
			Program.navigationForm.projectTreeView.selectedNode = tnTest;

			currTest.save(Program.currProject);

			// Add the test's default scenario group to the treeview...
			scenarioGroupTreeNode tnScenarioGroup = new scenarioGroupTreeNode(associatedScenarioGroup: newTest.scenarioGroups[0]);

			Program.navigationForm.projectTreeView.selectedNode.Nodes.Add(tnScenarioGroup);

			tnTest.Expand();

			// Now that the target node may have tests for the first
			// time, etc., update its status
			updateTreeViewNodeStatuses();

			ResumeLayout();
			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = false;
		}

		private void createScenarioGroup() {
			Program.fieldTracker.enabled = false;

			// Create and load a new scenario group...
			scenarioGroup newSCNGroup = new scenarioGroup(currTest);

			scenarioGroupTreeNode tnScenarioGroup = new scenarioGroupTreeNode(associatedScenarioGroup: newSCNGroup);

			currTest.scenarioGroups.Add(newSCNGroup);

			Program.navigationForm.projectTreeView.selectedNode.Nodes.Add(tnScenarioGroup);

			Program.navigationForm.projectTreeView.selectedNode.Expand();

			tnScenarioGroup.EnsureVisible();

			Program.navigationForm.projectTreeView.selectedNode = tnScenarioGroup;

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = true;
		}

		private void createUDC(RT.udc.enumCheckTypes udcType) {
			Program.fieldTracker.enabled = false;

			udc newUDC = new udc(parentScenarioGroup: currScenarioGroup, newCheckType: udcType);

			// Add a new UDC child node to the scenario group node...
			scenarioGroupTreeNode scnGroupTreeNode = (scenarioGroupTreeNode)((genericTreeNode)Program.navigationForm.projectTreeView.selectedNode).getParentNode(parentNodeType: schemaNodeType.scenarioGroup);

			udcTreeNode tnUDC = (udcTreeNode)scnGroupTreeNode.addChildNode(schemaNodeType.udc, associatedUDC: newUDC);

			scnGroupTreeNode.Expand();

			currScenarioGroup.udcCollection.Add(newUDC);

			Program.navigationForm.projectTreeView.selectedNode = tnUDC;

			tnUDC.EnsureVisible();

			Program.fieldTracker.enabled = true;
			Program.fieldTracker.needsSaving = true;
		}
		#endregion

		#region Load/Save
		public void promptToSaveAnyChanges(TreeViewCancelEventArgs e = null) {
			// DJV - This code is commented out because the interface would get into endless loops of
			// prompting the user to save stuff, when they already said yes. Also, the code doesn't
			// properly handle when the user says No (not to save) or when the user is just changing
			// nodes in the treeview, but is still on the same test.

			//genericTreeNode tnNewNode;

			//if (e == null)
			//	tnNewNode = null;
			//else
			//	tnNewNode = (genericTreeNode) e.Node;

			//if (currTest != null &&
			//	(tnNewNode == null || (tnNewNode.test == null || tnNewNode.test.guid != currTest.guid)) &&
			//	(Program.fieldTracker.enabled && Program.fieldTracker.needsSaving)) {
			//	if (MessageBox.Show("Do you want to save your changes?", "Save?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes) {
			//		saveEverything();
			//	} else {
			//		currTest = null;
			//		currScenarioGroup = null;
			//		currUDC = null;

			//		syncGUI();
			//	}
			//}
		}

		private void indicateWeNeedToSave(Boolean needsSaving) {
			this.Text = this.Text.TrimEnd('*');

			if (currTest != null && Program.fieldTracker.needsSaving)
				this.Text += "*";
		}

		private void saveEverything() {
			Program.testForm.saveToTest(objTest: currTest);

			if (currScenarioGroup != null) {
				Program.scnGroupForm.saveFormToSCNGroupObject(objSCNGroup: currScenarioGroup);
				Program.scenarioGroupStartupForm.saveFormToSCNGroupObject(objSCNGroup: currScenarioGroup);
				Program.scenarioStartupForm.saveFormToSCNGroupObject(currScenarioGroup);
				Program.scenariosForm.saveFormToSCNGroupObject(objSCNGroup: currScenarioGroup);
				Program.postParamAssignmentForm.saveFormToSCNGroupObject(objSCNGroup: currScenarioGroup);
				Program.preUDCsForm.saveFormToSCNGroupObject(objSCNGroup: currScenarioGroup);
				Program.scenarioTeardownForm.saveFormToSCNGroupObject(objSCNGroup: currScenarioGroup);
				Program.scenarioGroupTeardownForm.saveFormToSCNGroupObject(objSCNGroup: currScenarioGroup);
			}

			if (currUDC != null)
				Program.udcForm.saveFormToUDC(objUDC: currUDC);

			if (currTest != null) {
				currTest.save(Program.currProject);

				Program.testForm.refreshArguments(testArgs: currTest.testArguments);

				// Upon saving, we need to refresh any loaded UDC that is a row validation, because
				// the repository would have populated its individual fields to validate....
				if (currUDC != null && currUDC.checkType == udc.enumCheckTypes.ROW_VALIDATION) {
					Program.udcForm.loadFromUDCObject(objUDC: currUDC);
				}
			}

			// Because the user may have added/changed the scenarios, refresh
			// the scenarios listed in the scenarios folder in the treeview.
			if (currScenarioGroup != null) {
				((scenariosTreeNode)Program.navigationForm.projectTreeView.findNodeByGUID(schemaNodeType.scenarios, guid: currScenarioGroup.guid)).refresh();
			}
		}
		#endregion

		#region Running
		private void runCurrentItem() {
			if (Program.navigationForm.projectTreeView.selectedNode != null) {
				runAllTestsForObject((RT.genericTreeNode)Program.navigationForm.projectTreeView.selectedNode);
			}
		}

		private void runAllTestsForObject(genericTreeNode tnNode) {
			// Make sure the user isn't running all tests in the database before connecting to it.
			if (tnNode.nodeType == schemaNodeType.database
				&& Program.getCurrentTargetDBConnection().State != ConnectionState.Open) {
				MessageBox.Show("You must first open the connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// If we're sitting on a node beneath a method/function/procedure, we need to go
			// up the tree to figure out which type we're actually beneath, and run all tests
			// for it...
			genericTreeNode targetNode = tnNode.getParentNode(parentNodeTypes: new schemaNodeType[] { schemaNodeType.method, schemaNodeType.schemaLevelFunction, schemaNodeType.schemaLevelProcedure });
			if (targetNode != null)
				tnNode = targetNode;

			Program.outputForm.clearOutput();

			pbRunProgress.Value = 0;
			pbRunProgress.Style = ProgressBarStyle.Marquee;

			// Find out how many scenarios this object has associated with it...
			List<test> lstAssociatedTests = new List<test>();

			Program.currProject.repository.getTestList(
				lstTests: ref lstAssociatedTests,
				databaseName: tnNode.targetDatabase.name,
				objectType: tnNode.getObjectType(),
				schema: tnNode.schema,
				name: tnNode.name,
				method: tnNode.method,
				overload: tnNode.overload);

			int numScenarios = 0;
			foreach (test currTest in lstAssociatedTests) {
				for (int i = 0; i < currTest.scenarioGroups.Count; i++) {
					numScenarios += currTest.scenarioGroups[i].scenarios.Count;
				}
			}
			pbRunProgress.Maximum = numScenarios;

			tsRunProgress.Text = "Running...";
			ApplicationDoEvents();

			Cursor.Current = Cursors.WaitCursor; // Has to be below the Application.DoEvents() to stick...

			string objectDescription =
				Program.getCurrentTargetDBConnection().DatabaseName + "\\" +
				tnNode.schema + "\\" +
				tnNode.getObjectType() + "\\" +
				tnNode.name + "\\" +
				tnNode.method +
				(tnNode.overload == -1 ? "" : "(" + tnNode.overload.ToString() + ")");
			objectDescription = objectDescription.TrimEnd('\\');

			updateRunStatus("Running all tests for object " + objectDescription + "...\n", isError: false);

			Program.currProject.repository.runAllTestsInObject(
				conTarget: Program.getCurrentTargetDBConnection(),

				lstTests: lstAssociatedTests,
				
				runStatusChanged: new RT.runStatusChangedHandler(updateRunStatus),
				scenarioRunCompleted: new RT.scenarioRunCompletedHandler(scenarioRunCompleted)
			);

			updateRunStatus("Done running all tests for object " + objectDescription + "...\n", isError: false);

			pbRunProgress.Value = pbRunProgress.Maximum;

			pbRunProgress.Style = ProgressBarStyle.Continuous;

			updateTreeViewNodeStatuses();

			lstAssociatedTests.Clear();
			GC.Collect();

			Cursor.Current = Cursors.Default;
		}

		private void runCurrentScenarioGroup(OracleConnection conTarget) {
			Program.outputForm.clearOutput();

			if (currScenarioGroup == null) {
				MessageBox.Show("Please choose a scenario group to run.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} else {
				pbRunProgress.Value = 0;
				pbRunProgress.Style = ProgressBarStyle.Marquee;
				pbRunProgress.Maximum = currScenarioGroup.scenarios.Count;

				// Set focus to the output tab, if we're not on the parameters (scenarios) tab,
				// so the user can see the run results.
				//if (Program.scnGroupForm.tabGroup.SelectedTab != Program.scnGroupForm.tabParameters) {
				//   //Program.scnGroupForm.tabGroup.SelectedTab = tabDebug;
				//   this.Refresh();
				//}

				tsRunProgress.Text = "Running...";
				ApplicationDoEvents();

				currScenarioGroup.runStatusChanged -= updateRunStatus; // Remove any previous occurrence of this handler...
				currScenarioGroup.runStatusChanged += updateRunStatus; // ... to prevent duplicate events.

				currScenarioGroup.scenarioRunCompleted -= scenarioRunCompleted; // Remove any previous occurrence of this handler...
				currScenarioGroup.scenarioRunCompleted += scenarioRunCompleted; // ... to prevent duplicate events.

				currScenarioGroup.run(conTarget: conTarget, runResults: Program.currProject.repository.runResults);

				pbRunProgress.Style = ProgressBarStyle.Continuous;
			}

			Program.currProject.repository.runResults.saveResults();

			updateTreeViewNodeStatuses();
		}

		// This will be called whenever the running status of tests/scenarios changes.
		private void updateRunStatus(string message, bool isError) {
			//if (Program.testForm.txtOverload.InvokeRequired)
			//	return;

			if (Program.outputForm.onlyShowFailingScenarios == false || isError) {
				Program.outputForm.debugWrite(message);
				this.Refresh();
			}

			if (tsRunProgress.Text == "Running...")
				tsRunProgress.Text = String.Empty;
		}

		// This is called when a scenario within a scenario group has finished running
		private void scenarioRunCompleted(int folderKey, string testName, string scenarioGroupGuid, string scenarioGroupName, int scenarioIndex, string scenarioGuid, string status, int errorNumber, string errorMessage) {
			pbRunProgress.Style = ProgressBarStyle.Blocks;

			if (pbRunProgress.Value + 1 <= pbRunProgress.Maximum)
				pbRunProgress.Value = pbRunProgress.Value + 1;

			if (pbRunProgress.Value == pbRunProgress.Maximum) {
				pbRunProgress.Style = ProgressBarStyle.Continuous;
			}

			tsRunProgress.Text =
				pbRunProgress.Value.ToString() + "/" + pbRunProgress.Maximum.ToString() + " = " +
				(pbRunProgress.Value / (pbRunProgress.Maximum * 1.00)).ToString("0.00%");

			if (status != "OK") {
				Program.outputForm.debugWrite(
					testName + " - " + scenarioGroupName + " has failed:\n" +
					"   Scenario #" + scenarioIndex.ToString() + "    Result: " + status + "    Error #: " + errorNumber + "\n" +
					"   Error msg: " + errorMessage + "\n"
				);
			}

			ApplicationDoEvents();

			// Update the scenario group's parameter grid, if applicable
			if (currScenarioGroup != null && scenarioGroupGuid == currScenarioGroup.guid) {
				Program.scenariosForm.updateScenarioRunStatus(scenarioGuid, status, errorNumber, errorMessage);
			}
		}

		private void updateTreeViewNodeStatuses() {
			genericTreeNode currentNode = (genericTreeNode)Program.navigationForm.projectTreeView.selectedNode;

			if (currentNode != null) {
				currentNode.updateStatus();

				// Update every parent node's status
				genericTreeNode parentNode = (genericTreeNode)currentNode.Parent;

				while (parentNode != null) {
					parentNode.updateStatus();

					parentNode = (genericTreeNode)parentNode.Parent;
				}

				// Update every child node's status recursively
				updateChildNodeStatuses(currentNode);
			}
		}

		private void updateChildNodeStatuses(genericTreeNode tnNode) {
			foreach (TreeNode childNode in tnNode.Nodes) {
				((genericTreeNode)childNode).updateStatus();

				if (childNode.Nodes.Count > 0)
					updateChildNodeStatuses((genericTreeNode)childNode);
			}
		}

		#endregion

		#region Menu/Popup Menu Items
		private void tsOpenProject_Click(object sender, EventArgs e) {
			openProject();
		}

		private void projectStatsToolStripMenuItem_Click(object sender, EventArgs e) {
			Program.openDBConnections();

			MessageBox.Show(caption: "Project Stats", text: Program.currProject.getStats(), buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
		}

		private void runAllTestsToolStripMenuItem_Click(object sender, EventArgs e) {
			Program.openDBConnections();

			Program.currProject.repository.runAllTests(currProject: Program.currProject);

			Program.closeDBConnections();

			Process p = new Process();
			p.StartInfo.FileName = "runResults.html";
			p.Start();
		}

		private void databaseConnectionToolStripMenuItem_Click(object sender, EventArgs e) {
			createNewDBConnection();
		}

		private void newDatabaseConnectionToolStripMenuItem_Click(object sender, EventArgs e) {
			createNewDBConnection();
		}

		private void searchToolStripMenuItem_Click(object sender, EventArgs e) {
			Program.navigationForm.projectTreeView.search();
		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e) {
			createTest();
		}

		private void syncRepositoryWithDBToolStripMenuItem_Click(object sender, EventArgs e) {
			Program.outputForm.clearOutput();

			Program.outputForm.debugWrite("Starting...");

			Program.openDBConnections(showPreferencesDialog: false);

			foreach (targetDB db in Program.currProject.targetDBs) {
				Program.outputForm.debugWrite("Searching " + db.name + " for mismatched arguments...");
				
				tsRunProgress.Text = "Synching " + db.name;
				pbRunProgress.Value = 0;
				pbRunProgress.Style = ProgressBarStyle.Blocks;

				ApplicationDoEvents();

				List<test> lstTests = new List<test>();
					
				Program.currProject.repository.getTestList(
					lstTests: ref lstTests,
					databaseName: db.name);

				pbRunProgress.Maximum = lstTests.Count+1;

				foreach (test possibleTest in lstTests) {
					if (possibleTest.hasArgumentMismatch(conTargetDB: db.conTargetDB)) {
						Program.outputForm.debugWrite(
							"   " +
							possibleTest.unitSchema + "." +
							possibleTest.unitName +
							(possibleTest.unitMethod == "" ? "" : "." + possibleTest.unitMethod) +
							"(" + possibleTest.overload + ")" +
							"@" + db.name +
							" has mismatched arguments...");

						// Sync the arguments...
						frmArgumentUpdater frmArgUpd = new frmArgumentUpdater(currTest: possibleTest, conTargetDB: db.conTargetDB, automaticMode: true);
						frmArgUpd.Show();
						frmArgUpd.analyze();
						frmArgUpd.Close();

						possibleTest.save(associatedProject: Program.currProject);

						ApplicationDoEvents();
					}

					pbRunProgress.Value++;
				}

				pbRunProgress.Value = pbRunProgress.Maximum;
				tsRunProgress.Text = "Done";
			}

			Program.outputForm.debugWrite("All done.");
		}

		private void createProjectToolStripMenuItem_Click(object sender, EventArgs e) {
			// Create a brand new project...
			project newProject = new project();

			string projectFolderName = "";
			string projectName = "";
			string projectDescription = "";
			string targetDBName = "";

			// Obtain the needed project properties from the user...
			InputBox.Prompt(title: "Project Foldername", promptText: "Fully-qualified folder to hold the project. (Example: 'c:\\temp\\MyKillerApp')", value: ref projectFolderName);
			projectFolderName = projectFolderName.TrimEnd('\\') + "\\";

			newProject.filename = projectFolderName + "project.xml";

			InputBox.Prompt(title: "Project Name", promptText: "Project Name (Ex. MyKillerApp Tests)", value: ref projectName);
			newProject.name = projectName;

			InputBox.Prompt(title: "Project Description", promptText: "Project Description (Ex. Unit tests for my killer app.)", value: ref projectDescription);
			newProject.description = projectDescription;

			Directory.CreateDirectory(path: projectFolderName);

			newProject.save();

			// Not the best way to do this... Really should have the project or repository class start
			// tracking that it is associated with the given database name, even though the folder for it
			// doesn't necessarily exist, yet.
			InputBox.Prompt(title: "Database Name", promptText: "Target Database Name (Ex. XE)", value: ref targetDBName);
			Directory.CreateDirectory(path: projectFolderName + "\\" + targetDBName);

			// Load the new project...
			loadProject(projectFilename: newProject.filename);
		}

		private void navigationToolStripMenuItem_Click(object sender, EventArgs e) {
			constructNavigationPanel();
		}

		private void outputToolStripMenuItem_Click(object sender, EventArgs e) {
			constructOutputPanel();
		}

		// Open the given project file...
		private void openProjectToolStripMenuItem_Click(object sender, EventArgs e) {
			openProject();
		}

		private void openProject() {
			OpenFileDialog ofdProjectFilename = new OpenFileDialog();

			ofdProjectFilename.Title = "Project File to Open";
			ofdProjectFilename.DefaultExt = "*.xml";
			if (ofdProjectFilename.ShowDialog() == DialogResult.OK) {
				loadProject(projectFilename: ofdProjectFilename.FileName);
			}
		}

		// Loads the given project file into the interface...
		public void loadProject(string projectFilename = "") {
			string prjFilename = (projectFilename == "" ? Program.currProject.filename : projectFilename);

			Program.currProject = new project(projectFilename: prjFilename, excludedSchemas: Properties.Settings.Default.schemas_excludedSchemas);

			Properties.Settings.Default.repos_exportPath = Path.GetDirectoryName(prjFilename).TrimEnd('\\') + "\\";
			Properties.Settings.Default.Save();

			SuspendLayout();

			Program.navigationForm.projectTreeView.Populate(Program.currProject);

			currTest = null;
			currScenarioGroup = null;
			currUDC = null;

			syncGUI();

			ResumeLayout();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
			genericTreeNode selectedNode;

			selectedNode = (genericTreeNode) Program.navigationForm.projectTreeView.selectedNode;

			switch (selectedNode.nodeType) {
				case schemaNodeType.test:
					if (MessageBox.Show("Are you sure you want to delete this test?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						deleteTest(selectedNode);
					}
					break;
				case schemaNodeType.scenarioGroup:
					if (MessageBox.Show("Are you sure you want to delete this scenario group?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						currScenarioGroup.delete(Program.currProject.repository.runResults);
						currScenarioGroup = null;
						currUDC = null;

						selectedNode.Remove();

						Program.fieldTracker.needsSaving = true;
					}

					break;

				case schemaNodeType.scenarioGroupStartup:
					if (MessageBox.Show("Are you sure you want to delete this hook?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Program.scenarioGroupStartupForm.clear();
						Program.scenarioGroupStartupForm.Hide();

						selectedNode.Remove();
						Program.fieldTracker.needsSaving = true;
					}

					break;
				case schemaNodeType.scenarioStartup:
					if (MessageBox.Show("Are you sure you want to delete this hook?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Program.scenarioStartupForm.clear();
						Program.scenarioStartupForm.Hide();

						selectedNode.Remove();
						Program.fieldTracker.needsSaving = true;
					}

					break;
				case schemaNodeType.postParamAssignment:
					if (MessageBox.Show("Are you sure you want to delete this hook?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Program.postParamAssignmentForm.clear();
						Program.postParamAssignmentForm.Hide();

						selectedNode.Remove();
						Program.fieldTracker.needsSaving = true;
					}

					break;
				case schemaNodeType.preUDCs:
					if (MessageBox.Show("Are you sure you want to delete this hook?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Program.preUDCsForm.clear();
						Program.preUDCsForm.Hide();

						selectedNode.Remove();
						Program.fieldTracker.needsSaving = true;
					}

					break;
				case schemaNodeType.udc:
					if (MessageBox.Show("Are you sure you want to delete this user-defined check?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						currUDC.delete();
						currUDC = null;
						selectedNode.Remove();
						
						Program.fieldTracker.needsSaving = true;
					}

					break;
				case schemaNodeType.scenarioTeardown:
					if (MessageBox.Show("Are you sure you want to delete this hook?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Program.scenarioTeardownForm.clear();
						Program.scenarioTeardownForm.Hide();

						selectedNode.Remove();
						Program.fieldTracker.needsSaving = true;
					}

					break;
				case schemaNodeType.scenarioGroupTeardown:
					if (MessageBox.Show("Are you sure you want to delete this hook?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Program.scenarioGroupTeardownForm.clear();
						Program.scenarioGroupTeardownForm.Hide();
						
						selectedNode.Remove();
						Program.fieldTracker.needsSaving = true;
					}

					break;
				default:
					MessageBox.Show("You can't delete this node type!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;
			}
		}

		private void scenarioToolStripMenuItem_Click(object sender, EventArgs e) {
			createScenarioGroup();
		}

		private void testToolStripMenuItem3_Click(object sender, EventArgs e) {
			createTest();
		}

		private void scenarioGroupToolStripMenuItem2_Click(object sender, EventArgs e) {
			createScenarioGroup();
		}

		private void testToolStripMenuItem2_Click(object sender, EventArgs e) {
			createTest();
		}

		private void scenarioGroupToolStripMenuItem1_Click(object sender, EventArgs e) {
			createScenarioGroup();
		}
		
		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) {
			frmPreferences myForm = new frmPreferences();

			myForm.ShowDialog();
		}

		private void saveToolStripButton_Click(object sender, EventArgs e) {
			saveEverything();
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e) {
			saveEverything();
		}
		
		private void runCurrentItemMenuItem_Click(object sender, EventArgs e) {
			runCurrentItem();
		}
		
		private void runCurrentItemMainMenuItem_Click(object sender, EventArgs e) {
			runCurrentItem();
		}

		private void mnuClone_Click(object sender, EventArgs e) {
			genericTreeNode tnSelectedNode = (genericTreeNode) Program.navigationForm.projectTreeView.selectedNode;

			if (tnSelectedNode != null) {
				switch (tnSelectedNode.nodeType) {
					case schemaNodeType.scenarioGroup:
						cloneScenarioGroup();
						break;

					case schemaNodeType.udc:
						cloneUDC();
						break;
				}
			}
		}
		
		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void mainMenuRunButton_Click(object sender, EventArgs e) {
			runCurrentItem();
		}

		private void upgradeWorkingCopyToolStripMenuItem_Click(object sender, EventArgs e) {
			Program.currProject.repository.upgrade();

			MessageBox.Show(text: "Done. Please restart the program.", caption: "Upgrade Complete", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
		}
		
		private void clearAllRunResultsToolStripMenuItem_Click(object sender, EventArgs e) {
			clearRunResults();
		}
		
		private void SVNLogToolStripMenuItem_Click(object sender, EventArgs e) {
			showSVNLogHistory();
		}

		private void viewFileToolStripMenuItem_Click(object sender, EventArgs e) {
			viewFile();
		}
		
		private void tsbViewFile_Click(object sender, EventArgs e) {
			viewFile();
		}

		private void aboutToolStripMenuItem1_Click(object sender, EventArgs e) {
			MessageBox.Show(text: "Regression Tester " + Properties.Settings.Default.app_version + ". Created by David Valles. Copyright 2017 David Valles. Open sourced under the MIT License.\n\nhttps://github.com/millo-coog/rt-plsql\n\n" + "Dock Panel License: 'Dock Panel License.txt'", caption: "About...", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
			Process p = new Process();
			p.StartInfo.FileName = Application.StartupPath + "\\help.html";
			p.Start();
		}

		private void mainMenuEditFile_Click(object sender, EventArgs e) {
			viewFile();
		}
		
		private void tsbSVNLogHistory_Click(object sender, EventArgs e) {
			showSVNLogHistory();
		}

		private void tsbSVNUpdate_Click(object sender, EventArgs e) {
			showSVNUpdate();
		}

		private void tsbSVNCommit_Click(object sender, EventArgs e) {
			showSVNCommit();
		}

		private void tsbSVNRevert_Click(object sender, EventArgs e) {
			showSVNRevert();
		}

		private void currentObjectToolStripMenuItem_Click(object sender, EventArgs e) {			
			runCurrentItem();
		}

		private void clearAllRunResultsToolStripMenuItem_Click_1(object sender, EventArgs e) {
			clearRunResults();
		}

		private void testToolStripMenuItem2_Click_1(object sender, EventArgs e) {
			createTest();
		}

		private void scenarioGroupToolStripMenuItem1_Click_1(object sender, EventArgs e) {
			createScenarioGroup();
		}
		
		private void debugToolStripMenuItem_Click(object sender, EventArgs e) {
			getRunBlock(generateDebugBlock: true);
		}

		private void actualToolStripMenuItem_Click(object sender, EventArgs e) {
			getRunBlock(generateDebugBlock: false);
		}

		private void debugToolStripMenuItem1_Click(object sender, EventArgs e) {
			getRunBlock(generateDebugBlock: true);
		}

		private void actualToolStripMenuItem1_Click(object sender, EventArgs e) {
			getRunBlock(generateDebugBlock: false);
		}

		private void pLSQLBlockToolStripMenuItem_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.PLSQL_BLOCK);
		}

		private void cursorReturningNoRowsToolStripMenuItem_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.CURSOR_RETURNING_ROWS);
		}

		private void cursorReturningNoRowsToolStripMenuItem1_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.CURSOR_RETURNING_NO_ROWS);
		}

		private void rowValidatorToolStripMenuItem_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.ROW_VALIDATION);
		}

		private void compareCursorsToolStripMenuItem_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.COMPARE_CURSORS);
		}

		private void cursorVsMatrixToolStripMenuItem_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX);
		}

		private void pLSQLBlockToolStripMenuItem1_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.PLSQL_BLOCK);
		}

		private void cursorReturningRowsToolStripMenuItem_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.CURSOR_RETURNING_ROWS);
		}

		private void cursorReturningNoRowsToolStripMenuItem2_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.CURSOR_RETURNING_NO_ROWS);
		}

		private void rowValidatorToolStripMenuItem1_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.ROW_VALIDATION);
		}

		private void cursorVsCursorToolStripMenuItem_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.COMPARE_CURSORS);
		}

		private void cursorVsMatrixToolStripMenuItem1_Click(object sender, EventArgs e) {
			createUDC(udcType: udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX);
		}
		
		private void scenarioGroupStartupToolStripMenuItem_Click(object sender, EventArgs e) {
			addHook(schemaNodeType.scenarioGroupStartup);
		}

		private void scenarioStartupToolStripMenuItem_Click(object sender, EventArgs e) {
			addHook(schemaNodeType.scenarioStartup);
		}

		private void postParamAssignmentToolStripMenuItem_Click(object sender, EventArgs e) {
			addHook(schemaNodeType.postParamAssignment);
		}

		private void preUDCsToolStripMenuItem_Click(object sender, EventArgs e) {
			addHook(schemaNodeType.preUDCs);
		}

		private void scenarioTeardownToolStripMenuItem_Click(object sender, EventArgs e) {
			addHook(schemaNodeType.scenarioTeardown);
		}

		private void scenarioGroupTeardownToolStripMenuItem_Click(object sender, EventArgs e) {
			addHook(schemaNodeType.scenarioGroupTeardown);
		}

		private void addHook(schemaNodeType nodeType) {
			scenarioGroupTreeNode scnGroupTreeNode = (scenarioGroupTreeNode) Program.navigationForm.projectTreeView.selectedNode.getParentNode(schemaNodeType.scenarioGroup);

			scnGroupTreeNode.Expand();

			Program.navigationForm.projectTreeView.selectedNode = scnGroupTreeNode.addChildNode(nodeType);

			Program.fieldTracker.needsSaving = true;
		}
		#endregion

		#region Panel Methods
		private void loadPanelLayout() {
			string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RT PL_SQL\\";

			DeserializeDockContent deserializeDelegate = new DeserializeDockContent(getPanelContentFromSerializedString);

			Program.outputForm = new frmOutput();
			Program.navigationForm = new frmNavigation();

			if (File.Exists(appFolder + "panelLayout.xml")) {
				dckPanel.LoadFromXml(appFolder + "panelLayout.xml", deserializeDelegate);

				// Wire up the project treeview
				Program.navigationForm.projectTreeView.popupMenu = cmsNavigator;
				Program.navigationForm.projectTreeView.afterSelect += new RT.User_Controls.ctlProjectTreeView.AfterSelectHandler(ctlProjectTreeView_afterSelect);
				Program.navigationForm.projectTreeView.beforeSelect += new RT.User_Controls.ctlProjectTreeView.BeforeSelectHandler(ctlProjectTreeView_beforeSelect);			
			} else {
				constructOutputPanel();
				constructNavigationPanel();				
			}
			
			// Add docked forms
			dckPanel.BackColor = Color.LightGray;
			dckPanel.ShowDocumentIcon = true;

			constructTestPanel();
			constructSCNGroupPanel();
			constructLibraryItemsPanel();
			constructGroupStartupPanel();
			constructScenarioStartupPanel();
			constructScenariosPanel();
			constructPostParamAssignmentPanel();
			constructPreUDCsPanel();			
			constructUDCPanel();
			constructScenarioTeardownPanel();
			constructScenarioGroupTeardownPanel();
		}

		private void savePanelLayout() {
			string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RT PL_SQL\\";

			if (!Directory.Exists(appFolder))
				Directory.CreateDirectory(appFolder);

			dckPanel.SaveAsXml(appFolder + "panelLayout.xml");
		}

		private IDockContent getPanelContentFromSerializedString(string persistString) {
			if (persistString == typeof(frmNavigation).ToString())
				return Program.navigationForm;
			else if (persistString == typeof(frmOutput).ToString())
				return Program.outputForm;
			else if (persistString == typeof(frmTest).ToString())
				return null;
			else if (persistString == typeof(frmSCNGroup).ToString())
				return null;
			else if (persistString == typeof(frmUDC).ToString())
				return null;

			return null;
		}

		private void constructOutputPanel() {
			// Because the user might close the output panel, and then exit the program,
			// we need to completely rebuild the output panel when called.
			Program.outputForm = new frmOutput();
			Program.outputForm.ShowHint = DockState.Float;
			Program.outputForm.Visible = false;
			Program.outputForm.Show(dckPanel);
			Program.outputForm.DockHandler.FloatPane.DockTo(dckPanel.DockWindows[DockState.DockBottomAutoHide]);
			Program.outputForm.Visible = true;
			Program.outputForm.CloseButtonVisible = false;
			Program.outputForm.Show();
		}

		private void constructNavigationPanel() {
			// Because the user might close the navigation panel, and then exit the program,
			// we need to completely rebuild the navigation panel when called.
			Program.navigationForm = new frmNavigation();
			Program.navigationForm.Visible = false;
			Program.navigationForm.ShowHint = DockState.DockLeft;
			Program.navigationForm.Show(dckPanel);
			Program.navigationForm.Visible = true;
			Program.navigationForm.CloseButtonVisible = false;

            if (Program.currProject != null)
                Program.navigationForm.projectTreeView.Populate(Program.currProject);

			// Wire up the project treeview
			Program.navigationForm.projectTreeView.popupMenu = cmsNavigator;
			Program.navigationForm.projectTreeView.afterSelect += new RT.User_Controls.ctlProjectTreeView.AfterSelectHandler(ctlProjectTreeView_afterSelect);
			Program.navigationForm.projectTreeView.beforeSelect += new RT.User_Controls.ctlProjectTreeView.BeforeSelectHandler(ctlProjectTreeView_beforeSelect);

			Program.navigationForm.Show();
		}

		private void constructTestPanel() {
			if (Program.testForm == null || Program.testForm.IsDisposed) {
				Program.testForm = new frmTest();
				Program.testForm.ShowHint = DockState.Document;
				Program.testForm.Visible = false;
				Program.testForm.Show(dckPanel);
				Program.testForm.CloseButtonVisible = false;
				Program.testForm.Hide();
			}
		}

		private void constructSCNGroupPanel() {
			if (Program.scnGroupForm == null || Program.scnGroupForm.IsDisposed) {
				Program.scnGroupForm = new frmSCNGroup();
				Program.scnGroupForm.ShowHint = DockState.Document;
				Program.scnGroupForm.Visible = false;
				Program.scnGroupForm.Show(dckPanel);
				Program.scnGroupForm.CloseButtonVisible = false;
				Program.scnGroupForm.Hide();
			}
		}

		private void constructLibraryItemsPanel() {
			if (Program.libraryItemsForm == null || Program.libraryItemsForm.IsDisposed) {
				Program.libraryItemsForm = new frmLibraryItems();
				Program.libraryItemsForm.ShowHint = DockState.Document;
				Program.libraryItemsForm.Visible = false;
				Program.libraryItemsForm.Show(dckPanel);
				Program.libraryItemsForm.CloseButtonVisible = false;
				Program.libraryItemsForm.Hide();
			}
		}

		private void constructGroupStartupPanel() {
			if (Program.scenarioGroupStartupForm == null || Program.scenarioGroupStartupForm.IsDisposed) {
				Program.scenarioGroupStartupForm = new frmScenarioGroupStartupHook();
				Program.scenarioGroupStartupForm.ShowHint = DockState.Document;
				Program.scenarioGroupStartupForm.Visible = false;
				Program.scenarioGroupStartupForm.Show(dckPanel);
				Program.scenarioGroupStartupForm.CloseButtonVisible = false;
				Program.scenarioGroupStartupForm.Hide();
			}
		}

		private void constructScenarioStartupPanel() {
			if (Program.scenarioStartupForm == null || Program.scenarioStartupForm.IsDisposed) {
				Program.scenarioStartupForm = new frmScenarioStartupHook();
				Program.scenarioStartupForm.ShowHint = DockState.Document;
				Program.scenarioStartupForm.Visible = false;
				Program.scenarioStartupForm.Show(dckPanel);
				Program.scenarioStartupForm.CloseButtonVisible = false;
				Program.scenarioStartupForm.Hide();
			}
		}

		private void constructScenariosPanel() {
			if (Program.scenariosForm == null || Program.scenariosForm.IsDisposed) {
				Program.scenariosForm = new frmScenarios();
				Program.scenariosForm.ShowHint = DockState.Document;
				Program.scenariosForm.Visible = false;
				Program.scenariosForm.Show(dckPanel);
				Program.scenariosForm.CloseButtonVisible = false;
				Program.scenariosForm.Hide();
			}
		}

		private void constructPostParamAssignmentPanel() {
			if (Program.postParamAssignmentForm == null || Program.postParamAssignmentForm.IsDisposed) {
				Program.postParamAssignmentForm = new frmPostParamAssignmentHook();
				Program.postParamAssignmentForm.ShowHint = DockState.Document;
				Program.postParamAssignmentForm.Visible = false;
				Program.postParamAssignmentForm.Show(dckPanel);
				Program.postParamAssignmentForm.CloseButtonVisible = false;
				Program.postParamAssignmentForm.Hide();
			}
		}

		private void constructPreUDCsPanel() {
			if (Program.preUDCsForm == null || Program.preUDCsForm.IsDisposed) {
				Program.preUDCsForm = new frmPreUDCsHook();
				Program.preUDCsForm.ShowHint = DockState.Document;
				Program.preUDCsForm.Visible = false;
				Program.preUDCsForm.Show(dckPanel);
				Program.preUDCsForm.CloseButtonVisible = false;
				Program.preUDCsForm.Hide();
			}
		}

		private void constructUDCPanel() {
			if (Program.udcForm == null || Program.udcForm.IsDisposed) {
				Program.udcForm = new frmUDC();
				Program.udcForm.ShowHint = DockState.Document;
				Program.udcForm.Visible = false;
				Program.udcForm.Show(dckPanel);
				Program.udcForm.CloseButtonVisible = false;
				Program.udcForm.Hide();
			}
		}

		private void constructScenarioTeardownPanel() {
			if (Program.scenarioTeardownForm == null || Program.scenarioTeardownForm.IsDisposed) {
				Program.scenarioTeardownForm = new frmScenarioTeardownHook();
				Program.scenarioTeardownForm.ShowHint = DockState.Document;
				Program.scenarioTeardownForm.Visible = false;
				Program.scenarioTeardownForm.Show(dckPanel);
				Program.scenarioTeardownForm.CloseButtonVisible = false;
				Program.scenarioTeardownForm.Hide();
			}
		}

		private void constructScenarioGroupTeardownPanel() {
			if (Program.scenarioGroupTeardownForm == null || Program.scenarioGroupTeardownForm.IsDisposed) {
				Program.scenarioGroupTeardownForm = new frmScenarioGroupTeardownHook();
				Program.scenarioGroupTeardownForm.ShowHint = DockState.Document;
				Program.scenarioGroupTeardownForm.Visible = false;
				Program.scenarioGroupTeardownForm.Show(dckPanel);
				Program.scenarioGroupTeardownForm.CloseButtonVisible = false;
				Program.scenarioGroupTeardownForm.Hide();
			}
		}
		
		public void showAppropriatePanels() {
			SuspendLayout();
			
			if (currTest == null && !Program.testForm.IsDisposed) {
				Program.testForm.Hide();
			} else {
				constructTestPanel();
			}
			
			// If there isn't a current scenario group, hide any scenario group panels...
			if (currScenarioGroup == null) {
				if (Program.scnGroupForm.IsDisposed == false)
					Program.scnGroupForm.Hide();
				if (Program.scenarioGroupStartupForm.IsDisposed == false)
					Program.scenarioGroupStartupForm.Hide();
				if (Program.libraryItemsForm.IsDisposed == false)
					Program.libraryItemsForm.Hide();
				if (Program.scenarioStartupForm.IsDisposed == false)
					Program.scenarioStartupForm.Hide();
				if (Program.scenariosForm.IsDisposed == false)
					Program.scenariosForm.Hide();
				if (Program.postParamAssignmentForm.IsDisposed == false)
					Program.postParamAssignmentForm.Hide();
				if (Program.preUDCsForm.IsDisposed == false)
					Program.preUDCsForm.Hide();
				if (Program.scenarioTeardownForm.IsDisposed == false)
					Program.scenarioTeardownForm.Hide();
				if (Program.scenarioGroupTeardownForm.IsDisposed == false)
					Program.scenarioGroupTeardownForm.Hide();
			} else {
				constructSCNGroupPanel();
				constructScenarioStartupPanel();
				constructLibraryItemsPanel();
				constructGroupStartupPanel();
				constructScenarioStartupPanel();
				constructScenariosPanel();
				constructPostParamAssignmentPanel();
				constructPreUDCsPanel();
				constructScenarioTeardownPanel();
				constructScenarioGroupTeardownPanel();
			}

			// If there isn't a current UDC, hide any UDC GUI items...
			if (currUDC == null) {
				if (!Program.udcForm.IsDisposed) {
					Program.udcForm.Hide();
				}
			} else {
				constructUDCPanel();
			}

			if (Program.navigationForm.projectTreeView.selectedNode == null) {
				if (!Program.testForm.IsDisposed)
					Program.testForm.Hide();
			} else {
				switch (Program.navigationForm.projectTreeView.selectedNode.nodeType) {
					case schemaNodeType.test:
						Program.testForm.Show();
						break;

					case schemaNodeType.scenarioGroup:
						Program.scnGroupForm.Show();
						break;

					case schemaNodeType.libraryItems:
						Program.libraryItemsForm.Show();
						break;

					case schemaNodeType.scenarioGroupStartup:
						Program.scenarioGroupStartupForm.Show();
						break;

					case schemaNodeType.scenarioStartup:
						Program.scenarioStartupForm.Show();
						break;

					case schemaNodeType.scenarios:
					case schemaNodeType.scenario:
						Program.scenariosForm.Show();
						break;

					case schemaNodeType.postParamAssignment:
						Program.postParamAssignmentForm.Show();
						break;

					case schemaNodeType.preUDCs:
						Program.preUDCsForm.Show();
						break;

					case schemaNodeType.udc:
						Program.udcForm.Show();
						break;

					case schemaNodeType.scenarioTeardown:
						Program.scenarioTeardownForm.Show();
						break;

					case schemaNodeType.scenarioGroupTeardown:
						Program.scenarioGroupTeardownForm.Show();
						break;

					default:
						break;
				}
			}

			ResumeLayout();
		}
		#endregion

		public void getRunBlock(bool generateDebugBlock) {
			if (currScenarioGroup == null) {
				MessageBox.Show("You need to choose a scenario group first!", "Error", MessageBoxButtons.OK);
			} else {
				if (Properties.Settings.Default.plsql_editor == "") {
					MessageBox.Show("You need to set the path to your PL/SQL editor in the preferences, and then try again!", "PL/SQL Editor Path Not Set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					frmPreferences myForm = new frmPreferences();
					myForm.ShowDialog();
				} else {
					// Save the run block to a temp file, and then launch it in the user's editor
					string runBlock = (new runBlock()).getPLSQLRunBlock(conTargetDB: Program.getCurrentTargetDBConnection(), scnGroup: currScenarioGroup, targetDBName: Program.getCurrentTargetDBConnection().DatabaseName, generateAsDebugBlock: generateDebugBlock);

					string runBlockFilename = Path.GetTempPath() + "\\runblock.sql";

					System.IO.File.WriteAllText(runBlockFilename, runBlock);

					Process p = new Process();
					p.StartInfo.FileName = Properties.Settings.Default.plsql_editor;
					p.StartInfo.Arguments = "\"" + runBlockFilename + "\"";
					p.Start();
				}
			}
		}

		private void cloneUDC() {
			if (currUDC == null) {
				MessageBox.Show("You must choose a UDC to clone!");
			} else {
				// Clone the current UDC into the same scenario group as I'm in, and then load the clone in the interface
				currUDC = currUDC.clone();

				udcTreeNode tnUDC = new udcTreeNode(associatedUDC: currUDC);

				Program.navigationForm.projectTreeView.selectedNode.Parent.Nodes.Add(tnUDC);
				Program.navigationForm.projectTreeView.selectedNode.Expand();

				Program.navigationForm.projectTreeView.selectedNode = tnUDC;
				
				tnUDC.EnsureVisible();
			}
		}

		private void cloneScenarioGroup() {
			if (currScenarioGroup == null) {
				MessageBox.Show("You must choose a scenario group to clone!");
			} else {
				// Clone the current scenario group into the same test as I'm in, and then load the clone in the interface
				currScenarioGroup = currScenarioGroup.clone(newTest: currTest);

				scenarioGroupTreeNode tnScenarioGroup = new scenarioGroupTreeNode(associatedScenarioGroup: currScenarioGroup);

				Program.navigationForm.projectTreeView.selectedNode.Parent.Nodes.Add(tnScenarioGroup);
				Program.navigationForm.projectTreeView.selectedNode.Expand();

				Program.navigationForm.projectTreeView.selectedNode = tnScenarioGroup;
				
				tnScenarioGroup.EnsureVisible();
			}
		}
		
		// Because Application.DoEvents loses the cursor, I use this one, instead....
		private void ApplicationDoEvents() {
			Cursor oldCursor = Cursor.Current;
			Application.DoEvents();
			Cursor.Current = oldCursor;
		}
						
		public void loadAppTitle() {
			this.Text =
				Application.ProductName +
				" - Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

			if (currTest != null)
				this.Text += " - " + currTest.name;

			if (currScenarioGroup != null)
				this.Text += " - " + currScenarioGroup.name;
		}
		
		void syncGUI() {
			SuspendLayout();

			genericTreeNode selectedNode = Program.navigationForm.projectTreeView.selectedNode;

			/*
			 * Handle any change of the current test, scenario group, or UDC.
			 */
			if (selectedNode != null) {
				bool forceSCNGroupBreak = false;
				bool forceUDCBreak = false;

				// Handle the user changing tests
				if ((currTest == null && selectedNode.test != null) // User is loading a test for the first time
					|| (currTest != null && selectedNode.test != null // User is changing tests
						&& currTest != selectedNode.test)) {
					// Force SCN Group change
					if (currScenarioGroup != null)
						forceSCNGroupBreak = true;

					currTest = selectedNode.test;

					showAppropriatePanels();

					Program.testForm.loadFromTestObject(objTest: currTest);
				}

				// Handle the user changing scenario groups
				if (forceSCNGroupBreak
					|| (currScenarioGroup == null && selectedNode.scenarioGroup != null) // User is loading an scn group for the first time
					|| (currScenarioGroup != null && selectedNode.scenarioGroup != null // User is changing scn groups
						&& currScenarioGroup != selectedNode.scenarioGroup)) {
					// Force UDC change
					if (currUDC != null)
						forceUDCBreak = true;

					// Load selected scenario group...
					if (currScenarioGroup != selectedNode.scenarioGroup) {
						currScenarioGroup = selectedNode.scenarioGroup;

						showAppropriatePanels();

						tsRunProgress.Text = String.Empty;
						pbRunProgress.Value = 0;
						pbRunProgress.Style = ProgressBarStyle.Blocks;

						Program.scnGroupForm.loadFromSCNGroupObject(currScenarioGroup);
						Program.scenarioGroupStartupForm.loadFromSCNGroupObject(currScenarioGroup);
						Program.scenarioStartupForm.loadFromSCNGroupObject(currScenarioGroup);
						Program.libraryItemsForm.loadFromSCNGroupObject(currScenarioGroup);
						Program.scenariosForm.loadFromSCNGroupObject(currScenarioGroup);
						Program.postParamAssignmentForm.loadFromSCNGroupObject(currScenarioGroup);
						Program.preUDCsForm.loadFromSCNGroupObject(currScenarioGroup);
						Program.scenarioTeardownForm.loadFromSCNGroupObject(currScenarioGroup);
						Program.scenarioGroupTeardownForm.loadFromSCNGroupObject(currScenarioGroup);
					}
				}

				// Handle the user changing UDC's
				if (forceUDCBreak
					|| (currUDC == null && selectedNode.udc != null && selectedNode.nodeType == schemaNodeType.udc) // User is loading a UDC for the first time
					|| (currUDC != null && selectedNode.udc != null && selectedNode.nodeType == schemaNodeType.udc // User is changing UDC's
						&& currUDC != selectedNode.udc)) {
					// Load selected udc...
					currUDC = selectedNode.udc;

					showAppropriatePanels();

					Program.udcForm.loadFromUDCObject(objUDC: currUDC);
				}
			}

			showAppropriatePanels();
			loadAppTitle();

			/*
			 * Update the menus.
			 */

			// Disable all optional menu items - they'll be turned back on, as appropriate...
			tsbSVNLogHistory.Enabled = false;
			tsbViewFile.Enabled = false;

			mainMenuViewFile.Enabled = false;
			tsbGetRunBlock.Enabled = false;

			testToolStripMenuItem2.Visible = false;
			scenarioGroupToolStripMenuItem1.Visible = false;
			userDefinedCheckToolStripMenuItem.Visible = false;

			viewFileToolStripMenuItem.Visible = false;

			// Hide every element in the popup menu, because we'll turn just
			// the ones we need back on.
			foreach (ToolStripItem item in cmsNavigator.Items) {
				item.Visible = false;
			}

			// Process the node selection...
			if (selectedNode != null) {
				switch (selectedNode.nodeType) {
					case schemaNodeType.schemaLevelFunction:
					case schemaNodeType.schemaLevelProcedure:
					case schemaNodeType.trigger:
					case schemaNodeType.method:
					case schemaNodeType.argumentsFolder:
					case schemaNodeType.methodArgument:
					case schemaNodeType.view:
						// Enable appropriate popup menu items...
						testToolStripMenuItem.Visible =
						testToolStripMenuItem2.Visible =
						tsNewSeparator.Visible =
						runCurrentItemToolStripMenuItem.Visible = true;

						if (selectedNode.nodeType != schemaNodeType.view) {
							mainMenuViewFile.Enabled = true;
							tsbViewFile.Enabled = true;
							viewFileToolStripMenuItem.Visible = true;
						}

						break;

					case schemaNodeType.database:
					case schemaNodeType.functionsFolder:
					case schemaNodeType.package:
					case schemaNodeType.packagesFolder:
					case schemaNodeType.proceduresFolder:
					case schemaNodeType.schema:
					case schemaNodeType.triggersFolder:
					case schemaNodeType.type:
					case schemaNodeType.typesFolder:
					case schemaNodeType.viewsFolder:
						// Enable appropriate popup menu items...

						if (selectedNode.nodeType == schemaNodeType.database) {
							tsSepNewDBConnection.Visible =
							newDatabaseConnectionToolStripMenuItem.Visible = true;
						}

						runCurrentItemToolStripMenuItem.Visible =	true;

						if (selectedNode.nodeType == schemaNodeType.package || selectedNode.nodeType == schemaNodeType.type) {
							mainMenuViewFile.Enabled = true;
							tsbViewFile.Enabled = true;
							viewFileToolStripMenuItem.Visible = true;
						}

						break;

					case schemaNodeType.test:
						// Enable appropriate menu items...
						tsbSVNLogHistory.Enabled = true;
						tsbViewFile.Enabled = true;
						mainMenuViewFile.Enabled = true;
						scenarioGroupToolStripMenuItem1.Visible = true;

						// Enable appropriate popup menu items...
						scenarioToolStripMenuItem.Visible =
						tsNewSeparator.Visible =
						runCurrentItemToolStripMenuItem.Visible =
						tsRunCurrentItemSeparator.Visible =
						deleteToolStripMenuItem.Visible =
						tsDeleteSeparator.Visible =
						SVNLogToolStripMenuItem.Visible =
						viewFileToolStripMenuItem.Visible = true;
						
						break;

					case schemaNodeType.scenarioGroup:
					case schemaNodeType.libraryItems:
					case schemaNodeType.scenarios:
						// Enable appropriate menu items...
						tsbSVNLogHistory.Enabled =
						tsbViewFile.Enabled =
						tsbGetRunBlock.Enabled =
						mainMenuViewFile.Enabled = true;
						
						userDefinedCheckToolStripMenuItem.Visible = true;
						
						// Enable appropriate popup menu items...
						addHookToolStripMenuItem.Visible = 
						checkToolStripMenuItem.Visible =
						tsNewSeparator.Visible =
						runCurrentItemToolStripMenuItem.Visible =
						tsRunCurrentItemSeparator.Visible =
						mnuClone.Visible =
						tsCloneSeparator.Visible =
						deleteToolStripMenuItem.Visible =
						tsDeleteSeparator.Visible =
						SVNLogToolStripMenuItem.Visible =
						viewFileToolStripMenuItem.Visible =
						getRunBlockToolStripMenuItem.Visible = true;
						
						break;
					
					case schemaNodeType.scenarioGroupStartup:
					case schemaNodeType.scenarioStartup:
					case schemaNodeType.postParamAssignment:
					case schemaNodeType.preUDCs:
					case schemaNodeType.scenarioTeardown:
					case schemaNodeType.scenarioGroupTeardown:
						// Enable appropriate menu items...
						tsbSVNLogHistory.Enabled =
						tsbViewFile.Enabled =
						tsbGetRunBlock.Enabled =
						mainMenuViewFile.Enabled = true;

						userDefinedCheckToolStripMenuItem.Visible = true;

						// Enable appropriate popup menu items...
						addHookToolStripMenuItem.Visible = 
						checkToolStripMenuItem.Visible =
						tsNewSeparator.Visible =
						runCurrentItemToolStripMenuItem.Visible =
						tsRunCurrentItemSeparator.Visible =
						deleteToolStripMenuItem.Visible =
						tsDeleteSeparator.Visible =
						SVNLogToolStripMenuItem.Visible =
						viewFileToolStripMenuItem.Visible =
						getRunBlockToolStripMenuItem.Visible = true;

						break;

					case schemaNodeType.udc:
						// Enable appropriate menu items...
						tsbSVNLogHistory.Enabled =
						tsbViewFile.Enabled =
						tsbGetRunBlock.Enabled =
						userDefinedCheckToolStripMenuItem.Visible =
						mainMenuViewFile.Enabled = true;
						
						// Load selected scenario group...
						addHookToolStripMenuItem.Visible =
						checkToolStripMenuItem.Visible =
						runCurrentItemToolStripMenuItem.Visible =
						deleteToolStripMenuItem.Visible =
						tsDeleteSeparator.Visible =
						tsRunCurrentItemSeparator.Visible =
						SVNLogToolStripMenuItem.Visible =
						viewFileToolStripMenuItem.Visible =
						getRunBlockToolStripMenuItem.Visible =
							true;
						
						break;

					default:
						break;
				}
			}

			ResumeLayout();
		}

		private void deleteTest(genericTreeNode node) {
			// Remove the exported test from the WC
			Program.currProject.repository.deleteTestFromWC(testToDelete: currTest);

			// Delete the test from the repository
			currTest.delete();
			
			currTest = null;
			currScenarioGroup = null;
			currUDC = null;
			
			node.Remove();

			// Now that a target might not have any tests, etc.,
			// update its status.
			updateTreeViewNodeStatuses();
		}

		private void clearRunResults() {
			Program.currProject.repository.runResults.clearResults();

			Program.navigationForm.projectTreeView.Populate(Program.currProject);
		}

		// Opens up the user's file viewer, passing the associated file.
		protected void viewFile() {
			testTreeNode tnTest = (testTreeNode)((genericTreeNode)Program.navigationForm.projectTreeView.selectedNode).getParentNode(schemaNodeType.test);

			if (tnTest == null) {
				// Find the nearest object above me that we can show the source for...
				genericTreeNode tnSQLObject = 
					(genericTreeNode) Program.navigationForm.projectTreeView.selectedNode.getParentNode(
						new schemaNodeType[] { schemaNodeType.schemaLevelFunction, schemaNodeType.schemaLevelProcedure, schemaNodeType.package, schemaNodeType.trigger, schemaNodeType.type });

				if (tnSQLObject != null) {
					OracleCommand cmdGetSource = new OracleCommand();
					String tempFilename = Path.GetTempPath() + "\\rtTempSource.sql";

					// Using dba_source because all_source doesn't contain PACKAGE BODY code.
					String sql = @"
						SELECT dba_source.text
						  FROM dba_source
						 WHERE dba_source.owner = :p_Owner
							AND dba_source.name = :p_Name
							AND dba_source.type = :p_Type
						 ORDER BY dba_source.line";

					cmdGetSource.Connection = Program.getCurrentTargetDBConnection();

					cmdGetSource.Parameters.Add("p_Owner", tnSQLObject.schema);
					cmdGetSource.Parameters.Add("p_Name", tnSQLObject.name);

					String type = "";
					switch (tnSQLObject.nodeType) {
						case schemaNodeType.schemaLevelFunction:
							type = "FUNCTION";
							break;
						case schemaNodeType.schemaLevelProcedure:
							type = "PROCEDURE";
							break;
						case schemaNodeType.package:
							type = "PACKAGE BODY";
							break;
						case schemaNodeType.trigger:
							type = "TRIGGER";
							break;
						case schemaNodeType.type:
							type = "TYPE BODY";
							break;
					}

					cmdGetSource.Parameters.Add("p_Type", type);

					cmdGetSource.CommandText = sql;
					OracleDataReader drSource = cmdGetSource.ExecuteReader();

					using (System.IO.StreamWriter file = new System.IO.StreamWriter(tempFilename)) {
						while (drSource.Read()) {
							file.Write(drSource.GetValue(0).ToString());
						}
					}

					drSource.Close();
					drSource.Dispose();

					cmdGetSource.Dispose();

					// Launch the source file in the user's PL/SQL editor...
					Process p = new Process();
					p.StartInfo.FileName = Properties.Settings.Default.plsql_editor;
					p.StartInfo.Arguments = "\"" + tempFilename + "\"";
					p.Start();
				}
			} else {
				// Launch the test's xml file in the user's XML editor...
				Process p = new Process();
				p.StartInfo.FileName = Properties.Settings.Default.repos_xmlEditor;
				p.StartInfo.Arguments = "\"" + tnTest.test.xmlFilename + "\"";
				p.Start();
			}
		}

		private void createNewDBConnection() {
			string newDBName = String.Empty;

			InputBox.Prompt(title: "New Database Connection", promptText: "Please enter the name of the database to connect to:", value: ref newDBName);

			if (newDBName.Trim() != String.Empty) {
				Program.currProject.repository.addDatabaseConnection(databaseName: newDBName);

				MessageBox.Show(text: "Database added - reloading project...", caption: "DB Added", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);

				loadProject();
			}
		}

		#region SVN Commands
		// Shows the SVN log of the currently selected test/scn group/udc...
		private void showSVNLogHistory() {
			genericTreeNode selectedNode = (genericTreeNode)Program.navigationForm.projectTreeView.selectedNode;

			// Make sure a node was selected in the schema treeview.
			if (selectedNode == null) {
				MessageBox.Show("You must select a test object first.");
				return;
			}

			// Make sure a valid node was selected.
			if (selectedNode.nodeType != schemaNodeType.test
				&& selectedNode.nodeType != schemaNodeType.scenarioGroup
				&& selectedNode.nodeType != schemaNodeType.udc)
			{
				MessageBox.Show("You can only view SVN log history on a test, scenario group, or UDC.");
				return;
			}

			// Launch the tortoiseSVN Log viewer...
			Process p = new Process();
			p.StartInfo.FileName = "tortoiseproc.exe";
			p.StartInfo.Arguments = "/command:log /path:\"" + selectedNode.test.xmlFilename + "\"";
			p.Start();
		}

		private void showSVNCommit() {
			// Launch the tortoiseSVN Commit viewer...
			Process p = new Process();
			p.StartInfo.FileName = "tortoiseproc.exe";
			p.StartInfo.Arguments = "/command:commit /path:\"" + Program.currProject.repository.workingCopyPath + "\"";
			p.Start();
		}

		private void showSVNUpdate() {
			// Launch the tortoiseSVN Update viewer...
			Process p = new Process();
			p.StartInfo.FileName = "tortoiseproc.exe";
			p.StartInfo.Arguments = "/command:update /path:\"" + Program.currProject.repository.workingCopyPath + "\"";
			p.Start();
		}

		private void showSVNRevert() {			
			// Launch the tortoiseSVN Revert viewer...
			Process p = new Process();
			p.StartInfo.FileName = "tortoiseproc.exe";
			p.StartInfo.Arguments = "/command:revert /path:\"" + Program.currProject.repository.workingCopyPath + "\"";
			p.Start();
		}
		#endregion
	}
}
