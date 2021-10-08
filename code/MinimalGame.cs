
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace MinimalExample
{

	/// <summary>
	/// This is your game class. This is an entity that is created serverside when
	/// the game starts, and is replicated to the client. 
	/// 
	/// You can use this to create things like HUDs and declare which player class
	/// to use for spawned players.
	/// </summary>
	public partial class MinimalGame : Sandbox.Game
	{
		[Net] public AbstractGameMode currentGameMode { get; set; } = new NullGameMode();
		private Type currentGameModeClient { get; set; }

		private TimeSince timeToStart = 0;

		public MinimalGame()
		{
			if ( IsServer )
			{
				Log.Info( "My Gamemode Has Created Serverside!" );

				// Create a HUD entity. This entity is globally networked
				// and when it is created clientside it creates the actual
				// UI panels. You don't have to create your HUD via an entity,
				// this just feels like a nice neat way to do it.
				new MinimalHudEntity();
				currentGameMode = new RedLightGreenLight();
				currentGameModeClient = typeof( RedLightGreenLightClient );

			}

			if ( IsClient )
			{
				Log.Info( "My Gamemode Has Created Clientside!" );
			}
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			MinimalPlayer player = new MinimalPlayer();
			player.currentGameMode = currentGameMode;
			client.Pawn = player;
			player.Respawn();
			if(IsServer)
			{
				ClientSpawn();
			}
		}

		public override void Simulate( Client cl )
		{
			if ( IsServer )
			{
				currentGameMode.OnTick();
				if (timeToStart >= 10 && currentGameMode.gameState == AbstractGameMode.GAME_STATE.READY)
				{
					currentGameMode.Init();
				}
			}
			base.Simulate( cl );
		}

		public override void PostLevelLoaded()
		{
			Log.Info( "PostLevelLoaded" );
			base.PostLevelLoaded();
			currentGameMode.gameState = AbstractGameMode.GAME_STATE.READY;
			if ( IsServer )
			{
				foreach ( Rlgls entity in All.OfType<Rlgls>() )
				{
					if (entity.Type.Equals(RlGlsEnum.PLAYER))
					{
						currentGameMode.playerSpawnPointList.Add( entity.Transform );
					}
				}

				foreach ( GameTimer timer in All.OfType<GameTimer>() )
				{
					AbstractGameMode.timerList.Add( timer );
				}
			}
		}
	}

}
