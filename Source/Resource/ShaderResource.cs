using Hjson;

namespace WinterEngine.Resource;

public class ShaderResource {
    public string FragmentCode;
    public string VertexCode;

    public ShaderResource(string name) {
        StreamReader shaderFile = ResourceManager.OpenResource(Path.Combine("shaders", $"{name}.glsl"));
        
        bool vertexMode = false;
        FragmentCode = "";
        VertexCode = "";
        
        string vtxOut = "";
        string frgOut = "";
        
        string line = "";
        // load code into strings
        while (true) {
            line = shaderFile.ReadLine();
            if (line == null) break;
            
            if (line == "VERTEX:") {
                vertexMode = true;
                continue;
            } else if (line == "FRAGMENT:") {
                vertexMode = false;
                continue;
            }
            
            if (vertexMode) {
                vtxOut += $"{line}\n";
            } else {
                frgOut += $"{line}\n";
            }
        }
        
        // parse shader code
        using (StringReader reader = new StringReader(vtxOut)) {
            line = reader.ReadLine();
            while (line != null) {
                if (line.StartsWith("#include")) {
                    // parse out include and load it's code
                    string inclFilename = line.Split(" ")[1].Trim("\"".ToCharArray(0, 1));
                    StreamReader inclFile = ResourceManager.OpenResource(Path.Combine("shaders", "include", inclFilename));
                    string inclLine = inclFile.ReadLine();
                    while (inclLine != null) {
                        VertexCode += $"{inclLine}\n";
                        inclLine = inclFile.ReadLine();
                    }
                    inclFile.Close();
                } else {
                    VertexCode += $"{line}\n";
                }
                
                line = reader.ReadLine();
            }
        }
        
        using (StringReader reader = new StringReader(frgOut)) {
            line = reader.ReadLine();
            while (line != null) {
                if (line.StartsWith("#include")) {
                    // parse out include and load it's code
                    string inclFilename = line.Split(" ")[1].Trim("\"".ToCharArray(0, 1));
                    StreamReader inclFile = ResourceManager.OpenResource(Path.Combine("shaders", "include", inclFilename));
                    string inclLine = inclFile.ReadLine();
                    while (inclLine != null) {
                        VertexCode += $"{inclLine}\n";
                        inclLine = inclFile.ReadLine();
                    }
                    inclFile.Close();
                } else {
                    FragmentCode += $"{line}\n";
                }
                
                line = reader.ReadLine();
            }
        }
    }
}
