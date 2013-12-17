MSBuild data\TimeVault.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=bin\Release

IF "%1" == "" (
  goto :CONFUSE
) ELSE (
  goto :END
)

:CONFUSE
tools\confuser\Confuser.Console.exe -preset Maximum -input data\bin\Release\TimeVault.exe -output out
rm -f data\bin\Release\TimeVault.exe

:END