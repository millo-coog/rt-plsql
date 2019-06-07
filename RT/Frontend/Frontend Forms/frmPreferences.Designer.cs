namespace RT {
	partial class frmPreferences {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPreferences));
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtTargetPassword = new System.Windows.Forms.TextBox();
			this.txtTargetUsername = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.txtFileDifferFilename = new System.Windows.Forms.TextBox();
			this.txtFileDifferParameters = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.txtExcludedSchemas = new System.Windows.Forms.TextBox();
			this.chkClearIloTasksBetweenScenarios = new System.Windows.Forms.CheckBox();
			this.label13 = new System.Windows.Forms.Label();
			this.txtNumThreads = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.txtExportPath = new System.Windows.Forms.TextBox();
			this.txtXMLEditorPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.ofdXMLEditorPath = new System.Windows.Forms.OpenFileDialog();
			this.btnBrowseForXMLEditorPath = new System.Windows.Forms.Button();
			this.label15 = new System.Windows.Forms.Label();
			this.txtPLSQLEditorPath = new System.Windows.Forms.TextBox();
			this.btnBrowseForPLSQLEditorPath = new System.Windows.Forms.Button();
			this.ofdPLSQLEditorPath = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(240, 459);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 11;
			this.btnSave.Text = "&Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(321, 459);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 12;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// txtTargetPassword
			// 
			this.txtTargetPassword.Location = new System.Drawing.Point(84, 56);
			this.txtTargetPassword.Name = "txtTargetPassword";
			this.txtTargetPassword.PasswordChar = '*';
			this.txtTargetPassword.Size = new System.Drawing.Size(222, 20);
			this.txtTargetPassword.TabIndex = 4;
			// 
			// txtTargetUsername
			// 
			this.txtTargetUsername.Location = new System.Drawing.Point(84, 30);
			this.txtTargetUsername.Name = "txtTargetUsername";
			this.txtTargetUsername.Size = new System.Drawing.Size(222, 20);
			this.txtTargetUsername.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(26, 59);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "Password:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(26, 32);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(58, 13);
			this.label6.TabIndex = 8;
			this.label6.Text = "Username:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(12, 9);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(102, 13);
			this.label8.TabIndex = 15;
			this.label8.Text = "Target Database";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(26, 110);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(82, 13);
			this.label9.TabIndex = 16;
			this.label9.Text = "Diff Executable:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(26, 136);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(82, 13);
			this.label10.TabIndex = 17;
			this.label10.Text = "Diff Parameters:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(12, 86);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(88, 13);
			this.label11.TabIndex = 18;
			this.label11.Text = "File Differ Info";
			// 
			// txtFileDifferFilename
			// 
			this.txtFileDifferFilename.Location = new System.Drawing.Point(108, 107);
			this.txtFileDifferFilename.Name = "txtFileDifferFilename";
			this.txtFileDifferFilename.Size = new System.Drawing.Size(290, 20);
			this.txtFileDifferFilename.TabIndex = 6;
			// 
			// txtFileDifferParameters
			// 
			this.txtFileDifferParameters.Location = new System.Drawing.Point(108, 133);
			this.txtFileDifferParameters.Name = "txtFileDifferParameters";
			this.txtFileDifferParameters.Size = new System.Drawing.Size(290, 20);
			this.txtFileDifferParameters.TabIndex = 7;
			this.txtFileDifferParameters.Text = "%expectedFilename% %actualFilename%";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(12, 165);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(116, 13);
			this.label12.TabIndex = 21;
			this.label12.Text = "Schema Exclusions";
			// 
			// txtExcludedSchemas
			// 
			this.txtExcludedSchemas.Location = new System.Drawing.Point(29, 185);
			this.txtExcludedSchemas.Multiline = true;
			this.txtExcludedSchemas.Name = "txtExcludedSchemas";
			this.txtExcludedSchemas.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtExcludedSchemas.Size = new System.Drawing.Size(369, 59);
			this.txtExcludedSchemas.TabIndex = 8;
			// 
			// chkClearIloTasksBetweenScenarios
			// 
			this.chkClearIloTasksBetweenScenarios.AutoSize = true;
			this.chkClearIloTasksBetweenScenarios.Location = new System.Drawing.Point(29, 277);
			this.chkClearIloTasksBetweenScenarios.Name = "chkClearIloTasksBetweenScenarios";
			this.chkClearIloTasksBetweenScenarios.Size = new System.Drawing.Size(197, 17);
			this.chkClearIloTasksBetweenScenarios.TabIndex = 9;
			this.chkClearIloTasksBetweenScenarios.Text = "Clear ILO Tasks Between Scenarios";
			this.chkClearIloTasksBetweenScenarios.UseVisualStyleBackColor = true;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(26, 303);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(122, 13);
			this.label13.TabIndex = 24;
			this.label13.Text = "Number of DB Sessions:";
			// 
			// txtNumThreads
			// 
			this.txtNumThreads.Location = new System.Drawing.Point(154, 300);
			this.txtNumThreads.Name = "txtNumThreads";
			this.txtNumThreads.Size = new System.Drawing.Size(37, 20);
			this.txtNumThreads.TabIndex = 10;
			this.txtNumThreads.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(6, 354);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(102, 13);
			this.label14.TabIndex = 25;
			this.label14.Text = "Working Copy Path:";
			// 
			// txtExportPath
			// 
			this.txtExportPath.Location = new System.Drawing.Point(108, 351);
			this.txtExportPath.Name = "txtExportPath";
			this.txtExportPath.Size = new System.Drawing.Size(290, 20);
			this.txtExportPath.TabIndex = 26;
			// 
			// txtXMLEditorPath
			// 
			this.txtXMLEditorPath.Location = new System.Drawing.Point(108, 377);
			this.txtXMLEditorPath.Name = "txtXMLEditorPath";
			this.txtXMLEditorPath.Size = new System.Drawing.Size(261, 20);
			this.txtXMLEditorPath.TabIndex = 29;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(45, 381);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 28;
			this.label1.Text = "XML Editor:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(125, 165);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(273, 13);
			this.label2.TabIndex = 30;
			this.label2.Text = " (Exclude system schemas you won\'t be using for speed.)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(12, 256);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(103, 13);
			this.label3.TabIndex = 31;
			this.label3.Text = "Runtime Settings";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(12, 331);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(38, 13);
			this.label7.TabIndex = 32;
			this.label7.Text = "Other";
			// 
			// ofdXMLEditorPath
			// 
			this.ofdXMLEditorPath.Filter = "Executes (*.exe)|*.exe";
			this.ofdXMLEditorPath.InitialDirectory = "%ProgramFiles%";
			this.ofdXMLEditorPath.SupportMultiDottedExtensions = true;
			this.ofdXMLEditorPath.Title = "Choose XML File Editor";
			this.ofdXMLEditorPath.FileOk += new System.ComponentModel.CancelEventHandler(this.ofdXMLEditorPath_FileOk);
			// 
			// btnBrowseForXMLEditorPath
			// 
			this.btnBrowseForXMLEditorPath.Location = new System.Drawing.Point(375, 377);
			this.btnBrowseForXMLEditorPath.Name = "btnBrowseForXMLEditorPath";
			this.btnBrowseForXMLEditorPath.Size = new System.Drawing.Size(25, 20);
			this.btnBrowseForXMLEditorPath.TabIndex = 33;
			this.btnBrowseForXMLEditorPath.Text = "...";
			this.btnBrowseForXMLEditorPath.UseVisualStyleBackColor = true;
			this.btnBrowseForXMLEditorPath.Click += new System.EventHandler(this.btnBrowseForXMLEditorPath_Click);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(28, 406);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(79, 13);
			this.label15.TabIndex = 34;
			this.label15.Text = "PL/SQL Editor:";
			// 
			// txtPLSQLEditorPath
			// 
			this.txtPLSQLEditorPath.Location = new System.Drawing.Point(108, 403);
			this.txtPLSQLEditorPath.Name = "txtPLSQLEditorPath";
			this.txtPLSQLEditorPath.Size = new System.Drawing.Size(261, 20);
			this.txtPLSQLEditorPath.TabIndex = 35;
			// 
			// btnBrowseForPLSQLEditorPath
			// 
			this.btnBrowseForPLSQLEditorPath.Location = new System.Drawing.Point(375, 402);
			this.btnBrowseForPLSQLEditorPath.Name = "btnBrowseForPLSQLEditorPath";
			this.btnBrowseForPLSQLEditorPath.Size = new System.Drawing.Size(25, 20);
			this.btnBrowseForPLSQLEditorPath.TabIndex = 36;
			this.btnBrowseForPLSQLEditorPath.Text = "...";
			this.btnBrowseForPLSQLEditorPath.UseVisualStyleBackColor = true;
			this.btnBrowseForPLSQLEditorPath.Click += new System.EventHandler(this.btnBrowseForPLSQLEditorPath_Click);
			// 
			// ofdPLSQLEditorPath
			// 
			this.ofdPLSQLEditorPath.Filter = "Executes (*.exe)|*.exe";
			this.ofdPLSQLEditorPath.InitialDirectory = "%ProgramFiles%";
			this.ofdPLSQLEditorPath.SupportMultiDottedExtensions = true;
			this.ofdPLSQLEditorPath.Title = "Choose PL/SQL File Editor";
			this.ofdPLSQLEditorPath.FileOk += new System.ComponentModel.CancelEventHandler(this.ofdPLSQLEditorPath_FileOk);
			// 
			// frmPreferences
			// 
			this.AcceptButton = this.btnSave;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(409, 492);
			this.Controls.Add(this.btnBrowseForPLSQLEditorPath);
			this.Controls.Add(this.txtPLSQLEditorPath);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.btnBrowseForXMLEditorPath);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtXMLEditorPath);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtExportPath);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.txtNumThreads);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.chkClearIloTasksBetweenScenarios);
			this.Controls.Add(this.txtExcludedSchemas);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.txtFileDifferParameters);
			this.Controls.Add(this.txtFileDifferFilename);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtTargetPassword);
			this.Controls.Add(this.txtTargetUsername);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmPreferences";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.Load += new System.EventHandler(this.frmPreferences_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtTargetPassword;
		private System.Windows.Forms.TextBox txtTargetUsername;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtFileDifferFilename;
		private System.Windows.Forms.TextBox txtFileDifferParameters;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox txtExcludedSchemas;
		private System.Windows.Forms.CheckBox chkClearIloTasksBetweenScenarios;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtNumThreads;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox txtExportPath;
		private System.Windows.Forms.TextBox txtXMLEditorPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.OpenFileDialog ofdXMLEditorPath;
		private System.Windows.Forms.Button btnBrowseForXMLEditorPath;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox txtPLSQLEditorPath;
		private System.Windows.Forms.Button btnBrowseForPLSQLEditorPath;
		private System.Windows.Forms.OpenFileDialog ofdPLSQLEditorPath;
	}
}