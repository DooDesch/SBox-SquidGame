using System.Linq;
using Sandbox;
using MinimalExample;
using System;

public partial class RedLightGreenLight : AbstractGameMode
{
	[Net] public bool MovementAllowed { get; set; } = false;

	public RedLightGreenLight()
	{
		Log.Info( "RedLightGreenLight::Constructor" );
		Tag = "RedLightGreenLight";
		maxTime = 300;
		gameState = GAME_STATE.NOT_STARTED;
		TimeUntil = new TimeUntil
		{
			GameSetup = 10,
			GameStarts = 10,
		};
	}

	public override void Init()
	{
		base.Init();

		Log.Info( "RedLightGreenLight::Init" );
		Log.Info( "GameState : " + gameState.ToString() );
	}

	public override void OnTick()
	{
		base.OnTick();

		if ( !gameState.Equals( GAME_STATE.STARTING ) ) return;

		MovementAllowed = GameStateTimer.Started % 10 > 1;
		if ( MovementAllowed ) return;

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is MinimalPlayer player )
			{
				if ( !player.currentGameModeClient.isMoving ) return;

				Log.Warning( "You dead Jimbo" ); // TODO : Remove

				//player.TakeDamage( DamageInfo.Generic( player.Health + 1 ) ); // TODO : Uncomment, so the player gets damaged again
			}
		}
	}

	public override void Setup()
	{
		base.Setup();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is MinimalPlayer player )
			{
				player.Velocity = 0;
				AddPlayer( player );
				player.CanMove = false;
			}
		}
	}

	public override void Start()
	{
		base.Start();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is MinimalPlayer player )
			{
				player.CanMove = true;
			}
		}
	}

	public override void AddPlayer( MinimalPlayer player )
	{
		Log.Info( "RedLightGreenLight::AddPlayer" );
		Log.Info( "TimerList : " + timerList.Count );

		player.currentGameModeClient = new RedLightGreenLightClient();
		player.currentGameModeClient.minimalPlayer = player;

		if ( playerSpawnPointList.Count > 0 )
		{
			player.Transform = playerSpawnPointList[Rand.Next( 0, playerSpawnPointList.Count )];
		}

		player.currentGameModeClient.Init();
	}

	public override string GetGameText()
	{
		return "________________" + MovementAllowed.ToString();
	}
}
