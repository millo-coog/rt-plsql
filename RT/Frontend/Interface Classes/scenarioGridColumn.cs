/*
 * This class holds custom datagridview column classes.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace RT {
	public static class columnsErrata {
		public static DataGridViewCellStyle paramTypeINColumnCellStyle = new DataGridViewCellStyle();
		public static DataGridViewCellStyle paramTypeOUTColumnCellStyle = new DataGridViewCellStyle();
	}
	
	// Key column
	public class keyScenarioColumn : DataGridViewTextBoxColumn {
		// Constructor
		public keyScenarioColumn() {
			this.Name = "Key";
			this.HeaderText = "Key";
			this.DataPropertyName = "Key";

			this.ReadOnly = true;
			
			this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			this.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
			
			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
		}
	}

	// Parameter Value column
	public class parameterValueScenarioColumn : DataGridViewTextBoxColumn {
		private static DataGridViewCellStyle parameterColumnCellStyle = new DataGridViewCellStyle();
		private static DataGridViewCellStyle paramINColumnCellStyle = new DataGridViewCellStyle();
		
		private testArgument prvAssociatedTestArg;

		#region Get/Set Methods
		public testArgument testArg {
			get { return prvAssociatedTestArg; }
		}

		public parameterValueTypeScenarioColumn correspondingValueTypeColumn { get; set; }

		public bool isOutColumnPortion { get; set; } // Indicates if this is the OUT column for an IN/OUT parameter.
		#endregion

		// Constructor
		public parameterValueScenarioColumn(testArgument testArg, bool isExpectedValueForInOutColumn = false) {
			parameterColumnCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			parameterColumnCellStyle.WrapMode = DataGridViewTriState.True;
			parameterColumnCellStyle.NullValue = String.Empty;

			paramINColumnCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			paramINColumnCellStyle.BackColor = Program.PARAMETER_COLOR;

			Debug.Assert(testArg != null);

			prvAssociatedTestArg = testArg;
			isOutColumnPortion = isExpectedValueForInOutColumn;

			if (isOutColumnPortion) {
				this.Name = "exp_out" + (testArg.position-1).ToString();
				this.HeaderText = prvAssociatedTestArg.argumentName + " (out)";
				this.DataPropertyName = "exp_out" + (testArg.position - 1).ToString();
			} else {
				this.Name = prvAssociatedTestArg.argumentName;

				if (testArg.inOut == "IN/OUT") {
					this.HeaderText = prvAssociatedTestArg.argumentName + " (in)";
				} else {
					this.HeaderText = prvAssociatedTestArg.argumentName;
				}

				this.DataPropertyName = prvAssociatedTestArg.argumentName;
			}

			this.DefaultCellStyle = parameterColumnCellStyle;
			
			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
			
			this.SortMode = DataGridViewColumnSortMode.NotSortable;
			
			if (prvAssociatedTestArg.inOut == "IN" ||
				(prvAssociatedTestArg.inOut == "IN/OUT" && isExpectedValueForInOutColumn == false)) {
				this.DefaultCellStyle = paramINColumnCellStyle;
			} else {
				this.DefaultCellStyle = columnsErrata.paramTypeOUTColumnCellStyle;
			}
		}
	}

	// Parameter Value Type column
	public class parameterValueTypeScenarioColumn : DataGridViewComboBoxColumn {
		private testArgument prvAssociatedTestArg;
		
		public testArgument testArg {
			get { return prvAssociatedTestArg; }
		}

		public parameterValueScenarioColumn correspondingValueColumn { get; set; }

		#region Constructors
		public parameterValueTypeScenarioColumn(
			testArgument testArg,
			Int32 paramIndex,
			bool isExpectedValueForInOutColumn = false
		) {
			columnsErrata.paramTypeINColumnCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			columnsErrata.paramTypeINColumnCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 255, 224);
			
			columnsErrata.paramTypeOUTColumnCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;	
			columnsErrata.paramTypeOUTColumnCellStyle.BackColor = Program.RETURN_VALUE_COLOR;
			
			prvAssociatedTestArg = testArg;

			if (isExpectedValueForInOutColumn) {
				this.DataPropertyName = "exp_out_pt" + paramIndex.ToString();
				this.HeaderText = this.DataPropertyName;
				this.Name = "exp_out_pt" + paramIndex.ToString();
				this.ToolTipText = prvAssociatedTestArg.argumentName;
			} else {
				this.DataPropertyName = "pt" + paramIndex.ToString();
				this.HeaderText = this.DataPropertyName;
				this.Name = "pt" + paramIndex.ToString();
				this.ToolTipText = prvAssociatedTestArg.argumentName;
			}

			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

			this.SortMode = DataGridViewColumnSortMode.NotSortable;

			if (prvAssociatedTestArg.inOut == "IN" || 
				(prvAssociatedTestArg.inOut == "IN/OUT" && isExpectedValueForInOutColumn == false)) {
				// This guy is an "in" parameter...
				if (prvAssociatedTestArg.dataType != "REF CURSOR")
					this.Items.Add("value");

				this.Items.Add("exp");

				if (prvAssociatedTestArg.canDefault)
					this.Items.Add("defaulted");

				if (plsTypeCouldBeARecord(plsType: prvAssociatedTestArg.plsType)) {
					this.Items.Add("matrix");

					if (testArg.childArguments.Count > 0)
						this.Items.Add("nested");

					this.Items.Add("select");
				}

				this.DefaultCellStyle = columnsErrata.paramTypeINColumnCellStyle;
			} else if (prvAssociatedTestArg.inOut == "OUT" || isExpectedValueForInOutColumn || prvAssociatedTestArg.inOut == "RETURN") {
				// This guy is some type of "out"-style parameter...
				if (prvAssociatedTestArg.dataType != "REF CURSOR")
					this.Items.Add("value");

				this.Items.Add("don't test");
				this.Items.Add("exp");
				this.Items.Add("is null");
				this.Items.Add("not null");

				// If the data type is not *known* to not contain records, then give the options for checking it for records and let the user decide.
				if (plsTypeCouldBeARecord(plsType: prvAssociatedTestArg.plsType)) {
					this.Items.Add("matrix");

					if (testArg.childArguments.Count > 0)
						this.Items.Add("nested");

					this.Items.Add("no rows");
					this.Items.Add("some rows");
					this.Items.Add("select");
				}

				this.DefaultCellStyle = columnsErrata.paramTypeOUTColumnCellStyle;
			}
		}
		#endregion

		private bool plsTypeCouldBeARecord(String plsType) {
			bool maybe = false;

			if (plsType != "VARCHAR2"
					&& plsType != "VARCHAR"
					&& plsType != "NVARCHAR2"
					&& plsType != "NCHAR"
					&& plsType != "CHAR"
					&& plsType != "CLOB"
					&& plsType != "NCLOB"
					&& plsType != "LONG"
					&& plsType != "LONG RAW"
					&& plsType != "RAW"
					&& plsType != "BOOLEAN"
					&& plsType != "PLS_INTEGER"
					&& plsType != "TIMESTAMP"
					&& plsType != "NUMBER"
					&& plsType != "DATE") {
				maybe = true;
			}

			return maybe;
		}
	}

	// Expected Exception column
	public class expectedExceptionScenarioColumn : DataGridViewTextBoxColumn {
		// Constructor
		public expectedExceptionScenarioColumn() {
			this.Name = "Expected Exception";
			this.HeaderText = "Exp.\nExcptn";
			this.DataPropertyName = "Expected Exception";

			this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
			this.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);
			
			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
			
			this.SortMode = DataGridViewColumnSortMode.NotSortable;
		}
	}

	// Expected Exception Message column
	public class expectedExceptionMessageScenarioColumn : DataGridViewTextBoxColumn {
		public expectedExceptionMessageScenarioColumn() {
			this.Name = "Expected Exception Message";
			this.HeaderText = "Exp.\nExptn\nMsg";
			this.DataPropertyName = "Expected Exception Message";
			
			this.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);
			this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			
			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
			
			this.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.ToolTipText = "Expected Exception Message - The thrown exception message text must be LIKE the given expression.";
		}
	}

	// Comments column
	public class commentsScenarioColumn : DataGridViewTextBoxColumn {
		// Constructor
		public commentsScenarioColumn() {
			this.Name = "Comments";
			this.HeaderText = "Comments";
			this.DataPropertyName = "Comments";

			this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			this.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
			
			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
			
			this.SortMode = DataGridViewColumnSortMode.NotSortable;
		}
	}

	// Last Run Results column
	public class lastRunResultsScenarioColumn : DataGridViewTextBoxColumn {
		// Constructor
		public lastRunResultsScenarioColumn() {
			this.Name = "Last Run Results";
			this.HeaderText = "Last\nRun\nResults";
			this.DataPropertyName = "result";
			
			this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;

			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

			this.SortMode = DataGridViewColumnSortMode.NotSortable;
		}
	}

	// Last Run Error Number column
	public class lastRunErrorNumberScenarioColumn : DataGridViewTextBoxColumn {
		// Constructor
		public lastRunErrorNumberScenarioColumn() {
			this.Name = "Last Run Error #";
			this.HeaderText = "Last Run\nError ";
			this.DataPropertyName = "error_number";
			
			this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
			
			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
			
			this.SortMode = DataGridViewColumnSortMode.NotSortable;
		}
	}

	// Last Run Error Message column
	public class lastRunErrorMessageScenarioColumn : DataGridViewTextBoxColumn {
		// Constructor
		public lastRunErrorMessageScenarioColumn() {
			this.Name = "Last Run Error Message";
			this.HeaderText = "Last Run\nError Message";
			this.DataPropertyName = "error_message";

			this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			this.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
			
			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
			
			this.SortMode = DataGridViewColumnSortMode.NotSortable;
		}
	}

	// GUID column
	public class GUIDScenarioColumn : DataGridViewTextBoxColumn {
		// Constructor
		public GUIDScenarioColumn() {
			this.Name = "GUID";
			this.HeaderText = "GUID";
			this.DataPropertyName = "guid";
			
			this.ReadOnly = true;
			
			this.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
			this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			
			this.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
		}
	}
}
