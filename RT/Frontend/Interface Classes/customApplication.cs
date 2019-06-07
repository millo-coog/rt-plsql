/*
 * This class allows us to have RT already running, but user clicks on RT hyperlinks
 * get directed to this currently running process, rather than another one.
 * 
 * From: http://stackoverflow.com/questions/6559416/c-sharp-register-commandline-argument-dont-start-new-instance
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;

namespace RT {
	public class myCustomApplication : WindowsFormsApplicationBase {
		private static myCustomApplication _myapp;

		public static void Run(Form startupform) {
			_myapp = new myCustomApplication(startupform);

			_myapp.StartupNextInstance += new Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventHandler(_myapp_StartupNextInstance);

			_myapp.Run(Environment.GetCommandLineArgs());
		}

		static void _myapp_StartupNextInstance(object sender, Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs e) {
			// Parse the command line arguments that were given to an already-running instance...
			for (int i = 1; i < e.CommandLine.Count; i++) {
				string argumentKeyValue = e.CommandLine[i];

				string argumentKey = argumentKeyValue.Trim('/').Split('=')[0].ToLower();
				string argumentValue = string.Empty;

				if (argumentKeyValue.Trim('/').Split('=').Count() == 2) {
					argumentValue = argumentKeyValue.Trim('/').Split('=')[1];
				}
				
				MessageBox.Show("Unrecognized argument: " + e.CommandLine[i]);
			}

			// Bring the form back to the front.
			e.BringToForeground = true;
		}

		private myCustomApplication(Form mainform) {
			this.IsSingleInstance = true;
			this.MainForm = mainform;
		}
	}
}
