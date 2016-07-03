dotnet-gitversion - Simple git versioning for dotnet cli
========================================================

**dotnet-gitversion** is a simple extension tool for [dotnet cli](https://github.com/dotnet/cli) that sets the version number based on git tags and branches.
It is based on the wonderful [GitVersion](https://github.com/GitTools/GitVersion).

## Usage

Reference **dotnet-gitversion** in your `project.json`:

```json
{
  "tools": {
    "dotnet-gitversion": {
      "version": "*",
      "imports": [
        "dotnet"
      ]
    }
  },
  "version": "0.0.0",
  "buildOptions": {
    "debugType": "portable",
    "emitEntryPoint": true
  },
  "frameworks": {
    "netcoreapp1.0": {
      "dependencies": {
        "Microsoft.NETCore.App": {
          "type": "platform",
          "version": "1.0.0"
        }
      }
    }
  }
}

```

Then run `dotnet restore` to fetch **dotnet-gitversion** and finally run `dotnet gitversion` to update the version field.
