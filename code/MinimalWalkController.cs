using Sandbox;
using MinimalExample;

public class MinimalWalkController : WalkController
{
	protected MinimalPlayer Player;

	public MinimalWalkController() { }

	public MinimalWalkController( MinimalPlayer player )
	{
		Player = player;
	}

	public override float GetWishSpeed()
	{
		if ( Player != null && !Player.CanMove ) return 0f;

		return base.GetWishSpeed();
	}
}
