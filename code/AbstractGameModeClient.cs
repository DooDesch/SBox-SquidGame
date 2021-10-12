using Sandbox;
using MinimalExample;
public abstract class AbstractGameModeClient : BaseNetworkable
{
	public MinimalPlayer MinimalPlayer { get; set; }
	public bool IsMoving { get; set; }
	public bool HasWon { get; set; } = false;

	public abstract void Init();
}
