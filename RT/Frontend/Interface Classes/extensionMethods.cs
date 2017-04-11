/*
 * This class adds some extension methods to the gridview that we use in the scenarios gridview,
 * to make scrolling through many rows/columns visually smooth and not flicker.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

public static class ExtensionMethods {
	// Toggles whether or not the given gridview will be double buffered.
	public static void DoubleBuffered(this DataGridView dgv, bool setting) {
		Type dgvType = dgv.GetType();
		PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
				BindingFlags.Instance | BindingFlags.NonPublic);
		pi.SetValue(dgv, setting, null);
	}

	#region Redraw Suspend/Resume
	[DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
	private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
	private const int WM_SETREDRAW = 0xB;

	public static void SuspendDrawing(this Control target) {
		SendMessage(target.Handle, WM_SETREDRAW, 0, 0);
	}

	public static void ResumeDrawing(this Control target) { ResumeDrawing(target, true); }
	public static void ResumeDrawing(this Control target, bool redraw) {
		SendMessage(target.Handle, WM_SETREDRAW, 1, 0);

		if (redraw) {
			target.Refresh();
		}
	}
	#endregion
}
