namespace RT.User_Controls {
	partial class ctlProjectTreeView {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlProjectTreeView));
			this.tvProject = new System.Windows.Forms.TreeView();
			this.imgMain = new System.Windows.Forms.ImageList(this.components);
			this.btnSearch = new System.Windows.Forms.Button();
			this.txtObjectFilter = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// tvProject
			// 
			this.tvProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvProject.HideSelection = false;
			this.tvProject.ImageIndex = 0;
			this.tvProject.ImageList = this.imgMain;
			this.tvProject.Location = new System.Drawing.Point(1, 26);
			this.tvProject.Name = "tvProject";
			this.tvProject.SelectedImageIndex = 0;
			this.tvProject.Size = new System.Drawing.Size(224, 464);
			this.tvProject.TabIndex = 0;
			this.tvProject.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvProject_BeforeExpand);
			this.tvProject.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvProject_BeforeSelect);
			this.tvProject.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvProject_AfterSelect);
			this.tvProject.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvProject_NodeMouseClick);
			// 
			// imgMain
			// 
			this.imgMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgMain.ImageStream")));
			this.imgMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imgMain.Images.SetKeyName(0, "Function");
			this.imgMain.Images.SetKeyName(1, "schemaLevelFunction-Status0");
			this.imgMain.Images.SetKeyName(2, "schemaLevelFunction-Status1");
			this.imgMain.Images.SetKeyName(3, "schemaLevelFunction-Status2");
			this.imgMain.Images.SetKeyName(4, "schemaLevelFunction-Status3");
			this.imgMain.Images.SetKeyName(5, "schemaLevelFunction-Status4");
			this.imgMain.Images.SetKeyName(6, "schemaLevelFunction-Status5");
			this.imgMain.Images.SetKeyName(7, "schemaLevelFunction-Status6");
			this.imgMain.Images.SetKeyName(8, "schemaLevelFunction-Status7");
			this.imgMain.Images.SetKeyName(9, "package");
			this.imgMain.Images.SetKeyName(10, "package-Status0");
			this.imgMain.Images.SetKeyName(11, "package-Status1");
			this.imgMain.Images.SetKeyName(12, "package-Status2");
			this.imgMain.Images.SetKeyName(13, "package-Status3");
			this.imgMain.Images.SetKeyName(14, "package-Status4");
			this.imgMain.Images.SetKeyName(15, "package-Status5");
			this.imgMain.Images.SetKeyName(16, "package-Status6");
			this.imgMain.Images.SetKeyName(17, "package-Status7");
			this.imgMain.Images.SetKeyName(18, "schema");
			this.imgMain.Images.SetKeyName(19, "schema-Status0");
			this.imgMain.Images.SetKeyName(20, "schema-Status1");
			this.imgMain.Images.SetKeyName(21, "schema-Status2");
			this.imgMain.Images.SetKeyName(22, "schema-Status3");
			this.imgMain.Images.SetKeyName(23, "schema-Status4");
			this.imgMain.Images.SetKeyName(24, "schema-Status5");
			this.imgMain.Images.SetKeyName(25, "schema-Status6");
			this.imgMain.Images.SetKeyName(26, "schema-Status7");
			this.imgMain.Images.SetKeyName(27, "procedure");
			this.imgMain.Images.SetKeyName(28, "schemaLevelProcedure-Status0");
			this.imgMain.Images.SetKeyName(29, "schemaLevelProcedure-Status1");
			this.imgMain.Images.SetKeyName(30, "schemaLevelProcedure-Status2");
			this.imgMain.Images.SetKeyName(31, "schemaLevelProcedure-Status3");
			this.imgMain.Images.SetKeyName(32, "schemaLevelProcedure-Status4");
			this.imgMain.Images.SetKeyName(33, "schemaLevelProcedure-Status5");
			this.imgMain.Images.SetKeyName(34, "schemaLevelProcedure-Status6");
			this.imgMain.Images.SetKeyName(35, "schemaLevelProcedure-Status7");
			this.imgMain.Images.SetKeyName(36, "trigger");
			this.imgMain.Images.SetKeyName(37, "trigger-Status0");
			this.imgMain.Images.SetKeyName(38, "trigger-Status1");
			this.imgMain.Images.SetKeyName(39, "trigger-Status2");
			this.imgMain.Images.SetKeyName(40, "trigger-Status3");
			this.imgMain.Images.SetKeyName(41, "trigger-Status4");
			this.imgMain.Images.SetKeyName(42, "trigger-Status5");
			this.imgMain.Images.SetKeyName(43, "trigger-Status6");
			this.imgMain.Images.SetKeyName(44, "trigger-Status7");
			this.imgMain.Images.SetKeyName(45, "test");
			this.imgMain.Images.SetKeyName(46, "test-Status1");
			this.imgMain.Images.SetKeyName(47, "test-Status2");
			this.imgMain.Images.SetKeyName(48, "test-Status3");
			this.imgMain.Images.SetKeyName(49, "test-Status4");
			this.imgMain.Images.SetKeyName(50, "test-Status5");
			this.imgMain.Images.SetKeyName(51, "test-Status6");
			this.imgMain.Images.SetKeyName(52, "test-Status7");
			this.imgMain.Images.SetKeyName(53, "database");
			this.imgMain.Images.SetKeyName(54, "database-Status0");
			this.imgMain.Images.SetKeyName(55, "database-Status1");
			this.imgMain.Images.SetKeyName(56, "database-Status2");
			this.imgMain.Images.SetKeyName(57, "database-Status3");
			this.imgMain.Images.SetKeyName(58, "database-Status4");
			this.imgMain.Images.SetKeyName(59, "database-Status5");
			this.imgMain.Images.SetKeyName(60, "database-Status6");
			this.imgMain.Images.SetKeyName(61, "database-Status7");
			this.imgMain.Images.SetKeyName(62, "scenarioGroup");
			this.imgMain.Images.SetKeyName(63, "scenarioGroup-status1");
			this.imgMain.Images.SetKeyName(64, "scenarioGroup-status2");
			this.imgMain.Images.SetKeyName(65, "scenarioGroup-status3");
			this.imgMain.Images.SetKeyName(66, "scenarioGroup-status4");
			this.imgMain.Images.SetKeyName(67, "scenarioGroup-status5");
			this.imgMain.Images.SetKeyName(68, "scenarioGroup-status6");
			this.imgMain.Images.SetKeyName(69, "scenarioGroup-status7");
			this.imgMain.Images.SetKeyName(70, "checkmark");
			this.imgMain.Images.SetKeyName(71, "graycheckmark");
			this.imgMain.Images.SetKeyName(72, "rowValidator");
			this.imgMain.Images.SetKeyName(73, "compareCursors");
			this.imgMain.Images.SetKeyName(74, "console");
			this.imgMain.Images.SetKeyName(75, "scenarios");
			this.imgMain.Images.SetKeyName(76, "cursor");
			this.imgMain.Images.SetKeyName(77, "plsql");
			this.imgMain.Images.SetKeyName(78, "search");
			this.imgMain.Images.SetKeyName(79, "matrixVsCursor");
			this.imgMain.Images.SetKeyName(80, "type-Status0");
			this.imgMain.Images.SetKeyName(81, "type-Status1");
			this.imgMain.Images.SetKeyName(82, "type-Status2");
			this.imgMain.Images.SetKeyName(83, "type-Status3");
			this.imgMain.Images.SetKeyName(84, "type-Status4");
			this.imgMain.Images.SetKeyName(85, "type-Status5");
			this.imgMain.Images.SetKeyName(86, "type-Status6");
			this.imgMain.Images.SetKeyName(87, "type-Status7");
			this.imgMain.Images.SetKeyName(88, "type");
			this.imgMain.Images.SetKeyName(89, "method-Status0");
			this.imgMain.Images.SetKeyName(90, "method-Status1");
			this.imgMain.Images.SetKeyName(91, "method-Status2");
			this.imgMain.Images.SetKeyName(92, "method-Status3");
			this.imgMain.Images.SetKeyName(93, "method-Status4");
			this.imgMain.Images.SetKeyName(94, "method-Status5");
			this.imgMain.Images.SetKeyName(95, "method-Status6");
			this.imgMain.Images.SetKeyName(96, "method-Status7");
			this.imgMain.Images.SetKeyName(97, "functionsFolder-Status0");
			this.imgMain.Images.SetKeyName(98, "functionsFolder-Status1");
			this.imgMain.Images.SetKeyName(99, "functionsFolder-Status2");
			this.imgMain.Images.SetKeyName(100, "functionsFolder-Status3");
			this.imgMain.Images.SetKeyName(101, "functionsFolder-Status4");
			this.imgMain.Images.SetKeyName(102, "functionsFolder-Status5");
			this.imgMain.Images.SetKeyName(103, "functionsFolder-Status6");
			this.imgMain.Images.SetKeyName(104, "functionsFolder-Status7");
			this.imgMain.Images.SetKeyName(105, "packagesFolder-Status0");
			this.imgMain.Images.SetKeyName(106, "packagesFolder-Status1");
			this.imgMain.Images.SetKeyName(107, "packagesFolder-Status2");
			this.imgMain.Images.SetKeyName(108, "packagesFolder-Status3");
			this.imgMain.Images.SetKeyName(109, "packagesFolder-Status4");
			this.imgMain.Images.SetKeyName(110, "packagesFolder-Status5");
			this.imgMain.Images.SetKeyName(111, "packagesFolder-Status6");
			this.imgMain.Images.SetKeyName(112, "packagesFolder-Status7");
			this.imgMain.Images.SetKeyName(113, "proceduresFolder-Status0");
			this.imgMain.Images.SetKeyName(114, "proceduresFolder-Status1");
			this.imgMain.Images.SetKeyName(115, "proceduresFolder-Status2");
			this.imgMain.Images.SetKeyName(116, "proceduresFolder-Status3");
			this.imgMain.Images.SetKeyName(117, "proceduresFolder-Status4");
			this.imgMain.Images.SetKeyName(118, "proceduresFolder-Status5");
			this.imgMain.Images.SetKeyName(119, "proceduresFolder-Status6");
			this.imgMain.Images.SetKeyName(120, "proceduresFolder-Status7");
			this.imgMain.Images.SetKeyName(121, "triggersFolder-Status0");
			this.imgMain.Images.SetKeyName(122, "triggersFolder-Status1");
			this.imgMain.Images.SetKeyName(123, "triggersFolder-Status2");
			this.imgMain.Images.SetKeyName(124, "triggersFolder-Status3");
			this.imgMain.Images.SetKeyName(125, "triggersFolder-Status4");
			this.imgMain.Images.SetKeyName(126, "triggersFolder-Status5");
			this.imgMain.Images.SetKeyName(127, "triggersFolder-Status6");
			this.imgMain.Images.SetKeyName(128, "triggersFolder-Status7");
			this.imgMain.Images.SetKeyName(129, "typesFolder");
			this.imgMain.Images.SetKeyName(130, "typesFolder-Status0");
			this.imgMain.Images.SetKeyName(131, "typesFolder-Status1");
			this.imgMain.Images.SetKeyName(132, "typesFolder-Status2");
			this.imgMain.Images.SetKeyName(133, "typesFolder-Status3");
			this.imgMain.Images.SetKeyName(134, "typesFolder-Status4");
			this.imgMain.Images.SetKeyName(135, "typesFolder-Status5");
			this.imgMain.Images.SetKeyName(136, "typesFolder-Status6");
			this.imgMain.Images.SetKeyName(137, "typesFolder-Status7");
			this.imgMain.Images.SetKeyName(138, "UDC");
			this.imgMain.Images.SetKeyName(139, "UDC-Status0");
			this.imgMain.Images.SetKeyName(140, "UDC-Status1");
			this.imgMain.Images.SetKeyName(141, "UDC-Status2");
			this.imgMain.Images.SetKeyName(142, "UDC-Status3");
			this.imgMain.Images.SetKeyName(143, "UDC-Status4");
			this.imgMain.Images.SetKeyName(144, "UDC-Status5");
			this.imgMain.Images.SetKeyName(145, "UDC-Status6");
			this.imgMain.Images.SetKeyName(146, "UDC-Status7");
			this.imgMain.Images.SetKeyName(147, "triggersFolder");
			this.imgMain.Images.SetKeyName(148, "packagesFolder");
			this.imgMain.Images.SetKeyName(149, "proceduresFolder");
			this.imgMain.Images.SetKeyName(150, "SVNLogHistory");
			this.imgMain.Images.SetKeyName(151, "viewsFolder");
			this.imgMain.Images.SetKeyName(152, "viewsFolder-Status0");
			this.imgMain.Images.SetKeyName(153, "viewsFolder-Status1");
			this.imgMain.Images.SetKeyName(154, "viewsFolder-Status2");
			this.imgMain.Images.SetKeyName(155, "viewsFolder-Status3");
			this.imgMain.Images.SetKeyName(156, "viewsFolder-Status4");
			this.imgMain.Images.SetKeyName(157, "viewsFolder-Status5");
			this.imgMain.Images.SetKeyName(158, "viewsFolder-Status6");
			this.imgMain.Images.SetKeyName(159, "viewsFolder-Status7");
			this.imgMain.Images.SetKeyName(160, "view");
			this.imgMain.Images.SetKeyName(161, "view-Status0");
			this.imgMain.Images.SetKeyName(162, "view-Status1");
			this.imgMain.Images.SetKeyName(163, "view-Status2");
			this.imgMain.Images.SetKeyName(164, "view-Status3");
			this.imgMain.Images.SetKeyName(165, "view-Status4");
			this.imgMain.Images.SetKeyName(166, "view-Status5");
			this.imgMain.Images.SetKeyName(167, "view-Status6");
			this.imgMain.Images.SetKeyName(168, "view-Status7");
			this.imgMain.Images.SetKeyName(169, "getRunBlock");
			this.imgMain.Images.SetKeyName(170, "folder");
			this.imgMain.Images.SetKeyName(171, "returnTestArgument");
			this.imgMain.Images.SetKeyName(172, "inOutTestArgument");
			this.imgMain.Images.SetKeyName(173, "inTestArgument");
			this.imgMain.Images.SetKeyName(174, "outTestArgument");
			this.imgMain.Images.SetKeyName(175, "scenario");
			this.imgMain.Images.SetKeyName(176, "libraryItem");
			this.imgMain.Images.SetKeyName(177, "scenarioStartup");
			this.imgMain.Images.SetKeyName(178, "scenarioTeardown");
			this.imgMain.Images.SetKeyName(179, "scenarioGroupStartup");
			this.imgMain.Images.SetKeyName(180, "scenarioGroupTeardown");
			this.imgMain.Images.SetKeyName(181, "postParamAssignment");
			this.imgMain.Images.SetKeyName(182, "preUDCs");
			// 
			// btnSearch
			// 
			this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSearch.ImageKey = "search";
			this.btnSearch.ImageList = this.imgMain;
			this.btnSearch.Location = new System.Drawing.Point(201, 1);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(25, 24);
			this.btnSearch.TabIndex = 38;
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// txtObjectFilter
			// 
			this.txtObjectFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtObjectFilter.Location = new System.Drawing.Point(1, 3);
			this.txtObjectFilter.Name = "txtObjectFilter";
			this.txtObjectFilter.Size = new System.Drawing.Size(197, 20);
			this.txtObjectFilter.TabIndex = 37;
			this.txtObjectFilter.Enter += new System.EventHandler(this.txtObjectFilter_Enter);
			this.txtObjectFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtObjectFilter_KeyDown);
			this.txtObjectFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtObjectFilter_KeyPress);
			// 
			// ctlProjectTreeView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.txtObjectFilter);
			this.Controls.Add(this.tvProject);
			this.Name = "ctlProjectTreeView";
			this.Size = new System.Drawing.Size(226, 493);
			this.Load += new System.EventHandler(this.ctlProjectTreeView_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView tvProject;
		private System.Windows.Forms.ImageList imgMain;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.TextBox txtObjectFilter;
	}
}
