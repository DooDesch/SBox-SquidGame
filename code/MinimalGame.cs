
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using SquidGame.Entities;

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
		[Net] public AbstractGameMode CurrentGameMode { get; set; } = new NullGameMode();
		private Type CurrentGameModeClient { get; set; }

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
				CurrentGameMode = new RedLightGreenLight();
				CurrentGameModeClient = typeof( RedLightGreenLightClient );

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
			player.currentGameMode = CurrentGameMode;
			client.Pawn = player;
			player.Respawn();
			if ( IsServer )
			{
				ClientSpawn();
			}
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( IsServer )
			{
				CurrentGameMode.OnTick();
			}
		}

		[Event.Hotload]
		public void debugOutput()
		{
			// foreach ( var entity in BaseTrigger.All.OfType<Zone>() )
			// {
			// 	if ( entity.Tags.Has( "RedLightGreenLight" ) )
			// 	{
			// 		Log.Info( entity.Name );
			// 	}
			// }
		}

		[Event.Entity.PostSpawn]
		public void Init()
		{
			Log.Info( "MinimalGame::Init" );

			if ( IsServer )
			{
				Log.Info( "ServerSide Call.." );
				return;
			};

			if ( AbstractGameMode.timerList.Count == 0 )
			{
				return;
			}

			Log.Info( AbstractGameMode.timerList.Count );

			foreach ( GameTimer timer in AbstractGameMode.timerList )
			{
				Log.Info( "Creating new Timer" );
				TimerUI timerUI = new TimerUI();
				timerUI.Transform = timer.Transform;
			}
		}

		public override void PostLevelLoaded()
		{
			Log.Info( "MinimalGame::PostLevelLoaded" );

			base.PostLevelLoaded();

			if ( IsServer )
			{
				// if ( timeToStart >= 10 && CurrentGameMode.gameState == AbstractGameMode.GAME_STATE.READY )
				// {
				// 	CurrentGameMode.Init();
				// }
				CurrentGameMode.Init();
			}
		}
	}

}
