#!/bin/bash
#To make the .sh file executable
#sudo chmod +x ./clean.sh


#must give version as an argument
if [[ -z $1 ]]; then
    echo "dotnet version of target framework must be provided, e.g. 8.0";
    exit 1;
fi
# Property to modify
VERSION="$1"

#update all csproj files within the workspace to new .net version
cd ..
find . -type f -name "*.csproj" -print0 | xargs -0 sed -i '.bak' -r "s/(<TargetFramework>)net[0-9]+[.][0-9](<\/TargetFramework>)/\1net$VERSION\2/"

dotnet build
