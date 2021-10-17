using SquidGame;
using Sandbox;

public class NullGameMode : AbstractGameMode
{
	public NullGameMode()
	{
		Log.Info( "NullGameMode::Constructor" );
		GameStateTimer = 0;
		GameState = GAME_STATE.ENDING;
		Tag = "Null";
		TimeUntil = new TimeUntil
		{
			NextGame = 15
		};
	}

	public override void AddPlayer( SquidGamePlayer player )
	{

	}

	public override string GetGameText()
	{
		return "";
	}

	public override void Init()
	{
		UpdateGameTimers( TimeUntil.NextGame );
	}

	public override void Ready()
	{

	}
}
