using Sandbox;
using SquidGame;

public class SquidGameWalkController : WalkController
{
	protected SquidGamePlayer Player;

	public SquidGameWalkController() { }

	public SquidGameWalkController( SquidGamePlayer player )
	{
		Player = player;
	}

	public override float GetWishSpeed()
	{
		if ( Player != null && !Player.CanMove ) return 0f;

		return base.GetWishSpeed();
	}
}
