@echo off
setlocal
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%CSC%" (
  echo .NET Framework compiler not found. Install .NET Framework developer tools.
  exit /b 1
)
"%CSC%" /nologo /target:winexe /out:"%~dp0MatrixCode.scr" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll "%~dp0src\MatrixCode.cs"
exit /b %errorlevel%
