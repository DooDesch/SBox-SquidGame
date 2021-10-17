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

		[Net] private bool DollDone { get; set; } = true;

		public RedLightGreenLight()
		{
			Log.Warning( "RedLightGreenLight::Constructor" );
			Tag = "RedLightGreenLight";
			GameState = GAME_STATE.NOT_STARTED;
			TimeUntil = new TimeUntil
			{
				GameSetup = 5,
				GameStarts = 5,
				// GameEnds = 60,
				GameEnds = 360,
			};
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
			await GameTask.DelaySeconds( .1f );

			if ( !Doll.CheckPlayer( player ) ) return;

			if ( GunnerSpawnPoints.Count > 0 )
			{
				Transform gunnerPos = GunnerSpawnPoints[Rand.Next( 0, GunnerSpawnPoints.Count )];

				Gunner gunner = new()
				{
					Transform = gunnerPos
				};

				gunner.ShootAtTarget( player, player.EyePos );
				gunner.Delete();
			}
			else
			{
				Doll.PlaySound( "rust_pistol.shoot" ).SetRandomPitch( .93f, 1.07f );
				player.TakeDamage( DamageInfo.Generic( player.Health ) );
			}
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

		public override void Ready()
		{
			base.Ready();

			GetDoll();
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

		public void GetDoll()
		{
			foreach ( RlGlDoll entity in Entity.All.OfType<RlGlDoll>() )
			{
				if ( !entity.Tags.Has( Tag ) ) return;

				Doll = entity;
			}
		}

		public override string GetGameText()
		{
			return "________________" + MovementAllowed.ToString();
		}
	}
}
