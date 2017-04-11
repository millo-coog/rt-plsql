namespace RT
{
	partial class frmArgumentUpdater
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmArgumentUpdater));
			this.btnCancel = new System.Windows.Forms.Button();
			this.imglArgumentNodes = new System.Windows.Forms.ImageList(this.components);
			this.btnAnalyze = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tvOldArguments = new System.Windows.Forms.TreeView();
			this.tvNewArguments = new System.Windows.Forms.TreeView();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(739, 598);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// imglArgumentNodes
			// 
			this.imglArgumentNodes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglArgumentNodes.ImageStream")));
			this.imglArgumentNodes.TransparentColor = System.Drawing.Color.Transparent;
			this.imglArgumentNodes.Images.SetKeyName(0, "inTestArgument");
			this.imglArgumentNodes.Images.SetKeyName(1, "outTestArgument");
			this.imglArgumentNodes.Images.SetKeyName(2, "inOutTestArgument");
			this.imglArgumentNodes.Images.SetKeyName(3, "returnTestArgument");
			// 
			// btnAnalyze
			// 
			this.btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAnalyze.Location = new System.Drawing.Point(658, 598);
			this.btnAnalyze.Name = "btnAnalyze";
			this.btnAnalyze.Size = new System.Drawing.Size(75, 23);
			this.btnAnalyze.TabIndex = 8;
			this.btnAnalyze.Text = "&Analyze";
			this.btnAnalyze.UseVisualStyleBackColor = true;
			this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(13, 12);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.tvOldArguments);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Panel2.Controls.Add(this.tvNewArguments);
			this.splitContainer1.Size = new System.Drawing.Size(801, 580);
			this.splitContainer1.SplitterDistance = 398;
			this.splitContainer1.TabIndex = 9;
			// 
			// tvOldArguments
			// 
			this.tvOldArguments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvOldArguments.ImageIndex = 0;
			this.tvOldArguments.ImageList = this.imglArgumentNodes;
			this.tvOldArguments.Location = new System.Drawing.Point(0, 16);
			this.tvOldArguments.Name = "tvOldArguments";
			this.tvOldArguments.SelectedImageIndex = 0;
			this.tvOldArguments.Size = new System.Drawing.Size(395, 564);
			this.tvOldArguments.TabIndex = 5;
			// 
			// tvNewArguments
			// 
			this.tvNewArguments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvNewArguments.ImageIndex = 0;
			this.tvNewArguments.ImageList = this.imglArgumentNodes;
			this.tvNewArguments.Location = new System.Drawing.Point(2, 16);
			this.tvNewArguments.Name = "tvNewArguments";
			this.tvNewArguments.SelectedImageIndex = 0;
			this.tvNewArguments.Size = new System.Drawing.Size(397, 564);
			this.tvNewArguments.TabIndex = 8;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "New Arguments:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Old Arguments:";
			// 
			// frmArgumentUpdater
			// 
			this.AcceptButton = this.btnAnalyze;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(826, 629);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.btnAnalyze);
			this.Controls.Add(this.btnCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmArgumentUpdater";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Argument Updater";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ImageList imglArgumentNodes;
		private System.Windows.Forms.Button btnAnalyze;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView tvOldArguments;
		private System.Windows.Forms.TreeView tvNewArguments;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}