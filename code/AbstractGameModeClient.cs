using Sandbox;
using MinimalExample;
public abstract class AbstractGameModeClient : Networked
{
	public MinimalPlayer minimalPlayer { get; set; }
	public bool isMoving { get; set; }
}
