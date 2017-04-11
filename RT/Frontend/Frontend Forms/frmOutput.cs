using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace RT {
	public partial class frmOutput : WeifenLuo.WinFormsUI.Docking.DockContent {
		#region Get/Set Methods
		public bool onlyShowFailingScenarios {
			get { return chkOnlyShowFailingScenarios.Checked; }
		}
		#endregion

		#region Constructor
		public frmOutput() {
			InitializeComponent();
		}
		#endregion

		#region Public Methods
		public void clearOutput() {
			rtfDebug.Text = string.Empty;
		}		

		public void debugWrite(string msg) {
			rtfDebug.Text += msg + Environment.NewLine;

			//highlightPLSQL(rtfDebug);

			//rtfDebug.ScrollToCaret();
			//rtfDebug.Refresh();
		}

		[Conditional("DEBUG")]
		public void debugTimingWrite(string msg) {
			rtfDebug.Text +=
				DateTime.Now.Hour.ToString() + ":" +
				DateTime.Now.Minute.ToString() + ":" +
				DateTime.Now.Second.ToString() + "." + 
				DateTime.Now.Millisecond.ToString() + " " +
				msg + Environment.NewLine;
		}

		public void highlightOutputAsPLSQL() {
			plsql.highlight(rtfDebug);
		}
		#endregion

		#region Events
		private void frmOutput_Load(object sender, EventArgs e) {
			rtfDebug.SelectionTabs = new int[] { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };			

			chkOnlyShowFailingScenarios.Checked = Properties.Settings.Default.run_onlyShowFailedScenarios;
		}

		private void rtfDebug_LinkClicked(object sender, LinkClickedEventArgs e) {
			if (e.LinkText.StartsWith("www.diffme.com")) {
				// Parse out the expected and actual values
				string filename = e.LinkText.Replace("www.diffme.com?guid=", String.Empty);
				
				// Diff the files
				String parameters = Properties.Settings.Default.fileDiffer_parameters;
				string tempPath = Path.GetTempPath();

				parameters = parameters.Replace("%expectedFilename%", tempPath + "\\rt_" + filename + "_Expected.txt");
				parameters = parameters.Replace("%actualFilename%", tempPath + "\\rt_" + filename + "_Actual.txt");
				
				Process p = new Process();
				p = Process.Start(Properties.Settings.Default.fileDiffer_executable, parameters);
			}
		}
		#endregion
	}
}
