# God2Iso 

<a rel="license" href="http://creativecommons.org/licenses/by-sa/4.0/"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by-sa/4.0/80x15.png" /></a>

This work is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-sa/4.0/">Creative Commons Attribution-ShareAlike 4.0 International License</a>.

## 1.1.1

***What's New?***

- Implemented the EnableUI button for 
- Added God2IsoCli, a command-line application for converting Games on Demand packages to ISO format without a GUI
- Added the ability to pass a CancellationToken into the ConvertPackage and ConvertPackages methods f GamesOnDemandConverter to cancel a conversion that's in progress
- Added the ability to cancel conversion in the God2Iso UI
- Added logic to disable the UI while a conversion job is in progress to prevent the user from harming the conversion while it's running

## v1.1.0

***What's new?***

- Updated the project from .NET Framework 3.5 to .NET 8
- Converted the UI from Windows Forms and WindowsAPICodePack to Avalonia UI
- Separated the logic for converting Games on Demand packages to ISO into a separate library
which can be used on its own

## v1.0.5

***What's new?***

- God2Iso is now open source, licensed under the Creative Commons Attribution-ShareAlike 4.0 International License.
- Adjusted copyright text to reflect Github repository and new licensing.
- Added assembly description.
- Use Path.Combine for better linux support (thanks to Megan Leet).
- This verison tagged (somewhat late) and binary version uploaded as requested.

## v1.0.4

***What's new?***

- Handle updating sector offsets for directory tables which span multiple sectors

# God2IsoCli Usage

`dotnet God2IsoCli.dll <arguments>`

### Arguments
##### -i, --include [option]
Specify a package to convert. Required. Can be used more than once to specify multiple packages. Must be followed by the path to the package which is to be included.
##### -o, --output_directory [option]
Specify an output directory. Optional. Must be followed by the path to the directory where the output files will go. If entered more than once then the last one is used. If not provided then this option defaults to the current directory.
##### -f, --fix_create_good_iso_header
Fix CreateGoodIso Header. Optional. Can only be used once. If not specified, this option defaults to false
##### -s, --silent
Silent. No output to stdout. Optional. If not specified, this option defaults to false
##### -h, --help
Show Usage
##### -v, --version
Show the Version information for God2IsoCli

# Building from Source

You can build this from the command line on Windows, Mac, or Linux with the following command:
`dotnet build -c Release God2Iso.sln`

You can then run the application by entering the following command:
`dotnet God2Iso/bin/Release/net8.0/God2Iso.dll`

Alternatively, you can run God2IsoCli by entering the following command:
`dotnet God2IsoCli/bin/Release/net8.0/God2IsoCli.dll`