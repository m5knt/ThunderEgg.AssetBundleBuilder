@echo off
cd /d %~dp0
for /f "usebackq" %%t in (`cd`) do set "Project=%%t"
set "Unity3D=%ProgramFiles%/Unity 5.3.4p2"
echo Unity3D : %Unity3D%
echo Project : %Project%
start "" "%Unity3D%/Editor/Unity" -projectPath "%Project%"
ping -n 1 -w 5000 1.1.1.1 >nul
