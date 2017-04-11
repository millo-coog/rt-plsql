/*
 * The class that inherits a treeview node and is the type of nodes used in the schema treeview.
 * This custom node type allows us to attach custom attributes to the nodes.
 */
using Oracle.DataAccess;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace RT {
	#region Enumerations
	// An enumeration of the possible node types.
	public enum schemaNodeType {
		database,
		schema,
		
		functionsFolder,
		packagesFolder,
		proceduresFolder,
		triggersFolder,
		typesFolder,
		viewsFolder,

		argumentsFolder,
		methodArgument,
		
		package,
		method, // A method within a package or type
		schemaLevelProcedure,
		schemaLevelFunction,
		trigger,
		type,
		view,

		test,
		scenarioGroup,
		libraryItems,
		scenarioGroupStartup,
		scenarioStartup,
		scenarios,
		scenario,
		postParamAssignment,
		preUDCs,
		udc,
		scenarioTeardown,
		scenarioGroupTeardown,

		dummy
	}
	#endregion

	public class genericTreeNode : TreeNode {
		#region Protected Variables
		protected bool proChildrenLoaded = false;
		#endregion

		#region Private Variables
		#endregion

		#region Public Variables
		public schemaNodeType nodeType;

		public string name = String.Empty;
		#endregion		

		#region Get/Set methods
		public test test {
			get {
				testTreeNode tnTest = (testTreeNode) getParentNode(schemaNodeType.test);

				if (tnTest == null)
					return null;
				else
					return tnTest.associatedTest;					
			}
		}

		public scenarioGroup scenarioGroup {
			get {
				scenarioGroupTreeNode tnSCNGroup = (scenarioGroupTreeNode) getParentNode(schemaNodeType.scenarioGroup);

				if (tnSCNGroup == null)
					return null;
				else {
					return tnSCNGroup.associatedSCNGroup;
				}
			}
		}

		public udc udc {
			get {
				udcTreeNode tnUDC = (udcTreeNode) getParentNode(schemaNodeType.udc);

				if (tnUDC == null)
					return null;
				else
					return tnUDC.associatedUDC;
			}
		}

		public targetDB targetDatabase {
			get {
				return ((databaseTreeNode)getParentNode(schemaNodeType.database)).targetDatabase;
			}
		}

		// Returns the parent schema name, if any...
		public string schema {
			get {
				if (this.nodeType == schemaNodeType.database)
					return "";

				if (this.Parent == null) {
					throw new Exception("This node must have a parent schema node, before you can determine its schema.");
				}

				return ((schemaTreeNode)getParentNode(schemaNodeType.schema)).schemaName;
			}
		}

		// Returns the parent method name, if any...
		public string method {
			get {
				methodTreeNode tnParentMethodName = (methodTreeNode) getParentNode(schemaNodeType.method);
				if (tnParentMethodName == null)
					return string.Empty;
				else
					return tnParentMethodName.methodName;
			}
		}

		// Returns the parent method's overload, if any...
		public int overload {
			get {
				methodTreeNode tnParentMethodName = (methodTreeNode) getParentNode(schemaNodeType.method);
				if (tnParentMethodName == null)
					return -1;
				else
					return tnParentMethodName.overload;
			}
		}
		#endregion

		#region Constructors
		public genericTreeNode() { }
		#endregion

		#region Methods
		public string getObjectType() {
			genericTreeNode currentNode = this; // Start inclusively
			string foundObjectType = "";

			// Make sure we have a parent
			if (currentNode.Parent == null)
				return "";

			// Go up the tree until we find a folder node, because we
			// can determine the object type from it...
			while (currentNode != null && !Enum.GetName(typeof(schemaNodeType), currentNode.nodeType).Contains("Folder")) {
				currentNode = (genericTreeNode) currentNode.Parent;
			}

			if (currentNode == null) {
				return "";
			} else {
				switch (currentNode.nodeType) {
					case schemaNodeType.functionsFolder:
						foundObjectType = "FUNCTION";
						break;

					case schemaNodeType.packagesFolder:
						foundObjectType = "PACKAGE";
						break;

					case schemaNodeType.proceduresFolder:
						foundObjectType = "PROCEDURE";
						break;

					case schemaNodeType.triggersFolder:
						foundObjectType = "TRIGGER";
						break;

					case schemaNodeType.typesFolder:
						foundObjectType = "TYPE";
						break;

					case schemaNodeType.viewsFolder:
						foundObjectType = "VIEW";
						break;
				}
			}

			return foundObjectType;
		}

		// Returns the earliest parent node of the given type
		public genericTreeNode getParentNode(schemaNodeType parentNodeType) {
			genericTreeNode desiredNode = this; // Start inclusively

			while (desiredNode != null && desiredNode.nodeType != parentNodeType) {
				desiredNode = (genericTreeNode) desiredNode.Parent;
			}

			return desiredNode;
		}

		public genericTreeNode getParentNode(schemaNodeType[] parentNodeTypes) {
			genericTreeNode desiredNode = this; // Start inclusively

			while (desiredNode != null) {
				bool foundRightType = false;

				for (int i = 0; i < parentNodeTypes.Length; i++) {
					if (desiredNode.nodeType == parentNodeTypes[i]) {
						foundRightType = true;
						break;
					}
				}

				if (foundRightType)
					break;
				
				desiredNode = (genericTreeNode) desiredNode.Parent;
			}

			return desiredNode;
		}

		public bool hasChild(schemaNodeType childNodeType) {
			bool childFound = false;

			for (int i = 0; i < this.Nodes.Count; i++) {
				if (((genericTreeNode)this.Nodes[i]).nodeType == childNodeType) {
					childFound = true;
					break;
				}
			}

			return childFound;
		}

		private void bwUpdateStatusDoWork(object sender, DoWorkEventArgs e) {
			targetStatus nodeStatus = targetStatus.noTests;

			switch (this.nodeType) {
				case schemaNodeType.database:
					nodeStatus = Program.currProject.repository.getTargetStatusCode(databaseName: targetDatabase.name);

					break;

				case schemaNodeType.schema:
					nodeStatus = Program.currProject.repository.getTargetStatusCode(databaseName: targetDatabase.name, schema: ((schemaTreeNode)this).schemaName);
					break;

				case schemaNodeType.test:
					nodeStatus = this.test.getStatus(runResults: Program.currProject.repository.runResults);
					break;

				case schemaNodeType.scenarioGroup:
					nodeStatus = this.scenarioGroup.getStatus(runResults: Program.currProject.repository.runResults);
					break;

				case schemaNodeType.udc:
					nodeStatus = targetStatus.testsOk; // UDC's don't really have a status...
					break;

				case schemaNodeType.method:
					methodTreeNode tnMethod = (methodTreeNode)this;

					nodeStatus = Program.currProject.repository.getTargetStatusCode(databaseName: targetDatabase.name, objectType: tnMethod.getObjectType(), schema: tnMethod.schema, name: tnMethod.name, method: tnMethod.method, overload: tnMethod.overload);
					break;

				default:
					nodeStatus = Program.currProject.repository.getTargetStatusCode(databaseName: targetDatabase.name, objectType: this.getObjectType(), schema: this.schema, name: this.name, method: this.method, overload: this.overload);
					break;
			}

			e.Result = nodeStatus;
		}

		void bwUpdateStatusRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			BackgroundWorker bgWorker = (BackgroundWorker)sender;
			targetStatus nodeStatus = (targetStatus)e.Result;

			//this.TreeView.SuspendLayout();

			// Set the node's icon, based on the status...
			if (this.Nodes.Count == 0 &&
				(this.nodeType == schemaNodeType.functionsFolder || this.nodeType == schemaNodeType.packagesFolder || this.nodeType == schemaNodeType.proceduresFolder || this.nodeType == schemaNodeType.triggersFolder || this.nodeType == schemaNodeType.typesFolder || this.nodeType == schemaNodeType.viewsFolder)) {
				// This target container has no targets, so just show the target type, without a status.
				this.ImageKey = this.SelectedImageKey = System.Enum.GetName(typeof(schemaNodeType), this.nodeType);
			} else {
				if (this.nodeType == schemaNodeType.viewsFolder) {
					;
				}

				// Node has a status associated with it...
				if (this.nodeType == schemaNodeType.udc) {
					switch (((udcTreeNode)this).udc.checkType) {
						case RT.udc.enumCheckTypes.COMPARE_CURSOR_TO_MATRIX:
							this.ImageKey = this.SelectedImageKey = "matrixVsCursor";
							break;
						case RT.udc.enumCheckTypes.COMPARE_CURSORS:
							this.ImageKey = this.SelectedImageKey = "compareCursors";
							break;
						case RT.udc.enumCheckTypes.CURSOR_RETURNING_NO_ROWS:
							this.ImageKey = this.SelectedImageKey = "cursor";
							break;
						case RT.udc.enumCheckTypes.CURSOR_RETURNING_ROWS:
							this.ImageKey = this.SelectedImageKey = "cursor";
							break;
						case RT.udc.enumCheckTypes.PLSQL_BLOCK:
							this.ImageKey = this.SelectedImageKey = "plsql";
							break;
						case RT.udc.enumCheckTypes.ROW_VALIDATION:
							this.ImageKey = this.SelectedImageKey = "rowValidator";
							break;
					}
				} else {
					this.ImageKey = System.Enum.GetName(typeof(schemaNodeType), this.nodeType) + "-Status" + ((int)nodeStatus).ToString();
					this.SelectedImageKey = System.Enum.GetName(typeof(schemaNodeType), this.nodeType) + "-Status" + ((int)nodeStatus).ToString();
				}
			}

			// Set the node's text color, based on the status...
			if (nodeStatus == targetStatus.noTests || nodeStatus == targetStatus.testsOk) {
				this.ForeColor = Color.Black;
			} else {
				this.ForeColor = Color.Red;
			}

			//this.TreeView.ResumeLayout();

			// Remove references to our background worker, so his extensive memory (list of lots of tests) can get cleaned up....
			bgWorker.DoWork -= new DoWorkEventHandler(bwUpdateStatusDoWork);
			bgWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(bwUpdateStatusRunWorkerCompleted);
			bgWorker.Dispose();
		}

		// Updates the node's icon and color to indicate status.
		public void updateStatus() {
			if (this.nodeType == schemaNodeType.argumentsFolder
				|| this.nodeType == schemaNodeType.methodArgument
				|| this.nodeType == schemaNodeType.libraryItems
				|| this.nodeType == schemaNodeType.scenarioGroupStartup
				|| this.nodeType == schemaNodeType.scenarioStartup
				|| this.nodeType == schemaNodeType.scenarios
				|| this.nodeType == schemaNodeType.scenario
				|| this.nodeType == schemaNodeType.postParamAssignment
				|| this.nodeType == schemaNodeType.preUDCs
				|| this.nodeType == schemaNodeType.scenarioTeardown
				|| this.nodeType == schemaNodeType.scenarioGroupTeardown) {
				// Do nothing: these nodes do not have a status, so their icons are already set.
			} else {
				// Give the node its default icon, so it'll look good until 
				// the background worker can give it its correctly status icon.
				this.ImageKey = this.SelectedImageKey = System.Enum.GetName(typeof(schemaNodeType), this.nodeType);

				// Calculate the node's status, based on the node's type.
				BackgroundWorker bwUpdateStatus = new BackgroundWorker();

				bwUpdateStatus.DoWork += new DoWorkEventHandler(bwUpdateStatusDoWork);
				bwUpdateStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwUpdateStatusRunWorkerCompleted);

				bwUpdateStatus.RunWorkerAsync();
			}
		}

		public void loadChildren() {
			switch (nodeType) {
				case schemaNodeType.method:
				case schemaNodeType.methodArgument:
				case schemaNodeType.scenarioGroup:
				case schemaNodeType.libraryItems:
				case schemaNodeType.scenarios:
				case schemaNodeType.udc:
					// These elements loaded any children themselves, earlier...
					break;
				case schemaNodeType.database:
					((databaseTreeNode)this).loadChildren();
					break;

				case schemaNodeType.argumentsFolder:
					((targetArgumentsFolder)this).loadChildren();
					break;

				default:
					if (proChildrenLoaded)
						return;

					switch (nodeType) {
						case schemaNodeType.functionsFolder:
						case schemaNodeType.packagesFolder:
						case schemaNodeType.proceduresFolder:
						case schemaNodeType.triggersFolder:
						case schemaNodeType.typesFolder:
						case schemaNodeType.viewsFolder:
							// Create child nodes for the targets within the folder...
							schemaNodeType newChildNodeType;

							switch (this.nodeType) {
								case schemaNodeType.functionsFolder:
									newChildNodeType = schemaNodeType.schemaLevelFunction;
									break;
								case schemaNodeType.packagesFolder:
									newChildNodeType = schemaNodeType.package;
									break;
								case schemaNodeType.proceduresFolder:
									newChildNodeType = schemaNodeType.schemaLevelProcedure;
									break;
								case schemaNodeType.triggersFolder:
									newChildNodeType = schemaNodeType.trigger;
									break;
								case schemaNodeType.typesFolder:
									newChildNodeType = schemaNodeType.type;
									break;
								case schemaNodeType.viewsFolder:
									newChildNodeType = schemaNodeType.view;
									break;
								default:
									throw new Exception("Panic!");
							}

							// Clear out any dummy nodes that might be in the collection already
							this.Nodes.Clear();

							// Find the child nodes....
							List<targetDBObject> lstTargetChildObjects = this.targetDatabase.getObjects(schema: this.schema, objectType: this.getObjectType());
							foreach (targetDBObject currObject in lstTargetChildObjects) {
								genericTreeNode tncObject = new genericTreeNode();

								tncObject.nodeType = newChildNodeType;
								tncObject.name = currObject.name;
								tncObject.Text = tncObject.name;

								tncObject.Nodes.Add(new dummyTreeNode());
								
								this.Nodes.Add(tncObject);

								// Loading the children is needed for the error report's RT hyperlink to be able to find
								// SCN group's guids in the treeview.
								tncObject.loadChildren();

								tncObject.updateStatus();
							}

							break;
							
						case schemaNodeType.schema:			
							// Add a folder for each of the supported target types beneath the schema node...
							genericTreeNode functionTypeFolder = new genericTreeNode();
							functionTypeFolder.nodeType = schemaNodeType.functionsFolder;
							functionTypeFolder.Text = "Functions";
							functionTypeFolder.Nodes.Add(new dummyTreeNode());
							this.Nodes.Add(functionTypeFolder);
							functionTypeFolder.updateStatus();
			
							genericTreeNode packageTypeFolder = new genericTreeNode();
							packageTypeFolder.nodeType = schemaNodeType.packagesFolder;
							packageTypeFolder.Text = "Packages";
							packageTypeFolder.Nodes.Add(new dummyTreeNode());
							this.Nodes.Add(packageTypeFolder);
							packageTypeFolder.updateStatus();

							genericTreeNode procedureTypeFolder = new genericTreeNode();
							procedureTypeFolder.nodeType = schemaNodeType.proceduresFolder;
							procedureTypeFolder.Text = "Procedures";
							procedureTypeFolder.Nodes.Add(new dummyTreeNode());
							this.Nodes.Add(procedureTypeFolder);
							procedureTypeFolder.updateStatus();

							genericTreeNode triggerTypeFolder = new genericTreeNode();
							triggerTypeFolder.nodeType = schemaNodeType.triggersFolder;
							triggerTypeFolder.Text = "Triggers";
							triggerTypeFolder.Nodes.Add(new dummyTreeNode());
							this.Nodes.Add(triggerTypeFolder);
							triggerTypeFolder.updateStatus();

							genericTreeNode typeTypeFolder = new genericTreeNode();
							typeTypeFolder.nodeType = schemaNodeType.typesFolder;
							typeTypeFolder.Text = "Types";
							typeTypeFolder.Nodes.Add(new dummyTreeNode());
							this.Nodes.Add(typeTypeFolder);
							typeTypeFolder.updateStatus();

							genericTreeNode viewTypeFolder = new genericTreeNode();
							viewTypeFolder.nodeType = schemaNodeType.viewsFolder;
							viewTypeFolder.Text = "Views";
							viewTypeFolder.Nodes.Add(new dummyTreeNode());
							this.Nodes.Add(viewTypeFolder);
							viewTypeFolder.updateStatus();

							break;
						case schemaNodeType.schemaLevelFunction:
						case schemaNodeType.schemaLevelProcedure:
						case schemaNodeType.trigger:
						case schemaNodeType.view:
							this.Nodes.Clear();

							// Create an arguments subfolder, if necessary...
							if (nodeType == schemaNodeType.schemaLevelFunction || nodeType == schemaNodeType.schemaLevelProcedure) {
								this.Nodes.Add(new targetArgumentsFolder());
							}

							// Create test nodes for any associated tests...
							List<test> associatedTests =
								Program.currProject.repository.getTestList(
									databaseName: this.targetDatabase.name,
									schema: this.schema,
									name: this.name,
									objectType: this.getObjectType());

							foreach (test associatedTest in associatedTests) {
								this.Nodes.Add(new testTreeNode(associatedTest: associatedTest));
							}

							break;

						case schemaNodeType.package: // Fall through
						case schemaNodeType.type:
							this.Nodes.Clear();

							// Add child methods of packages and types as child nodes...
							foreach (targetDBMethod method in this.targetDatabase.allMethods(schema: this.schema, objectType: this.getObjectType(), name: name)) {
								methodTreeNode tnMethod = new methodTreeNode(methodName: method.method, methodOverload: method.overload);

								tnMethod.name = method.name;

								this.Nodes.Add(tnMethod);

								tnMethod.loadChildren();
							}

							break;
					}

					proChildrenLoaded = true;

					break;
			}

			updateStatus();
		}
		#endregion
	}

	public class databaseTreeNode : genericTreeNode {
		public targetDB targetDatabase;

		// Constructor
		public databaseTreeNode(targetDB associatedTargetDB) {
			this.Text = associatedTargetDB.name;
			
			targetDatabase = associatedTargetDB;
			
			nodeType = schemaNodeType.database;
			
			//loadChildren();
			this.Nodes.Add(new dummyTreeNode());

			updateStatus();
		}

		// Loads all child elements in the entire tree.
		// Used when about to do a search across all nodes, so we
		// need them loaded.
		public void loadAllChildren() {
			for (int i = 0; i < Nodes.Count; i++) {
				if (((genericTreeNode) Nodes[i]).nodeType != schemaNodeType.dummy)
					((schemaTreeNode)Nodes[i]).loadAllChildren();
			}
		}

		public void loadChildren() {
			try {
				targetDatabase.open(
					username: Properties.Settings.Default.target_dbUsername,
					password: RT.security.DecryptString(Properties.Settings.Default.target_dbPassword)
				);
			} catch (Exception e) {
				MessageBox.Show(e.Message);
				return;
			}

			OracleCommand cmdGetUsers = new OracleCommand();
			String sql = @"
				SELECT all_users.username AS owner
				  FROM sys.all_users
				 WHERE EXISTS (SELECT /*+ FIRST_ROWS(1) */ 1
				                 FROM sys.all_objects
				                WHERE all_objects.owner = all_users.username
				                  AND all_objects.object_type IN ('FUNCTION', 'PACKAGE', 'PROCEDURE', 'TRIGGER', 'TYPE') -- Limit objects to rt-targetable units
				                  AND rownum = 1)";

			cmdGetUsers.Connection = this.targetDatabase.conTargetDB;

			if (Properties.Settings.Default.schemas_excludedSchemas != String.Empty) {
			   String[] arrSchemas = Properties.Settings.Default.schemas_excludedSchemas.Split(',');

			   sql += "AND all_users.username NOT IN (";

			   for (int i = 0; i < arrSchemas.Count(); i++) {
			      sql += ":schema" + i.ToString() + ",";

			      cmdGetUsers.Parameters.Add("schema" + i.ToString(), arrSchemas[i].Trim().ToUpper());
			   }

			   sql = sql.TrimEnd(',') + ")";
			}

			sql += " ORDER BY all_users.username";

			cmdGetUsers.CommandText = sql;

			List<String> lstSchemas = new List<String>();
			OracleDataReader drUsers = cmdGetUsers.ExecuteReader();

			while (drUsers.Read()) {
				lstSchemas.Add(drUsers["owner"].ToString());
			}

			drUsers.Close();
			drUsers.Dispose();

			cmdGetUsers.Dispose();

			this.Nodes.Clear();

			// Add the schemas as child nodes...
			foreach (string currSchema in lstSchemas)	{
				schemaTreeNode schemaNode = new schemaTreeNode(schemaName: currSchema);

				this.Nodes.Add(schemaNode);

				schemaNode.loadChildren();
			}
				
			//updateStatus();
		}
	}

	public class schemaTreeNode : genericTreeNode {
		// Private variables
		private string prvSchema;

		// Get/Set methods
		public string schemaName {
			get { return prvSchema; }
			set { prvSchema = value; }
		}

		// Constructor
		public schemaTreeNode(string schemaName) {
			this.nodeType = schemaNodeType.schema;
			this.prvSchema = schemaName;
			this.Text = this.prvSchema;
		}

		public void loadAllChildren() {
			for (int i = 0; i < Nodes.Count; i++) {
				((genericTreeNode)Nodes[i]).loadChildren();
			}
		}
	}

	public class testTreeNode : genericTreeNode {
		// Private variables
		private test prvAssociatedTest;

		// Get/Set methods
		public test associatedTest {
			get { return prvAssociatedTest; }
		}

		// Constructor
		public testTreeNode(test associatedTest) {
			prvAssociatedTest = associatedTest;

			this.nodeType = schemaNodeType.test;
			this.Text = prvAssociatedTest.name;
			this.Name = prvAssociatedTest.guid;

			loadChildren();

			updateStatus();
		}

		private void loadChildren() {
			// Create child nodes for each scenario group within the test...
			for (int i = 0; i < prvAssociatedTest.scenarioGroups.Count; i++) {
				this.Nodes.Add(new scenarioGroupTreeNode(associatedScenarioGroup: prvAssociatedTest.scenarioGroups[i]));
			}
		}
	}

	public class scenarioGroupTreeNode : genericTreeNode {
		private scenarioGroup prvSCNGroup;

		public scenarioGroup associatedSCNGroup {
			get { return prvSCNGroup; }
		}

		public scenariosTreeNode scenariosTN {
			get {
				for (int i = 0; i < this.Nodes.Count; i++) {
					if (((genericTreeNode)this.Nodes[i]).nodeType == schemaNodeType.scenarios) {
						return (scenariosTreeNode)this.Nodes[i];
					}
				}

				return null;
			}
		}

		// Constructor
		public scenarioGroupTreeNode(scenarioGroup associatedScenarioGroup) {
			prvSCNGroup = associatedScenarioGroup;

			this.Text = associatedScenarioGroup.name;
			this.Name = associatedScenarioGroup.guid;
			this.nodeType = schemaNodeType.scenarioGroup;

			loadChildren();
		}

		// Adds the given child node at the appropriate sort order position
		public genericTreeNode addChildNode(schemaNodeType childNodeType, udc associatedUDC = null) {
			int newIndex = 0;

			if (childNodeType == schemaNodeType.udc || !this.hasChild(childNodeType: childNodeType)) {
				schemaNodeType[] arrNodeTypeOrder =
					new schemaNodeType[] {
						schemaNodeType.libraryItems,
						schemaNodeType.scenarioGroupStartup,
						schemaNodeType.scenarioStartup,
						schemaNodeType.scenarios,
						schemaNodeType.postParamAssignment,
						schemaNodeType.preUDCs,
						schemaNodeType.udc,
						schemaNodeType.scenarioTeardown,
						schemaNodeType.scenarioGroupTeardown };

				int indexInOrderArray = 0;

				// Find the order of the child node type in the ordered child types array
				for (int i = 0; i < arrNodeTypeOrder.Count(); i++) {
					if (childNodeType == arrNodeTypeOrder[i]) {
						indexInOrderArray = i;
						break;
					}
				}

				// Find the right spot to add the child node
				for (int i = this.Nodes.Count - 1; i >= 0; i--) {
					int indexOfCurrentNodeInOrderArray = 0;

					// Find the order of this particular node's type in the ordered child types array
					for (int j = 0; j < arrNodeTypeOrder.Count(); j++) {
						if (((genericTreeNode) this.Nodes[i]).nodeType == arrNodeTypeOrder[j]) {
							indexOfCurrentNodeInOrderArray = j;
							break;
						}
					}

					if (indexOfCurrentNodeInOrderArray <= indexInOrderArray) {
						newIndex = i + 1;
						break;
					}
				}

				// Add the node
				switch (childNodeType) {
					case schemaNodeType.libraryItems:
						this.Nodes.Insert(newIndex, new libraryItemsTreeNode(associatedScenarioGroup: associatedSCNGroup));
						break;
					case schemaNodeType.scenarioGroupStartup:
						this.Nodes.Insert(newIndex, new scenarioGroupStartupTreeNode(associatedScenarioGroup: associatedSCNGroup));
						break;
					case schemaNodeType.scenarioStartup:
						this.Nodes.Insert(newIndex, new scenarioStartupTreeNode(associatedScenarioGroup: associatedSCNGroup));
						break;
					case schemaNodeType.scenarios:
						
						scenariosTreeNode scenariosNode = new scenariosTreeNode(associatedSCNGroup);
						this.Nodes.Add(scenariosNode);
						scenariosNode.refresh();
						break;
					case schemaNodeType.postParamAssignment:
						this.Nodes.Insert(newIndex, new postParamAssignmentTreeNode(associatedScenarioGroup: associatedSCNGroup));
						break;
					case schemaNodeType.preUDCs:
						this.Nodes.Insert(newIndex, new preUDCsTreeNode(associatedScenarioGroup: associatedSCNGroup));
						break;
					case schemaNodeType.udc:
						this.Nodes.Insert(newIndex, new udcTreeNode(associatedUDC: associatedUDC));
						break;
					case schemaNodeType.scenarioTeardown:
						this.Nodes.Insert(newIndex, new scenarioTeardownTreeNode(associatedScenarioGroup: associatedSCNGroup));
						break;
					case schemaNodeType.scenarioGroupTeardown:
						this.Nodes.Insert(newIndex, new scenarioGroupTeardownTreeNode(associatedScenarioGroup: associatedSCNGroup));
						break;
					default:
						throw new Exception("Unsupported child type node!");
				}				
			}

			return (genericTreeNode) this.Nodes[newIndex];
		}

		public void loadChildren() {
			addChildNode(schemaNodeType.libraryItems);

			if (associatedSCNGroup.scenarioGroupStartup != "")
				addChildNode(schemaNodeType.scenarioGroupStartup);

			if (associatedSCNGroup.scenarioStartup != "")
				addChildNode(schemaNodeType.scenarioStartup);
			
			// Add a child node to hold the child scenarios...
			addChildNode(schemaNodeType.scenarios);

			if (associatedSCNGroup.postParamAssignment != "")
				addChildNode(schemaNodeType.postParamAssignment);

			if (associatedSCNGroup.preUDC != "")
				addChildNode(schemaNodeType.preUDCs);

			// Create child nodes for every UDC within the scenario group...
			for (int i = 0; i < prvSCNGroup.udcCollection.Count; i++) {
				addChildNode(schemaNodeType.udc, prvSCNGroup.udcCollection[i]);
			}

			if (associatedSCNGroup.scenarioTeardown != "")
				addChildNode(schemaNodeType.scenarioTeardown);

			if (associatedSCNGroup.scenarioGroupTeardown != "")
				addChildNode(schemaNodeType.scenarioGroupTeardown);

			base.updateStatus();
		}
	}

	public class libraryItemsTreeNode : genericTreeNode {
		public libraryItemsTreeNode(scenarioGroup associatedScenarioGroup) {
			this.Text = "Library Items";
			this.nodeType = schemaNodeType.libraryItems;
			this.ImageKey = this.SelectedImageKey = "libraryItem";
		}
	}

	public class scenarioGroupStartupTreeNode : genericTreeNode {
		public scenarioGroupStartupTreeNode(scenarioGroup associatedScenarioGroup) {
			this.Text = "SCN Group Startup";
			this.nodeType = schemaNodeType.scenarioGroupStartup;
			this.ImageKey = this.SelectedImageKey = "ScenarioGroupStartup";
		}
	}

	public class scenarioStartupTreeNode : genericTreeNode {
		public scenarioStartupTreeNode(scenarioGroup associatedScenarioGroup) {
			this.Text = "Scenario Startup";
			this.nodeType = schemaNodeType.scenarioStartup;
			this.ImageKey = this.SelectedImageKey = "ScenarioStartup";
		}
	}

	public class scenariosTreeNode : genericTreeNode {
		public const string GUID_PREFIX = "Scenarios_";

		// Constructor
		public scenariosTreeNode(scenarioGroup associatedScenarioGroup) {
			this.Text = "Scenarios";
			this.nodeType = schemaNodeType.scenarios;
			this.ImageKey = this.SelectedImageKey = "scenarios";

			this.Name = GUID_PREFIX + associatedScenarioGroup.guid;
		}

		public void refresh() {
			Nodes.Clear();

			for (int i = 0; i < this.scenarioGroup.scenarios.Count; i++) {
				scenarioTreeNode scnNode = new scenarioTreeNode(associatedScenario: this.scenarioGroup.scenarios[i]);
				this.Nodes.Add(scnNode);
			}
		}
	}

	public class scenarioTreeNode : genericTreeNode {
		// Constructor
		public scenarioTreeNode(scenario associatedScenario) {
			if (associatedScenario.comments == "")
				this.Text = "<no comment>";
			else
				this.Text = associatedScenario.comments;

			this.nodeType = schemaNodeType.scenario;
			this.ImageKey = this.SelectedImageKey = "scenario";
		}
	}

	public class postParamAssignmentTreeNode : genericTreeNode {
		public postParamAssignmentTreeNode(scenarioGroup associatedScenarioGroup) {
			this.Text = "Post-Param Asgn.";
			this.nodeType = schemaNodeType.postParamAssignment;
			this.ImageKey = this.SelectedImageKey = "PostParamAssignment";
		}
	}

	public class preUDCsTreeNode : genericTreeNode {
		public preUDCsTreeNode(scenarioGroup associatedScenarioGroup) {
			this.Text = "Pre-UDC's";
			this.nodeType = schemaNodeType.preUDCs;
			this.ImageKey = this.SelectedImageKey = "preUDCs";
		}
	}

	public class udcTreeNode : genericTreeNode {
		private udc prvUDC;

		public udc associatedUDC {
			get { return prvUDC; }
		}

		// Constructor
		public udcTreeNode(udc associatedUDC) {
			prvUDC = associatedUDC;
			
			this.nodeType = schemaNodeType.udc;
			this.Name = this.prvUDC.guid;

			refresh();
		}

		// Refreshes the tree node's interface presentation
		// based on the UDC it is associated with (used when the
		// UDC is modified (renamed, etc.) and saved in the interface, and the tree
		// node needs to be refreshed).
		public void refresh() {
			this.Text = prvUDC.name;

			this.ToolTipText = Enum.GetName(typeof(udc.enumCheckTypes), prvUDC.checkType);

			base.updateStatus(); // The UDC type may have changed, so update the treeview icon.
		}
	}

	public class scenarioTeardownTreeNode : genericTreeNode {
		public scenarioTeardownTreeNode(scenarioGroup associatedScenarioGroup) {
			this.Text = "Scenario Teardown";
			this.nodeType = schemaNodeType.scenarioTeardown;
			this.ImageKey = this.SelectedImageKey = "scenarioTeardown";
		}
	}

	public class scenarioGroupTeardownTreeNode : genericTreeNode {
		public scenarioGroupTeardownTreeNode(scenarioGroup associatedScenarioGroup) {
			this.Text = "SCN Group Teardown";
			this.nodeType = schemaNodeType.scenarioGroupTeardown;
			this.ImageKey = this.SelectedImageKey = "scenarioGroupTeardown";
		}
	}

	public class methodTreeNode : genericTreeNode {
		// Private variables
		private string prvMethodName = "";
		private int prvOverload = 0;

		// Get/set methods
		public string methodName {
			get { return prvMethodName; }
		}

		public int overload {
			get { return prvOverload; }
			set { prvOverload = value; }
		}

		// Constructor
		public methodTreeNode(string methodName, int methodOverload) {
			this.prvMethodName = methodName;

			this.Text = methodName.ToLower() + (prvOverload == 0 ? "" : " (" + this.prvOverload.ToString() + ")");
			
			this.nodeType = schemaNodeType.method;
			
			this.prvOverload = methodOverload;
		}

		public void loadChildren() {
			// Create an arguments subfolder...
			this.Nodes.Add(new targetArgumentsFolder());
			
			// Create child nodes for any tests this method has....
			List<test> methodsAssociatedTests =
				Program.currProject.repository.getTestList(
					databaseName: this.targetDatabase.name,
					schema: this.schema,
					objectType: this.getObjectType(),
					name: this.name,
					method: this.prvMethodName,
					overload: this.prvOverload
				);

			foreach (test associatedTest in methodsAssociatedTests) {
				this.Nodes.Add(new testTreeNode(associatedTest: associatedTest));
			}

			updateStatus();
		}
	}

	public class targetArgumentsFolder : genericTreeNode {
		// Constructor
		public targetArgumentsFolder() {
			this.Text = "Arguments";
			this.nodeType = schemaNodeType.argumentsFolder;
			this.ImageKey = this.SelectedImageKey = "folder";

			// Create a dummy child node, to get the "expand" plus sign
			this.Nodes.Add(new dummyTreeNode());
		}

		public void loadChildren() {
			if (proChildrenLoaded)
				return;

			this.Nodes.Clear();

			genericTreeNode tnParentMethod = (genericTreeNode) this.getParentNode(parentNodeTypes: new schemaNodeType[] { schemaNodeType.method, schemaNodeType.schemaLevelFunction, schemaNodeType.schemaLevelProcedure });
			testArgumentCollection methodArgs;

			switch (tnParentMethod.nodeType) {
				case schemaNodeType.method:
					// Arguments for a method within a package or type...
					methodArgs =
						targetDB.getMethodArguments(
							conTargetDB: targetDatabase.conTargetDB,
							schema: tnParentMethod.schema,
							package: tnParentMethod.name,
							method: ((methodTreeNode) tnParentMethod).methodName,
							overload: ((methodTreeNode) tnParentMethod).overload
						);
					break;
				
				case schemaNodeType.schemaLevelFunction:
				case schemaNodeType.schemaLevelProcedure:
					// Arguments for a function/procedure...
					methodArgs =
						targetDB.getMethodArguments(
							conTargetDB: targetDatabase.conTargetDB,
							schema: tnParentMethod.schema,
							package: "",
							method: tnParentMethod.name,
							overload: 0
						);
					break;

				default:
					throw new Exception("Acceptable parent not found!");
			}

			foreach (testArgument testArg in methodArgs) {
				this.Nodes.Add(new methodArgument(arg: testArg));
			}

			proChildrenLoaded = true;
		}
	}

	public class methodArgument : genericTreeNode {
		// Constructor
		public methodArgument(testArgument arg) {
			this.Text = arg.argumentName + " " + arg.plsType + (arg.canDefault ? " (Dfltd)" : "");
			this.nodeType = schemaNodeType.methodArgument;
			this.ImageKey = this.SelectedImageKey = "folder";
			this.Name = arg.argumentName;

			switch (arg.inOut) {
				case "IN":
					this.ImageKey = this.SelectedImageKey = "inTestArgument";
					break;
				case "OUT":
					this.ImageKey = this.SelectedImageKey = "outTestArgument";
					break;
				case "IN/OUT":
					this.ImageKey = this.SelectedImageKey = "inOutTestArgument";
					break;
				case "RETURN":
					this.ImageKey = this.SelectedImageKey = "returnTestArgument";
					break;
			}

			if (arg.childArguments.Count > 0) {
				foreach (testArgument childArg in arg.childArguments) {
					this.Nodes.Add(new methodArgument(arg: childArg));
				}
			}
		}
	}

		public class dummyTreeNode : genericTreeNode {
		public dummyTreeNode() {
			this.nodeType = schemaNodeType.dummy;
			this.Text = "Loading...";
		}
	}
}
