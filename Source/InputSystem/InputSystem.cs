#if HAS_PROFILING
using WinterEngine.Diagnostics;
#endif
using Veldrid;
using Veldrid.Sdl2;
using log4net;
using System.Numerics;
using System.Text.RegularExpressions;

namespace WinterEngine.InputSystem;

internal enum InputState
{
    Pressed,
    Released,
    Down,
    Up
}

public static class InputManager
{
    private static readonly ILog log = LogManager.GetLogger("InputSystem");

    private static Dictionary<string, InputAction> m_Actions = new Dictionary<string, InputAction>();

    private static InputState[] m_KeyState;
    private static InputState[] m_PadState;
    private static InputState[] m_MouseState;

    private static bool m_MouseCaptured;
    private static Vector2 m_MousePosition;
    private static Vector2 m_MouseDelta;

    /// <summary>
    /// Determines whether or not inputs will be processed. Useful for global input locking.
    /// 
    /// NOTE: This will release all pressed inputs when set to false!
    /// </summary>
    public static bool ProcessInputs = true;

    public static void Init()
    {
        log.Info("Initializing InputSystem...");

        // initialize every input to Up state
        m_KeyState = new InputState[(int)Key.LastKey];
        for (int i = 0; i < (int)Key.LastKey; i++)
        {
            m_KeyState[i] = InputState.Up;
        }

        m_MouseState = new InputState[(int)MouseButton.LastButton];
        for (int i = 0; i < (int)MouseButton.LastButton; i++)
        {
            m_MouseState[i] = InputState.Up;
        }

        m_PadState = new InputState[(int)Gamepad.PadMax];
        for (int i = 0; i < (int)Gamepad.PadMax; i++)
        {
            m_PadState[i] = InputState.Up;
        }
    }

    // global update function used for updating the input status from a snapshot
    public static void UpdateEvents(InputSnapshot snapshot)
    {
#if HAS_PROFILING
		Profiler.PushProfile("InputSystem_UpdateEvents");
#endif
        if (!ProcessInputs)
        {
            for (int i = 0; i < (int)Key.LastKey; i++)
            {
                m_KeyState[i] = InputState.Up;
            }

            for (int i = 0; i < (int)MouseButton.LastButton; i++)
            {
                m_MouseState[i] = InputState.Up;
            }

            for (int i = 0; i < (int)Gamepad.PadMax; i++)
            {
                m_PadState[i] = InputState.Up;
            }
            Sdl2Native.SDL_SetRelativeMouseMode(false);
            Sdl2Native.SDL_CaptureMouse(false);
        }
        else
        {
            Sdl2Native.SDL_SetRelativeMouseMode(m_MouseCaptured);
            Sdl2Native.SDL_CaptureMouse(m_MouseCaptured);
        }

        // update the status of current events that were pressed or released last frame
        for (int i = 0; i < (int)Key.LastKey; i++)
        {
            if (m_KeyState[i] == InputState.Released)
                m_KeyState[i] = InputState.Up;
            else if (m_KeyState[i] == InputState.Pressed)
                m_KeyState[i] = InputState.Down;
        }

        for (int i = 0; i < (int)MouseButton.LastButton; i++)
        {
            if (m_MouseState[i] == InputState.Released)
                m_MouseState[i] = InputState.Up;
            else if (m_MouseState[i] == InputState.Pressed)
                m_MouseState[i] = InputState.Down;
        }

        for (int i = 0; i < (int)Gamepad.PadMax; i++)
        {
            if (m_PadState[i] == InputState.Released)
                m_PadState[i] = InputState.Up;
            else if (m_PadState[i] == InputState.Pressed)
                m_PadState[i] = InputState.Down;
        }

        if (ProcessInputs)
        {
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

            m_MouseDelta = m_MousePosition - snapshot.MousePosition;
            m_MousePosition = snapshot.MousePosition;
        }
        else
        {
            m_MouseDelta = new Vector2(0, 0);
        }

#if HAS_PROFILING
        Profiler.PopProfile();
#endif
    }

    #region Input Actions
    public static void RegisterAction(InputAction action)
    {
        if (m_Actions.ContainsKey(action.Name))
        {
            log.Error($"Name {action.Name} was already registered by another Action!");
            return;
        }

        if (!m_Actions.TryAdd(action.Name, action))
        {
            log.Error($"An error occurred while trying to register Action {action.Name}!");
            return;
        }
        log.Info($"Registered Action {action.Name}.");
    }

