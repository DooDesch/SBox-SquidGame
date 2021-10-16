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
		if ( Player == null ) return 0f;
		if ( Player != null && !Player.CanMove ) return 0f;

		var ws = Duck.GetWishSpeed();
		if ( ws >= 0 ) return ws;

		if ( Input.Down( InputButton.Run ) && CanSprint() ) return SprintSpeed;
		if ( Input.Down( InputButton.Walk ) ) return WalkSpeed;

		return DefaultSpeed;
	}

	public bool CanSprint()
	{
		return Player == null || Player.CanSprint;
	}
}
