using Sandbox;
using MinimalExample;
public partial class RedLightGreenLight : AbstractGameMode
{
	[Net] public bool movementAllowed { get; set; } = false;
	private TimeSince gameStarted;

	public RedLightGreenLight()
	{
		gameState = GAME_STATE.NOT_STARTED;
		Log.Info( "Fuck me im the green GameMode" );
	}

	public override void AddPlayer( MinimalPlayer player )
	{
		player.currentGameModeClient = new RedLightGreenLightClient();
		player.currentGameModeClient.minimalPlayer = player;
	}


	public override void OnTick()
	{
		if ( gameState.Equals( GAME_STATE.NOT_STARTED ) )
		{
			Log.Info( "Let the Games begin!" );
			gameState = GAME_STATE.RUNNING;
			gameStarted = 0;
		}

		if ( !gameState.Equals( GAME_STATE.RUNNING ) ) return;
		movementAllowed = gameStarted % 10 > 1;
		if ( movementAllowed ) return;

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is MinimalPlayer player )
			{
				if ( !player.currentGameModeClient.isMoving ) return;

				if ( movementAllowed ) return;

				player.TakeDamage( DamageInfo.Generic( player.Health + 1 ) );
			}
		}
	}

	public override string GetGameText()
	{
		return "________________" + movementAllowed.ToString();
	}
}
