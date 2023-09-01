@echo off

..\Tool\Data_PackTool.exe 1 enum "EnumList Table.xlsx" GameDataEnumList
		
..\Tool\Data_PackTool.exe 1 source data "Character Table.xlsx" CharacterData
..\Tool\Data_PackTool.exe 1 source data "UserProperty Table.xlsx" UserPropertyData
..\Tool\Data_PackTool.exe 1 source data "Weapon Table.xlsx" WeaponData
..\Tool\Data_PackTool.exe 1 source data "Armor Table.xlsx" ArmorData
..\Tool\Data_PackTool.exe 1 source data "Helmet Table.xlsx" HelmetData
..\Tool\Data_PackTool.exe 1 source data "Boots Table.xlsx" BootsData
..\Tool\Data_PackTool.exe 1 source data "Skill Table.xlsx" SkillData
..\Tool\Data_PackTool.exe 1 source data "Pet Table.xlsx" PetData
..\Tool\Data_PackTool.exe 1 source data "Relic Table.xlsx" RelicData
..\Tool\Data_PackTool.exe 1 source data "RandomOption Table.xlsx" RandomOptionData

..\Tool\Data_PackTool.exe 1 source data "ExpValue Table.xlsx" ExpValueData
..\Tool\Data_PackTool.exe 1 source data "NPC Table.xlsx" NPCData

..\Tool\Data_PackTool.exe 1 source data "Quest Table.xlsx" QuestData
..\Tool\Data_PackTool.exe 1 source data "TutorialQuest Table.xlsx" TutorialQuestData

..\Tool\Data_PackTool.exe 1 source data "NormalDungeon Table.xlsx" NormalDungeonData
..\Tool\Data_PackTool.exe 1 source data "GoodsDungeon Table.xlsx" GoodsDungeonData
..\Tool\Data_PackTool.exe 1 source data "BossDungeon Table.xlsx" BossDungeonData
..\Tool\Data_PackTool.exe 1 source data "ChasingDungeon Table.xlsx" ChasingDungeonData
..\Tool\Data_PackTool.exe 1 source data "DailyDungeonMode Table.xlsx" DailyDungeonModeData
..\Tool\Data_PackTool.exe 1 source data "DailyDungeonModeReward Table.xlsx" DailyDungeonModeRewardData
..\Tool\Data_PackTool.exe 1 source data "RankingReward Table.xlsx" RankingRewardData
..\Tool\Data_PackTool.exe 1 source data "ArenaModeReward Table.xlsx" ArenaModeRewardData

..\Tool\Data_PackTool.exe 1 source data "Cash Table.xlsx" CashData
..\Tool\Data_PackTool.exe 1 source data "ShopProbability Table.xlsx" ShopProbabilityData
..\Tool\Data_PackTool.exe 1 source data "Shop Table.xlsx" ShopData
..\Tool\Data_PackTool.exe 1 source data "ShopPackage Table.xlsx" ShopPackageData

..\Tool\Data_PackTool.exe 1 source data "EventAttend Table.xlsx" EventAttendData
..\Tool\Data_PackTool.exe 1 source data "EventConnection Table.xlsx" EventConnectionData
..\Tool\Data_PackTool.exe 1 source data "EventDailyPass Table.xlsx" EventDailyPassData
..\Tool\Data_PackTool.exe 1 source data "EventGrowthPass Table.xlsx" EvenntGrowthPassData

xcopy *.cs ..\..\Program\Client\Assets\Scripts\data\ /Y/F
del *.cs

pause