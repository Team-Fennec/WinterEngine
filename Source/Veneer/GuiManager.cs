using ImGuiNET;
using log4net;

namespace Veneer;

public static class GuiManager
{
	private static readonly ILog m_Log = LogManager.GetLogger("Veneer");

	private static List<Panel> m_Panels = new List<Panel>();

	public static void AddPanel(Panel panel)
	{
		foreach (Panel nPanel in m_Panels)
        {
            if (nPanel.ID == panel.ID)
            {
            	m_Log.Error($"ID Conflict: a panel with ID {panel.ID} already exists!");
                return;
            }
        }
        m_Log.Info($"Added panel {panel.ID}");
        m_Panels.Add(panel);
	}

	public static void RemovePanel(Panel panel)
	{
		m_Panels.Remove(panel);
	}

	public static Panel? GetPanel(string ID)
	{
		foreach (Panel panel in m_Panels)
        {
            if (panel.ID == ID)
            {
                return panel;
            }
        }
        m_Log.Warn($"No panel was found with ID {ID}");
        return null;
	}

	public static void SetPanelVisible(string ID, bool v)
    {
        foreach (Panel panel in m_Panels)
        {
            if (panel.ID == ID)
            {
                panel.Visible = v;
                return;
            }
        }
        m_Log.Error($"No panel was found with ID {ID}");
    }

    public static bool GetPanelVisible(string ID)
    {
        foreach (Panel panel in m_Panels)
        {
            if (panel.ID == ID)
            {
                return panel.Visible;
            }
        }
        m_Log.Warn($"No panel was found with ID {ID}");
        return false;
    }

	public static void Update()
	{
		foreach (Panel panel in m_Panels)
        {
            panel.DoLayout();
        }
	}
}
