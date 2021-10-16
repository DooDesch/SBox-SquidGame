using Sandbox;
using System;

namespace SquidGame.Entities
{
	public class RlGlDoll : Prop
	{
		public RlGlDoll() { }

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/props_squidgame/squidGameDoll/squidgamedoll.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			Scale = 0.2f;
		}

		public void SayRedLightGreenLight()
		{
			Log.Info( "RlGlDoll::SayRedLightGreenLight" );
			PlaySound( "doll_sound_speaking" );
			// Sound.FromEntity( "doll_sound_speaking", this );
		}

		public void TurnAround()
		{
			Log.Info( "RlGlDoll::TurnAround" );
			Rotation = Rotation.RotateAroundAxis( Vector3.Up, 180f );
			PlaySound( "doll_sound_ending" );
			// Sound.FromEntity( "doll_sound_ending", this );
		}

		public void TurnBack()
		{
			Log.Info( "RlGlDoll::TurnBack" );
			Rotation = Rotation.RotateAroundAxis( Vector3.Up, 180f );
		}
	}
}
