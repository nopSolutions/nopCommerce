@IF NOT EXIST %windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe @ECHO COULDN'T FIND MSBUILD: %windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe (Is .NET 4 installed?)

%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe nop.proj /p:DebugSymbols=false /p:DebugType=None /maxcpucount /l:FileLogger,Microsoft.Build.Engine;logfile=log.log %*