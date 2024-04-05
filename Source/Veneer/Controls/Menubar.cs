using ImGuiNET;

namespace Veneer.Controls;

public struct MenuItem
{
	public string Name;
	public Action Action;
	public string Shortcut;
	public bool Enabled;
	public bool IsSeparator;
}

public class MenuBar : Control
{
	private List<(string name, List<MenuItem> items)> m_Menus = new();

	public void AddMenu(string MenuName)
	{
		foreach (var menu in m_Menus)
		{
			if (menu.name == MenuName)
			{
				throw new Exception("Menu Already Exists!");
			}
		}
		m_Menus.Add((MenuName, new List<MenuItem>()));
	}

	public void AddMenuItem(string MenuName, MenuItem Item)
	{
		foreach (var menu in m_Menus)
		{
			if (menu.name == MenuName)
			{
				menu.items.Add(Item);
				return;
			}
		}
		throw new Exception("Menu Doesn't Exist!");
	}

	protected override void OnLayout()
	{
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
