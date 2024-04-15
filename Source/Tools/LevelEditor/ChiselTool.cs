using ImGuiNET;
using WinterEngine.ToolsFramework;

namespace Chisel;

public sealed class ChiselTool : EngineTool
{
    public override string ToolName => "Chisel";

    private ChiselMain m_MainWindow;

    public override void GameThink(double deltaTime)
    {
        
    }

    public override void OnEnable()
    {
        if (m_MainWindow == null)
        {
            m_MainWindow = new ChiselMain();
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
        
    }
}

