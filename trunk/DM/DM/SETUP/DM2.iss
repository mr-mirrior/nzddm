; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
AppName=大坝填筑质量GPS监控系统
AppVerName=大坝填筑质量GPS监控系统
DefaultDirName={pf}\大坝填筑质量GPS监控系统
DefaultGroupName=大坝填筑质量GPS监控系统
OutputDir=.\
OutputBaseFilename=SETUP 2-25 debug
Compression=lzma
SolidCompression=yes
SetupIconFile=..\DM21.ico

[Languages]
Name: "Chinese"; MessagesFile: "compiler:Languages\Chinese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
;Source: "..\bin\release\HVMRuntm.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\debug\DM_Secure\DM.exe"; DestDir: "{app}"; Flags: ignoreversion
source: "TrackPoint.wav"; destdir:"{app}"; flags: ignoreversion
Source: "..\bin\debug\difference.config"; DestDir: "{app}"; Flags: ignoreversion
;Source: ".\TrackingExp.txt"; DestDir: "{app}"; Flags: ignoreversion
;Source: "C:\DAMDATA\yingdi\*"; DestDir: "{app}\DAMDATA\yingdi"; Flags: ignoreversion recursesubdirs
;Source: "..\bin\debug\db.config"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
;Source: "msyh.TTF"; DestDir: "{fonts}"; FontInstall: "微软雅黑"; Flags: onlyifdoesntexist uninsneveruninstall


[Icons]
Name: "{group}\大坝填筑质量GPS监控系统"; Filename: "{app}\dm.exe"
Name: "{commondesktop}\大坝填筑质量GPS监控系统"; Filename: "{app}\dm.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\DM2"; Filename: "{app}\dm.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\dm.exe"; Description: "{cm:LaunchProgram,大坝填筑质量GPS监控系统}"; Flags: nowait postinstall skipifsilent

