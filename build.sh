#!/bin/sh
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version latest -InstallDir ./dotnet
./dotnet/dotnet --version
./dotnet/dotnet publish -c Release -o output
./dotnet/dotnet run --configuration Release --project ./src/AnEoT.Vintage.Tool/AnEoT.Vintage.Tool.csproj remove-unnecessary-component --webroot-path ./src/AnEoT.Vintage/wwwroot
./dotnet/dotnet run --configuration Release --project ./src/AnEoT.Vintage/AnEoT.Vintage.csproj -- static-only --ConvertWebP true --urls "http://localhost:5048" --RssBaseUri "https://aneot-vintage.baka632.com/" --RssIncludeAllArticles true --RssAddCssStyle true