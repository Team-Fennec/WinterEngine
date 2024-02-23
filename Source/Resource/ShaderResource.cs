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
                vtxOut += line;
            } else {
                frgOut += line;
            }
        }
        
        // parse shader code
        using (StringReader reader = new StringReader(vtxOut)) {
            line = reader.ReadLine();
            while (line != null) {
                if (line.StartsWith("#include")) {
                    // parse out include and load it's code
                } else {
                    VertexCode += line;
                }
                
                line = reader.ReadLine();
            }
        }
        
        using (StringReader reader = new StringReader(frgOut)) {
            line = reader.ReadLine();
            while (line != null) {
                if (line.StartsWith("#include")) {
                    // parse out include and load it's code
                } else {
                    FragmentCode += line;
                }
                
                line = reader.ReadLine();
            }
        }
    }
}
