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
    #endregion

    public List<Control> Children = new List<Control>();

	private string m_Name = "";
	private Guid m_Guid = Guid.NewGuid();

	public void Draw()
	{
		foreach (Control child in Children)
		{
			child.Draw();
		}

        switch (VerticalAnchor)
        {
            case AnchorPos.Start:
                ImGui.SetCursorPosY(Position.Y);
                break;
            case AnchorPos.End:
                ImGui.SetCursorPosY(ImGui.GetWindowHeight() - Position.Y);
                break;
            case AnchorPos.Center:
                throw new NotImplementedException("Center anchoring isn't implemented yet!");
        }
        switch (HorizontalAnchor)
        {
            case AnchorPos.Start:
                ImGui.SetCursorPosX(Position.X);
                break;
            case AnchorPos.End:
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - Position.X);
                break;
            case AnchorPos.Center:
                throw new NotImplementedException("Center anchoring isn't implemented yet!");
        }

        OnLayout();
	}

	protected virtual void OnLayout() {}
}
