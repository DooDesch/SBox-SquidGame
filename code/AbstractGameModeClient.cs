using Sandbox;
using MinimalExample;
public abstract class AbstractGameModeClient : BaseNetworkable
{
	public MinimalPlayer minimalPlayer { get; set; }
	public bool isMoving { get; set; }
	public bool HasWon { get; set; } = false;

	public abstract void Init();
}
