namespace RT {
	partial class frmLibraryItems {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLibraryItems));
			this.txtLibraryItemValue = new System.Windows.Forms.TextBox();
			this.txtLibraryItemDescription = new System.Windows.Forms.TextBox();
			this.btnDeleteLibraryItem = new System.Windows.Forms.Button();
			this.btnAddLibraryItem = new System.Windows.Forms.Button();
			this.label17 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cmbLibraryItemName = new System.Windows.Forms.ComboBox();
			this.label33 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtLibraryItemValue
			// 
			this.txtLibraryItemValue.AcceptsReturn = true;
			this.txtLibraryItemValue.AcceptsTab = true;
			this.txtLibraryItemValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtLibraryItemValue.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtLibraryItemValue.Location = new System.Drawing.Point(2, 106);
			this.txtLibraryItemValue.MaxLength = 0;
			this.txtLibraryItemValue.Multiline = true;
			this.txtLibraryItemValue.Name = "txtLibraryItemValue";
			this.txtLibraryItemValue.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLibraryItemValue.Size = new System.Drawing.Size(430, 573);
			this.txtLibraryItemValue.TabIndex = 44;
			this.txtLibraryItemValue.WordWrap = false;
			this.txtLibraryItemValue.TextChanged += new System.EventHandler(this.txtLibraryItemValue_TextChanged);
			// 
			// txtLibraryItemDescription
			// 
			this.txtLibraryItemDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtLibraryItemDescription.Location = new System.Drawing.Point(68, 40);
			this.txtLibraryItemDescription.Multiline = true;
			this.txtLibraryItemDescription.Name = "txtLibraryItemDescription";
			this.txtLibraryItemDescription.Size = new System.Drawing.Size(364, 44);
			this.txtLibraryItemDescription.TabIndex = 43;
			this.txtLibraryItemDescription.TextChanged += new System.EventHandler(this.txtLibraryItemDescription_TextChanged);
			// 
			// btnDeleteLibraryItem
			// 
			this.btnDeleteLibraryItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDeleteLibraryItem.Location = new System.Drawing.Point(385, 6);
			this.btnDeleteLibraryItem.Name = "btnDeleteLibraryItem";
			this.btnDeleteLibraryItem.Size = new System.Drawing.Size(46, 25);
			this.btnDeleteLibraryItem.TabIndex = 46;
			this.btnDeleteLibraryItem.Text = "Delete";
			this.btnDeleteLibraryItem.UseVisualStyleBackColor = true;
			this.btnDeleteLibraryItem.Click += new System.EventHandler(this.btnDeleteLibraryItem_Click);
			// 
			// btnAddLibraryItem
			// 
			this.btnAddLibraryItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddLibraryItem.Location = new System.Drawing.Point(330, 6);
			this.btnAddLibraryItem.Name = "btnAddLibraryItem";
			this.btnAddLibraryItem.Size = new System.Drawing.Size(49, 25);
			this.btnAddLibraryItem.TabIndex = 45;
			this.btnAddLibraryItem.Text = "Add";
			this.btnAddLibraryItem.UseVisualStyleBackColor = true;
			this.btnAddLibraryItem.Click += new System.EventHandler(this.btnAddLibraryItem_Click);
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(-1, 40);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(63, 13);
			this.label17.TabIndex = 42;
			this.label17.Text = "Description:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(-2, 12);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 13);
			this.label5.TabIndex = 41;
			this.label5.Text = "Library Item:";
			// 
			// cmbLibraryItemName
			// 
			this.cmbLibraryItemName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cmbLibraryItemName.FormattingEnabled = true;
			this.cmbLibraryItemName.Location = new System.Drawing.Point(68, 9);
			this.cmbLibraryItemName.Name = "cmbLibraryItemName";
			this.cmbLibraryItemName.Size = new System.Drawing.Size(256, 21);
			this.cmbLibraryItemName.TabIndex = 40;
			this.cmbLibraryItemName.SelectedIndexChanged += new System.EventHandler(this.cmbLibraryItemName_SelectedIndexChanged);
			// 
			// label33
			// 
			this.label33.AutoSize = true;
			this.label33.Location = new System.Drawing.Point(-1, 90);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(37, 13);
			this.label33.TabIndex = 47;
			this.label33.Text = "Value:";
			// 
			// frmLibraryItems
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 682);
			this.Controls.Add(this.label33);
			this.Controls.Add(this.txtLibraryItemValue);
			this.Controls.Add(this.txtLibraryItemDescription);
			this.Controls.Add(this.btnDeleteLibraryItem);
			this.Controls.Add(this.btnAddLibraryItem);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cmbLibraryItemName);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmLibraryItems";
			this.Text = "Library Items";
			this.Load += new System.EventHandler(this.frmLibraryItems_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtLibraryItemValue;
		private System.Windows.Forms.TextBox txtLibraryItemDescription;
		private System.Windows.Forms.Button btnDeleteLibraryItem;
		private System.Windows.Forms.Button btnAddLibraryItem;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cmbLibraryItemName;
		private System.Windows.Forms.Label label33;
	}
}