using WinterEngine.SceneSystem;

namespace SceneSystemTest;

public sealed class TestEntA : Entity
{
	public int Counter = 0;

	public override void Spawn()
	{
		Console.WriteLine("I was spawned!");
	}

	public override void Think(double deltaTime)
	{
		Counter++;
		base.Think(deltaTime);
	}
}
