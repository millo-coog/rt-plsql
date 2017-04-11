namespace RT.Controls {
	partial class matrix {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.grdMatrix = new System.Windows.Forms.DataGridView();
			this.btnAddColumn = new System.Windows.Forms.Button();
			this.btnDropColumn = new System.Windows.Forms.Button();
			this.btnRenameColumn = new System.Windows.Forms.Button();
			this.btnPasteFromClipboard = new System.Windows.Forms.Button();
			this.chkShowComparisonTypeColumns = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.grdMatrix)).BeginInit();
			this.SuspendLayout();
			// 
			// grdMatrix
			// 
			this.grdMatrix.AllowUserToOrderColumns = true;
			this.grdMatrix.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdMatrix.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.grdMatrix.Location = new System.Drawing.Point(4, 33);
			this.grdMatrix.Name = "grdMatrix";
			this.grdMatrix.Size = new System.Drawing.Size(658, 271);
			this.grdMatrix.TabIndex = 0;
			this.grdMatrix.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdMatrix_CellDoubleClick);
			this.grdMatrix.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdMatrix_KeyDown);
			// 
			// btnAddColumn
			// 
			this.btnAddColumn.Location = new System.Drawing.Point(4, 4);
			this.btnAddColumn.Name = "btnAddColumn";
			this.btnAddColumn.Size = new System.Drawing.Size(75, 23);
			this.btnAddColumn.TabIndex = 1;
			this.btnAddColumn.Text = "Add Colum&n";
			this.btnAddColumn.UseVisualStyleBackColor = true;
			this.btnAddColumn.Click += new System.EventHandler(this.btnAddColumn_Click);
			// 
			// btnDropColumn
			// 
			this.btnDropColumn.Location = new System.Drawing.Point(86, 4);
			this.btnDropColumn.Name = "btnDropColumn";
			this.btnDropColumn.Size = new System.Drawing.Size(80, 23);
			this.btnDropColumn.TabIndex = 2;
			this.btnDropColumn.Text = "&Drop Column";
			this.btnDropColumn.UseVisualStyleBackColor = true;
			this.btnDropColumn.Click += new System.EventHandler(this.btnDropColumn_Click);
			// 
			// btnRenameColumn
			// 
			this.btnRenameColumn.Location = new System.Drawing.Point(172, 4);
			this.btnRenameColumn.Name = "btnRenameColumn";
			this.btnRenameColumn.Size = new System.Drawing.Size(116, 23);
			this.btnRenameColumn.TabIndex = 3;
			this.btnRenameColumn.Text = "Rename Column (F2)";
			this.btnRenameColumn.UseVisualStyleBackColor = true;
			this.btnRenameColumn.Click += new System.EventHandler(this.btnRenameColumn_Click);
			// 
			// btnPasteFromClipboard
			// 
			this.btnPasteFromClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPasteFromClipboard.Location = new System.Drawing.Point(531, 4);
			this.btnPasteFromClipboard.Name = "btnPasteFromClipboard";
			this.btnPasteFromClipboard.Size = new System.Drawing.Size(131, 23);
			this.btnPasteFromClipboard.TabIndex = 4;
			this.btnPasteFromClipboard.Text = "Overlay from Clipboard";
			this.btnPasteFromClipboard.UseVisualStyleBackColor = true;
			this.btnPasteFromClipboard.Click += new System.EventHandler(this.btnPasteFromClipboard_Click);
			// 
			// chkShowComparisonTypeColumns
			// 
			this.chkShowComparisonTypeColumns.AutoSize = true;
			this.chkShowComparisonTypeColumns.Checked = true;
			this.chkShowComparisonTypeColumns.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShowComparisonTypeColumns.Location = new System.Drawing.Point(328, 8);
			this.chkShowComparisonTypeColumns.Name = "chkShowComparisonTypeColumns";
			this.chkShowComparisonTypeColumns.Size = new System.Drawing.Size(181, 17);
			this.chkShowComparisonTypeColumns.TabIndex = 5;
			this.chkShowComparisonTypeColumns.Text = "Show Comparison Type Columns";
			this.chkShowComparisonTypeColumns.UseVisualStyleBackColor = true;
			this.chkShowComparisonTypeColumns.CheckedChanged += new System.EventHandler(this.chkShowComparisonTypeColumns_CheckedChanged);
			// 
			// matrix
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.chkShowComparisonTypeColumns);
			this.Controls.Add(this.btnPasteFromClipboard);
			this.Controls.Add(this.btnRenameColumn);
			this.Controls.Add(this.btnDropColumn);
			this.Controls.Add(this.btnAddColumn);
			this.Controls.Add(this.grdMatrix);
			this.Name = "matrix";
			this.Size = new System.Drawing.Size(665, 307);
			((System.ComponentModel.ISupportInitialize)(this.grdMatrix)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView grdMatrix;
		private System.Windows.Forms.Button btnAddColumn;
		private System.Windows.Forms.Button btnDropColumn;
		private System.Windows.Forms.Button btnRenameColumn;
		private System.Windows.Forms.Button btnPasteFromClipboard;
		private System.Windows.Forms.CheckBox chkShowComparisonTypeColumns;
	}
}
