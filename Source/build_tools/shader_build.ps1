<#
    WinterEngine Shader Compiler

    Used for compiling WinterEngine shaders down to spirv byte code.

    While the engine is capable of doing this at runtime, it may be
    desired to precompile down for performance, or other reasons.

    2024 K. 'ashi/eden' J.
#>

param (
    [Parameter(Mandatory=$true)][string]$shader
)

$shDepthMode = "false";
$shCullMode = "None";
$ScriptDir = Split-Path -Parent $PSCommandPath;

function CompileShader([string]$name)
{
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
        elseif ($line -match '^#version')
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
        elseif ($line -match '^#cull_mode')
        {
            $shCullMode = $line.split(" ")[1];
            
            continue;
        }
        elseif ($line -match '^#depth_clip' -or $line -match '^#depth_test')
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

    $VertexCode = ParseShaderCode $vtxOut;
    $FragmentCode = ParseShaderCode $frgOut;

    [System.Console]::WriteLine("Compiling intermediary shader code...");
    Set-Content "temp_vert_shd.glsl" $VertexCode;
    Set-Content "temp_frag_shd.glsl" $FragmentCode;

    $shaderName = [System.IO.Path]::GetFileNameWithoutExtension($name);
    $validatorPath = [System.IO.Path]::Combine($ScriptDir, "glslangValidator");

    [System.Console]::WriteLine("Compiling vertex shader code...");
    & $validatorPath -V "temp_vert_shd.glsl" -o "$shaderName.vtx.spv" -S vert
    [System.Console]::WriteLine("Compiling fragment shader code...");
    & $validatorPath -V "temp_frag_shd.glsl" -o "$shaderName.frg.spv" -S frag

    [System.Console]::WriteLine("Cleaning up...");
    [System.IO.File]::Delete("temp_vert_shd.glsl");
    [System.IO.File]::Delete("temp_frag_shd.glsl");
}

function ParseShaderCode([string]$code)
{
    $output = "";

    $contentList = $code.split("`n");
    foreach ($line in $contentList)
    {
        if ($line -match '^#include')
        {
            # parse out include and load it's code
            [string]$inclFilename = $line.replace("#include ", "").replace('"', "");

            [string]$inclConts = "";
            $inclPath = [System.IO.Path]::Combine("include", $inclFilename);
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

[System.Console]::WriteLine("===========================================");
[System.Console]::WriteLine("     WinterEngine Shader Compiler v1.0     ");
[System.Console]::WriteLine("===========================================");
[System.Console]::WriteLine("Compiling shader $shader");

CompileShader $shader;
