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

	public AbstractGameMode()
	{
	}

	public abstract void OnTick();

	public abstract void AddPlayer( MinimalPlayer player );
	public abstract string GetGameText();
}
