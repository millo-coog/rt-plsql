namespace RT {
	partial class frmNavigation {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNavigation));
			this.projectTreeView = new RT.User_Controls.ctlProjectTreeView();
			this.SuspendLayout();
			// 
			// projectTreeView
			// 
			this.projectTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.projectTreeView.Location = new System.Drawing.Point(0, 0);
			this.projectTreeView.Name = "projectTreeView";
			this.projectTreeView.selectedNode = null;
			this.projectTreeView.Size = new System.Drawing.Size(306, 348);
			this.projectTreeView.TabIndex = 38;
			// 
			// frmNavigation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(306, 348);
			this.Controls.Add(this.projectTreeView);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmNavigation";
			this.Text = "Navigation";
			this.ResumeLayout(false);

		}

		#endregion

		public User_Controls.ctlProjectTreeView projectTreeView;

	}
}