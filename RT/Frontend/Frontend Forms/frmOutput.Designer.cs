namespace RT {
	partial class frmOutput {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOutput));
			this.chkOnlyShowFailingScenarios = new System.Windows.Forms.CheckBox();
			this.rtfDebug = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// chkOnlyShowFailingScenarios
			// 
			this.chkOnlyShowFailingScenarios.AutoSize = true;
			this.chkOnlyShowFailingScenarios.Checked = true;
			this.chkOnlyShowFailingScenarios.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkOnlyShowFailingScenarios.Location = new System.Drawing.Point(4, 6);
			this.chkOnlyShowFailingScenarios.Name = "chkOnlyShowFailingScenarios";
			this.chkOnlyShowFailingScenarios.Size = new System.Drawing.Size(160, 17);
			this.chkOnlyShowFailingScenarios.TabIndex = 7;
			this.chkOnlyShowFailingScenarios.Text = "Only Show Failing Scenarios";
			this.chkOnlyShowFailingScenarios.UseVisualStyleBackColor = true;
			// 
			// rtfDebug
			// 
			this.rtfDebug.AcceptsTab = true;
			this.rtfDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfDebug.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfDebug.Location = new System.Drawing.Point(2, 30);
			this.rtfDebug.Name = "rtfDebug";
			this.rtfDebug.Size = new System.Drawing.Size(529, 127);
			this.rtfDebug.TabIndex = 8;
			this.rtfDebug.Text = "";
			this.rtfDebug.WordWrap = false;
			this.rtfDebug.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtfDebug_LinkClicked);
			// 
			// frmOutput
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(533, 159);
			this.Controls.Add(this.chkOnlyShowFailingScenarios);
			this.Controls.Add(this.rtfDebug);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmOutput";
			this.Text = "Output";
			this.Load += new System.EventHandler(this.frmOutput_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox chkOnlyShowFailingScenarios;
		private System.Windows.Forms.RichTextBox rtfDebug;

	}
}