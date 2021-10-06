using Sandbox;
using MinimalExample;
public class RedLightGreenLight : AbstractGameMode
{
	private bool movementAllowed = false;

	public RedLightGreenLight()
	{
		gameState = GAME_STATE.NOT_STARTED;
		Log.Info( "Fuck me im the green GameMode" );
	}

	public override void AddPlayer(MinimalPlayer player)
	{
		player.currentGameModeClient = new RedLightGreenLightClient();
		player.currentGameModeClient.minimalPlayer = player;
	}


	public override void OnTick()
	{
		if(gameState.Equals(GAME_STATE.NOT_STARTED))
		{
			Log.Info( "Let the Games begin!" );
			gameState = GAME_STATE.RUNNING;
		}

		if ( !gameState.Equals( GAME_STATE.RUNNING ) ) return;
		if ( movementAllowed ) return;

		foreach(Client client in Client.All)
		{
			if(client.Pawn is MinimalPlayer player)
			{
				if(!player.currentGameModeClient.isMoving) return;

				player.TakeDamage( DamageInfo.Generic( player.Health + 1 ) );
			}
		}
	}
}
