using ImGuiNET;
using Veneer;
using Veneer.Controls;
using WinterEngine.Utilities;
using System.Numerics;

namespace WinterEngine.Gui.DevUI;

public sealed class VeneerTestPanel : Panel
{
	public VeneerTestPanel()
    {
        Title = "Veneer Test Panel";
        Size = new Vector2(500, 400);
        Pos = new Vector2(100, 100);
        Flags = ImGuiWindowFlags.NoSavedSettings;
        Visible = true;
        ID = "veneer_test_panel";

        LoadSchemeFile("ToolsScheme.res");

        CreateGui();
    }

	protected override void CreateGui()
	{
		Button testButton = new Button()
		{
			Size = new Vector2(120, 20),
			Position = new Vector2(20, 250),
			Text = "Veneer Button"
		};
		testButton.OnPushed += (o, e) => {
			log4net.LogManager.GetLogger("TestButton").Notice("Button Pressed!");
		};

        InputField testField = new InputField()
        {
            Position = new Vector2(20, 280),
            Label = "Testing Field"
        };
        testField.OnConfirmed += (o, e) =>
        {
            log4net.LogManager.GetLogger("TestField").Notice($"Field Confirmed: {e}!");
        };
        testField.OnModified += (o, e) =>
        {
            log4net.LogManager.GetLogger("TestField").Notice($"Field Modified: {e}!");
        };

        Label testLabel = new Label()
		{
			Position = new Vector2(20, 150),
			Text = "Veneer Test Label"
		};

		AddControl(testButton);
		AddControl(testLabel);
        AddControl(testField);
	}
}
