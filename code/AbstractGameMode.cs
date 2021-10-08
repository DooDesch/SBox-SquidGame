using System.Collections.Generic;
using MinimalExample;
using Sandbox;


public abstract partial class AbstractGameMode : Networked
{
	public enum GAME_STATE
	{
		NOT_STARTED,
		STARTING,
		RUNNING,
		NULL
	}
	[Net] public GAME_STATE gameState { get; set; }
	[Net] public TimeSince timeSinceStarted { get; set; }
	[Net] public int maxTime { get; set; } = 300;

	public AbstractGameMode()
	{
	}

	public virtual void OnTick()
	{
		if (!gameState.Equals(GAME_STATE.RUNNING))
		{
			timeSinceStarted = 0;
		}
	}

	public abstract void AddPlayer( MinimalPlayer player );
	public abstract string GetGameText();
}
