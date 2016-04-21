rem Edit this number to change the version number for the generated assembly
set VersionNumber=1.0.2

rem This is the version number used in the ClickOnce setup.
rem ClickOnce requires the version number to have four digits, so if the "real" VersionNumber (above) is set 
rem to a value that DOES NOT HAVE EXACTLY TWO DIGITS, the ClickOnceVersionNumber needs to be changed as well.
set ClickOnceVersionNumber=%VersionNumber%.0.0