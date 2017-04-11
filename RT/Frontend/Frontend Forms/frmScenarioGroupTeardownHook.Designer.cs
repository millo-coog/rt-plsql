namespace RT {
	partial class frmScenarioGroupTeardownHook {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScenarioGroupTeardownHook));
			this.label24 = new System.Windows.Forms.Label();
			this.rtfScenarioGroupTeardownBlock = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// label24
			// 
			this.label24.AutoSize = true;
			this.label24.Location = new System.Drawing.Point(2, 5);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(455, 13);
			this.label24.TabIndex = 30;
			this.label24.Text = "This PL/SQL block runs only once, after all scenarios in the scenario group are d" +
    "one being run.";
			// 
			// rtfScenarioGroupTeardownBlock
			// 
			this.rtfScenarioGroupTeardownBlock.AcceptsTab = true;
			this.rtfScenarioGroupTeardownBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfScenarioGroupTeardownBlock.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfScenarioGroupTeardownBlock.Location = new System.Drawing.Point(3, 21);
			this.rtfScenarioGroupTeardownBlock.Name = "rtfScenarioGroupTeardownBlock";
			this.rtfScenarioGroupTeardownBlock.Size = new System.Drawing.Size(670, 556);
			this.rtfScenarioGroupTeardownBlock.TabIndex = 29;
			this.rtfScenarioGroupTeardownBlock.Text = "";
			this.rtfScenarioGroupTeardownBlock.WordWrap = false;
			// 
			// frmScenarioGroupTeardownHook
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(676, 581);
			this.Controls.Add(this.label24);
			this.Controls.Add(this.rtfScenarioGroupTeardownBlock);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmScenarioGroupTeardownHook";
			this.Text = "SCN Group Teardown";
			this.Load += new System.EventHandler(this.frmScenarioGroupTeardownHook_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.RichTextBox rtfScenarioGroupTeardownBlock;
	}
}