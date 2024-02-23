using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WinterEngine.Resource;

public struct LevelData {
    public LevelInfo info;
    public List<LevelSector> sectors;
    public List<LevelWall> walls;
    public List<Vector2> vertices;
    public List<LevelEntity> entities;
}

public struct LevelInfo {
    public string name;
    public string author;
    public string skyTexture;
}

public struct LevelSector {
    public int wallStart;
    public int wallEnd;
    public int floorHeight;
    public int ceilingHeight;
    public string surfaceTexture;
    public int surfaceScale;
    //public Color floorColor;
    //public Color ceilingColor;
}

public struct LevelWall {
    public int startPoint;
    public int endPoint;
    public string texture;
    //public Color color;
    public Vector2 textureScale;
}

public struct LevelEntity {
    public string className;
    public Vector3 position;
    public int angle;
    public Dictionary<string, string> parameters;
}
