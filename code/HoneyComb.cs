using Sandbox;
using SquidGame;
using SquidGame.Entities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SquidGame.Games
{
	public partial class HoneyComb : AbstractGameMode
	{
		public HoneyComb()
		{
			Log.Warning( "HoneyComb::Constructor" );
			Tag = "HoneyComb";
			GameState = GAME_STATE.NOT_STARTED;
			TimeUntil = new TimeUntil
			{
				GameSetup = 5,
				GameStarts = 5,
				GameEnds = 60,
				// GameEnds = 360,
				NextGame = 15,
			};
		}

		public override void Init()
		{
			base.Init();

			Log.Warning( "HoneyComb::Init" );
			Log.Info( "GameState : " + GameState.ToString() );
		}

		public override void OnTick()
		{
			base.OnTick();

			if ( !GameState.Equals( GAME_STATE.STARTING ) ) return;

			// TODO : Add some honeycomb winning logic?!
		}

		public override void Ready()
		{
			base.Ready();
		}

		public override void Setup()
		{
			base.Setup();

			foreach ( Client client in Client.All )
			{
				if ( client.Pawn is SquidGamePlayer player )
				{
					player.Velocity = 0;
					AddPlayer( player );
					// player.CanMove = false;
				}
			}
		}

		public override void Start()
		{
			base.Start();

			// foreach ( Client client in Client.All )
			// {
			// 	if ( client.Pawn is SquidGamePlayer player )
			// 	{
			// 		player.CanMove = true;
			// 		player.CanSprint = false;
			// 		player.CanRespawn = false;
			// 	}
			// }
		}

		public override void AddPlayer( SquidGamePlayer player )
		{
			player.CurrentGameModeClient = new HoneyCombClient
			{
				Player = player
			};

			if ( PlayerSpawnPoints.Count > 0 )
			{
				player.Transform = PlayerSpawnPoints[Rand.Next( 0, PlayerSpawnPoints.Count )];
			}

			player.CurrentGameModeClient.Init();
		}

		public override string GetGameText()
		{
			return "";
		}
	}
}
