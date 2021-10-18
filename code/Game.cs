
using Sandbox;
using System;
using SquidGame.Games;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace SquidGame
{

	/// <summary>
	/// This is your game class. This is an entity that is created serverside when
	/// the game starts, and is replicated to the client. 
	/// 
	/// You can use this to create things like HUDs and declare which player class
	/// to use for spawned players.
	/// </summary>
	[Library( "squidgame", Title = "SquidGame" )]
	public partial class Game : Sandbox.Game
	{
		public Hud Hud { get; set; }

		public enum GAME_PHASE
		{
			NULL,
			RLGL,
			HOCO,
			TOW,
			MARBLES,
			BRIDGE,
			SQUID,
		}

		[Net] public GAME_PHASE GamePhase { get; set; } = GAME_PHASE.NULL;
		[Net] public AbstractGameMode CurrentGameMode { get; set; } = new NullGameMode();
		private Type CurrentGameModeClient { get; set; } = typeof( NullGameModeClient );

		public Game()
		{
			if ( IsServer ) Hud = new();
		}

		[Event( "SquidGame.NextPhase" )]
		public void NextGame()
		{
			Log.Warning( "SquidGame::NextGame" );
			Log.Info( "Starting the next game!!" );

			if ( GamePhase.Equals( GAME_PHASE.NULL ) )
			{
				Log.Info( "Next game is RLGL" );
				GamePhase = GAME_PHASE.RLGL;
				CurrentGameMode = new RedLightGreenLight();
				CurrentGameModeClient = typeof( RedLightGreenLightClient );
			}
			else if ( GamePhase.Equals( GAME_PHASE.RLGL ) )
			{
				Log.Info( "Next game is HOCO" );
				GamePhase = GAME_PHASE.HOCO;
				CurrentGameMode = new HoneyComb();
				CurrentGameModeClient = typeof( HoneyCombClient );
			}
			else if ( GamePhase.Equals( GAME_PHASE.HOCO ) )
			{
				Log.Info( "Next game is NULL" );
				// TODO : Replace with next game
				GamePhase = GAME_PHASE.NULL;
				CurrentGameMode = new NullGameMode();
				CurrentGameModeClient = typeof( NullGameModeClient );
			}

			if ( IsServer )
			{
				CurrentGameMode.Init();
			}
		}

		[Event( "client.tick" )]
		public void UpdatePlayerGameMode()
		{
			if ( IsServer ) return;

			if ( Local.Pawn is SquidGamePlayer player )
			{
				if ( player.CurrentGameMode == CurrentGameMode ) return;
				Log.Warning( "SquidGame::UpdatePlayerGameMode" );
				Log.Info( " Updating client gamemode " );
				player.CurrentGameMode = CurrentGameMode;
				Log.Info( "Current gamemode is now : " + CurrentGameMode.Tag );
			}
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			SquidGamePlayer player = new( cl )
			{
				CurrentGameMode = CurrentGameMode
			};

			cl.Pawn = player;

			player.Respawn();

			player.Tags.Add( "player" );

			if ( IsServer )
			{
				ClientSpawn();
			}

			player.SpawnPosition = player.Transform;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( IsServer )
			{
				CurrentGameMode.OnTick();
			}
		}

		[Event.Entity.PostSpawn]
		public void Init()
		{
			Log.Info( "SquidGame::Init" );

			if ( IsServer )
			{
				Log.Info( "ServerSide Call.." );
				return;
			};
		}

		public override void PostLevelLoaded()
		{
			Log.Info( "SquidGame::PostLevelLoaded" );

			base.PostLevelLoaded();

			if ( IsServer )
			{
				CurrentGameMode.Init();
			}
		}
	}

}
