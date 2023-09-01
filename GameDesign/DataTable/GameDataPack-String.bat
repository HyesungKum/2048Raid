@echo off

..\Tool\Data_PackTool.exe 1 gameByUnicodeText "GameStringDataFileList.txt" "StringData.bytes"

xcopy *.bytes ..\..\Program\Client\Assets\Resources\GameData\ /Y/F
del *.bytes
del "String Table.xlsx.csv"

pause