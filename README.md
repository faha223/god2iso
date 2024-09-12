# God2Iso 

<a rel="license" href="http://creativecommons.org/licenses/by-sa/4.0/"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by-sa/4.0/80x15.png" /></a>

This work is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-sa/4.0/">Creative Commons Attribution-ShareAlike 4.0 International License</a>.

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

# Building from Source

You can build this from the command line on Windows, Mac, or Linux with the following command:
`dotnet build -c Release God2Iso.sln`

You can then run the application by navigating to God2Iso/bin/Release/net8.0 and entering the following command:
`dotnet God2Iso.dll`