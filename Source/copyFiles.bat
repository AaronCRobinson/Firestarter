@echo off
SET "ProjectName=Firestarter"
SET "SolutionDir=C:\Users\robin\Desktop\Games\RimWorld Modding\Source\Firestarter\Source"
SET "RWModsDir=D:\SteamLibrary\steamapps\common\RimWorld\Mods"
@echo on

del /S /Q "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\%ProjectName%\Defs\*"

xcopy /S /Y "%SolutionDir%\..\About\*" "%RWModsDir%\%ProjectName%\About\"
xcopy /S /Y "%SolutionDir%\..\Defs\*" "%RWModsDir%\%ProjectName%\Defs\"
xcopy /S /Y "%SolutionDir%\..\Patches\*" "%RWModsDir%\%ProjectName%\Patches\"
xcopy /S /Y "%SolutionDir%\..\Textures\*" "%RWModsDir%\%ProjectName%\Textures\"
xcopy /S /Y "%SolutionDir%\..\Languages\*" "%RWModsDir%\%ProjectName%\Languages\"