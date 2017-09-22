# LiquibaseNuGetPacker
Build script for creating nuget packages from and for liquibase releases.
It downloads a liquibase release from a given URL (use the tar.gz version), extracts the version number from the file name and 
creates a nuget package for the version. It can then push the package to nuget.org with the given api-key.

## Dependencies
The script is a cake build script, so it requires a windows machine with .NET Framework or a Linux machine with Mono installed.

## Targets
The most important targets for the script are: 
* Pack - Download the liquibase distribution from the given url and create the package
* Push - (Pack+) Push the created package to nuget.org

## Usage
Windows:
```powershell
./build.ps1 -Target Pack --url https://liquibase/download/url/file-version.tar.gz
```
or (for directly pushing after creating the package)
```powershell
./build.ps1 -Target Push --url https://liquibase/download/url/file-version.tar.gz --api-key "your-api-key"
```

Linux:
```bash
./build.sh --target=Pack ---url=https://liquibase/download/url/file-version.tar.gz
```
or (for directly pushing after creating the package)
```bash
./build.sh --target=Push ---url=https://liquibase/download/url/file-version.tar.gz --api-key="your-api-key"
```