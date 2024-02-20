
namespace Celeste64;

/// <summary>
/// Strawberries search for any of these within their Target GroupName, and
/// waits until they're all satisfied or destroyed.
/// </summary>
public interface IUnlockStrawberry
{
	public bool Satisfied { get; }
}
