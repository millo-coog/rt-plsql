namespace RT {
	partial class frmScenarioGroupStartupHook {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScenarioGroupStartupHook));
			this.label10 = new System.Windows.Forms.Label();
			this.rtfScenarioGroupStartupBlock = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(0, 9);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(455, 13);
			this.label10.TabIndex = 29;
			this.label10.Text = "This PL/SQL block runs only once, before we start running the scenarios in the sc" +
    "enario group.";
			// 
			// rtfScenarioGroupStartupBlock
			// 
			this.rtfScenarioGroupStartupBlock.AcceptsTab = true;
			this.rtfScenarioGroupStartupBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfScenarioGroupStartupBlock.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfScenarioGroupStartupBlock.Location = new System.Drawing.Point(3, 28);
			this.rtfScenarioGroupStartupBlock.Name = "rtfScenarioGroupStartupBlock";
			this.rtfScenarioGroupStartupBlock.Size = new System.Drawing.Size(582, 525);
			this.rtfScenarioGroupStartupBlock.TabIndex = 28;
			this.rtfScenarioGroupStartupBlock.Text = "";
			this.rtfScenarioGroupStartupBlock.WordWrap = false;
			// 
			// frmGroupStartup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(589, 558);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.rtfScenarioGroupStartupBlock);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmGroupStartup";
			this.Text = "SCN Group Startup";
			this.Load += new System.EventHandler(this.frmGroupStartup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.RichTextBox rtfScenarioGroupStartupBlock;
	}
}