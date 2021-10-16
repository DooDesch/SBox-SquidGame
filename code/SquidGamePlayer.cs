using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SquidGame
{
	public partial class SquidGamePlayer : Player
	{
		[Net] public List<GameTimer> GameTimers { get; set; } = new List<GameTimer>();

		[Net] public AbstractGameMode CurrentGameMode { get; set; } = new NullGameMode();
		[Net] public AbstractGameModeClient CurrentGameModeClient { get; set; } = new NullGameModeClient();
		/// <summary>
		/// The clothing container is what dresses the citizen
		/// </summary>
		public Clothing.Container Clothing = new();

		public bool CanMove { get; set; } = true;

		public bool CanRespawn { get; set; } = true;

		public bool CanSprint { get; set; } = true;

		/// <summary>
		/// Default init
		/// </summary>
		public SquidGamePlayer() { }

		/// <summary>
		/// Initialize using this client
		/// </summary>
		public SquidGamePlayer( Client cl )
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );

			// foreach ( GameTimer gameTimer in Entity.All.OfType<GameTimer>() )
			// {
			// 	GameTimers.Add( gameTimer );
			// }
		}

		[Event.Entity.PostSpawn]
		private void AddGameTimers()
		{
			Log.Warning( "SquidGamePlayer::AddGameTimers" );
			foreach ( GameTimer gameTimer in All.OfType<GameTimer>() )
			{
				GameTimers.Add( gameTimer );
			}
		}

		public override void Respawn()
		{
			if ( !CanRespawn ) return;

			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new SquidGameWalkController( this );

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new ThirdPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Clothing.DressEntity( this );

			base.Respawn();
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( cl.Pawn is SquidGamePlayer player )
			{
				player.CurrentGameModeClient.IsMoving = player.Velocity.Length > 0;
			}

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}

		public virtual void UpdateGameTimers( int time )
		{
			Log.Warning( "SquidGamePlayer::UpdateGameTimers" );
			Log.Info( "GameTimers : " + GameTimers.Count );


			foreach ( GameTimer gameTimer in All.OfType<GameTimer>() )
			{
				gameTimer.UpdateTimer( time );
			}

			// foreach ( GameTimer gameTimer in GameTimers )
			// {
			// 	gameTimer.UpdateTimer( time );
			// }
		}
	}
}
