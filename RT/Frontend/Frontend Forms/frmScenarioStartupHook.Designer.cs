namespace RT {
	partial class frmScenarioStartupHook {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScenarioStartupHook));
			this.label11 = new System.Windows.Forms.Label();
			this.rtfScenarioStartupBlock = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(-1, 5);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(681, 13);
			this.label11.TabIndex = 30;
			this.label11.Text = "This PL/SQL block runs before executing every single scenario in the scenario gro" +
    "up. Use p_ScenarioGUID$ to know what scenario you\'re on.";
			// 
			// rtfScenarioStartupBlock
			// 
			this.rtfScenarioStartupBlock.AcceptsTab = true;
			this.rtfScenarioStartupBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfScenarioStartupBlock.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfScenarioStartupBlock.Location = new System.Drawing.Point(2, 21);
			this.rtfScenarioStartupBlock.Name = "rtfScenarioStartupBlock";
			this.rtfScenarioStartupBlock.Size = new System.Drawing.Size(857, 529);
			this.rtfScenarioStartupBlock.TabIndex = 29;
			this.rtfScenarioStartupBlock.Text = "";
			this.rtfScenarioStartupBlock.WordWrap = false;
			// 
			// frmScenarioStartupHook
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(860, 551);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.rtfScenarioStartupBlock);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmScenarioStartupHook";
			this.Text = "Scenario Startup";
			this.Load += new System.EventHandler(this.frmScenarioStartupHook_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.RichTextBox rtfScenarioStartupBlock;
	}
}