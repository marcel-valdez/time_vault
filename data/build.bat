@echo off

CD %~dp0

IF "%1" == "-test" (
  MSBuild TimeVault.Test\TimeVault.Test.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=bin\Release
  call TimeVault.Test\bin\Release\TimeVault.Test.exe
  goto :END
)

MSBuild TimeVault.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=bin\Release

IF "%1" == "-no-confuse" (
  goto :END
)

:CONFUSE
..\tools\confuser\Confuser.Console.exe -preset Maximum -input bin\Release\TimeVault.exe -output out
rm -f bin\Release\TimeVault.exe

:END
