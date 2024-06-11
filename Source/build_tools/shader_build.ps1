<#
    WinterEngine Shader Compiler

    Used for compiling WinterEngine shaders down to spirv byte code.

    While the engine is capable of doing this at runtime, it may be
    desired to precompile down for performance, or other reasons.

    2024 K. 'ashi/eden' J.
#>
using namespace System;
using namespace System.IO;

param (
    [Parameter(Mandatory=$true)][string]$shader = "",
    [Parameter(Mandatory=$false)][string]$outputDir = ""
)

$includeDir = "";
$shDepthMode = "false";
$shCullMode = "None";
$ScriptDir = Split-Path -Parent $PSCommandPath;

function CompileShader([string]$name)
{
    if ((Test-Path $name) -eq $false)
    {
        throw "Couldn't compile shader: file '$name' not found.";
    }

    $shaderFile = Get-Content $name;

    $writerMode = 0; # 0 - global, 1 - vertex, 2 - fragment

    $vtxOut = "#define VERTEX_SHADER 1`n";
    $frgOut = "#define FRAGMENT_SHADER 1`n";

    foreach ($line in $shaderFile)
    {

        if ($line -eq "VERTEX:")
        {
            $writerMode = 1;
            continue;
        }
        elseif ($line -eq "FRAGMENT:")
        {
            $writerMode = 2;
            continue;
        }
        elseif ($line.StartsWith("#version"))
        {
            if ($writerMode -eq 0 -Or $writerMode -eq 1)
            {
                $vtxOut = "$line`n$vtxOut";
            }
            if ($writerMode -eq 0 -or $writerMode -eq 2)
            {
                $frgOut = "$line`n$frgOut";
            }
            continue;
        }
        elseif ($line.StartsWith("#cull_mode"))
        {
            $shCullMode = $line.split(" ")[1];
            
            continue;
        }
        elseif ($line.StartsWith("#depth_clip") -or $line.StartsWith("#depth_test"))
        {
            $shDepthMode = $line.split(" ")[1];
            continue;
        }

        if ($writerMode -eq 1 -or $writerMode -eq 0)
        {
            $vtxOut += "$line`n";
        }

        if ($writerMode -eq 2 -or $writerMode -eq 0)
        {
            $frgOut += "$line`n";
        }
    }

    # Get shader base path so we know where to look for includes
    $includeDir = [Path]::GetDirectoryName($name);

    $VertexCode = ParseShaderCode $vtxOut;
    $FragmentCode = ParseShaderCode $frgOut;

    [Console]::WriteLine("Compiling intermediary shader code...");
    Set-Content "temp_vert_shd.glsl" $VertexCode;
    Set-Content "temp_frag_shd.glsl" $FragmentCode;

    $shaderName = [Path]::GetFileNameWithoutExtension($name);
    $validatorPath = [Path]::Combine($ScriptDir, "glslangValidator");

    $full_file_path = [Path]::Combine($outputDir, $shaderName);

    [Console]::WriteLine("Compiling vertex shader code...");
    & $validatorPath -V "temp_vert_shd.glsl" -o "$full_file_path.vtx.spv" -S vert
    [Console]::WriteLine("Compiling fragment shader code...");
    & $validatorPath -V "temp_frag_shd.glsl" -o "$full_file_path.frg.spv" -S frag

    [Console]::WriteLine("Writing shader info...");
    Set-Content "$full_file_path.json" "{
`"cull_mode`": `"$shCullMode`",
`"depth_mode`": `"$shDepthMode`"
}";

    [Console]::WriteLine("Cleaning up...");
    Remove-Item "temp_frag_shd.glsl";
    Remove-Item "temp_vert_shd.glsl";
}

function ParseShaderCode([string]$code)
{
    $output = "";

    $contentList = $code.split("`n");
    foreach ($line in $contentList)
    {
        if ($line.StartsWith("#include"))
        {
            # parse out include and load it's code
            [string]$inclFilename = $line.replace("#include ", "").replace('"', "");

            [string]$inclConts = "";
            $inclPath = [Path]::Combine($includeDir, "include", $inclFilename);
            $inclFile = Get-Content $inclPath;
            foreach ($inclLine in $inclFile)
            {
                $inclConts += "$inclLine`n";
            }
            $output += ParseShaderCode $inclConts;
        }
        else
        {
            $output += "$line`n";
        }
    }

    return $output;
}

[Console]::WriteLine("===========================================");
[Console]::WriteLine("     WinterEngine Shader Compiler v1.0     ");
[Console]::WriteLine("===========================================");
[Console]::WriteLine("Compiling shader $shader");

CompileShader $shader;
