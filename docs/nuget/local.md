# Building Packages Locally

You can build a package locally with `dotnet pack`. Run that from a project directory to
create a single package, or from the root of the repo to package everything in the
solution. The -o flag controls where the packages are output to.




## Consuming Local Packages
If you want to try out local builds of your packages, you can add a directory as a package
source to NuGet. To do so, add the feed to your NuGet.Config file. You can do this for
your entire user account or on a per-project basis. On windows, you personal config file
is located at %appdata%\NuGet\NuGet.Config. More details from Microsoft's docs
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