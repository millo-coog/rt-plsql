using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RT {
	public partial class frmArgumentUpdater : Form {
		private const string NEW_PARAMETER = "*New*";
		private const string REMOVED_PARAMETER = "*Removed*";
		private const string RENAMED_PARAMETER = "*Renamed*";

		private test prvCurrTest;
		private OracleConnection prvConTargetDB;
		private bool prvAutomaticMode;
		testArgumentCollection newNestedArgs;

		public frmArgumentUpdater(test currTest, OracleConnection conTargetDB, bool automaticMode = false) {
			InitializeComponent();

			prvCurrTest = currTest;
			prvConTargetDB = conTargetDB;
			prvAutomaticMode = automaticMode;

			this.Text = this.Text + " - " + (currTest.unitSchema + "." + currTest.unitName + "." + currTest.unitMethod).TrimEnd('.') + "(" + currTest.overload.ToString() + ")";

			if (automaticMode || prvCurrTest.hasArgumentMismatch(conTargetDB: prvConTargetDB)) {
				btnAnalyze.Enabled = true;

				// Populate the treeview of old arguments
				loadOldArgTreeview();

				// Populate the treeview of new arguments
				loadNewArgTreeView();
			} else {
				btnAnalyze.Enabled = false;

				if (prvAutomaticMode == false)
					MessageBox.Show(text: "No changes needed!", caption: "Info", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
			}
		}

		private void loadNewArgTreeView() {
			newNestedArgs =
				targetDB.getMethodArguments(
					conTargetDB: prvConTargetDB,
					schema: prvCurrTest.unitSchema,
					package: prvCurrTest.unitMethod != String.Empty ? prvCurrTest.unitName : String.Empty,
					method: prvCurrTest.unitMethod == String.Empty ? prvCurrTest.unitName : prvCurrTest.unitMethod,
					overload: prvCurrTest.overload,
					returnedAsNested: true);

			tvNewArguments.Nodes.Add(new targetArgumentsFolder());
			tvNewArguments.Nodes[0].Nodes.Clear();

			foreach (testArgument arg in newNestedArgs) {
				methodArgument node = new methodArgument(arg: arg);
				tvNewArguments.Nodes[0].Nodes.Add(node);
			}

			tvNewArguments.ExpandAll();
		}

		private void loadOldArgTreeview() {
			tvOldArguments.Nodes.Clear();

			tvOldArguments.Nodes.Add(new targetArgumentsFolder());
			tvOldArguments.Nodes[0].Nodes.Clear();
						
			foreach (testArgument arg in prvCurrTest.testArguments) {
				methodArgument node = new methodArgument(arg: arg);
				tvOldArguments.Nodes[0].Nodes.Add(node);
			}

			tvOldArguments.ExpandAll();
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void btnAnalyze_Click(object sender, EventArgs e) {
			analyze();
		}

		public void analyze() {
			string rpt = updateArguments(tnParentNode: tvOldArguments.Nodes[0], oldArgs: prvCurrTest.testArguments, newArgs: newNestedArgs);

			if (rpt != "") {
				Program.fieldTracker.needsSaving = true;

				if (prvAutomaticMode == false) {
					MessageBox.Show("The following changes were made:" + Environment.NewLine + rpt);

					MessageBox.Show("If all looks good, Save, and then reload the project.");
				}

				prvCurrTest.testArguments = newNestedArgs;

				this.Close();
			}
		}
		
		private string updateArguments(TreeNode tnParentNode, testArgumentCollection oldArgs, testArgumentCollection newArgs, testArgument parentArgument = null) {
			string rpt = "";
			testArgumentCollection possiblyRemovedArgs = new testArgumentCollection();
			testArgumentCollection possiblyAddedArgs = new testArgumentCollection();

			testArgument correspondingOldArg;
			testArgument correspondingNewArg;

			// Change any properties of any old arguments in this current level, noting any
			// that we can't find a corresponding new argument for, due to deletion or rename...
			foreach (methodArgument argNode in tnParentNode.Nodes) {
				correspondingOldArg = oldArgs[argNode.Name];
				correspondingNewArg = newArgs[correspondingOldArg.argumentName];
				
				if (correspondingNewArg == null) {
					// Can't find a new argument that correspondings with this old argument. It was either
					// deleted or renamed!
					possiblyRemovedArgs.Add(correspondingOldArg);
				} else {
					// We found a match based on name; now, check the other attributes to see if they need updating...
					if (correspondingOldArg.inOut != correspondingNewArg.inOut) {
						rpt += "Updating " + correspondingOldArg.argumentName + "'s inOut => " + correspondingNewArg.inOut + "." + Environment.NewLine;

						correspondingOldArg.inOut = correspondingNewArg.inOut;
					}

					if (correspondingOldArg.sequence != correspondingNewArg.sequence) {
						rpt += "Updating " + correspondingOldArg.argumentName + "'s sequence => " + correspondingNewArg.sequence + "." + Environment.NewLine;

						correspondingOldArg.sequence = correspondingNewArg.sequence;
					}

					if (correspondingOldArg.position != correspondingNewArg.position) {
						rpt += "Updating " + correspondingOldArg.argumentName + "'s position => " + correspondingNewArg.position + "." + Environment.NewLine;

						correspondingOldArg.position = correspondingNewArg.position;
					}

					if (correspondingOldArg.dataType != correspondingNewArg.dataType) {
						rpt += "Updating " + correspondingOldArg.argumentName + "'s dataType => " + correspondingNewArg.dataType + "." + Environment.NewLine;

						correspondingOldArg.dataType = correspondingNewArg.dataType;
					}

					if (correspondingOldArg.dataLevel != correspondingNewArg.dataLevel) {
						rpt += "Updating " + correspondingOldArg.argumentName + "'s dataLevel => " + correspondingNewArg.dataLevel.ToString() + "." + Environment.NewLine;

						correspondingOldArg.dataLevel = correspondingNewArg.dataLevel;
					}

					if (correspondingOldArg.plsType != correspondingNewArg.plsType) {
						rpt += "Updating " + correspondingOldArg.argumentName + "'s plsType => " + correspondingNewArg.plsType + "." + Environment.NewLine;

						correspondingOldArg.plsType = correspondingNewArg.plsType;
					}

					if (correspondingOldArg.canDefault != correspondingNewArg.canDefault) {
						rpt += "Updating " + correspondingOldArg.argumentName + "'s canDefault => " + correspondingNewArg.canDefault.ToString() + "." + Environment.NewLine;

						correspondingOldArg.canDefault = correspondingNewArg.canDefault;
					}
				}
			}

			// Now, find any new arguments (which may be renamed old arguments)
			foreach (testArgument arg in newArgs) {
				if (oldArgs[arg.argumentName] == null) {
					// Couldn't find an corresponding old arg for this new arg: possibly new or renamed argument....
					possiblyAddedArgs.Add(arg);
				}
			}

			// Identify any renames/deletes...
			for (int i = possiblyRemovedArgs.Count-1; i >= 0; i--) {
				// If this is the only argument at this level that doesn't have a match, then it was simply removed...
				if (possiblyAddedArgs.Count == 0) {
					// There aren't any "new" arguments at this level, so there was no rename - remove this parameter...
					
					// Cascade removal into the test...
					prvCurrTest.removeArgument(possiblyRemovedArgs[i]);

					// Remove the corresponding argument node from the treeview...
					foreach (TreeNode argumentNode in tnParentNode.Nodes) {
						if (argumentNode.Name == possiblyRemovedArgs[i].argumentName) {
							tnParentNode.Nodes.Remove(node: argumentNode);
							break;
						}
					}

					rpt += "Removed the " + possiblyRemovedArgs[i].argumentName + " parameter." + Environment.NewLine;

					possiblyRemovedArgs.Remove(possiblyRemovedArgs[i]);
				} else {
					// There are "new" args at this level, too, so the old arg could have been renamed to the new arg...
					string newArgName = "";

					foreach (testArgument newArg in possiblyAddedArgs) {
						newArgName += newArg.argumentName + ",";
					}
					newArgName = newArgName.TrimEnd(',');

					InputBox.Prompt(title: "Parameter Renamed?", promptText: "What is the new name of the old parameter " + possiblyRemovedArgs[i].argumentName + "? If it was entirely removed, please just clear the box.", value: ref newArgName);
					
					if (newArgName == "") {
						// Parameter was removed - toast it...
						prvCurrTest.removeArgument(possiblyRemovedArgs[i]);

						// Remove the corresponding argument node from the treeview...
						foreach (TreeNode argumentNode in tnParentNode.Nodes) {
							if (argumentNode.Name == possiblyRemovedArgs[i].argumentName) {
								tnParentNode.Nodes.Remove(node: argumentNode);
								break;
							}
						}

						rpt += "Removed the " + possiblyRemovedArgs[i].argumentName + " parameter." + Environment.NewLine;

						possiblyRemovedArgs.Remove(possiblyRemovedArgs[i]);
					} else {
						// Parameter was renamed to the new given name - find it and overlay the old arg with the new one...
						foreach (testArgument possibleNewArg in possiblyAddedArgs) {
							if (possibleNewArg.argumentName.ToUpper() == newArgName.ToUpper()) {
								rpt += "Renamed " + possiblyRemovedArgs[i].argumentName + " to " + possibleNewArg.argumentName + "." + Environment.NewLine;

								prvCurrTest.renameArgument(oldArg: possiblyRemovedArgs[i], newArg: possibleNewArg);
								
								// Remove the corresponding argument node in the treeview because there's
								// no reason to recurse into it later - its child parameters will match
								// the new argument's exactly, because it was overlaid.
								foreach (TreeNode argumentNode in tnParentNode.Nodes) {
									if (argumentNode.Name == possiblyRemovedArgs[i].argumentName) {
										tnParentNode.Nodes.Remove(node: argumentNode);
										break;
									}
								}

								possiblyRemovedArgs.Remove(possiblyRemovedArgs[i]);
								possiblyAddedArgs.Remove(possibleNewArg);

								break;
							}
						}
					}
				}
			}

			Debug.Assert(condition: possiblyRemovedArgs.Count == 0, message: "There are unsolved possibly removed args - this shouldn't happen!");

			foreach (testArgument newArg in possiblyAddedArgs) {
				// Add each new parameter to the test...
				prvCurrTest.addArgument(parentArgument: parentArgument, newArg: newArg);

				rpt += "Added the " + newArg.argumentName + " argument." + Environment.NewLine;
			}			

			// Now that we're done looking for any changes in this level, go into any nested nodes....
			foreach (methodArgument argNode in tnParentNode.Nodes) {
				correspondingOldArg = oldArgs[argNode.Name];
				correspondingNewArg = newArgs[correspondingOldArg.argumentName];

				// Now, check the nested arguments...
				if (correspondingOldArg.childArguments.Count > 0) {
					rpt += updateArguments(tnParentNode: argNode, parentArgument: correspondingOldArg, oldArgs: correspondingOldArg.childArguments, newArgs: correspondingNewArg.childArguments);
				}
			}

			return rpt;
		}
	}
}
