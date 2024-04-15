using ImGuiNET;
using Veneer;
using Veneer.Controls;
using WinterEngine.Utilities;
using System.Numerics;
using WinterEngine.Core;

namespace WinterEngine.DevUI;

public sealed class VeneerTestPanel : Panel
{
	public VeneerTestPanel()
    {
        Title = "Veneer Test Panel";
        Size = new Vector2(500, 400);
        Pos = new Vector2(100, 100);
        Flags = ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.MenuBar;
        Visible = false;
        ID = "veneer_test_panel";

        LoadSchemeFile("ToolsScheme.res");

        CreateGui();
    }

	protected override void CreateGui()
	{
        MenuBar testMenuBar = new MenuBar();
        testMenuBar.AddMenu("Menu 1");
        testMenuBar.AddMenuItem("Menu 1", new MenuItem("Print Msg", () =>
        {
            LogManager.GetLogger("TestPanel").Notice("Printing message from menu item!");
        }));
        testMenuBar.AddMenuItem("Menu 1", new MenuItem("Print Warn", () =>
        {
            LogManager.GetLogger("TestPanel").Warn("Printing warning from menu item!");
        }));
        testMenuBar.AddMenuItem("Menu 1", new MenuItem("separator 1"));
        testMenuBar.AddMenuItem("Menu 1", new MenuItem("Cause Error", () =>
        {
            Engine.Error("Menubar caused Error");
        }));

        Button testButton = new Button()
		{
			Size = new Vector2(120, 20),
			Position = new Vector2(20, 20),
			Text = "Veneer Button",
            VerticalAnchor = Control.AnchorPos.End
		};
		testButton.OnPushed += (o, e) => {
			LogManager.GetLogger("TestPanel").Notice("Button Pressed!");
		};

        InputField testField = new InputField()
        {
            Position = new Vector2(20, 280),
            Label = "Testing Field"
        };
        testField.OnConfirmed += (o, e) =>
        {
            LogManager.GetLogger("TestPanel").Notice($"Field Confirmed: {e}!");
        };
        testField.OnModified += (o, e) =>
        {
            LogManager.GetLogger("TestPanel").Notice($"Field Modified: {e}!");
        };

        Label testLabel = new Label()
		{
			Position = new Vector2(20, 150),
			Text = "Veneer Test Label"
		};

		AddControl(testButton);
		AddControl(testLabel);
        AddControl(testField);
        AddControl(testMenuBar);
	}
}
