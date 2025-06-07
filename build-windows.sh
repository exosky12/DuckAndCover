#!/bin/bash

echo "Build Windows x64..."
dotnet publish DuckAndCover/DuckAndCover/DuckAndCover.csproj \
    -c Release \
    -p:BUILD_MAUI=true \
    -f net8.0-windows10.0.19041.0 \
    -r win-x64 \
    --self-contained true \
    -p:PublishTrimmed=true \
    -p:TrimMode=link \
    -p:EnableCompressionInSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o publish/windows

echo "Build Windows terminé !" 