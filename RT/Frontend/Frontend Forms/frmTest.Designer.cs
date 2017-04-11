namespace RT {
	partial class frmTest {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTest));
			this.label1 = new System.Windows.Forms.Label();
			this.lblTestGuid = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.txtTestName = new System.Windows.Forms.TextBox();
			this.lblTestXMLFilename = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtTestDescription = new System.Windows.Forms.TextBox();
			this.txtUnitName = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.chkIsPipelinedFunction = new System.Windows.Forms.CheckBox();
			this.lblCreationDate = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.txtOverload = new System.Windows.Forms.TextBox();
			this.cmbUnitType = new System.Windows.Forms.ComboBox();
			this.label19 = new System.Windows.Forms.Label();
			this.grdTestArguments = new System.Windows.Forms.DataGridView();
			this.position = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.argumentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.inOut = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.plsType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Defaulted = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tabCtrlTest = new System.Windows.Forms.TabControl();
			this.tabTestDeclare = new System.Windows.Forms.TabPage();
			this.rtfTestDeclares = new System.Windows.Forms.RichTextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.tabPLSQLBlock = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.rtfPLSQLBLock = new System.Windows.Forms.RichTextBox();
			this.btnUpdateArguments = new System.Windows.Forms.Button();
			this.imgListDefault = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.grdTestArguments)).BeginInit();
			this.tabCtrlTest.SuspendLayout();
			this.tabTestDeclare.SuspendLayout();
			this.tabPLSQLBlock.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(5, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 33;
			this.label1.Text = "Name:";
			// 
			// lblTestGuid
			// 
			this.lblTestGuid.AutoSize = true;
			this.lblTestGuid.Location = new System.Drawing.Point(70, 209);
			this.lblTestGuid.Name = "lblTestGuid";
			this.lblTestGuid.Size = new System.Drawing.Size(60, 13);
			this.lblTestGuid.TabIndex = 51;
			this.lblTestGuid.Text = "lblTestGuid";
			// 
			// label31
			// 
			this.label31.AutoSize = true;
			this.label31.Location = new System.Drawing.Point(3, 209);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(61, 13);
			this.label31.TabIndex = 50;
			this.label31.Text = "Test GUID:";
			// 
			// txtTestName
			// 
			this.txtTestName.AccessibleDescription = "";
			this.txtTestName.Location = new System.Drawing.Point(45, 6);
			this.txtTestName.Name = "txtTestName";
			this.txtTestName.Size = new System.Drawing.Size(355, 20);
			this.txtTestName.TabIndex = 32;
			// 
			// lblTestXMLFilename
			// 
			this.lblTestXMLFilename.AutoSize = true;
			this.lblTestXMLFilename.Location = new System.Drawing.Point(61, 222);
			this.lblTestXMLFilename.Name = "lblTestXMLFilename";
			this.lblTestXMLFilename.Size = new System.Drawing.Size(102, 13);
			this.lblTestXMLFilename.TabIndex = 49;
			this.lblTestXMLFilename.Text = "lblTestXMLFilename";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(-1, 86);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 36;
			this.label2.Text = "Description:";
			// 
			// label29
			// 
			this.label29.AutoSize = true;
			this.label29.Location = new System.Drawing.Point(3, 222);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(52, 13);
			this.label29.TabIndex = 48;
			this.label29.Text = "Filename:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 38;
			this.label3.Text = "Unit:";
			// 
			// txtTestDescription
			// 
			this.txtTestDescription.AcceptsReturn = true;
			this.txtTestDescription.Location = new System.Drawing.Point(3, 102);
			this.txtTestDescription.Multiline = true;
			this.txtTestDescription.Name = "txtTestDescription";
			this.txtTestDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtTestDescription.Size = new System.Drawing.Size(397, 104);
			this.txtTestDescription.TabIndex = 34;
			// 
			// txtUnitName
			// 
			this.txtUnitName.Location = new System.Drawing.Point(45, 32);
			this.txtUnitName.Name = "txtUnitName";
			this.txtUnitName.Size = new System.Drawing.Size(355, 20);
			this.txtUnitName.TabIndex = 37;
			this.txtUnitName.Validating += new System.ComponentModel.CancelEventHandler(this.txtUnitName_Validating);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(715, 8);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(47, 13);
			this.label7.TabIndex = 41;
			this.label7.Text = "Created:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// chkIsPipelinedFunction
			// 
			this.chkIsPipelinedFunction.AutoSize = true;
			this.chkIsPipelinedFunction.Location = new System.Drawing.Point(292, 62);
			this.chkIsPipelinedFunction.Name = "chkIsPipelinedFunction";
			this.chkIsPipelinedFunction.Size = new System.Drawing.Size(113, 17);
			this.chkIsPipelinedFunction.TabIndex = 46;
			this.chkIsPipelinedFunction.Text = "Pipelined Function";
			this.chkIsPipelinedFunction.UseVisualStyleBackColor = true;
			// 
			// lblCreationDate
			// 
			this.lblCreationDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblCreationDate.Location = new System.Drawing.Point(768, 8);
			this.lblCreationDate.Name = "lblCreationDate";
			this.lblCreationDate.Size = new System.Drawing.Size(66, 16);
			this.lblCreationDate.TabIndex = 42;
			this.lblCreationDate.Text = "1/2/2003";
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(188, 62);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(63, 13);
			this.label22.TabIndex = 45;
			this.label22.Text = "Overload #:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(9, 62);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(68, 13);
			this.label8.TabIndex = 43;
			this.label8.Text = "Target Type:";
			// 
			// txtOverload
			// 
			this.txtOverload.Location = new System.Drawing.Point(252, 59);
			this.txtOverload.Name = "txtOverload";
			this.txtOverload.Size = new System.Drawing.Size(34, 20);
			this.txtOverload.TabIndex = 39;
			this.txtOverload.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// cmbUnitType
			// 
			this.cmbUnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbUnitType.FormattingEnabled = true;
			this.cmbUnitType.Items.AddRange(new object[] {
            "FUNCTION",
            "PACKAGE",
            "PROCEDURE",
            "TRIGGER",
            "TYPE",
            "VIEW"});
			this.cmbUnitType.Location = new System.Drawing.Point(78, 58);
			this.cmbUnitType.Name = "cmbUnitType";
			this.cmbUnitType.Size = new System.Drawing.Size(107, 21);
			this.cmbUnitType.TabIndex = 35;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(408, 8);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(126, 13);
			this.label19.TabIndex = 44;
			this.label19.Text = "Target Unit\'s Parameters:";
			// 
			// grdTestArguments
			// 
			this.grdTestArguments.AllowUserToResizeRows = false;
			this.grdTestArguments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdTestArguments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grdTestArguments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
			this.grdTestArguments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdTestArguments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.position,
            this.argumentName,
            this.inOut,
            this.plsType,
            this.Defaulted});
			dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.grdTestArguments.DefaultCellStyle = dataGridViewCellStyle11;
			this.grdTestArguments.Location = new System.Drawing.Point(411, 28);
			this.grdTestArguments.Name = "grdTestArguments";
			this.grdTestArguments.ReadOnly = true;
			dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grdTestArguments.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
			this.grdTestArguments.Size = new System.Drawing.Size(423, 178);
			this.grdTestArguments.TabIndex = 40;
			this.grdTestArguments.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.grdTestArguments_DataBindingComplete);
			// 
			// position
			// 
			this.position.DataPropertyName = "position";
			this.position.HeaderText = "#";
			this.position.Name = "position";
			this.position.ReadOnly = true;
			this.position.Width = 39;
			// 
			// argumentName
			// 
			this.argumentName.DataPropertyName = "argumentName";
			this.argumentName.HeaderText = "Name";
			this.argumentName.Name = "argumentName";
			this.argumentName.ReadOnly = true;
			this.argumentName.Width = 60;
			// 
			// inOut
			// 
			this.inOut.DataPropertyName = "inOut";
			this.inOut.HeaderText = "In/Out";
			this.inOut.Name = "inOut";
			this.inOut.ReadOnly = true;
			this.inOut.Width = 63;
			// 
			// plsType
			// 
			this.plsType.DataPropertyName = "plsType";
			this.plsType.HeaderText = "Type";
			this.plsType.Name = "plsType";
			this.plsType.ReadOnly = true;
			this.plsType.Width = 56;
			// 
			// Defaulted
			// 
			this.Defaulted.DataPropertyName = "canDefault";
			this.Defaulted.HeaderText = "Defaulted";
			this.Defaulted.Name = "Defaulted";
			this.Defaulted.ReadOnly = true;
			this.Defaulted.Width = 78;
			// 
			// tabCtrlTest
			// 
			this.tabCtrlTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabCtrlTest.Controls.Add(this.tabTestDeclare);
			this.tabCtrlTest.Controls.Add(this.tabPLSQLBlock);
			this.tabCtrlTest.Location = new System.Drawing.Point(2, 238);
			this.tabCtrlTest.Name = "tabCtrlTest";
			this.tabCtrlTest.SelectedIndex = 0;
			this.tabCtrlTest.Size = new System.Drawing.Size(836, 454);
			this.tabCtrlTest.TabIndex = 52;
			// 
			// tabTestDeclare
			// 
			this.tabTestDeclare.Controls.Add(this.rtfTestDeclares);
			this.tabTestDeclare.Controls.Add(this.label20);
			this.tabTestDeclare.Location = new System.Drawing.Point(4, 22);
			this.tabTestDeclare.Name = "tabTestDeclare";
			this.tabTestDeclare.Padding = new System.Windows.Forms.Padding(3);
			this.tabTestDeclare.Size = new System.Drawing.Size(828, 428);
			this.tabTestDeclare.TabIndex = 0;
			this.tabTestDeclare.Text = "Declare";
			this.tabTestDeclare.UseVisualStyleBackColor = true;
			// 
			// rtfTestDeclares
			// 
			this.rtfTestDeclares.AcceptsTab = true;
			this.rtfTestDeclares.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfTestDeclares.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfTestDeclares.Location = new System.Drawing.Point(3, 20);
			this.rtfTestDeclares.Name = "rtfTestDeclares";
			this.rtfTestDeclares.Size = new System.Drawing.Size(822, 402);
			this.rtfTestDeclares.TabIndex = 26;
			this.rtfTestDeclares.Text = "";
			this.rtfTestDeclares.WordWrap = false;
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(0, 4);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(437, 13);
			this.label20.TabIndex = 25;
			this.label20.Text = "DECLARE (Variables, constants, and sub-procedures available to all child scenario" +
    " groups.)";
			// 
			// tabPLSQLBlock
			// 
			this.tabPLSQLBlock.Controls.Add(this.label4);
			this.tabPLSQLBlock.Controls.Add(this.rtfPLSQLBLock);
			this.tabPLSQLBlock.Location = new System.Drawing.Point(4, 22);
			this.tabPLSQLBlock.Name = "tabPLSQLBlock";
			this.tabPLSQLBlock.Padding = new System.Windows.Forms.Padding(3);
			this.tabPLSQLBlock.Size = new System.Drawing.Size(828, 428);
			this.tabPLSQLBlock.TabIndex = 1;
			this.tabPLSQLBlock.Text = "PL/SQL Block";
			this.tabPLSQLBlock.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(-1, 3);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(628, 13);
			this.label4.TabIndex = 30;
			this.label4.Text = "PL/SQL Block (Anonymous block to run instead of calling the target unit. Use p_Sc" +
    "enarioGUID$ to know which scenario you\'re on.)";
			// 
			// rtfPLSQLBLock
			// 
			this.rtfPLSQLBLock.AcceptsTab = true;
			this.rtfPLSQLBLock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfPLSQLBLock.DetectUrls = false;
			this.rtfPLSQLBLock.EnableAutoDragDrop = true;
			this.rtfPLSQLBLock.Location = new System.Drawing.Point(2, 19);
			this.rtfPLSQLBLock.Name = "rtfPLSQLBLock";
			this.rtfPLSQLBLock.Size = new System.Drawing.Size(823, 447);
			this.rtfPLSQLBLock.TabIndex = 29;
			this.rtfPLSQLBLock.Text = "";
			this.rtfPLSQLBLock.WordWrap = false;
			// 
			// btnUpdateArguments
			// 
			this.btnUpdateArguments.ImageKey = "refresh";
			this.btnUpdateArguments.ImageList = this.imgListDefault;
			this.btnUpdateArguments.Location = new System.Drawing.Point(531, 3);
			this.btnUpdateArguments.Name = "btnUpdateArguments";
			this.btnUpdateArguments.Size = new System.Drawing.Size(27, 23);
			this.btnUpdateArguments.TabIndex = 53;
			this.btnUpdateArguments.UseVisualStyleBackColor = false;
			this.btnUpdateArguments.Click += new System.EventHandler(this.btnUpdateArguments_Click);
			// 
			// imgListDefault
			// 
			this.imgListDefault.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListDefault.ImageStream")));
			this.imgListDefault.TransparentColor = System.Drawing.Color.Transparent;
			this.imgListDefault.Images.SetKeyName(0, "refresh");
			// 
			// frmTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(841, 695);
			this.Controls.Add(this.btnUpdateArguments);
			this.Controls.Add(this.tabCtrlTest);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblTestGuid);
			this.Controls.Add(this.label31);
			this.Controls.Add(this.txtTestName);
			this.Controls.Add(this.lblTestXMLFilename);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label29);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtTestDescription);
			this.Controls.Add(this.txtUnitName);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.chkIsPipelinedFunction);
			this.Controls.Add(this.lblCreationDate);
			this.Controls.Add(this.label22);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtOverload);
			this.Controls.Add(this.cmbUnitType);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.grdTestArguments);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmTest";
			this.Text = "Test";
			this.Load += new System.EventHandler(this.frmTest_Load);
			this.Shown += new System.EventHandler(this.frmTest_Shown);
			((System.ComponentModel.ISupportInitialize)(this.grdTestArguments)).EndInit();
			this.tabCtrlTest.ResumeLayout(false);
			this.tabTestDeclare.ResumeLayout(false);
			this.tabTestDeclare.PerformLayout();
			this.tabPLSQLBlock.ResumeLayout(false);
			this.tabPLSQLBlock.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox txtTestName;
		private System.Windows.Forms.TextBox txtTestDescription;
		private System.Windows.Forms.TextBox txtUnitName;
		private System.Windows.Forms.Label lblCreationDate;
		private System.Windows.Forms.Label lblTestGuid;
		private System.Windows.Forms.CheckBox chkIsPipelinedFunction;
		private System.Windows.Forms.TextBox txtOverload;
		private System.Windows.Forms.ComboBox cmbUnitType;
		private System.Windows.Forms.DataGridView grdTestArguments;
		private System.Windows.Forms.Label lblTestXMLFilename;
		private System.Windows.Forms.TabControl tabCtrlTest;
		private System.Windows.Forms.TabPage tabTestDeclare;
		private System.Windows.Forms.RichTextBox rtfTestDeclares;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TabPage tabPLSQLBlock;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RichTextBox rtfPLSQLBLock;
		private System.Windows.Forms.Button btnUpdateArguments;
		private System.Windows.Forms.ImageList imgListDefault;
		private System.Windows.Forms.DataGridViewTextBoxColumn position;
		private System.Windows.Forms.DataGridViewTextBoxColumn argumentName;
		private System.Windows.Forms.DataGridViewTextBoxColumn inOut;
		private System.Windows.Forms.DataGridViewTextBoxColumn plsType;
		private System.Windows.Forms.DataGridViewTextBoxColumn Defaulted;
	}
}