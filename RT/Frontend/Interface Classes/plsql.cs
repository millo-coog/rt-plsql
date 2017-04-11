using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;

namespace RT {
	public static class plsql {
		public static void highlight(RichTextBox rtfDestination) {
			if (rtfDestination.Text.Trim() == string.Empty)
				return;

			//Program.outputForm.debugTimingWrite("Initializing highlighter");
			
			Regex expLines = new Regex("\n");
			string trimmedLine;
			String[] tokens;
			bool inString = false;
			bool inMultiLineComment = false;
			bool inSingleLineComment = false;

			// Check whether the token is a keyword.
			String[] keywords = {
				// Keywords
				"DECLARE", "BEGIN", "END", "EXCEPTION", "WHEN", "OTHERS", "THEN", "IF", "ELSE", "RAISE", 
				"CASE", "NULL", "CURSOR", "IS", "PROCEDURE", "SAVEPOINT", "ROLLBACK", "TRUE", "FALSE", "TO", "OPEN", "FETCH",
				"INTO", "CLOSE", "FOUND", "NOTFOUND", "ISOPEN", "FOR", "LOOP", "CONSTANT", "TABLE", "EXIT", "RETURN",
				"TYPE", "SUBTYPE", "RECORD", "FUNCTION", "BULK", "COLLECT", "ELSIF", "EXECUTE", "IMMEDIATE", "USING",
				"INDEX", "OF", "FIRST", "NEXT", "MINUS",
				
				// Functions
				"REPLACE", "TRIM", "LTRIM", "RTRIM", "RAISE_APPLICATION_ERROR", "CHR", "SQLCODE", "SUBSTR", 
				"TO_CLOB", "TO_DATE", "TO_NUMBER", "SQLERRM", "COUNT", "MAX", "MIN", "SUM",

				// Symbols
				":=", "(", ")", ";", "=>", "'", "-", ",", ":", "'", "+", "|", "=", "!", "<", ">", "..",

				// Types
				"VARCHAR2", "VARCHAR", "CHAR", "CLOB", "BOOLEAN", "FUNCTION", "RETURN", "PLS_INTEGER", "DATE", "TIMESTAMP", "NUMBER",
				
				// SQL
				"UPDATE", "SET", "WHERE", "INSERT", "AND", "NOT", "IN", "DELETE", "FROM", "OR",
				"SELECT", "GROUP", "BY", "ORDER", "UNION", "ALL", "AS", "JOIN", "LEFT", "RIGHT", "OUTER", "NO_DATA_FOUND", "HAVING" };

			Regex r = new Regex("(/\\*)|(\\*/)|( )|(:=)|(--)|([,=;\\|\\(\\):'])", RegexOptions.Compiled);

			rtfDestination.Visible = false; // For speed
			
			rtfDestination.Text = rtfDestination.Text.Replace("\r\n", "\n").Replace("\r", "\n");
			rtfDestination.Text = rtfDestination.Text.Trim();
			
			String[] lines = expLines.Split(rtfDestination.Text);

			rtfDestination.Text = string.Empty;

			for (int i = 0; i < lines.Length; i++) {
				string line = lines[i];

				trimmedLine = line.Trim();

				if (trimmedLine == String.Empty) {
					// Blank lines don't need to be syntax colored
					rtfDestination.SelectedText = line + Environment.NewLine;

					inSingleLineComment = false;
				} else {
					if (trimmedLine.Length >= 2 && trimmedLine.Substring(0, 2) == "--") {
						// In whole-line comments, the whole line gets highlighted the same way
						rtfDestination.SelectionFont = new Font("Courier New", 9, FontStyle.Regular);
						rtfDestination.SelectionColor = Color.DarkGreen;

						rtfDestination.SelectedText = line + Environment.NewLine;
					} else {
						// We have to go through extra work to tokenize this line, and search
						// for individual tokens to highlight....
						tokens = r.Split(line);

						foreach (string token in tokens) {
							if (token != String.Empty) {
								// Set the tokens default color and font.
								if (rtfDestination.SelectionFont.Bold == true
									 || rtfDestination.SelectionColor != Color.FromArgb(96, 96, 96)) {
									rtfDestination.SelectionFont = new Font("Courier New", 9, FontStyle.Regular);
									rtfDestination.SelectionColor = Color.FromArgb(96, 96, 96);
								}

								// See if we're entering a multi-line comment
								if (token == "/*") {
									inMultiLineComment = true;
								}

								// Highlight the words in a multi-line comment as being in a comment
								if (inMultiLineComment) {
									rtfDestination.SelectionColor = Color.DarkGreen;
									//rtfDestination.SelectionFont = new Font("Courier New", 9, FontStyle.Regular);
								}

								// See if we're exiting a multi-line comment
								if (token == "*/") {
									inMultiLineComment = false;
								}

								if (inMultiLineComment == false) {
									// See if we're entering/exiting a string literal
									if (inSingleLineComment == false && token == "'") {
										if (inString) {
											// We're already in a string - he's the end of the string, if he's not doubled up @@@
											inString = false;
										} else {
											inString = true;
										}

										// Highlight the single quote as a reserved word
										rtfDestination.SelectionColor = Color.DarkBlue;
										rtfDestination.SelectionFont = new Font("Courier New", 9, FontStyle.Bold);
									} else {
										// Highlight reserved words, operators, etc., if we're not in a string
										if (inString == false) {
											if (token == "--") {
												inSingleLineComment = true;
											}

											if (inSingleLineComment) {
												rtfDestination.SelectionColor = Color.DarkGreen;
												//rtfDestination.SelectionFont = new Font("Courier New", 9, FontStyle.Regular);
											} else {
												if (token != " " && token != String.Empty) { // We're not going to be highlighting a space
													string PARSED_TOKEN = token.ToUpper().Trim();

													for (int j = 0; j < keywords.Length; j++) {
														if (keywords[j] == PARSED_TOKEN) {
															// Apply alternative color and font to highlight keyword.
															rtfDestination.SelectionColor = Color.DarkBlue;
															rtfDestination.SelectionFont = new Font("Courier New", 9, FontStyle.Bold);

															break;
														}
													}
												}
											}
										}
									}
								}

								rtfDestination.SelectedText = token;
							}
						}

						if (i < lines.Length-1)
							rtfDestination.SelectedText = Environment.NewLine;

						inSingleLineComment = false;
					}
				}
			}

			rtfDestination.Visible = true; // For speed

			//Program.outputForm.debugTimingWrite("Done highlighting");
		}
	}
}
