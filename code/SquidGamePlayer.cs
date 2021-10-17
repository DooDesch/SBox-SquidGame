using Sandbox;
using System.Linq;
using System.Collections.Generic;
using SquidGame.OriginSandbox;

namespace SquidGame
{
	public partial class SquidGamePlayer : Player
	{
		private DamageInfo lastDamage;

		public AbstractGameMode CurrentGameMode { get; set; }
		public AbstractGameModeClient CurrentGameModeClient { get; set; } = new NullGameModeClient();
		[Net, Predicted] public ICamera MainCamera { get; set; }
		[Net] public Transform SpawnPosition { get; set; }

		public ICamera LastCamera { get; set; }

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
		public SquidGamePlayer()
		{
			Inventory = new Inventory( this );
		}

		/// <summary>
		/// Initialize using this client
		/// </summary>
		public SquidGamePlayer( Client cl ) : this()
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );
		}

		public override void Spawn()
		{
			MainCamera = new ThirdPersonCamera();
			LastCamera = MainCamera;

			base.Spawn();
		}

		public override void Respawn()
		{
			if ( !CanRespawn ) return;

			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new SquidGameWalkController( this );
			Animator = new StandardPlayerAnimator();

			MainCamera = LastCamera;
			Camera = MainCamera;

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Clothing.DressEntity( this );

			Inventory.Add( new Fists() );

			base.Respawn();
		}

		public override void OnKilled()
		{
			base.OnKilled();

			BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
			LastCamera = MainCamera;
			MainCamera = new SpectateRagdollCamera();
			Camera = MainCamera;
			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;

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

		public ICamera GetActiveCamera()
		{
			// if ( VehicleCamera != null ) return VehicleCamera;

			return MainCamera;
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

			if ( LifeState != LifeState.Alive )
				return;

			// TickPlayerUse();
			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
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

			ent.DeleteAsync( 90.0f );
		}
	}
}
