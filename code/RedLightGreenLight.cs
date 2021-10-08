using Sandbox;
using MinimalExample;
using System;

public partial class RedLightGreenLight : AbstractGameMode
{
	[Net] public bool movementAllowed { get; set; } = false;
	private TimeSince gameStarted;

	public RedLightGreenLight()
	{
		gameStarted = 0;
		maxTime = 300;
		gameState = GAME_STATE.NOT_STARTED;
		Log.Info( "Fuck me im the green GameMode" );
	}

	public override void AddPlayer( MinimalPlayer player )
	{
		player.currentGameModeClient = new RedLightGreenLightClient();
		player.currentGameModeClient.minimalPlayer = player;
		player.currentGameModeClient.Init();
		player.Transform = playerSpawnPointList[Rand.Next( 0, playerSpawnPointList.Count )];
	}


	public override void OnTick()
	{
		base.OnTick();

		if ( !gameState.Equals( GAME_STATE.RUNNING ) ) return;
		movementAllowed = gameStarted % 10 > 1;
		if ( movementAllowed ) return;
		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is MinimalPlayer player )
			{
				if ( !player.currentGameModeClient.isMoving ) return;
				//player.TakeDamage( DamageInfo.Generic( player.Health + 1 ) );
			}
		}
	}

	public override string GetGameText()
	{
		return "________________" + movementAllowed.ToString();
	}

	public override void Init()
	{
		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is MinimalPlayer player )
			{
				AddPlayer( player );
			}
		}
		base.Init();
	}
}