    public static void RemoveAction(string name)
    {
        if (!m_Actions.ContainsKey(name))
        {
            log.Error($"No Action named {name} was found in the Action Registry!");
            return;
        }

        m_Actions.Remove(name);
    }

    public static InputAction? GetAction(string name)
    {
        if (!m_Actions.ContainsKey(name))
        {
            log.Error($"No Action named {name} was found in the Action Registry!");
            return null;
        }

        m_Actions.TryGetValue(name, out InputAction action);
        return action;
    }

    #region Input Action state
    public static bool ActionCheckPressed(string name)
    {
        if (!m_Actions.ContainsKey(name))
        {
            log.Error($"No Action named {name} was found in the Action Registry!");
            return false;
        }

        m_Actions.TryGetValue(name, out InputAction action);

        foreach (InputBinding binding in action.Bindings)
        {
            switch (binding.Type)
            {
                case BindingType.Keyboard:
                    if (KeyCheckPressed((Key)binding.Value))
                        return true;
                    break;
                case BindingType.Mouse:
                    if (MouseCheckPressed((MouseButton)binding.Value))
                        return true;
                    break;
                case BindingType.Gamepad:
                    log.Error("Gamepad bindings are not currently supported!");
                    break;
            }
        }
        return false;
    }

    public static bool ActionCheckReleased(string name)
    {
        if (!m_Actions.ContainsKey(name))
        {
            log.Error($"No Action named {name} was found in the Action Registry!");
            return false;
        }

        m_Actions.TryGetValue(name, out InputAction action);

        foreach (InputBinding binding in action.Bindings)
        {
            switch (binding.Type)
            {
                case BindingType.Keyboard:
                    if (KeyCheckReleased((Key)binding.Value))
                        return true;
                    break;
                case BindingType.Mouse:
                    if (MouseCheckReleased((MouseButton)binding.Value))
                        return true;
                    break;
                case BindingType.Gamepad:
                    log.Error("Gamepad bindings are not currently supported!");
                    break;
            }
        }
        return false;
    }

    public static bool ActionCheck(string name)
    {
        if (!m_Actions.ContainsKey(name))
        {
            log.Error($"No Action named {name} was found in the Action Registry!");
            return false;
        }

        m_Actions.TryGetValue(name, out InputAction action);

        foreach (InputBinding binding in action.Bindings)
        {
            switch (binding.Type)
            {
                case BindingType.Keyboard:
                    if (KeyCheck((Key)binding.Value))
                        return true;
                    break;
                case BindingType.Mouse:
                    if (MouseCheck((MouseButton)binding.Value))
                        return true;
                    break;
                case BindingType.Gamepad:
                    log.Error("Gamepad bindings are not currently supported!");
                    break;
            }
        }
        return false;
    }
    #endregion
    #endregion

    #region Keyboard key state
    public static bool KeyCheckPressed(Key input)
    {
        return (m_KeyState[(int)input] == InputState.Pressed);
    }

    public static bool KeyCheckReleased(Key input)
    {
        return (m_KeyState[(int)input] == InputState.Released);
    }

    public static bool KeyCheck(Key input)
    {
        return (m_KeyState[(int)input] == InputState.Down);
    }
    #endregion

    #region Gamepad button state
    public static bool GamepadCheckPressed(Gamepad input)
    {
        return (m_PadState[(int)input] == InputState.Pressed);
    }

    public static bool GamepadCheckReleased(Gamepad input)
    {
        return (m_PadState[(int)input] == InputState.Released);
    }

    public static bool GamepadCheck(Gamepad input)
    {
        return (m_PadState[(int)input] == InputState.Down);
    }
    #endregion

    #region Mouse button state
    public static bool MouseCheckPressed(MouseButton input)
    {
        return (m_MouseState[(int)input] == InputState.Pressed);
    }

    public static bool MouseCheckReleased(MouseButton input)
    {
        return (m_MouseState[(int)input] == InputState.Released);
    }

    public static bool MouseCheck(MouseButton input)
    {
        return (m_MouseState[(int)input] == InputState.Down);
    }
    #endregion

    public static bool GetMouseCaptured() => m_MouseCaptured;
    public static void SetMouseCapture(bool capture)
    {
        m_MouseCaptured = capture;
    }

    /// <summary>
    /// Returns the current position of the mouse.
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetMousePosition() => m_MousePosition;

    /// <summary>
    /// Returns the difference between last frame's mouse position and current frame's mouse position.
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetMouseDelta() => m_MouseDelta;
}
