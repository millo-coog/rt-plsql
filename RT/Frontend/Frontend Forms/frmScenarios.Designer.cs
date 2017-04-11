namespace RT {
	partial class frmScenarios {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScenarios));
			this.btnGetIndividualScenarioRunBlock = new System.Windows.Forms.Button();
			this.btnGenerateAutoScenarios = new System.Windows.Forms.Button();
			this.chkHideParameterTypes = new System.Windows.Forms.CheckBox();
			this.btnShrinkColumns = new System.Windows.Forms.Button();
			this.btnCloneScenario = new System.Windows.Forms.Button();
			this.chkDisplayParamTypesInline = new System.Windows.Forms.CheckBox();
			this.grdScenarios = new System.Windows.Forms.DataGridView();
			this.cmsCellPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cmbValueComparisonType = new System.Windows.Forms.ToolStripComboBox();
			((System.ComponentModel.ISupportInitialize)(this.grdScenarios)).BeginInit();
			this.cmsCellPopup.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnGetIndividualScenarioRunBlock
			// 
			this.btnGetIndividualScenarioRunBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGetIndividualScenarioRunBlock.Location = new System.Drawing.Point(793, 10);
			this.btnGetIndividualScenarioRunBlock.Name = "btnGetIndividualScenarioRunBlock";
			this.btnGetIndividualScenarioRunBlock.Size = new System.Drawing.Size(114, 23);
			this.btnGetIndividualScenarioRunBlock.TabIndex = 13;
			this.btnGetIndividualScenarioRunBlock.Text = "Get Scn. Run Block";
			this.btnGetIndividualScenarioRunBlock.UseVisualStyleBackColor = true;
			this.btnGetIndividualScenarioRunBlock.Click += new System.EventHandler(this.btnGetIndividualScenarioRunBlock_Click);
			// 
			// btnGenerateAutoScenarios
			// 
			this.btnGenerateAutoScenarios.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGenerateAutoScenarios.Location = new System.Drawing.Point(448, 12);
			this.btnGenerateAutoScenarios.Name = "btnGenerateAutoScenarios";
			this.btnGenerateAutoScenarios.Size = new System.Drawing.Size(75, 23);
			this.btnGenerateAutoScenarios.TabIndex = 12;
			this.btnGenerateAutoScenarios.Text = "Auto Scn.";
			this.btnGenerateAutoScenarios.UseVisualStyleBackColor = true;
			this.btnGenerateAutoScenarios.Click += new System.EventHandler(this.btnGenerateAutoScenarios_Click);
			// 
			// chkHideParameterTypes
			// 
			this.chkHideParameterTypes.AutoSize = true;
			this.chkHideParameterTypes.Location = new System.Drawing.Point(144, 16);
			this.chkHideParameterTypes.Name = "chkHideParameterTypes";
			this.chkHideParameterTypes.Size = new System.Drawing.Size(131, 17);
			this.chkHideParameterTypes.TabIndex = 9;
			this.chkHideParameterTypes.Text = "Hide Parameter Types";
			this.chkHideParameterTypes.UseVisualStyleBackColor = true;
			this.chkHideParameterTypes.CheckedChanged += new System.EventHandler(this.chkHideParameterTypes_CheckedChanged);
			// 
			// btnShrinkColumns
			// 
			this.btnShrinkColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnShrinkColumns.Location = new System.Drawing.Point(529, 12);
			this.btnShrinkColumns.Name = "btnShrinkColumns";
			this.btnShrinkColumns.Size = new System.Drawing.Size(75, 23);
			this.btnShrinkColumns.TabIndex = 10;
			this.btnShrinkColumns.Text = "Shrink Cols.";
			this.btnShrinkColumns.UseVisualStyleBackColor = true;
			this.btnShrinkColumns.Click += new System.EventHandler(this.btnShrinkColumns_Click);
			// 
			// btnCloneScenario
			// 
			this.btnCloneScenario.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCloneScenario.Location = new System.Drawing.Point(610, 12);
			this.btnCloneScenario.Name = "btnCloneScenario";
			this.btnCloneScenario.Size = new System.Drawing.Size(78, 23);
			this.btnCloneScenario.TabIndex = 11;
			this.btnCloneScenario.Text = "Clone Scn.(s)";
			this.btnCloneScenario.UseVisualStyleBackColor = true;
			this.btnCloneScenario.Click += new System.EventHandler(this.btnCloneScenario_Click);
			// 
			// chkDisplayParamTypesInline
			// 
			this.chkDisplayParamTypesInline.AutoSize = true;
			this.chkDisplayParamTypesInline.Location = new System.Drawing.Point(4, 16);
			this.chkDisplayParamTypesInline.Name = "chkDisplayParamTypesInline";
			this.chkDisplayParamTypesInline.Size = new System.Drawing.Size(134, 17);
			this.chkDisplayParamTypesInline.TabIndex = 8;
			this.chkDisplayParamTypesInline.Text = "Inline Parameter Types";
			this.chkDisplayParamTypesInline.UseVisualStyleBackColor = true;
			this.chkDisplayParamTypesInline.CheckedChanged += new System.EventHandler(this.chkDisplayParamTypesInline_CheckedChanged);
			// 
			// grdScenarios
			// 
			this.grdScenarios.AllowUserToOrderColumns = true;
			this.grdScenarios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdScenarios.CausesValidation = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grdScenarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.grdScenarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdScenarios.ContextMenuStrip = this.cmsCellPopup;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.grdScenarios.DefaultCellStyle = dataGridViewCellStyle2;
			this.grdScenarios.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.grdScenarios.Location = new System.Drawing.Point(4, 39);
			this.grdScenarios.Name = "grdScenarios";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grdScenarios.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.grdScenarios.Size = new System.Drawing.Size(903, 668);
			this.grdScenarios.TabIndex = 14;
			this.grdScenarios.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.grdScenarios_CellContextMenuStripNeeded);
			this.grdScenarios.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdScenarios_CellDoubleClick);
			this.grdScenarios.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdScenarios_CellEnter);
			this.grdScenarios.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdScenarios_CellMouseEnter);
			this.grdScenarios.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.grdScenarios_CellToolTipTextNeeded);
			this.grdScenarios.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdScenarios_CellValueChanged);
			this.grdScenarios.CurrentCellDirtyStateChanged += new System.EventHandler(this.grdScenarios_CurrentCellDirtyStateChanged);
			this.grdScenarios.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grdScenarios_DataError);
			this.grdScenarios.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.grdScenarios_DefaultValuesNeeded);
			this.grdScenarios.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdScenarios_RowHeaderMouseClick);
			this.grdScenarios.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.grdScenarios_UserDeletingRow);
			// 
			// cmsCellPopup
			// 
			this.cmsCellPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbValueComparisonType});
			this.cmsCellPopup.Name = "cmsCellPopup";
			this.cmsCellPopup.ShowImageMargin = false;
			this.cmsCellPopup.Size = new System.Drawing.Size(157, 31);
			// 
			// cmbValueComparisonType
			// 
			this.cmbValueComparisonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbValueComparisonType.Name = "cmbValueComparisonType";
			this.cmbValueComparisonType.Size = new System.Drawing.Size(121, 23);
			this.cmbValueComparisonType.SelectedIndexChanged += new System.EventHandler(this.cmbValueComparisonType_SelectedIndexChanged);
			// 
			// frmScenarios
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(909, 709);
			this.Controls.Add(this.grdScenarios);
			this.Controls.Add(this.btnGetIndividualScenarioRunBlock);
			this.Controls.Add(this.btnGenerateAutoScenarios);
			this.Controls.Add(this.chkHideParameterTypes);
			this.Controls.Add(this.btnShrinkColumns);
			this.Controls.Add(this.btnCloneScenario);
			this.Controls.Add(this.chkDisplayParamTypesInline);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmScenarios";
			this.Text = "Scenarios";
			this.Load += new System.EventHandler(this.frmScenarios_Load);
			((System.ComponentModel.ISupportInitialize)(this.grdScenarios)).EndInit();
			this.cmsCellPopup.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnGetIndividualScenarioRunBlock;
		private System.Windows.Forms.Button btnGenerateAutoScenarios;
		private System.Windows.Forms.CheckBox chkHideParameterTypes;
		private System.Windows.Forms.Button btnShrinkColumns;
		private System.Windows.Forms.Button btnCloneScenario;
		private System.Windows.Forms.CheckBox chkDisplayParamTypesInline;
		private System.Windows.Forms.DataGridView grdScenarios;
		private System.Windows.Forms.ContextMenuStrip cmsCellPopup;
		private System.Windows.Forms.ToolStripComboBox cmbValueComparisonType;
	}
}