using System.Linq;
using Sandbox;
using MinimalExample;
using System;

public partial class RedLightGreenLight : AbstractGameMode
{
	[Net] public bool movementAllowed { get; set; } = false;
	private TimeSince gameStarted;

	public RedLightGreenLight()
	{
		Tag = "RedLightGreenLight";

		gameStarted = 0;
		maxTime = 300;
		gameState = GAME_STATE.NOT_STARTED;
		Log.Info( "RedLightGreenLight::Constructor" );
	}

	public override void Init()
	{
		base.Init();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is MinimalPlayer player )
			{
				AddPlayer( player );
			}
		}
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
				//player.TakeDamage( DamageInfo.Generic( player.Health + 1 ) ); // TODO : Uncomment, so the player gets damaged again
			}
		}
	}

	public override string GetGameText()
	{
		return "________________" + movementAllowed.ToString();
	}

	public override void AddPlayer( MinimalPlayer player )
	{
		player.currentGameModeClient = new RedLightGreenLightClient();
		player.currentGameModeClient.minimalPlayer = player;
		player.Transform = playerSpawnPointList[Rand.Next( 0, playerSpawnPointList.Count )];
		Log.Info( "RedLightGreenLight::AddPlayer" );
		Log.Info( timerList.Count );
		player.currentGameModeClient.Init();
	}
}
