# Regression Tester - PL/SQL
Copyright 2017 David Valles

This project is a GUI-based regression tester for Oracle PL/SQL code and is open source under the MIT License.

![Screenshot](https://github.com/millo-coog/rt-plsql/blob/master/files/Screenshot.png)

# Project Goals
* To contribute to and improve the quality and maturity of Oracle PL/SQL development and provide the PL/SQL language with a solid GUI-based regression tester.
* To make it easy to for PL/SQL programmers to do the right thing - that is, to regression test their code, and hard to do the wrong thing - that is, to make excuses for not regression testing.

# Get Started
1. Download the [latest version](https://github.com/millo-coog/rt-plsql/releases/latest).
1. Run the application.
1. Under the Preferences menu:
	- Set the Oracle Username for the target database.
	- Set the Oracle Password for the target database.
	- Set the Diff Executable to your favorite diff utility "C:\Program Files\TortoiseSVN\bin\TortoiseMerge.exe" (optional)
	- Set the XML Editor to your favorite text editor "C:\Program Files (x86)\Notepad++\notepad++.exe" (optional)
	- Set the PL/SQL Editor to your favorite text editor "C:\Program Files (x86)\Notepad++\notepad++.exe" (optional)
1. Hit Save.
1. Choose File -> New Project, and choose an empty folder to put your new project in.
1. You can then right-click on a PL/SQL function or procedure, and choose New Test, to start a new test.
