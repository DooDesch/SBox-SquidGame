using Sandbox;

namespace SquidGame.OriginSandbox
{
	[Library( "weapon_fists", Title = "Fists", Spawnable = false )]
	partial class Fists : Weapon
	{
		public override string ViewModelPath => "models/firstperson/temp_punch/temp_punch.vmdl";
		public override float PrimaryRate => 2.0f;
		public override float SecondaryRate => 2.0f;

		public override bool CanReload()
		{
			return false;
		}

		private void Attack( bool leftHand )
		{
			if ( MeleeAttack() )
			{
				OnMeleeHit( leftHand );
			}
			else
			{
				OnMeleeMiss( leftHand );
			}

			(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );
		}

		public override void AttackPrimary()
		{
			Attack( true );
		}

		public override void AttackSecondary()
		{
			Attack( false );
		}

		public override void OnCarryDrop( Entity dropper )
		{
		}

		public override void SimulateAnimator( PawnAnimator anim )
		{
			anim.SetParam( "holdtype", 5 );
			anim.SetParam( "aimat_weight", 1.0f );
		}

		private bool MeleeAttack()
		{
			var forward = Owner.EyeRot.Forward;
			forward = forward.Normal;

			bool hit = false;

			foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * 80, 20.0f ) )
			{
				if ( !tr.Entity.IsValid() ) continue;

				tr.Surface.DoBulletImpact( tr );

				hit = true;

				if ( !IsServer ) continue;

				using ( Prediction.Off() )
				{
					var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100, 25 )
						.UsingTraceResult( tr )
						.WithAttacker( Owner )
						.WithWeapon( this );

					tr.Entity.TakeDamage( damageInfo );
				}
			}

			return hit;
		}

		[ClientRpc]
		private void OnMeleeMiss( bool leftHand )
		{
			Host.AssertClient();

			if ( IsLocalPawn )
			{
				_ = new Sandbox.ScreenShake.Perlin();
			}

			ViewModelEntity?.SetAnimBool( "attack", true );
			ViewModelEntity?.SetAnimFloat( "holdtype_attack", leftHand ? 2 : 1 );
		}

		[ClientRpc]
		private void OnMeleeHit( bool leftHand )
		{
			Host.AssertClient();

			if ( IsLocalPawn )
			{
				_ = new Sandbox.ScreenShake.Perlin( 1.0f, 1.0f, 3.0f );
			}

			ViewModelEntity?.SetAnimBool( "attack", true );
			ViewModelEntity?.SetAnimFloat( "holdtype_attack", leftHand ? 2 : 1 );
		}
	}
}
