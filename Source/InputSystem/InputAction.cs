using log4net;
using Veldrid;

namespace WinterEngine.InputSystem;

public enum BindingType
{
	Keyboard,
	Mouse,
	Gamepad
}

public record struct InputBinding
{
	public BindingType Type;
	public uint Value;

    public InputBinding(BindingType type, uint value)
    {
        Type = type;
        Value = value;
    }
}

public class InputAction
{
	private static readonly ILog log = LogManager.GetLogger("InputSystem");

	/// <summary>The name/id of the action</summary>
	public string Name;

	private List<InputBinding> m_Bindings;

	public InputAction(string name)
	{
		Name = name;

		m_Bindings = new List<InputBinding>();
	}

#region Binding Operations
	public IReadOnlyList<InputBinding> Bindings => m_Bindings;

#region Mouse
	/// <summary>Bind a Mouse input to this action</summary> 
	public void AddBinding(MouseButton input)
	{
		if (HasBinding(input))
		{return;} // don't bother, it's already there.

		m_Bindings.Add(new InputBinding(BindingType.Mouse, (uint)input));
	}

	public bool HasBinding(MouseButton input)
	{
		foreach (InputBinding binding in m_Bindings)
		{
			if (binding.Type != BindingType.Mouse) 
			{continue;}

			if (binding.Value == (uint)input)
			{return true;}
		}

		return false;
	}

	public void RemoveBinding(MouseButton input)
	{
		for (int i = 0; i < m_Bindings.Count; i++)
		{
			if (m_Bindings[i].Type != BindingType.Mouse)
			{continue;}

			if (m_Bindings[i].Value == (uint)input)
			{
				m_Bindings.RemoveAt(i);
				log.Info($"Removed binding {Enum.GetName(typeof(MouseButton), input)} from Action {Name}");
				return;
			}
		}

		log.Warn($"Binding for {Enum.GetName(typeof(MouseButton), input)} doesn't exist on Action {Name}!");
	}
#endregion

#region Keyboard
	/// <summary>Bind a Keyboard input to this action</summary> 
	public void AddBinding(Key input)
	{
		if (HasBinding(input))
		{return;} // don't bother, it's already there.

		m_Bindings.Add(new InputBinding(BindingType.Keyboard, (uint)input));
	}

	public bool HasBinding(Key input)
	{
		foreach (InputBinding binding in m_Bindings)
		{
			if (binding.Type != BindingType.Keyboard) 
			{continue;}

			if (binding.Value == (uint)input)
			{return true;}
		}

		return false;
	}

	public void RemoveBinding(Key input)
	{
		for (int i = 0; i < m_Bindings.Count; i++)
		{
			if (m_Bindings[i].Type != BindingType.Keyboard)
			{continue;}

			if (m_Bindings[i].Value == (uint)input)
			{
				m_Bindings.RemoveAt(i);
				log.Info($"Removed binding {Enum.GetName(typeof(Key), input)} from Action {Name}");
				return;
			}
		}

		log.Warn($"Binding for {Enum.GetName(typeof(Key), input)} doesn't exist on Action {Name}!");
	}
#endregion

#region Gamepad
	/// <summary>Bind a Keyboard input to this action</summary> 
	public void AddBinding(Gamepad input)
	{
		if (HasBinding(input))
		{return;} // don't bother, it's already there.

		m_Bindings.Add(new InputBinding(BindingType.Gamepad, (uint)input));
	}

	public bool HasBinding(Gamepad input)
	{
		foreach (InputBinding binding in m_Bindings)
		{
			if (binding.Type != BindingType.Gamepad) 
			{continue;}

			if (binding.Value == (uint)input)
			{return true;}
		}

		return false;
	}

	public void RemoveBinding(Gamepad input)
	{
		for (int i = 0; i < m_Bindings.Count; i++)
		{
			if (m_Bindings[i].Type != BindingType.Gamepad)
			{continue;}

			if (m_Bindings[i].Value == (uint)input)
			{
				m_Bindings.RemoveAt(i);
				log.Info($"Removed binding {Enum.GetName(typeof(Gamepad), input)} from Action {Name}");
				return;
			}
		}

		log.Warn($"Binding for {Enum.GetName(typeof(Gamepad), input)} doesn't exist on Action {Name}!");
	}
#endregion

#endregion
}
