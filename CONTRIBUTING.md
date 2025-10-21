# Contributing

- [Building the Project](#building-the-project)
  - [Installing Dependencies](#installing-dependencies)
  - [Building](#building)
  - [Formatting](#formatting)

# Submitting a New Patch
## Prerequisites
* Find a specific method in a mod that can be optimized.
* Create a PR that fixes that specific issue and submit it for that mod. This way
  we can track when issues get fixed upstream and remove them KSPMCF. As a bonus,
  this helps the KSP ecosystem improve, one PR at a time.

Once you have done that you can create a PR to add your patch!

> [!INFO]
> There are some exceptions to the rule about creating a PR before creating a
> patch. The current list of exceptions is:
> - MechJeb2 CKAN version. The dev branch of mechjeb has been being developed
>   for multiple years since the last release so backports of changes to the
>   CKAN version do not need a PR to the mechjeb repo.
>
> If you have another reason that you think there's no reason to submit a PR
> with your patch then please include an explanation in your PR to KSPMCF.

## Writing a Patch
* We use Harmony2 in order to patch methods at runtime. Check out
  [the harmony documentation](https://harmony.pardeike.net/articles/intro.html)
  in order to see what you can do.
* Each mod has its own project. If you want to add a patch for a mod that
  doesn't have any yet then:
  1. Copy an existing project (e.g. `KSPCommunityModFixes.MechJeb2`) and rename
     it to `KSPCommunityModFixes.<ModName>`.
  2. Remove all source files from the new project.
  3. Replace any mod dependencies with the one for your new mod.
  4. Create a new file and start writing your patch class.

When writing a patch make sure to follow these conventions:
* Each patch goes into its own class.
* Patch classes are named `<TargetType>_<Method>_<Patch>`.

# Building the Project
## Installing Dependencies
KSPCommunityModFixes needs a whole bunch of other mods installed to build. It
isn't really useful to have you install these into a KSP install, so we have
provided a script that takes care of that for you.

On Windows, run
```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\setup-deps.ps1 -ckan path\to\ckan.exe
```

On Linux/MacOS run
```bash
./setup-deps.sh --ckan command/to/run/ckan
```

These must be run in the repository root. They will create some fake ckan
installs under `deps/installs` and install the relevant mods within.


## Building
In order to build the mod you will need:
- the `dotnet` CLI

Next, you will want to create a `KSPCommunityModFixes.props.user` file
in the repository root, like this one:
```xml
<?xml version="1.0" encoding="UTF-8"?>

<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <ReferencePath>$KSP_ROOT_PATH</ReferencePath>
    </PropertyGroup>
</Project>
```

Make sure to replace `$KSP_ROOT_PATH` with the path to your KSP installation.
If you have an install made via steam then you might be able to skip this step.

Finally, you can build by running either:
- `dotnet build` (for a debug build), or,
- `dotnet build -c Release` (for a release build)

This will create a `GameData\KSPCommunityModFixes` folder which you can then drop
into your KSP install's `GameData` folder.

> ### Linking the output into your `GameData` folder
> If you're iterating on patches/code/whatever then you'll find that manually
> copying stuff into the `GameData` folder will get old really quickly. You can
> instead create a junction (on windows) or a symlink (on mac/linux) so that
> KSP will just look into the build artifact directory.
>
> To do this you will need to run the following command in an admin `cmd.exe`
> prompt (for windows) in your `GameData` directory:
> ```batch
> mklink /j KSPCommunityModFixes C:\path\to\KSPCommunityModFixes\repo\GameData\KSPCommunityModFixes
> ```
>
> On Linux or MacOS you should be able to accomplish the same thing using `ln`.

## Formatting
We use `csharpier` to format C# code. You can first install it by running
```sh
dotnet tool restore
```

Once you have done that you can then format everything by running
```sh
dotnet csharpier format .
```
