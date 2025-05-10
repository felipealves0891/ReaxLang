dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet test --collect:"XPlat Code Coverage"

reportgenerator -reports:"D:\Source\Repos\Dotnet\ReaxLang\tests\Reax.Tests\TestResults\*\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html