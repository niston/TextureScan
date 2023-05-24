**WHAT IS THIS**

This tool will scan a folder (including all subfolders) for DDS texture files with abnormal dimensions. It allows you to find textures in your folders that have the potential to crash your game or 3D application. Also, it goes _very_  fast if your harddrive can keep up.


**HOW TO USE**

- Requires [Microsoft .NET Framework 4.7.1](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net471) or higher
- Exctract and run the executable
- Select scan folder with textures in it
- Hit GO button
- Observe
- Doubleclick on any listed texture file to open it with your default DDS viewer/editor program


**HOW DOES IT WORK**

- The tool finds all .DDS files in the given path and reads the DDS header of each file.
- If the cubemap bit flag is set in the file, it will validate that the texture dimensions are multiples of 4.
- If the cubemap bit flag is not set, it will validate that the texture dimensions be powers of two.
- If the file doesn't have a DDS header, it'll list it as INVALID
- If the file has a malformed DDS header, it'll list it as CORRUPTED


**NOTE**

Not every texture it lists will necessarily crash the game. For example, background screens will not be powers of two sized. Use your own judgement.
