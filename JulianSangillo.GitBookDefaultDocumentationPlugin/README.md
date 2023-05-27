# JulianSangillo.GitBookDefaultDocumentationPlugin

DefaultDocumentation plugin that organizes markdown output into a format that can be used with GitBook

### What's Included?

#### FolderFileNameFactory

Implementation of DefaultDocumentation's IFileNameFactory that returns the file path for a DocItem. Organizes the DocItems into subfolders.

#### TableOfContentsTask

A custom msbuild task that generates a Table of Contents (SUMMARY.md) file from the markdown file paths in the output directory and subdirectories. Below are examples of the properties that can be passed to this task.

##### OutputPath

```xml
<PropertyGroup>
    <TableOfContentsFolder>$(MSBuildProjectDirectory)\docs</TableOfContentsFolder>
</PropertyGroup>
```

### Download

#### PackageReference

```xml
<PackageReference Include="JulianSangillo.GitBookDefaultDocumentationPlugin" Version="<Version>" />
```

#### .NET CLI

```bash
dotnet add package JulianSangillo.GitBookDefaultDocumentationPlugin --version <Version>
```

#### Package Manager

```
NuGet\Install-Package JulianSangillo.GitBookDefaultDocumentationPlugin -Version <Version>
```
