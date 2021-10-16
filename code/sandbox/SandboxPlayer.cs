using Sandbox;

namespace SquidGame.OriginSandbox
{
	partial class SandboxPlayer : Player
	{
		private TimeSince timeSinceDropped;
		// private TimeSince timeSinceJumpReleased;

		private DamageInfo lastDamage;

		[Net] public PawnController VehicleController { get; set; }
		[Net] public PawnAnimator VehicleAnimator { get; set; }
		[Net, Predicted] public ICamera VehicleCamera { get; set; }
		[Net, Predicted] public Entity Vehicle { get; set; }
		[Net, Predicted] public ICamera MainCamera { get; set; }

		public ICamera LastCamera { get; set; }


		/// <summary>
		/// The clothing container is what dresses the citizen
		/// </summary>
		public Clothing.Container Clothing = new();

		/// <summary>
		/// Default init
		/// </summary>
		public SandboxPlayer()
		{
			Inventory = new Inventory( this );
		}

		/// <summary>
		/// Initialize using this client
		/// </summary>
		public SandboxPlayer( Client cl ) : this()
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );
		}

		public override void Spawn()
		{
			MainCamera = new FirstPersonCamera();
			LastCamera = MainCamera;

			base.Spawn();
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();

			MainCamera = LastCamera;
			Camera = MainCamera;

			if ( DevController is NoclipController )
			{
				DevController = null;
			}

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Clothing.DressEntity( this );

			// Inventory.Add( new PhysGun(), true );
			// Inventory.Add( new GravGun() );
			// Inventory.Add( new Tool() );
			// Inventory.Add( new Pistol() );
			// Inventory.Add( new Flashlight() );
			Inventory.Add( new Fists() );

			base.Respawn();
		}

		public override void OnKilled()
		{
			base.OnKilled();

			if ( lastDamage.Flags.HasFlag( DamageFlags.Vehicle ) )
			{
				Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", lastDamage.Position );
				Particles.Create( "particles/impact.flesh-big.vpcf", lastDamage.Position );
				PlaySound( "kersplat" );
			}

			VehicleController = null;
			VehicleAnimator = null;
			VehicleCamera = null;
			Vehicle = null;

			BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
			LastCamera = MainCamera;
			MainCamera = new SpectateRagdollCamera();
			Camera = MainCamera;
			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;

			Inventory.DropActive();
			Inventory.DeleteContents();
		}

		public override void TakeDamage( DamageInfo info )
		{
			if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
			{
				info.Damage *= 10.0f;
			}

			lastDamage = info;

			TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );

			base.TakeDamage( info );
		}

		[ClientRpc]
		public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
		{
		}

		public override PawnController GetActiveController()
		{
			if ( VehicleController != null ) return VehicleController;
			if ( DevController != null ) return DevController;

			return base.GetActiveController();
		}

		public override PawnAnimator GetActiveAnimator()
		{
			if ( VehicleAnimator != null ) return VehicleAnimator;

			return base.GetActiveAnimator();
		}

		public ICamera GetActiveCamera()
		{
			if ( VehicleCamera != null ) return VehicleCamera;

			return MainCamera;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( Input.ActiveChild != null )
			{
				ActiveChild = Input.ActiveChild;
			}

			if ( LifeState != LifeState.Alive )
				return;

			if ( VehicleController != null && DevController is NoclipController )
			{
				DevController = null;
			}

			var controller = GetActiveController();
			if ( controller != null )
				EnableSolidCollisions = !controller.HasTag( "noclip" );

			TickPlayerUse();
			SimulateActiveChild( cl, ActiveChild );

			if ( Input.Pressed( InputButton.View ) )
			{
				if ( MainCamera is not FirstPersonCamera )
				{
					MainCamera = new FirstPersonCamera();
				}
				else
				{
					MainCamera = new ThirdPersonCamera();
				}
			}

			Camera = GetActiveCamera();

			if ( Input.Pressed( InputButton.Drop ) )
			{
				var dropped = Inventory.DropActive();
				if ( dropped != null )
				{
					dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRot.Forward * 500.0f + Vector3.Up * 100.0f, true );
					dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

					timeSinceDropped = 0;
				}
			}

			// if ( Input.Released( InputButton.Jump ) )
			// {
			// 	if ( timeSinceJumpReleased < 0.3f )
			// 	{
			// 		Game.Current?.DoPlayerNoclip( cl );
			// 	}

			// 	timeSinceJumpReleased = 0;
			// }

			// if ( Input.Left != 0 || Input.Forward != 0 )
			// {
			// 	timeSinceJumpReleased = 1;
			// }
		}

		public override void StartTouch( Entity other )
		{
			if ( timeSinceDropped < 1 ) return;

			base.StartTouch( other );
		}

		[ServerCmd( "inventory_current" )]
		public static void SetInventoryCurrent( string entName )
		{
			var target = ConsoleSystem.Caller.Pawn;
			if ( target == null ) return;

			var inventory = target.Inventory;
			if ( inventory == null )
				return;

			for ( int i = 0; i < inventory.Count(); ++i )
			{
				var slot = inventory.GetSlot( i );
				if ( !slot.IsValid() )
					continue;

				if ( !slot.ClassInfo.IsNamed( entName ) )
					continue;

				inventory.SetActiveSlot( i, false );

				break;
			}
		}

		[ClientRpc]
		private void BecomeRagdollOnClient( Vector3 velocity, DamageFlags damageFlags, Vector3 forcePos, Vector3 force, int bone )
		{
			var ent = new ModelEntity
			{
				Position = Position,
				Rotation = Rotation,
				Scale = Scale,
				MoveType = MoveType.Physics,
				UsePhysicsCollision = true,
				EnableAllCollisions = true,
				CollisionGroup = CollisionGroup.Debris
			};
			ent.SetModel( GetModelName() );
			ent.CopyBonesFrom( this );
			ent.CopyBodyGroups( this );
			ent.CopyMaterialGroup( this );
			ent.TakeDecalsFrom( this );
			ent.EnableHitboxes = true;
			ent.EnableAllCollisions = true;
			ent.SurroundingBoundsMode = SurroundingBoundsType.Physics;
			ent.RenderColor = RenderColor;
			ent.PhysicsGroup.Velocity = velocity;

			if ( Local.Pawn == this )
			{
				//ent.EnableDrawing = false; wtf
			}

			ent.SetInteractsAs( CollisionLayer.Debris );
			ent.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
			ent.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

			foreach ( var child in Children )
			{
				if ( !child.Tags.Has( "clothes" ) ) continue;
				if ( child is not ModelEntity e ) continue;

				var model = e.GetModelName();

				var clothing = new ModelEntity();
				clothing.SetModel( model );
				clothing.SetParent( ent, true );
				clothing.RenderColor = e.RenderColor;
				clothing.CopyBodyGroups( e );
				clothing.CopyMaterialGroup( e );
			}

			if ( damageFlags.HasFlag( DamageFlags.Bullet ) ||
				 damageFlags.HasFlag( DamageFlags.PhysicsImpact ) )
			{
				PhysicsBody body = bone > 0 ? ent.GetBonePhysicsBody( bone ) : null;

				if ( body != null )
				{
					body.ApplyImpulseAt( forcePos, force * body.Mass );
				}
				else
				{
					ent.PhysicsGroup.ApplyImpulse( force );
				}
			}

			if ( damageFlags.HasFlag( DamageFlags.Blast ) )
			{
				if ( ent.PhysicsGroup != null )
				{
					ent.PhysicsGroup.AddVelocity( (Position - (forcePos + Vector3.Down * 100.0f)).Normal * (force.Length * 0.2f) );
					var angularDir = (Rotation.FromYaw( 90 ) * force.WithZ( 0 ).Normal).Normal;
					ent.PhysicsGroup.AddAngularVelocity( angularDir * (force.Length * 0.02f) );
				}
			}

			Corpse = ent;

			ent.DeleteAsync( 10.0f );
		}

		// TODO

		//public override bool HasPermission( string mode )
		//{
		//	if ( mode == "noclip" ) return true;
		//	if ( mode == "devcam" ) return true;
		//	if ( mode == "suicide" ) return true;
		//
		//	return base.HasPermission( mode );
		//	}
	}
}
