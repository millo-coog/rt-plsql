namespace RT {
	partial class frmSCNGroup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSCNGroup));
			this.imgLstMain = new System.Windows.Forms.ImageList(this.components);
			this.chkRollbackAfterEachScenario = new System.Windows.Forms.CheckBox();
			this.chkReopenConnectionBeforeEveryScenario = new System.Windows.Forms.CheckBox();
			this.txtMaxAllowedRunTime = new System.Windows.Forms.TextBox();
			this.rtfScenarioGroupDeclareBlock = new System.Windows.Forms.RichTextBox();
			this.txtScenarioDescription = new System.Windows.Forms.TextBox();
			this.txtScenarioName = new System.Windows.Forms.TextBox();
			this.label38 = new System.Windows.Forms.Label();
			this.lblScenarioGroupGuid = new System.Windows.Forms.Label();
			this.label32 = new System.Windows.Forms.Label();
			this.label30 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// imgLstMain
			// 
			this.imgLstMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstMain.ImageStream")));
			this.imgLstMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imgLstMain.Images.SetKeyName(0, "graycheckmark");
			this.imgLstMain.Images.SetKeyName(1, "checkmark");
			this.imgLstMain.Images.SetKeyName(2, "scenarioGroup");
			this.imgLstMain.Images.SetKeyName(3, "scenarios");
			// 
			// chkRollbackAfterEachScenario
			// 
			this.chkRollbackAfterEachScenario.AutoSize = true;
			this.chkRollbackAfterEachScenario.Checked = true;
			this.chkRollbackAfterEachScenario.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkRollbackAfterEachScenario.Location = new System.Drawing.Point(467, 86);
			this.chkRollbackAfterEachScenario.Name = "chkRollbackAfterEachScenario";
			this.chkRollbackAfterEachScenario.Size = new System.Drawing.Size(164, 17);
			this.chkRollbackAfterEachScenario.TabIndex = 55;
			this.chkRollbackAfterEachScenario.Text = "Rollback after every scenario";
			this.chkRollbackAfterEachScenario.UseVisualStyleBackColor = true;
			// 
			// chkReopenConnectionBeforeEveryScenario
			// 
			this.chkReopenConnectionBeforeEveryScenario.AutoSize = true;
			this.chkReopenConnectionBeforeEveryScenario.Location = new System.Drawing.Point(467, 63);
			this.chkReopenConnectionBeforeEveryScenario.Name = "chkReopenConnectionBeforeEveryScenario";
			this.chkReopenConnectionBeforeEveryScenario.Size = new System.Drawing.Size(328, 17);
			this.chkReopenConnectionBeforeEveryScenario.TabIndex = 54;
			this.chkReopenConnectionBeforeEveryScenario.Text = "Reopen Connection Before Every Scenario (Serialize Scenarios)";
			this.chkReopenConnectionBeforeEveryScenario.UseVisualStyleBackColor = true;
			// 
			// txtMaxAllowedRunTime
			// 
			this.txtMaxAllowedRunTime.Location = new System.Drawing.Point(671, 37);
			this.txtMaxAllowedRunTime.MaxLength = 5;
			this.txtMaxAllowedRunTime.Name = "txtMaxAllowedRunTime";
			this.txtMaxAllowedRunTime.Size = new System.Drawing.Size(53, 20);
			this.txtMaxAllowedRunTime.TabIndex = 53;
			this.txtMaxAllowedRunTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// rtfScenarioGroupDeclareBlock
			// 
			this.rtfScenarioGroupDeclareBlock.AcceptsTab = true;
			this.rtfScenarioGroupDeclareBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfScenarioGroupDeclareBlock.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfScenarioGroupDeclareBlock.Location = new System.Drawing.Point(7, 126);
			this.rtfScenarioGroupDeclareBlock.Name = "rtfScenarioGroupDeclareBlock";
			this.rtfScenarioGroupDeclareBlock.Size = new System.Drawing.Size(913, 576);
			this.rtfScenarioGroupDeclareBlock.TabIndex = 49;
			this.rtfScenarioGroupDeclareBlock.Text = "";
			this.rtfScenarioGroupDeclareBlock.WordWrap = false;
			// 
			// txtScenarioDescription
			// 
			this.txtScenarioDescription.AcceptsReturn = true;
			this.txtScenarioDescription.AcceptsTab = true;
			this.txtScenarioDescription.Location = new System.Drawing.Point(77, 34);
			this.txtScenarioDescription.Multiline = true;
			this.txtScenarioDescription.Name = "txtScenarioDescription";
			this.txtScenarioDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtScenarioDescription.Size = new System.Drawing.Size(371, 62);
			this.txtScenarioDescription.TabIndex = 46;
			// 
			// txtScenarioName
			// 
			this.txtScenarioName.Location = new System.Drawing.Point(53, 8);
			this.txtScenarioName.Name = "txtScenarioName";
			this.txtScenarioName.Size = new System.Drawing.Size(395, 20);
			this.txtScenarioName.TabIndex = 45;
			// 
			// label38
			// 
			this.label38.AutoSize = true;
			this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label38.Location = new System.Drawing.Point(464, 40);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(201, 13);
			this.label38.TabIndex = 52;
			this.label38.Text = "Max Allowed Run Time (Seconds):";
			// 
			// lblScenarioGroupGuid
			// 
			this.lblScenarioGroupGuid.AutoSize = true;
			this.lblScenarioGroupGuid.Location = new System.Drawing.Point(562, 15);
			this.lblScenarioGroupGuid.Name = "lblScenarioGroupGuid";
			this.lblScenarioGroupGuid.Size = new System.Drawing.Size(110, 13);
			this.lblScenarioGroupGuid.TabIndex = 51;
			this.lblScenarioGroupGuid.Text = "lblScenarioGroupGuid";
			// 
			// label32
			// 
			this.label32.AutoSize = true;
			this.label32.Location = new System.Drawing.Point(464, 15);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(94, 13);
			this.label32.TabIndex = 50;
			this.label32.Text = "SCN Group GUID:";
			// 
			// label30
			// 
			this.label30.Location = new System.Drawing.Point(9, 110);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(443, 13);
			this.label30.TabIndex = 48;
			this.label30.Text = "DECLARE (Variables, constants, and sub-procedures available to just this scenario" +
    " group.)";
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(8, 37);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(63, 13);
			this.label9.TabIndex = 47;
			this.label9.Text = "Description:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(9, 11);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(43, 13);
			this.label6.TabIndex = 44;
			this.label6.Text = "Name:";
			// 
			// frmSCNGroup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(932, 714);
			this.Controls.Add(this.chkRollbackAfterEachScenario);
			this.Controls.Add(this.chkReopenConnectionBeforeEveryScenario);
			this.Controls.Add(this.txtMaxAllowedRunTime);
			this.Controls.Add(this.txtScenarioDescription);
			this.Controls.Add(this.txtScenarioName);
			this.Controls.Add(this.label38);
			this.Controls.Add(this.lblScenarioGroupGuid);
			this.Controls.Add(this.label32);
			this.Controls.Add(this.label30);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.rtfScenarioGroupDeclareBlock);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmSCNGroup";
			this.Text = "Scenario Group";
			this.Load += new System.EventHandler(this.frmSCNGroup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ImageList imgLstMain;
		private System.Windows.Forms.CheckBox chkRollbackAfterEachScenario;
		private System.Windows.Forms.CheckBox chkReopenConnectionBeforeEveryScenario;
		private System.Windows.Forms.TextBox txtMaxAllowedRunTime;
		private System.Windows.Forms.RichTextBox rtfScenarioGroupDeclareBlock;
		private System.Windows.Forms.TextBox txtScenarioDescription;
		private System.Windows.Forms.TextBox txtScenarioName;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.Label lblScenarioGroupGuid;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label6;
	}
}