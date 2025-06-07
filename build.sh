#!/bin/bash

PROJECT_PATH="DuckAndCover/DuckAndCover/DuckAndCover.csproj"

echo "Nettoyage des anciens fichiers..."
rm -rf DuckAndCover/DuckAndCover/bin
rm -rf DuckAndCover/DuckAndCover/obj
rm -rf DuckAndCover/DuckAndCover/obj/project.assets.json

echo "Restauration des packages..."
dotnet restore "$PROJECT_PATH" --force

echo "Build macOS (Mac Catalyst)..."
dotnet publish "$PROJECT_PATH" -c Release \
    -p:BUILD_MAUI=true \
    -f net8.0-maccatalyst \
    -r maccatalyst-x64 \
    --self-contained true \
    -p:PublishTrimmed=true \
    -p:TrimMode=link \
    -p:EnableCompressionInSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o publish/macos

echo "Build Windows x64..."
if [ -f "build-windows.sh" ]; then
    chmod +x build-windows.sh
    ./build-windows.sh
else
    echo "Le script build-windows.sh n'existe pas. Construction de Windows ignorée."
fi

echo "Compression des builds..."
cd publish
if [ -d "macos" ]; then
    echo "Compression de la version macOS..."
    tar -czf macos.tar.gz macos/
fi
if [ -d "windows" ]; then
    echo "Compression de la version Windows..."
    tar -czf windows.tar.gz windows/
fi
cd ..

echo "Build terminé !"
