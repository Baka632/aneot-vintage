#!/bin/sh
# 假定运行环境已经安装了 dotnet

# dotnet run --configuration Release \
#  --project ./src/AnEoT.Vintage.Tool/AnEoT.Vintage.Tool.csproj remove-unnecessary-component \
#  --webroot-path ./src/AnEoT.Vintage/wwwroot

dotnet run --configuration Release \
 --project ./src/AnEoT.Vintage/AnEoT.Vintage.csproj \
 -- static-only \
 --urls "http://localhost:5048" \
 --BaseUri "http://aneot-vintage.arktca.com/"

dotnet publish ./src/AnEoT.Vintage.StaticServer/AnEoT.Vintage.StaticServer.csproj \
 -r linux-x64 \
 --self-contained