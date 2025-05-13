@echo off
set "folderPath=D:\Source\Repos\Dotnet\ReaxLang\tests\Reax.Tests\TestResults\"

if exist "%folderPath%" (
    rmdir /s /q "%folderPath%"
    echo All files in "%folderPath%" have been deleted.
) else (
    echo The folder "%folderPath%" does not exist.
)

set "folderPath=D:\Source\Repos\Dotnet\ReaxLang\tests\Reax.Tests\coveragereport\"

if exist "%folderPath%" (
    rmdir /s /q "%folderPath%"
    echo All files in "%folderPath%" have been deleted.
) else (
    echo The folder "%folderPath%" does not exist.
)

dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"D:\Source\Repos\Dotnet\ReaxLang\tests\Reax.Tests\TestResults\*\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
pause