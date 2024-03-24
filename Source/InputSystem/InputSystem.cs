#if HAS_PROFILING
using WinterEngine.Diagnostics;
#endif
using Veldrid;
using Veldrid.Sdl2;
using log4net;

namespace WinterEngine.InputSystem;

internal enum InputState
{
	Pressed,
	Released,
	Down,
	Up
}

public static class InputSystem
{
	private static readonly ILog log = LogManager.GetLogger("InputSystem");

	private Dictionary<string, InputAction> m_Actions;
	
	private List<InputState> m_KeyState = new List<InputState>();
	private List<InputState> m_PadState = new List<InputState>();
	private List<InputState> m_MouseState = new List<InputState>();

	public void Init()
	{
		log.Info("Initializing InputSystem...");

		// initialize every input to Up state
		m_KeyState.EnsureCapacity(Key.LastKey-1);
		foreach (int i = 0; i < Key.LastKey; i++)
		{
			m_KeyState[i] = InputState.Up;
		}

		m_MouseState.EnsureCapacity(MouseButton.LastButton-1);
		foreach (int i = 0; i < MouseButton.LastButton; i++)
		{
			m_MouseState[i] = InputState.Up;
		}

		m_PadState.EnsureCapacity(Gamepad.PadMax-1);
		foreach (int i = 0; i < Gamepad.PadMax; i++)
		{
			m_PadState[i] = InputState.Up;
		}
	}

	// global update function used for updating the input status from a snapshot
	public void UpdateEvents(InputSnapshot snapshot)
	{
#if HAS_PROFILING
		Profiler.PushProfile("InputSystem_UpdateEvents");
#endif
		// update the status of current events that were pressed or released last frame
		foreach (int i = 0; i < Key.LastKey; i++)
		{
			if (m_KeyState[i] == InputState.Released)
				m_KeyState[i] = InputState.Up;
			else if (m_KeyState[i] == InputState.Pressed)
				m_KeyState[i] = InputState.Down;
		}

		foreach (int i = 0; i < Mouse.LastButton; i++)
		{
			if (m_MouseState[i] == InputState.Released)
				m_MouseState[i] = InputState.Up;
			else if (m_MouseState[i] == InputState.Pressed)
				m_MouseState[i] = InputState.Down;
		}

		foreach (int i = 0; i < Gamepad.PadMax; i++)
		{
			if (m_PadState[i] == InputState.Released)
				m_PadState[i] = InputState.Up;
			else if (m_PadState[i] == InputState.Pressed)
				m_PadState[i] = InputState.Down;
		}

		// Set event data based on snapshot
		foreach (KeyEvent kEvent in snapshot.KeyEvents)
		{
			if (kEvent.Down)
			{
				if (m_KeyState[(int)kEvent.Key] == InputState.Up
					|| m_KeyState[(int)kEvent.Key] == InputState.Released)
					m_KeyState[(int)kEvent.Key] = InputState.Pressed;
			}
			else
			{
				if (m_KeyState[(int)kEvent.Key] == InputState.Down
					|| m_KeyState[(int)kEvent.Key] == InputState.Pressed)
					m_KeyState[(int)kEvent.Key] = InputState.Released;
			}
		}

		foreach (MouseEvent mEvent in snapshot.MouseEvents)
		{
			if (mEvent.Down)
			{
				if (m_MouseState[(int)mEvent.MouseButton] == InputState.Up
					|| m_MouseState[(int)mEvent.MouseButton] == InputState.Released)
					m_MouseState[(int)mEvent.MouseButton] = InputState.Pressed;
			}
			else
			{
				if (m_MouseState[(int)mEvent.MouseButton] == InputState.Down
					|| m_MouseState[(int)mEvent.MouseButton] == InputState.Pressed)
					m_MouseState[(int)mEvent.MouseButton] = InputState.Released;
			}
		}

#if HAS_PROFILING
		Profiler.PopProfile();
#endif
	}

#region Keyboard key state
	public bool KeyCheckPressed(Key input)
	{
		return (m_KeyState[(int)input] == InputState.Pressed);
	}

	public bool KeyCheckReleased(Key input)
	{
		return (m_KeyState[(int)input] == InputState.Released);
	}

	public bool KeyCheck(Key input)
	{
		return (m_KeyState[(int)input] == InputState.Down);
	}
#endregion

#region Gamepad button state
	public bool GamepadCheckPressed(Gamepad input)
	{
		return (m_PadState[(int)input] == InputState.Pressed);
	}

	public bool GamepadCheckReleased(Gamepad input)
	{
		return (m_PadState[(int)input] == InputState.Released);
	}

	public bool GamepadCheck(Gamepad input)
	{
		return (m_PadState[(int)input] == InputState.Down);
	}
#endregion

#region Mouse button state
	public bool MouseCheckPressed(Mouse input)
	{
		return (m_MouseState[(int)input] == InputState.Pressed);
	}

	public bool MouseCheckReleased(Mouse input)
	{
		return (m_MouseState[(int)input] == InputState.Released);
	}

	public bool MouseCheck(Mouse input)
	{
		return (m_MouseState[(int)input] == InputState.Down);
	}
#endregion
}
