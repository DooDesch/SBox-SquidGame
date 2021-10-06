using MinimalExample;
using Sandbox;

public class NullGameMode : AbstractGameMode
{
	public NullGameMode()
	{
		gameState = GAME_STATE.NULL;
		Log.Info( "Fuck me im the null GameMode" );
	}

	public override void AddPlayer( MinimalPlayer player)
	{
		
	}

	public override void OnTick()
	{
		
	}
}
