using Sandbox;
using System;
using System.Linq;

namespace MinimalExample
{
	public partial class MinimalPlayer : Player
	{

		[Net] public AbstractGameMode currentGameMode { get; set; } = new NullGameMode();
		[Net] public AbstractGameModeClient currentGameModeClient { get; set; } = new NullGameModeClient();
		/// <summary>
		/// The clothing container is what dresses the citizen
		/// </summary>
		public Clothing.Container Clothing = new();

		/// <summary>
		/// Default init
		/// </summary>
		public MinimalPlayer()
		{
			
		}

		/// <summary>
		/// Initialize using this client
		/// </summary>
		public MinimalPlayer( Client cl )
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();

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
			if (cl.Pawn is MinimalPlayer player)
			{
				player.currentGameModeClient.isMoving = player.Velocity.Length > 0;
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

		public override void ClientSpawn()
		{
			base.ClientSpawn();
			//Log.Info( All.OfType<GameTimer>().ToList().Count );
			foreach ( GameTimer timer in All.OfType<GameTimer>())
			{
				TimerUI timerPanel = new TimerUI(this);
				timerPanel.Transform = timer.Transform;
			}
		}
	}
}
