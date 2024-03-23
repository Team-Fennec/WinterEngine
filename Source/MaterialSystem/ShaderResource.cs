using WinterEngine.RenderSystem;

namespace WinterEngine.Resource;

public class ShaderResource : IResource
{
#pragma warning disable
    public string ShaderName { get; private set; }
    public string FragmentCode { get; private set; }
    public string VertexCode { get; private set; }

    public CullMode CullMode;
    public bool DepthTest;
#pragma warning restore

    public void LoadData(Stream stream)
    {
        StreamReader shaderFile = new StreamReader(stream);

        int writerMode = 0; // 0 - global, 1 - vertex, 2 - fragment
        FragmentCode = "";
        VertexCode = "";

        string vtxOut = "#define VERTEX_SHADER\n";
        string frgOut = "#define FRAGMENT_SHADER\n";

        // load code into strings
#pragma warning disable
        while (true)
        {
            string line = shaderFile.ReadLine();
            if (line == null)
                break;

            if (line == "VERTEX:")
            {
                writerMode = 1;
                continue;
            }
            else if (line == "FRAGMENT:")
            {
                writerMode = 2;
                continue;
            }
            else if (line.StartsWith("#cull_mode"))
            {
                string mode = line.Split(" ")[1];
                // we aren't worried about performance here.
                if (!Enum.TryParse($"CullMode.{mode}", out CullMode))
                {
                    throw new ArgumentException(
                        $"Unexpected CullMode value in shader!\n" +
                        $"Got {mode}, Expected Back, Front, or None."
                    );
                }
                continue;
            }
            else if (line.StartsWith("#depth_test"))
            {
                DepthTest = bool.Parse(line.Split(" ")[1]);
                continue;
            }

            switch (writerMode)
            {
                case 0:
                    vtxOut += $"{line}\n";
                    frgOut += $"{line}\n";
                    break;
                case 1:
                    vtxOut += $"{line}\n";
                    break;
                case 2:
                    frgOut += $"{line}\n";
                    break;
            }
        }
        VertexCode = ParseShaderCode(vtxOut);
        FragmentCode = ParseShaderCode(frgOut);

        shaderFile.Close();
#pragma warning disable
    }

    string ParseShaderCode(string input)
    {
        string output = "";

        using (StringReader reader = new StringReader(input))
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("#include"))
                {
                    // parse out include and load it's code
                    string inclFilename = line.Split(" ")[1].Trim("\"".ToCharArray(0, 1));
                    Stream inclFileData = ResourceManager.GetData(Path.Combine("shaders", "include", inclFilename));
                    StreamReader inclFile = new StreamReader(inclFileData);

                    string inclConts = "";
                    string inclLine = inclFile.ReadLine();
                    while (inclLine != null)
                    {
                        inclConts += $"{inclLine}\n";
                        inclLine = inclFile.ReadLine();
                    }
                    inclFile.Close();
                    output += $"{ParseShaderCode(inclConts)}";
                }
                else
                {
                    output += $"{line}\n";
                }

                line = reader.ReadLine();
            }
        }
        return output;
    }
}
