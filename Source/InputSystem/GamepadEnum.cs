namespace WinterEngine.InputSystem;

public enum Gamepad : uint
{
	Face1,
	Face2,
	Face3,
	Face4,
	Start,
	Select,
	HatUp,
	HatDown,
	HatLeft,
	HatRight,
	RShoulder,
	LShoulder,
	RTrigger,
	LTrigger
}

public enum GamepadAxis
{
	Axis0, // left horiz
	Axis1, // left verti
	Axis2, // right horiz
	Axis3, // right verti
	Axis4, // trigger l
	Axis5, // trigger r
	Axis6
}
