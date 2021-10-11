using System;
using System.Collections.Generic;
using MinimalExample;
using Sandbox;


public abstract partial class AbstractGameMode : Entity
{
	public static List<GameTimer> timerList { get; set; } = new List<GameTimer>();

	public enum GAME_STATE
	{
		NOT_STARTED,
		READY,
		STARTING,
		RUNNING,
		NULL
	}
	[Net] public GAME_STATE gameState { get; set; }
	[Net] public TimeSince timeSinceStarted { get; set; }
	[Net] public int maxTime { get; set; } = 300;
	[Net] public List<Transform> playerSpawnPointList { get; set; } = new List<Transform>();
	[Net] public Random Rand { get; set; }
	public AbstractGameMode()
	{
		Rand = new Random( DateTime.Now.ToString().GetHashCode() );
	}

	public virtual void Init()
	{
		gameState = GAME_STATE.STARTING;
	}

	public virtual void OnTick()
	{
		if ( !gameState.Equals( GAME_STATE.RUNNING ) )
		{
			timeSinceStarted = 0;
		}
	}

	public abstract void AddPlayer( MinimalPlayer player );
	public abstract string GetGameText();
}
