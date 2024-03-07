using log4net;
using System.Text;

namespace WinterEngine.Resource;

public class ShaderResource {
    public string ShaderName { get; private set; }
    public string FragmentCode { get; private set; }
    public string VertexCode { get; private set; }

    public ShaderResource(string name) {
        ShaderName = name;
        StreamReader shaderFile = ResourceManager.OpenResource(Path.Combine("shaders", $"{name}.glsl"));

        int writerMode = 0; // 0 - global, 1 - vertex, 2 - fragment
        FragmentCode = "";
        VertexCode = "";

        string vtxOut = "";
        string frgOut = "";
        
        // load code into strings
        while (true) {
            string line = shaderFile.ReadLine();
            if (line == null) break;
            
            if (line == "VERTEX:") {
                writerMode = 1;
                continue;
            } else if (line == "FRAGMENT:") {
                writerMode = 2;
                continue;
            }

            switch (writerMode) {
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
    }

    string ParseShaderCode(string input) {
        string output = "";

        using (StringReader reader = new StringReader(input)) {
            string line = reader.ReadLine();
            while (line != null) {
                if (line.StartsWith("#include")) {
                    // parse out include and load it's code
                    string inclFilename = line.Split(" ")[1].Trim("\"".ToCharArray(0, 1));
                    StreamReader inclFile = ResourceManager.OpenResource(Path.Combine("shaders", "include", inclFilename));
                    string inclLine = inclFile.ReadLine();
                    while (inclLine != null) {
                        output += $"{inclLine}\n";
                        inclLine = inclFile.ReadLine();
                    }
                    inclFile.Close();
                } else {
                    output += $"{line}\n";
                }

                line = reader.ReadLine();
            }
        }
        return output;
    }
}
