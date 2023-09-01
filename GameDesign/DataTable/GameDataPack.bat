@echo off

..\Tool\Data_PackTool.exe 1 game "GameDataFileList.txt" "GameData.bytes"

xcopy *.bytes ..\..\Program\Client\Assets\Resources\GameData\ /Y/F
del *.bytes

pause