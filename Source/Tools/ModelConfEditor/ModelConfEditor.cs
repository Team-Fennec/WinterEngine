using ImGuiNET;
using WinterEngine.ToolsFramework;

namespace ModelConfEditor;

public sealed class ModelConfEditorTool : EngineTool
{
    public override string ToolName => "Model Config Editor";

    private MainWindow m_MainWindow;

    public override void GameThink(double deltaTime)
    {

    }

    public override void OnEnable()
    {
        if (m_MainWindow == null)
        {
            m_MainWindow = new MainWindow();
            ToolsFramework.m_gtkApplication.AddWindow(m_MainWindow);
        }
        m_MainWindow.Show();
    }

    public override void Init()
    {
    }

    public override void CreateGui()
    {
        
    }

    public override void Shutdown()
    {
        m_MainWindow.Close();
    }
}
