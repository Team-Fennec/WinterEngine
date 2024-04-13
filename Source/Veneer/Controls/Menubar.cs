using ImGuiNET;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Veneer.Controls;

/// <summary>
/// The structure defining an item inside a MenuBar Menu 
/// </summary>
public struct MenuItem
{
	public string Name;
	public Action? Action;
	public string Shortcut;
	public bool Enabled;
	public bool IsSeparator;


    /// <summary>
    /// Quick shortcut to make a separator.
    /// </summary>
    /// <param name="name">Menu item name, required no matter what.</param>
    public MenuItem(string name) : this(name, "", null, true, true) { }
    /// <summary>
    /// Create a MenuItem.
    /// </summary>
    /// <param name="name">Name of item in menu</param>
    /// <param name="action">Action to perform when clicked</param>
    public MenuItem(string name, Action action) : this(name, "", action, true, false) { }
    /// <summary>
    /// Create a MenuItem with a shortcut assigned.
    /// </summary>
    /// <param name="name">Name of item in menu</param>
    /// <param name="shortcut">Keyboard shortcut</param>
    /// <param name="action">Action to perform when clicked</param>
    public MenuItem(string name, string shortcut, Action action) : this(name, shortcut, action, true, false) { }
    /// <summary>
    /// Create a MenuItem.
    /// </summary>
    /// <param name="name">Name of item in menu</param>
    /// <param name="shortcut">Keyboard shortcut</param>
    /// <param name="action">Action to perform when clicked</param>
    /// <param name="enabled">The initial enabled state of the item</param>
    /// <param name="isSeparator">Whether or not this is a separator or a functional item</param>
    public MenuItem(string name, string shortcut, Action? action, bool enabled, bool isSeparator)
    {
        Name = name;
        Shortcut = shortcut;
        Enabled = enabled;
        IsSeparator = isSeparator;
        Action = action;
    }
}

/// <summary>
/// A per Panel MenuBar.
/// 
/// If multiple of these are present within a single panel, their contents will be merged.
/// </summary>
public class MenuBar : Control
{
	private List<(string name, List<MenuItem> items)> m_Menus = new();

    /// <summary>
    /// Adds a new Menu to the list of menus in the MenuBar.
    /// </summary>
    /// <param name="menuName">String name of the Menu</param>
    /// <exception cref="Exception">Duplicate Menu Name error</exception>
	public void AddMenu(string menuName)
	{
		foreach (var menu in m_Menus)
		{
			if (menu.name == menuName)
			{
				throw new Exception($"Menu {menuName} Already Exists!");
			}
		}
		m_Menus.Add((menuName, new List<MenuItem>()));
	}

    /// <summary>
    /// Assigns a MenuItem object to a specific Menu in the MenuBar.
    /// </summary>
    /// <param name="menuName">Name of the Menu to assign to</param>
    /// <param name="item">MenuItem to assign</param>
    /// <exception cref="Exception">Invalid Menu name error</exception>
	public void AddMenuItem(string menuName, MenuItem item)
	{
		foreach (var menu in m_Menus)
		{
			if (menu.name == menuName)
			{
				menu.items.Add(item);
				return;
			}
		}
		throw new Exception($"Menu {menuName} Doesn't Exist!");
	}

	protected override void OnLayout()
	{
        ImGui.SetCursorPos(Vector2.Zero);

        /*ImDrawListPtr drawList = ImGui.GetForegroundDrawList();
        Vector2 startPos = ImGui.GetCursorScreenPos() + new Vector2(0, ImGui.GetTextLineHeight() + 6);
        Vector4 cTop = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
        Vector4 cBottom = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);

        drawList.AddRectFilledMultiColor(
            startPos,
            startPos + new Vector2(ImGui.GetWindowSize().X, ImGui.GetTextLineHeight() + 6),
            ImGui.ColorConvertFloat4ToU32(cTop),
            ImGui.ColorConvertFloat4ToU32(cTop),
            ImGui.ColorConvertFloat4ToU32(cBottom),
            ImGui.ColorConvertFloat4ToU32(cBottom)
        );*/

        if (ImGui.BeginMenuBar())
		{
			foreach (var menuStruct in m_Menus)
			{
				if (ImGui.BeginMenu(menuStruct.name))
				{
					foreach (MenuItem item in menuStruct.items)
					{
						if (item.IsSeparator)
							ImGui.Separator();
						else
						{
							if (ImGui.MenuItem(item.Name, item.Shortcut, false, item.Enabled))
							{
								item.Action();
							}
						}
					}

					ImGui.EndMenu();
				}
			}

			ImGui.EndMenuBar();
		}
    }
}
