@echo off

..\Tool\Data_PackTool.exe 1 source data "String Table.xlsx" StringData

xcopy *.cs ..\..\Program\Client\Assets\Scripts\data\ /Y/F
del *.cs

pause