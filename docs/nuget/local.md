# Building Packages Locally

## PackAndMove
Each project in this repository includes a `PackAndMove` target (see
Directory.Build.targets), which will compile and package the project as a NuGet package
and then copy the output to a configurable directory (\packages by default). You can
`PackAndMove` a single project by changing to its directory and running `dotnet build
-t:PackAndMove`, or you can run the same command from the root of the repository to pack
and move all packages in the repo.

The intent of PackAndMove is to be a convenience when building packages locally. By
default, `dotnet pack` scatters its output into many nested subdirectories like
`\identity-model\src\IdentityModel\bin\Debug\`. Moving all the packages into a single
location makes it quick to access them and allows you to easily consume the package from a
test or sample project.

### Configuring Output Dir for PackAndMove
To configure the output directory, either set the `DuendePackageDir` environment variable,
or pass the `DuendePackageDir` property to the build command:

#### Environment Variable
```ps
# powershell
$Env:DuendePackageDir=C:\packages
dotnet build -t:PackAndMove
```

#### Command Line Argument
```
dotnet build -t:PackAndMove -p:DuendePackageDir=C:\packages
```


## Consuming Local Packages
If you want to try out local builds of your packages, you can add a directory (such as the
directory you use for PackAndMove) as a package source to NuGet. To do so, add the feed to
your NuGet.Config file. You can do this for your entire user account or on a per-project
basis. On windows, you personal config file is located at %appdata%\NuGet\NuGet.Config.
More details from Microsoft's docs
[here](https://learn.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior). 

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Local Builds" value="C:\packages" />
    <!-- Other sources omitted -->
  </packageSources>
</configuration>
```

You can also set this configuration via most IDEs.