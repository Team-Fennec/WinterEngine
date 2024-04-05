<#
    C# Macros

    (C) 2023-2024 K. "ashi/eden" J.
#>
using namespace System.Collections.Generic;
using namespace System.Text.RegularExpressions;

class MacroData
{
    [string]$Name;
    [string]$Contents;
    [List[string]]$Parameters;

    MacroData($name, $contents, $params)
    {
        $this.Name = $name;
        $this.Contents = $contents;
        $this.Parameters = $params;
    }
}

$macros = New-Object List[MacroData];

function ReadFile([string]$path)
{
    $wd = Get-Location;
    [string]$fileContents = [System.IO.File]::ReadAllLines([System.IO.Path]::Combine($wd, $path));
    [Console]::WriteLine($fileContents);

    # foreach ($line in $fileContents)
    # {
    #     if ($line.StartsWith('#macro'))
    #     {
    #         $macroDef = [Regex]::Replace($line, "#macro\s+", "");

    #         [string]$macroName;
    #         [string]$macroContents;
    #         [List[string]]$macroParameters;

    #         # Loop through contents
    #         for ($i = 0; $i -lt $macroDef.Length; $i++) {
    #             $c = $macroDef[$i];

    #             # check the value of the character

    #         }
    #     }
    # }
}

ReadFile "Engine.cs";
