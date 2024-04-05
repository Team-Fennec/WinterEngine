using System.Numerics;

namespace Veneer;

///<summary>
/// Base class for all items that can be placed within a Panel
///</summary>
public class Control
{
	public string Name {
		get {
			return m_Name == "" ? m_Guid.ToString() : m_Name;
		}
		set {
			m_Name = value;
		}
	}
	public Guid Guid => m_Guid;
	public Vector2 Position = Vector2.Zero;
	public Vector2 Size = Vector2.Zero;
	public List<Control> Children = new List<Control>();
	
	private string m_Name = "";
	private Guid m_Guid = Guid.NewGuid();

	public void Draw()
	{
		foreach (Control child in Children)
		{
			child.Draw();
		}
		OnLayout();
	}

	protected virtual void OnLayout() {}
}
