#!/bin/bash
dotnet build ./src/Frontend -c Release --no-incremental
dotnet publish ./src/Frontend -c Release -o fe-release --no-build
cp ./fe-release/wwwroot/* ../../FishCard -r
touch ../../FishCard/.nojekyll