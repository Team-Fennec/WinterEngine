using System;

namespace WinterEngine.ToolsFramework;

public abstract class EngineTool
{
    public abstract string ToolName { get; }

    public abstract void Init();
    public abstract void Shutdown();

    public abstract void OnEnable();

    public abstract void GameThink(double deltaTime);
    public abstract void CreateGui();
}
