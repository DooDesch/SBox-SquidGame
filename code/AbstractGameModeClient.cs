using Sandbox;
using MinimalExample;
public abstract class AbstractGameModeClient : Entity
{
	public MinimalPlayer minimalPlayer { get; set; }
	public bool isMoving { get; set; }

	public abstract void Init();
}
