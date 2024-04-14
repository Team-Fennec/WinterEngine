using ImGuiNET;
using WinterEngine.ToolsFramework;

namespace ModelConfEditor;

public sealed class ModelConfEditorTool : EngineTool
{
    public override string ToolName => "Model Config Editor";

    public override void GameThink(double deltaTime)
    {

    }

    public override void Init()
    {
        MainWindow mainWindow = new MainWindow();

        ToolsFramework.m_gtkApplication.AddWindow(mainWindow);
        mainWindow.Show();
    }

    public override void CreateGui()
    {
        
    }

    public override void Shutdown()
    {

    }
}
