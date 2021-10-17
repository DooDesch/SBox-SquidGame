using System.Reflection.Metadata;
using System;
using System.Linq;
using System.Collections.Generic;
using Sandbox;
using SquidGame;
using SquidGame.Entities;

public class TimeUntil
{
	public int GameSetup = 120;
	public int GameStarts = 120;
	public int GameEnds = 300;
	public int NextGame = 120;
}

public abstract partial class AbstractGameMode : BaseNetworkable
{
	public enum GAME_STATE
	{
		NOT_STARTED,
		READY,
		SETUP,
		STARTING,
		ENDING,
		NULL
	}

	[Net] public Random Rand { get; set; }
	[Net] public List<Transform> PlayerSpawnPoints { get; set; } = new List<Transform>();
	[Net] public List<Transform> SupervisorSpawnPoints { get; set; } = new List<Transform>();
	[Net] public List<Transform> GunnerSpawnPoints { get; set; } = new List<Transform>();
	[Net] public TimeUntil TimeUntil { get; set; } = new TimeUntil();
	[Net] public GAME_STATE GameState { get; set; } = GAME_STATE.NOT_STARTED;
	[Net] public TimeSince GameStateTimer { get; set; }
	[Net] public int NextGameStateTime { get; set; } = 0;

	/// <summary>
	/// Set to define entities that belong to this gamemode
	/// </summary>
	/// <value></value>
	[Net] protected string Tag { get; set; }

	public AbstractGameMode()
	{
		Rand = new Random( DateTime.Now.ToString().GetHashCode() );
	}

	public virtual void Init()
	{
		Log.Info( "AbstractGameMode::Init" );
		Ready();
	}

	public virtual void OnTick()
	{
		if ( GameState.Equals( GAME_STATE.READY ) && GameStateTimer > TimeUntil.GameSetup )
		{
			// Game is ready to start, it'll be setup (e.g. all players will be teleported)
			Setup();
		}
		else if ( GameState.Equals( GAME_STATE.SETUP ) && GameStateTimer > TimeUntil.GameStarts )
		{
			// All players are already teleported and introduced to the game, the game will start now
			Start();
		}
		else if ( GameState.Equals( GAME_STATE.STARTING ) && GameStateTimer > TimeUntil.GameEnds )
		{
			// The time is over, the game ends now
			End();
		}
	}

	public virtual void Ready()
	{
		Log.Warning( "AbstractGameMode::Ready" );

		foreach ( SgSp entity in Entity.All.OfType<SgSp>() )
		{
			HandleSgSpEntity( entity );
		}

		GameState = GAME_STATE.READY;
		// GameStateTimer = 0;
		UpdateGameTimers( TimeUntil.GameSetup );
	}

	public virtual void Setup()
	{
		Log.Info( "AbstractGameMode::Setup" );

		GameState = GAME_STATE.SETUP;
		// GameStateTimer = 0;
		UpdateGameTimers( TimeUntil.GameStarts );
	}

	public virtual void Start()
	{
		Log.Info( "AbstractGameMode::Start" );

		GameState = GAME_STATE.STARTING;
		// GameStateTimer = 0;
		UpdateGameTimers( TimeUntil.GameEnds );
	}

	public virtual void End()
	{
		Log.Info( "AbstractGameMode::End" );

		GameState = GAME_STATE.ENDING;
		// GameStateTimer = 0;
		UpdateGameTimers( TimeUntil.NextGame );

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is SquidGamePlayer player )
			{
				player.CanMove = true;
				player.CanSprint = true;
				player.CanRespawn = true;

				if ( player.CurrentGameModeClient.HasWon ) continue;

				// Everbody that didn't win the game, will be disqualified
				var dmgInfo = new DamageInfo().WithForce( Vector3.Up );
				dmgInfo.Damage = player.Health;
				player.TakeDamage( dmgInfo );
			}
		}
	}

	public virtual void HandleSgSpEntity( SgSp entity )
	{
		if ( !entity.Tags.Has( Tag ) ) return;

		if ( entity.Type.Equals( SgSpEnum.PLAYER ) )
		{
			PlayerSpawnPoints.Add( entity.Transform );
		}
		else if ( entity.Type.Equals( SgSpEnum.GUNNER ) )
		{
			GunnerSpawnPoints.Add( entity.Transform );
		}
		else if ( entity.Type.Equals( SgSpEnum.SUPERVISOR ) )
		{
			SupervisorSpawnPoints.Add( entity.Transform );
		}
	}
	public virtual void UpdateGameTimers( int time )
	{
		GameStateTimer = 0;
		NextGameStateTime = time;
		// foreach ( Client client in Client.All )
		// {
		// 	if ( client.Pawn is SquidGamePlayer player )
		// 	{
		// 		player.UpdateGameTimers( time );
		// 	}
		// }
	}

	public abstract void AddPlayer( SquidGamePlayer player );
	public abstract string GetGameText();
}
