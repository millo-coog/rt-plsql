using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace RT.User_Controls {
	public partial class ctlProjectTreeView : UserControl {
		// Delegates
		public delegate void AfterSelectHandler(genericTreeNode selectedNode);
		public delegate void BeforeSelectHandler(TreeViewCancelEventArgs e);
		
		// Events
		public event AfterSelectHandler afterSelect;
		public event BeforeSelectHandler beforeSelect;

		#region Get/Set methods
		public genericTreeNode selectedNode {
			get { return (genericTreeNode)tvProject.SelectedNode; }
			set { tvProject.SelectedNode = value; }
		}
		
		public ContextMenuStrip popupMenu {
			set { tvProject.ContextMenuStrip = value; }
		}
		#endregion

		#region Constructors
		public ctlProjectTreeView() {
			InitializeComponent();
		}
		#endregion
		
		#region Methods
				
		// Finds the node with the given key that's of the given node type
		public genericTreeNode findNodeByGUID(RT.schemaNodeType nodeType, string guid, bool loadChildren = false) {
			TreeNode[] arrFoundNodes;
			string calculatedGUID = guid;

			if (nodeType == schemaNodeType.scenarios)
				calculatedGUID = scenariosTreeNode.GUID_PREFIX + calculatedGUID;

			if (loadChildren)
				for (int i = 0; i < tvProject.Nodes.Count; i++)
					((databaseTreeNode)tvProject.Nodes[i]).loadAllChildren();

			arrFoundNodes = tvProject.Nodes.Find(calculatedGUID, true);

			for (int i = 0; i < arrFoundNodes.Count(); i++) {
				if (nodeType == ((RT.genericTreeNode)arrFoundNodes[i]).nodeType)
					return (genericTreeNode)arrFoundNodes[i];
			}

			return null;
		}

		/* Loads the testable target units from the target database, and
			load the list of existing tests from the repository database. */
		public void Populate(project currProject) {
			tvProject.Nodes.Clear();

			for (int i = 0; i < currProject.targetDBs.Count; i++) {
				tvProject.Nodes.Add(new databaseTreeNode(associatedTargetDB: currProject.targetDBs[i]));
			}

			// If there's only one database in the project, then
			// expand its node.
			if (tvProject.Nodes.Count == 1) {
				tvProject.Nodes[0].Expand();
			}
		}

		#endregion

		#region Events
		private void ctlProjectTreeView_Load(object sender, EventArgs e) {			
			txtObjectFilter.Select();
		}

		private void tvProject_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
			tvProject.SelectedNode = e.Node;
		}
		
		private void tvProject_AfterSelect(object sender, TreeViewEventArgs e) {			
			if (afterSelect != null)
				afterSelect(selectedNode: (genericTreeNode) e.Node);
		}

		private void tvProject_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
			if (beforeSelect != null)
				beforeSelect(e);
		}

		private void tvProject_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
			((genericTreeNode) e.Node).loadChildren();
		}
		#endregion

		#region Search Code		
		private void txtObjectFilter_KeyDown(object sender, KeyEventArgs e) {
			// Select all text, if told to...
			if (e.Control && e.KeyCode == Keys.A) {
				txtObjectFilter.SelectAll();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void txtObjectFilter_KeyPress(object sender, KeyPressEventArgs e) {
			// Search if the user presses Enter
			if (e.KeyChar == 13) {
				search();
				e.Handled = true;
			}
		}

		private void btnSearch_Click(object sender, EventArgs e) {
			search();
		}

		public void search() {
			if (txtObjectFilter.Text.Trim() == String.Empty) {
				txtObjectFilter.Select();
			} else {
				bool found = false;
				bool reachedStartingNode;

				Cursor.Current = Cursors.WaitCursor;

				SuspendLayout();
				tvProject.BeginUpdate();

				if (tvProject.SelectedNode == null || tvProject.SelectedNode == tvProject.Nodes[0]) {
					reachedStartingNode = true;
				} else {
					reachedStartingNode = false;
				}

				findNodeObject((genericTreeNode)tvProject.Nodes[0], txtObjectFilter.Text, ref found, ref reachedStartingNode);

				tvProject.EndUpdate();
				ResumeLayout();

				Cursor.Current = Cursors.Default;

				if (found == false) {
					MessageBox.Show("Not found!", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		// Recursive method to find a node that matches the given db object name
		private void findNodeObject(genericTreeNode tnStartingNode, string text, ref bool found, ref bool reachedStartingNode) {
			tnStartingNode.loadChildren();

			for (int i = 0; i < tnStartingNode.Nodes.Count; i++) {
				if (found) return; // Stop recursion, if we're done.

				genericTreeNode tnCurrentNode = (genericTreeNode)tnStartingNode.Nodes[i];

				tnCurrentNode.loadChildren();

				switch (tnCurrentNode.nodeType) {
					case schemaNodeType.functionsFolder:
					case schemaNodeType.packagesFolder:
					case schemaNodeType.proceduresFolder:
					case schemaNodeType.triggersFolder:
					case schemaNodeType.typesFolder:
					case schemaNodeType.viewsFolder:
						findNodeObject(tnCurrentNode, text, ref found, ref reachedStartingNode); // Recurse
						break;

					case schemaNodeType.method:
					case schemaNodeType.package: // Packages need to both show up in the search results and be recursed into
					case schemaNodeType.schema: // Schemas need to both show up in the search results and be recursed into
					case schemaNodeType.schemaLevelFunction:
					case schemaNodeType.schemaLevelProcedure:
					case schemaNodeType.trigger:
					case schemaNodeType.type:
					case schemaNodeType.view:
						if (reachedStartingNode) {
							if (tnCurrentNode.nodeType == schemaNodeType.method) {
								// Methods have to be searched specially...

								if (text.IndexOf('.') >= 0) {
									// Only consider the schema and name when searching methods, if the user specified a dot in his search text...
									if (Regex.IsMatch(tnCurrentNode.schema + "." + tnCurrentNode.name + "." + tnCurrentNode.method, ".*" + text.Replace(".", "\\.") + ".*", RegexOptions.IgnoreCase)) {
										found = true;
									}
								} else {
									// If we're possibly searching only by method name, don't include the schema and name in the search - 
									// just search the method names
									if (Regex.IsMatch(tnCurrentNode.method, ".*" + text + ".*", RegexOptions.IgnoreCase)) {
										found = true;
									}
								}
							} else {
								if (Regex.IsMatch(tnCurrentNode.schema + "." + tnCurrentNode.name + "." + tnCurrentNode.method, ".*" + text.Replace(".", "\\.") + ".*", RegexOptions.IgnoreCase)) {
									found = true;
								}
							}

							if (found) {
								// Try to make sure the schema node I'm in is visible.
								genericTreeNode tnParent = tnCurrentNode;
								while (tnParent.nodeType != schemaNodeType.schema) {
									tnParent = (genericTreeNode)tnParent.Parent;
								}

								tvProject.SelectedNode = tnCurrentNode;
								tvProject.TopNode = tnParent; // Need to do this after selecting the node, or the top node setting won't always work.								
								tnCurrentNode.EnsureVisible();
								tvProject.Focus();

								return;
							}
						}
					
						if (found == false && (tnCurrentNode.nodeType == schemaNodeType.package || tnCurrentNode.nodeType == schemaNodeType.schema)) {
							findNodeObject(tnCurrentNode, text, ref found, ref reachedStartingNode); // Recurse
						}

						break;
				}

				// Only start searching from the currently selected node...
				if (reachedStartingNode == false && tnCurrentNode == tvProject.SelectedNode)
					reachedStartingNode = true;
			}

			if (found == false && tnStartingNode == (genericTreeNode)tvProject.Nodes[0]) {
				if (MessageBox.Show("Not found. Do you wish to restart at the beginning?", "Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes) {
					reachedStartingNode = true;
					found = false;
					findNodeObject((genericTreeNode)tvProject.Nodes[0], text, ref found, ref reachedStartingNode); // Recurse
				}
			}
		}

		private void txtObjectFilter_Enter(object sender, EventArgs e) {
			txtObjectFilter.SelectAll();
		}
		#endregion

	}
}
