namespace RT {
	partial class frmPreUDCsHook {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPreUDCsHook));
			this.rtfPreUDCPLSQL = new System.Windows.Forms.RichTextBox();
			this.label27 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// rtfPreUDCPLSQL
			// 
			this.rtfPreUDCPLSQL.AcceptsTab = true;
			this.rtfPreUDCPLSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfPreUDCPLSQL.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfPreUDCPLSQL.Location = new System.Drawing.Point(3, 21);
			this.rtfPreUDCPLSQL.Name = "rtfPreUDCPLSQL";
			this.rtfPreUDCPLSQL.Size = new System.Drawing.Size(765, 533);
			this.rtfPreUDCPLSQL.TabIndex = 7;
			this.rtfPreUDCPLSQL.Text = "";
			this.rtfPreUDCPLSQL.WordWrap = false;
			// 
			// label27
			// 
			this.label27.AutoSize = true;
			this.label27.Location = new System.Drawing.Point(0, 4);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(677, 13);
			this.label27.TabIndex = 6;
			this.label27.Text = "This PL/SQL block runs after the target unit is called, but before the UDC\'s are " +
    "run.  Use p_ScenarioGUID$ to know which scenario you\'re on.";
			// 
			// frmPreUDCsHook
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(771, 558);
			this.Controls.Add(this.rtfPreUDCPLSQL);
			this.Controls.Add(this.label27);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmPreUDCsHook";
			this.Text = "Pre-UDC\'s";
			this.Load += new System.EventHandler(this.frmPreUDCsHook_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox rtfPreUDCPLSQL;
		private System.Windows.Forms.Label label27;
	}
}