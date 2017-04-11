'Read the results file into a variable
Dim fso, resultsFile
Set fso = CreateObject("Scripting.FileSystemObject")
Set resultsFile = fso.OpenTextFile("C:\Temp\runResults.html", 1)
resultsHTML = resultsFile.ReadAll

Set objEmail = CreateObject("CDO.Message")
objEmail.From = "noreply@pcci.edu"
objEmail.To = "it-pccprogramming-g@pcci.edu"
objEmail.Subject = "Failed RT Tests"
objEmail.HTMLBody = resultsHTML

objEmail.Configuration.Fields.Item ("http://schemas.microsoft.com/cdo/configuration/sendusing") = 2
objEmail.Configuration.Fields.Item ("http://schemas.microsoft.com/cdo/configuration/smtpserver") = "exchange2.pcci.edu"
objEmail.Configuration.Fields.Item ("http://schemas.microsoft.com/cdo/configuration/smtpserverport") = 25
objEmail.Configuration.Fields.Update

objEmail.Send
