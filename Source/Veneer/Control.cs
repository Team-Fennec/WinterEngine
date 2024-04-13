using System.Numerics;
using ImGuiNET;

namespace Veneer;

///<summary>
/// Base class for all items that can be placed within a Panel
///</summary>
public class Control
{
    public enum AnchorPos
    {
        Start,
        End,
        Center
    }

    public string Name {
		get {
			return m_Name == "" ? m_Guid.ToString() : m_Name;
		}
		set {
			m_Name = value;
		}
	}
	public Guid Guid => m_Guid;

    #region Layout Properties
    public Vector2 Position = Vector2.Zero;
	public Vector2 Size = Vector2.Zero;
    public AnchorPos VerticalAnchor = AnchorPos.Start;
    public AnchorPos HorizontalAnchor = AnchorPos.Start;
    public bool AutoSizeX = false;
    public bool AutoSizeY = false;
    #endregion

    public List<Control> Children = new List<Control>();
    public Panel Parent;

	private string m_Name = "";
	private Guid m_Guid = Guid.NewGuid();

	public void Draw()
	{
		foreach (Control child in Children)
		{
			child.Draw();
		}

        Vector2 panelPadding = ImGui.GetStyle().WindowPadding;
        Vector2 startPos = ImGui.GetCursorStartPos();

        switch (VerticalAnchor)
        {
            case AnchorPos.Start:
                ImGui.SetCursorPosY(startPos.Y + Position.Y);
                break;
            case AnchorPos.End:
                ImGui.SetCursorPosY(ImGui.GetWindowHeight() - Position.Y - panelPadding.Y);
                break;
            case AnchorPos.Center:
                throw new NotImplementedException("Center anchoring isn't implemented yet!");
        }
        switch (HorizontalAnchor)
        {
            case AnchorPos.Start:
                ImGui.SetCursorPosX(startPos.X + Position.X);
                break;
            case AnchorPos.End:
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - Position.X - panelPadding.X);
                break;
            case AnchorPos.Center:
                throw new NotImplementedException("Center anchoring isn't implemented yet!");
        }

        OnLayout();
	}

    // Returns the size adjusted for AutoSizing
    public Vector2 GetSize()
    {
        Vector2 outSize = new Vector2(0.0f, 0.0f);

        if (AutoSizeX)
        {
            float offset = Parent.Size.X - Size.X;
            outSize.X = ImGui.GetWindowSize().X - offset;
        }
        else
        {
            outSize.X = Size.X;
        }

        if (AutoSizeY)
        {
            float offset = Parent.Size.Y - Size.Y;
            outSize.Y = ImGui.GetWindowSize().Y - offset;
        }
        else
        {
            outSize.Y = Size.Y;
        }

        return outSize;
    }

	protected virtual void OnLayout() {}
}
