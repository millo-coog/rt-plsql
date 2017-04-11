namespace RT {
	partial class frmUDC {
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUDC));
			this.label36 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.rtfUDCPLSQLCondition = new System.Windows.Forms.RichTextBox();
			this.txtUDCSortOrder = new System.Windows.Forms.TextBox();
			this.txtUDCDescription = new System.Windows.Forms.TextBox();
			this.txtUDCName = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.tabUDCOptions = new System.Windows.Forms.TabControl();
			this.tabUDCPLSQLBlock = new System.Windows.Forms.TabPage();
			this.rtfUDCPLSQLBlock = new System.Windows.Forms.RichTextBox();
			this.tabUDCRowExistenceCursor = new System.Windows.Forms.TabPage();
			this.rtfUDCRowExistenceCursor = new System.Windows.Forms.RichTextBox();
			this.tabUDCRowValidator = new System.Windows.Forms.TabPage();
			this.splitContainerUDCRowValidation = new System.Windows.Forms.SplitContainer();
			this.rtfUDCRowValidationCursor = new System.Windows.Forms.RichTextBox();
			this.grdRowValidation = new System.Windows.Forms.DataGridView();
			this.tabUDCCompareCursors = new System.Windows.Forms.TabPage();
			this.txtCSVExcludedColumns = new System.Windows.Forms.TextBox();
			this.label39 = new System.Windows.Forms.Label();
			this.label28 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.rtfActualCursor = new System.Windows.Forms.RichTextBox();
			this.rtfExpectedCursor = new System.Windows.Forms.RichTextBox();
			this.tabUDCompareCursorVsMatrix = new System.Windows.Forms.TabPage();
			this.mtxExpectedResults = new RT.Controls.matrix();
			this.rtfMatrixComparisonActualCursor = new System.Windows.Forms.RichTextBox();
			this.label34 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.imgLstMain = new System.Windows.Forms.ImageList(this.components);
			this.lblUDCGuid = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.cmbUDCType = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.tabUDCOptions.SuspendLayout();
			this.tabUDCPLSQLBlock.SuspendLayout();
			this.tabUDCRowExistenceCursor.SuspendLayout();
			this.tabUDCRowValidator.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerUDCRowValidation)).BeginInit();
			this.splitContainerUDCRowValidation.Panel1.SuspendLayout();
			this.splitContainerUDCRowValidation.Panel2.SuspendLayout();
			this.splitContainerUDCRowValidation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grdRowValidation)).BeginInit();
			this.tabUDCCompareCursors.SuspendLayout();
			this.tabUDCompareCursorVsMatrix.SuspendLayout();
			this.SuspendLayout();
			// 
			// label36
			// 
			this.label36.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label36.Location = new System.Drawing.Point(445, 120);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(468, 31);
			this.label36.TabIndex = 34;
			this.label36.Text = "Use p_ScenarioGUID$ to know which scenario you\'re on. Use v_arrDefaulted(\'paramet" +
    "er name\') [boolean array] to know if a parameter was actually passed in this sce" +
    "nario.";
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.Location = new System.Drawing.Point(445, 15);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(58, 13);
			this.label25.TabIndex = 33;
			this.label25.Text = "UDC Guid:";
			// 
			// rtfUDCPLSQLCondition
			// 
			this.rtfUDCPLSQLCondition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfUDCPLSQLCondition.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfUDCPLSQLCondition.Location = new System.Drawing.Point(448, 53);
			this.rtfUDCPLSQLCondition.Name = "rtfUDCPLSQLCondition";
			this.rtfUDCPLSQLCondition.Size = new System.Drawing.Size(465, 64);
			this.rtfUDCPLSQLCondition.TabIndex = 27;
			this.rtfUDCPLSQLCondition.Text = "";
			this.rtfUDCPLSQLCondition.WordWrap = false;
			// 
			// txtUDCSortOrder
			// 
			this.txtUDCSortOrder.Location = new System.Drawing.Point(299, 38);
			this.txtUDCSortOrder.Name = "txtUDCSortOrder";
			this.txtUDCSortOrder.Size = new System.Drawing.Size(38, 20);
			this.txtUDCSortOrder.TabIndex = 25;
			// 
			// txtUDCDescription
			// 
			this.txtUDCDescription.AcceptsReturn = true;
			this.txtUDCDescription.Location = new System.Drawing.Point(6, 77);
			this.txtUDCDescription.Multiline = true;
			this.txtUDCDescription.Name = "txtUDCDescription";
			this.txtUDCDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtUDCDescription.Size = new System.Drawing.Size(414, 74);
			this.txtUDCDescription.TabIndex = 26;
			// 
			// txtUDCName
			// 
			this.txtUDCName.Location = new System.Drawing.Point(47, 12);
			this.txtUDCName.Name = "txtUDCName";
			this.txtUDCName.Size = new System.Drawing.Size(373, 20);
			this.txtUDCName.TabIndex = 21;
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(445, 37);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(291, 13);
			this.label16.TabIndex = 32;
			this.label16.Text = "Conditionally run the UDC based on this boolean expression:";
			// 
			// tabUDCOptions
			// 
			this.tabUDCOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabUDCOptions.Controls.Add(this.tabUDCPLSQLBlock);
			this.tabUDCOptions.Controls.Add(this.tabUDCRowExistenceCursor);
			this.tabUDCOptions.Controls.Add(this.tabUDCRowValidator);
			this.tabUDCOptions.Controls.Add(this.tabUDCCompareCursors);
			this.tabUDCOptions.Controls.Add(this.tabUDCompareCursorVsMatrix);
			this.tabUDCOptions.ImageList = this.imgLstMain;
			this.tabUDCOptions.Location = new System.Drawing.Point(6, 157);
			this.tabUDCOptions.Name = "tabUDCOptions";
			this.tabUDCOptions.SelectedIndex = 0;
			this.tabUDCOptions.Size = new System.Drawing.Size(913, 440);
			this.tabUDCOptions.TabIndex = 31;
			// 
			// tabUDCPLSQLBlock
			// 
			this.tabUDCPLSQLBlock.Controls.Add(this.rtfUDCPLSQLBlock);
			this.tabUDCPLSQLBlock.ImageKey = "plsql";
			this.tabUDCPLSQLBlock.Location = new System.Drawing.Point(4, 23);
			this.tabUDCPLSQLBlock.Name = "tabUDCPLSQLBlock";
			this.tabUDCPLSQLBlock.Padding = new System.Windows.Forms.Padding(3);
			this.tabUDCPLSQLBlock.Size = new System.Drawing.Size(905, 413);
			this.tabUDCPLSQLBlock.TabIndex = 0;
			this.tabUDCPLSQLBlock.Text = "PL/SQL";
			this.tabUDCPLSQLBlock.UseVisualStyleBackColor = true;
			// 
			// rtfUDCPLSQLBlock
			// 
			this.rtfUDCPLSQLBlock.AcceptsTab = true;
			this.rtfUDCPLSQLBlock.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtfUDCPLSQLBlock.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfUDCPLSQLBlock.Location = new System.Drawing.Point(3, 3);
			this.rtfUDCPLSQLBlock.Name = "rtfUDCPLSQLBlock";
			this.rtfUDCPLSQLBlock.Size = new System.Drawing.Size(899, 407);
			this.rtfUDCPLSQLBlock.TabIndex = 0;
			this.rtfUDCPLSQLBlock.Text = "";
			this.rtfUDCPLSQLBlock.WordWrap = false;
			// 
			// tabUDCRowExistenceCursor
			// 
			this.tabUDCRowExistenceCursor.Controls.Add(this.rtfUDCRowExistenceCursor);
			this.tabUDCRowExistenceCursor.ImageKey = "cursor";
			this.tabUDCRowExistenceCursor.Location = new System.Drawing.Point(4, 23);
			this.tabUDCRowExistenceCursor.Name = "tabUDCRowExistenceCursor";
			this.tabUDCRowExistenceCursor.Padding = new System.Windows.Forms.Padding(3);
			this.tabUDCRowExistenceCursor.Size = new System.Drawing.Size(905, 413);
			this.tabUDCRowExistenceCursor.TabIndex = 3;
			this.tabUDCRowExistenceCursor.Text = "Row Existence Cursor";
			this.tabUDCRowExistenceCursor.UseVisualStyleBackColor = true;
			// 
			// rtfUDCRowExistenceCursor
			// 
			this.rtfUDCRowExistenceCursor.AcceptsTab = true;
			this.rtfUDCRowExistenceCursor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtfUDCRowExistenceCursor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfUDCRowExistenceCursor.Location = new System.Drawing.Point(3, 3);
			this.rtfUDCRowExistenceCursor.Name = "rtfUDCRowExistenceCursor";
			this.rtfUDCRowExistenceCursor.Size = new System.Drawing.Size(899, 407);
			this.rtfUDCRowExistenceCursor.TabIndex = 1;
			this.rtfUDCRowExistenceCursor.Text = "";
			this.rtfUDCRowExistenceCursor.WordWrap = false;
			// 
			// tabUDCRowValidator
			// 
			this.tabUDCRowValidator.Controls.Add(this.splitContainerUDCRowValidation);
			this.tabUDCRowValidator.ImageKey = "rowValidator";
			this.tabUDCRowValidator.Location = new System.Drawing.Point(4, 23);
			this.tabUDCRowValidator.Name = "tabUDCRowValidator";
			this.tabUDCRowValidator.Padding = new System.Windows.Forms.Padding(3);
			this.tabUDCRowValidator.Size = new System.Drawing.Size(905, 413);
			this.tabUDCRowValidator.TabIndex = 1;
			this.tabUDCRowValidator.Text = "Row Validator";
			this.tabUDCRowValidator.UseVisualStyleBackColor = true;
			// 
			// splitContainerUDCRowValidation
			// 
			this.splitContainerUDCRowValidation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerUDCRowValidation.Location = new System.Drawing.Point(3, 3);
			this.splitContainerUDCRowValidation.Name = "splitContainerUDCRowValidation";
			this.splitContainerUDCRowValidation.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerUDCRowValidation.Panel1
			// 
			this.splitContainerUDCRowValidation.Panel1.Controls.Add(this.rtfUDCRowValidationCursor);
			// 
			// splitContainerUDCRowValidation.Panel2
			// 
			this.splitContainerUDCRowValidation.Panel2.Controls.Add(this.grdRowValidation);
			this.splitContainerUDCRowValidation.Size = new System.Drawing.Size(899, 407);
			this.splitContainerUDCRowValidation.SplitterDistance = 124;
			this.splitContainerUDCRowValidation.TabIndex = 8;
			// 
			// rtfUDCRowValidationCursor
			// 
			this.rtfUDCRowValidationCursor.AcceptsTab = true;
			this.rtfUDCRowValidationCursor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtfUDCRowValidationCursor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfUDCRowValidationCursor.Location = new System.Drawing.Point(0, 0);
			this.rtfUDCRowValidationCursor.Name = "rtfUDCRowValidationCursor";
			this.rtfUDCRowValidationCursor.Size = new System.Drawing.Size(899, 124);
			this.rtfUDCRowValidationCursor.TabIndex = 7;
			this.rtfUDCRowValidationCursor.Text = "";
			this.rtfUDCRowValidationCursor.WordWrap = false;
			// 
			// grdRowValidation
			// 
			this.grdRowValidation.AllowUserToAddRows = false;
			this.grdRowValidation.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grdRowValidation.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.grdRowValidation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.grdRowValidation.DefaultCellStyle = dataGridViewCellStyle2;
			this.grdRowValidation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grdRowValidation.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.grdRowValidation.Location = new System.Drawing.Point(0, 0);
			this.grdRowValidation.Name = "grdRowValidation";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grdRowValidation.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.grdRowValidation.Size = new System.Drawing.Size(899, 279);
			this.grdRowValidation.TabIndex = 6;
			this.grdRowValidation.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdRowValidation_CellEnter);
			this.grdRowValidation.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdRowValidation_RowHeaderMouseClick);
			// 
			// tabUDCCompareCursors
			// 
			this.tabUDCCompareCursors.Controls.Add(this.txtCSVExcludedColumns);
			this.tabUDCCompareCursors.Controls.Add(this.label39);
			this.tabUDCCompareCursors.Controls.Add(this.label28);
			this.tabUDCCompareCursors.Controls.Add(this.label18);
			this.tabUDCCompareCursors.Controls.Add(this.rtfActualCursor);
			this.tabUDCCompareCursors.Controls.Add(this.rtfExpectedCursor);
			this.tabUDCCompareCursors.ImageKey = "compareCursors";
			this.tabUDCCompareCursors.Location = new System.Drawing.Point(4, 23);
			this.tabUDCCompareCursors.Name = "tabUDCCompareCursors";
			this.tabUDCCompareCursors.Padding = new System.Windows.Forms.Padding(3);
			this.tabUDCCompareCursors.Size = new System.Drawing.Size(905, 413);
			this.tabUDCCompareCursors.TabIndex = 2;
			this.tabUDCCompareCursors.Text = "Compare Cursors";
			this.tabUDCCompareCursors.UseVisualStyleBackColor = true;
			// 
			// txtCSVExcludedColumns
			// 
			this.txtCSVExcludedColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtCSVExcludedColumns.Location = new System.Drawing.Point(327, 386);
			this.txtCSVExcludedColumns.Name = "txtCSVExcludedColumns";
			this.txtCSVExcludedColumns.Size = new System.Drawing.Size(572, 20);
			this.txtCSVExcludedColumns.TabIndex = 6;
			// 
			// label39
			// 
			this.label39.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label39.AutoSize = true;
			this.label39.Location = new System.Drawing.Point(6, 389);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(315, 13);
			this.label39.TabIndex = 5;
			this.label39.Text = "CSV of Columns Names to Ignore in Comparison (Case Sensitive):";
			// 
			// label28
			// 
			this.label28.AutoSize = true;
			this.label28.Location = new System.Drawing.Point(3, 188);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(111, 13);
			this.label28.TabIndex = 4;
			this.label28.Text = "Actual Results Cursor:";
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(3, 9);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(126, 13);
			this.label18.TabIndex = 3;
			this.label18.Text = "Expected Results Cursor:";
			// 
			// rtfActualCursor
			// 
			this.rtfActualCursor.AcceptsTab = true;
			this.rtfActualCursor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfActualCursor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfActualCursor.Location = new System.Drawing.Point(6, 204);
			this.rtfActualCursor.Name = "rtfActualCursor";
			this.rtfActualCursor.Size = new System.Drawing.Size(893, 176);
			this.rtfActualCursor.TabIndex = 2;
			this.rtfActualCursor.Text = "";
			this.rtfActualCursor.WordWrap = false;
			// 
			// rtfExpectedCursor
			// 
			this.rtfExpectedCursor.AcceptsTab = true;
			this.rtfExpectedCursor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfExpectedCursor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfExpectedCursor.Location = new System.Drawing.Point(6, 25);
			this.rtfExpectedCursor.Name = "rtfExpectedCursor";
			this.rtfExpectedCursor.Size = new System.Drawing.Size(893, 158);
			this.rtfExpectedCursor.TabIndex = 1;
			this.rtfExpectedCursor.Text = "";
			this.rtfExpectedCursor.WordWrap = false;
			// 
			// tabUDCompareCursorVsMatrix
			// 
			this.tabUDCompareCursorVsMatrix.Controls.Add(this.mtxExpectedResults);
			this.tabUDCompareCursorVsMatrix.Controls.Add(this.rtfMatrixComparisonActualCursor);
			this.tabUDCompareCursorVsMatrix.Controls.Add(this.label34);
			this.tabUDCompareCursorVsMatrix.Controls.Add(this.label35);
			this.tabUDCompareCursorVsMatrix.ImageKey = "matrixVsCursor";
			this.tabUDCompareCursorVsMatrix.Location = new System.Drawing.Point(4, 23);
			this.tabUDCompareCursorVsMatrix.Name = "tabUDCompareCursorVsMatrix";
			this.tabUDCompareCursorVsMatrix.Padding = new System.Windows.Forms.Padding(3);
			this.tabUDCompareCursorVsMatrix.Size = new System.Drawing.Size(905, 413);
			this.tabUDCompareCursorVsMatrix.TabIndex = 4;
			this.tabUDCompareCursorVsMatrix.Text = "Compare Cursor vs. Matrix";
			this.tabUDCompareCursorVsMatrix.UseVisualStyleBackColor = true;
			// 
			// mtxExpectedResults
			// 
			this.mtxExpectedResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mtxExpectedResults.DataSourceDataTable = null;
			this.mtxExpectedResults.DataSourceXmlNode = null;
			this.mtxExpectedResults.Location = new System.Drawing.Point(9, 29);
			this.mtxExpectedResults.Name = "mtxExpectedResults";
			this.mtxExpectedResults.Size = new System.Drawing.Size(882, 226);
			this.mtxExpectedResults.TabIndex = 9;
			// 
			// rtfMatrixComparisonActualCursor
			// 
			this.rtfMatrixComparisonActualCursor.AcceptsTab = true;
			this.rtfMatrixComparisonActualCursor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtfMatrixComparisonActualCursor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfMatrixComparisonActualCursor.Location = new System.Drawing.Point(9, 275);
			this.rtfMatrixComparisonActualCursor.Name = "rtfMatrixComparisonActualCursor";
			this.rtfMatrixComparisonActualCursor.Size = new System.Drawing.Size(882, 132);
			this.rtfMatrixComparisonActualCursor.TabIndex = 8;
			this.rtfMatrixComparisonActualCursor.Text = "";
			this.rtfMatrixComparisonActualCursor.WordWrap = false;
			// 
			// label34
			// 
			this.label34.AutoSize = true;
			this.label34.Location = new System.Drawing.Point(6, 259);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(111, 13);
			this.label34.TabIndex = 7;
			this.label34.Text = "Actual Results Cursor:";
			// 
			// label35
			// 
			this.label35.AutoSize = true;
			this.label35.Location = new System.Drawing.Point(6, 13);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(93, 13);
			this.label35.TabIndex = 6;
			this.label35.Text = "Expected Results:";
			// 
			// imgLstMain
			// 
			this.imgLstMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstMain.ImageStream")));
			this.imgLstMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imgLstMain.Images.SetKeyName(0, "plsql");
			this.imgLstMain.Images.SetKeyName(1, "cursor");
			this.imgLstMain.Images.SetKeyName(2, "rowValidator");
			this.imgLstMain.Images.SetKeyName(3, "compareCursors");
			this.imgLstMain.Images.SetKeyName(4, "matrixVsCursor");
			// 
			// lblUDCGuid
			// 
			this.lblUDCGuid.AutoSize = true;
			this.lblUDCGuid.Location = new System.Drawing.Point(509, 15);
			this.lblUDCGuid.Name = "lblUDCGuid";
			this.lblUDCGuid.Size = new System.Drawing.Size(55, 13);
			this.lblUDCGuid.TabIndex = 30;
			this.lblUDCGuid.Text = "UDC Guid";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(235, 41);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(58, 13);
			this.label15.TabIndex = 29;
			this.label15.Text = "Sort Order:";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(7, 40);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(34, 13);
			this.label14.TabIndex = 28;
			this.label14.Text = "Type:";
			// 
			// cmbUDCType
			// 
			this.cmbUDCType.DisplayMember = "name";
			this.cmbUDCType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbUDCType.Enabled = false;
			this.cmbUDCType.FormattingEnabled = true;
			this.cmbUDCType.Location = new System.Drawing.Point(47, 37);
			this.cmbUDCType.Name = "cmbUDCType";
			this.cmbUDCType.Size = new System.Drawing.Size(181, 21);
			this.cmbUDCType.TabIndex = 23;
			this.cmbUDCType.ValueMember = "check_type";
			this.cmbUDCType.SelectedIndexChanged += new System.EventHandler(this.cmbUDCType_SelectedIndexChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(7, 61);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(63, 13);
			this.label13.TabIndex = 24;
			this.label13.Text = "Description:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(3, 15);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(38, 13);
			this.label12.TabIndex = 22;
			this.label12.Text = "Name:";
			// 
			// frmUDC
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(925, 601);
			this.Controls.Add(this.label36);
			this.Controls.Add(this.label25);
			this.Controls.Add(this.rtfUDCPLSQLCondition);
			this.Controls.Add(this.txtUDCSortOrder);
			this.Controls.Add(this.txtUDCDescription);
			this.Controls.Add(this.txtUDCName);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.tabUDCOptions);
			this.Controls.Add(this.lblUDCGuid);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.cmbUDCType);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label12);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmUDC";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Unknown;
			this.Text = "UDC";
			this.Load += new System.EventHandler(this.frmUDC_Load);
			this.tabUDCOptions.ResumeLayout(false);
			this.tabUDCPLSQLBlock.ResumeLayout(false);
			this.tabUDCRowExistenceCursor.ResumeLayout(false);
			this.tabUDCRowValidator.ResumeLayout(false);
			this.splitContainerUDCRowValidation.Panel1.ResumeLayout(false);
			this.splitContainerUDCRowValidation.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerUDCRowValidation)).EndInit();
			this.splitContainerUDCRowValidation.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grdRowValidation)).EndInit();
			this.tabUDCCompareCursors.ResumeLayout(false);
			this.tabUDCCompareCursors.PerformLayout();
			this.tabUDCompareCursorVsMatrix.ResumeLayout(false);
			this.tabUDCompareCursorVsMatrix.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TabPage tabUDCPLSQLBlock;
		private System.Windows.Forms.TabPage tabUDCRowExistenceCursor;
		private System.Windows.Forms.TabPage tabUDCRowValidator;
		private System.Windows.Forms.SplitContainer splitContainerUDCRowValidation;
		private System.Windows.Forms.TabPage tabUDCCompareCursors;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TabPage tabUDCompareCursorVsMatrix;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.RichTextBox rtfUDCRowExistenceCursor;
		private System.Windows.Forms.RichTextBox rtfUDCRowValidationCursor;
		private System.Windows.Forms.DataGridView grdRowValidation;
		private System.Windows.Forms.RichTextBox rtfActualCursor;
		private System.Windows.Forms.RichTextBox rtfExpectedCursor;
		private Controls.matrix mtxExpectedResults;
		private System.Windows.Forms.RichTextBox rtfMatrixComparisonActualCursor;
		private System.Windows.Forms.TextBox txtCSVExcludedColumns;
		private System.Windows.Forms.TabControl tabUDCOptions;
		private System.Windows.Forms.ImageList imgLstMain;
		private System.Windows.Forms.RichTextBox rtfUDCPLSQLCondition;
		private System.Windows.Forms.TextBox txtUDCSortOrder;
		private System.Windows.Forms.TextBox txtUDCDescription;
		private System.Windows.Forms.TextBox txtUDCName;
		private System.Windows.Forms.RichTextBox rtfUDCPLSQLBlock;
		private System.Windows.Forms.ComboBox cmbUDCType;
		private System.Windows.Forms.Label lblUDCGuid;
	}
}