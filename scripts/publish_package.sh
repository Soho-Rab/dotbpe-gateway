set -ex

cd $(dirname $0)/../src/

artifactsFolder="../artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder



versionNumber="1.2.2"

dotnet pack ./DotBPE.AspNetGateway/DotBPE.AspNetGateway.csproj -c Release -o ../$artifactsFolder  --version-suffix=$versionNumber



dotnet nuget push ./$artifactsFolder/DotBPE.AspNetGateway.${versionNumber}.nupkg -k $NUGET_KEY -s https://www.nuget.org


