namespace RT {
	partial class frmPostParamAssignmentHook {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPostParamAssignmentHook));
			this.rtfPostParamAssignment = new System.Windows.Forms.RichTextBox();
			this.label26 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// rtfPostParamAssignment
			// 
			this.rtfPostParamAssignment.AcceptsTab = true;
			this.rtfPostParamAssignment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfPostParamAssignment.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfPostParamAssignment.Location = new System.Drawing.Point(2, 22);
			this.rtfPostParamAssignment.Name = "rtfPostParamAssignment";
			this.rtfPostParamAssignment.Size = new System.Drawing.Size(750, 616);
			this.rtfPostParamAssignment.TabIndex = 5;
			this.rtfPostParamAssignment.Text = "";
			this.rtfPostParamAssignment.WordWrap = false;
			// 
			// label26
			// 
			this.label26.AutoSize = true;
			this.label26.Location = new System.Drawing.Point(-1, 6);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(746, 13);
			this.label26.TabIndex = 4;
			this.label26.Text = "This PL/SQL block runs after the parameter variables are set, but before the call" +
    " to the target unit.  Use p_ScenarioGUID$ to know which scenario you\'re on.";
			// 
			// frmPostParamAssignmentHook
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(754, 642);
			this.Controls.Add(this.rtfPostParamAssignment);
			this.Controls.Add(this.label26);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmPostParamAssignmentHook";
			this.Text = "Post-Param Asgn";
			this.Load += new System.EventHandler(this.frmPostParamAssignmentHook_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox rtfPostParamAssignment;
		private System.Windows.Forms.Label label26;
	}
}