using WinterEngine.ToolsFramework;

namespace ConsoleTool;

public sealed class ConsoleTool : EngineTool
{
    public override string ToolName => "Console";
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
