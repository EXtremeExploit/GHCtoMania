# GHCtoMania
# [Download](https://github.com/EXtremeExploit/GHCtoMania/releases/download/1.1.1/GHCtoMania.zip)
## About
This is a simple input converter for Guitars from the game Guitar Hero meant to be used with [osu!](https://osu.ppy.sh) in the mania gamemode.

### Supported operating systems:
- Windows 10 64-bit
- Windows 8.1 64-bit
- Windows 8 64-bit
- Windows 7 64-bit

## Supported Guitars
- Guitar Hero3 for PlayStation (R) 3

### My Guitar is unsupported
 * You can download the repository and debug the guitar yourself.
 * Don't ask me to add a guitar, make a pull request with the guitar propeties instead.


## Configuration
* The configuration file is located at the Data folder, it's name is config.cfg, there you will be able to edit wich keys presses any button, the Keys.txt file shows you each number that each key has. For more information check the [wiki](https://github.com/EXtremeExploit/GHCtoMania/wiki/Config_File).

## Contributing
- Check the [wiki](https://github.com/EXtremeExploit/GHCtoMania/wiki/Adding_guitar) on how a guitar is added, also pull requests are welcome

# Changelog
> **v1.1.1**
> * [[6e2ce16](https://github.com/EXtremeExploit/GHCtoMania/commit/6e2ce169f65733b0728f7afaa0bdd3042f8dec4a)]  **FIXED** Line breaks
> * [[6e2ce16](https://github.com/EXtremeExploit/GHCtoMania/commit/6e2ce169f65733b0728f7afaa0bdd3042f8dec4a)]   **ADDED** Guitar name, VID and PID,
> * [[6e2ce16](https://github.com/EXtremeExploit/GHCtoMania/commit/6e2ce169f65733b0728f7afaa0bdd3042f8dec4a)]   **IMPROVMENTS** Now the console in debug-mode is bigger with black background and white font
> * [[017beb6](https://github.com/EXtremeExploit/GHCtoMania/commit/017beb6089565eebcc1a629aa638fb4b9c910aea)] **FIXED** Lines not selecting correctly


> **v1.1.0**
> * Now the program does not use Z-Eval-Expression.NET (Doesn't require a license each month). By making this now you are able to use this at any time without any conflicts, also makes a huge optimization.
> * Deleted "Reload config" button, it was doing the same as Find Guitar.
> * Added a Debug-Mode, this allows you to see exactly what is the guitar sending to the program and with this you can add your own guitar. This is enabled adding -debug at the end of the program shortcut.
> * Added Auto-Hide, the programs starts minimized, usefull if you run the application when windows starts.
> * Optimizations on how the guitar state is recieved, instead of polling the guitar at 1000Hz, now works like a PS/2 port. *Less CPU work, less latency*.

> **v1.0.0**
> * Initial release with the executable file and its cfg files.