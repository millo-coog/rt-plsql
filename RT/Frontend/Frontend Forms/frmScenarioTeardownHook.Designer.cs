namespace RT {
	partial class frmScenarioTeardownHook {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScenarioTeardownHook));
			this.label23 = new System.Windows.Forms.Label();
			this.rtfScenarioTeardownBlock = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(-1, 9);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(526, 13);
			this.label23.TabIndex = 43;
			this.label23.Text = "This PL/SQL block runs after every single scenario. Use p_ScenarioGUID$ to know w" +
    "hich scenario you\'re on.";
			// 
			// rtfScenarioTeardownBlock
			// 
			this.rtfScenarioTeardownBlock.AcceptsTab = true;
			this.rtfScenarioTeardownBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfScenarioTeardownBlock.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfScenarioTeardownBlock.Location = new System.Drawing.Point(2, 26);
			this.rtfScenarioTeardownBlock.Name = "rtfScenarioTeardownBlock";
			this.rtfScenarioTeardownBlock.Size = new System.Drawing.Size(617, 474);
			this.rtfScenarioTeardownBlock.TabIndex = 42;
			this.rtfScenarioTeardownBlock.Text = "";
			this.rtfScenarioTeardownBlock.WordWrap = false;
			// 
			// frmScenarioTeardownHook
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(620, 505);
			this.Controls.Add(this.label23);
			this.Controls.Add(this.rtfScenarioTeardownBlock);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmScenarioTeardownHook";
			this.Text = "Scenario Teardown";
			this.Load += new System.EventHandler(this.frmScenarioTeardownHook_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.RichTextBox rtfScenarioTeardownBlock;
	}
}