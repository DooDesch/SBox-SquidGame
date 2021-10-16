using Sandbox;
using SquidGame;
using SquidGame.Entities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SquidGame.Games
{
	public partial class RedLightGreenLight : AbstractGameMode
	{
		[Net] public bool MovementAllowed { get; set; } = true;
		[Net] public RlGlDoll Doll { get; set; }

		private bool DollDone = true;

		public RedLightGreenLight()
		{
			Log.Info( "RedLightGreenLight::Constructor" );
			Tag = "RedLightGreenLight";
			GameState = GAME_STATE.NOT_STARTED;
			TimeUntil = new TimeUntil
			{
				GameSetup = 5,
				GameStarts = 5,
				GameEnds = 60,
			};
			Doll = new RlGlDoll();
		}

		public override void Init()
		{
			base.Init();

			Log.Info( "RedLightGreenLight::Init" );
			Log.Info( "GameState : " + GameState.ToString() );
		}

		public override void OnTick()
		{
			base.OnTick();

			if ( !GameState.Equals( GAME_STATE.STARTING ) ) return;



			// MovementAllowed = GameStateTimer.Started % 10 > 1;
			DollRound();
			if ( MovementAllowed ) return;

			foreach ( Client client in Client.All )
			{
				if ( client.Pawn is SquidGamePlayer player )
				{
					if ( player.LifeState != LifeState.Alive ) return;

					if ( player.CurrentGameModeClient.HasWon ) return;

					if ( !player.CurrentGameModeClient.IsMoving ) return;

					ShootPlayer( player );
				}
			}
		}

		public async void ShootPlayer( SquidGamePlayer player )
		{
			// Log.Info("You dead");
			await GameTask.DelaySeconds( .1f );
			Doll.PlaySound( "rust_pistol.shoot" ).SetRandomPitch( .93f, 1.07f );
			player.TakeDamage( DamageInfo.Generic( player.Health ) ); // TODO : Uncomment, so the player gets damaged again
		}

		public async void DollRound()
		{
			if ( !DollDone ) return;
			Log.Info( "RedLightGreenLight::DollRound" );
			Log.Info( "Starting new doll round" );
			DollDone = false;

			await GameTask.DelaySeconds( .1f );
			Doll.SayRedLightGreenLight();
			await GameTask.DelaySeconds( 4f );

			float secondsUntilShooting = .3f;
			Doll.TurnAround();
			await GameTask.DelaySeconds( secondsUntilShooting );
			MovementAllowed = false;

			float secondsUntilTurningBack = 3f;
			await GameTask.DelaySeconds( secondsUntilTurningBack );
			MovementAllowed = true;
			Doll.TurnBack();
			DollDone = true;
			Log.Info( "Doll round is done" );
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
					player.CanMove = false;
				}
			}
		}

		public override void Start()
		{
			base.Start();

			foreach ( Client client in Client.All )
			{
				if ( client.Pawn is SquidGamePlayer player )
				{
					player.CanMove = true;
					player.CanSprint = false;
					player.CanRespawn = false;
				}
			}
		}

		public override void AddPlayer( SquidGamePlayer player )
		{
			player.CurrentGameModeClient = new RedLightGreenLightClient
			{
				Player = player
			};

			if ( PlayerSpawnPoints.Count > 0 )
			{
				player.Transform = PlayerSpawnPoints[Rand.Next( 0, PlayerSpawnPoints.Count )];
			}

			player.CurrentGameModeClient.Init();
		}

		public override void HandleSgSpEntity( SgSp entity )
		{
			base.HandleSgSpEntity( entity );

			if ( entity.Type.Equals( SgSpEnum.DOLL ) )
			{
				Doll.Position = entity.Position;
				Doll.Rotation = entity.Rotation;
			}
		}

		public override string GetGameText()
		{
			return "________________" + MovementAllowed.ToString();
		}
	}
}
