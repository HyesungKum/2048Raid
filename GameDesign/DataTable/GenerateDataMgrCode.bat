@echo off

..\Tool\DataMgrGenerator.exe batMode GameDataFileList.txt TemplateDataMgr.txt GameStringDataFileList.txt

xcopy DataMgr*.cs ..\..\Program\Client\Assets\Scripts\Data\ /Y/F
del DataMgr*.cs

pause