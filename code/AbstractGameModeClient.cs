using Sandbox;
using SquidGame;
public abstract class AbstractGameModeClient : BaseNetworkable
{
	public SquidGamePlayer Player { get; set; }
	public bool IsMoving { get; set; }
	public bool HasWon { get; set; } = false;

	public abstract void Init();
}
