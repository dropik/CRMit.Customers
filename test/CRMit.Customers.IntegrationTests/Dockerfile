FROM mcr.microsoft.com/dotnet/sdk:5.0
COPY . .
WORKDIR /test/CRMit.Customers.IntegrationTests
RUN dotnet restore "CRMit.Customers.IntegrationTests.csproj"
ENTRYPOINT ["dotnet", "test", "-c", "Development", "--results-directory", "/testresults", "--logger", "trx;LogFileName=results.trx"]