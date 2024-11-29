# garlic
## For Grade 12 Computer Science
This is a smart fridge UI that displays the time (in either a 12-hour or 24-hour format), and the weather (with degrees in either fahrenheit or celsius for degrees).
It also displays news from The Onion's RSS feed. You're sure to stay informed if you use garlic.

*Note for teacher: Much of the project is just a template. See **MainWindow.axaml.cs, MainWindow.axaml,** **Weather.cs,** and **StrToFloat.cs** for the stuff I coded personally.*

## Compatibility
Works on Windows and Linux. Developed specifically on/for Fedora Linux.

## Dependencies
- .NET 9.0
-   Earlier .NET versions work too, but garlic.csproj needs to be updated to target earlier .NET versions.

## Installation
Run the following commands to install the program.
```bash
git clone https://github.com/LobsterRoast/garlic.git
cd garlic
make # If you don't have make installed, run the dotnet publish command inside the Makefile
```
After installing, run the executable in the build directory. If the exe doesn't work, open the terminal and run the following:
```bash
dotnet garlic.dll
```
**The Windows executable doesn't like running from file explorer for some reason, so even if you're running the .exe rather than the .dll, you must run it from the command line.**


**Note:** This targets  .NET 9.0 by default. You may need to edit garlic.csproj to target a different version of .NET depending on what works for you and your environment.

## Resources used:
- [Avalonia](https://avaloniaui.net/)
- [ipinfo.io](https://ipinfo.io/)
- [Open-Meteo](https://open-meteo.com/)
- [fluentui-emoji](https://github.com/microsoft/fluentui-emoji)
- [Google Noto Sans](https://fonts.google.com/noto/specimen/Noto+Sans)
- [The Onion](https://theonion.com/)
