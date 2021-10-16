using System;
using System.Linq;
using System.Collections.Generic;
using SquidGame;
using Sandbox;

public class TimeUntil
{
	public int GameSetup = 120;
	public int GameStarts = 120;
	public int GameEnds = 300;
	public int NextGame = 120;
}

public class GameStateTimer
{
	public TimeSince Ready;
	public TimeSince Setup;
	public TimeSince Started;
	public TimeSince Ended;
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
	[Net] public TimeUntil TimeUntil { get; set; } = new TimeUntil();
	[Net] public GAME_STATE GameState { get; set; } = GAME_STATE.NOT_STARTED;
	[Net] public GameStateTimer GameStateTimer { get; set; } = new GameStateTimer();

	/// <summary>
	/// Set to define entities that belong to this gamemode
	/// </summary>
	/// <value></value>
	protected string Tag { get; set; }

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
		if ( GameState.Equals( GAME_STATE.READY ) && GameStateTimer.Ready > TimeUntil.GameSetup )
		{
			// Game is ready to start, it'll be setup (e.g. all players will be teleported)
			Setup();
		}
		else if ( GameState.Equals( GAME_STATE.SETUP ) && GameStateTimer.Setup > TimeUntil.GameStarts )
		{
			// All players are already teleported and introduced to the game, the game will start now
			Start();
		}
		else if ( GameState.Equals( GAME_STATE.STARTING ) && GameStateTimer.Started > TimeUntil.GameEnds )
		{
			// The time is over, the game ends now
			End();
		}
	}

	public virtual void Ready()
	{
		Log.Info( "AbstractGameMode::Ready" );

		Log.Info( "Entities : " + Entity.All.OfType<SgSp>().Count() );
		foreach ( SgSp entity in Entity.All.OfType<SgSp>() )
		{
			HandleSgSpEntity( entity );
		}

		GameState = GAME_STATE.READY;
		GameStateTimer.Ready = 0;
		UpdateGameTimers( TimeUntil.GameSetup );
	}

	public virtual void Setup()
	{
		Log.Info( "AbstractGameMode::Setup" );

		GameState = GAME_STATE.SETUP;
		GameStateTimer.Setup = 0;
		UpdateGameTimers( TimeUntil.GameStarts );
	}

	public virtual void Start()
	{
		Log.Info( "AbstractGameMode::Start" );

		GameState = GAME_STATE.STARTING;
		GameStateTimer.Started = 0;
		UpdateGameTimers( TimeUntil.GameEnds );
	}

	public virtual void End()
	{
		Log.Info( "AbstractGameMode::End" );

		GameState = GAME_STATE.ENDING;
		GameStateTimer.Ended = 0;
		UpdateGameTimers( TimeUntil.NextGame );

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is SquidGamePlayer player )
			{
				player.CanMove = true;
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
		Log.Info( "Checking entity " + entity.Name );

		if ( !entity.Tags.Has( Tag ) ) return;

		if ( entity.Type.Equals( SgSpEnum.PLAYER ) )
		{
			Log.Info( "Found spawnpoint, adding" );
			PlayerSpawnPoints.Add( entity.Transform );
		}

		// TODO : Add Entity-Type GUNNER
		// TODO : Add Entity-Type OVERSEER
	}
	public virtual void UpdateGameTimers( int time )
	{
		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is SquidGamePlayer player )
			{
				player.UpdateGameTimers( time );
			}
		}
	}

	public abstract void AddPlayer( SquidGamePlayer player );
	public abstract string GetGameText();
}
