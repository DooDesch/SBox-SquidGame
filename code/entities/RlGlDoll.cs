using Sandbox;
using System;

namespace SquidGame.Entities
{
	[Library( "sg_rlgl_doll", Description = "Red light green light doll" )]
	[Hammer.DrawAngles]
	[Hammer.EntityTool( "Doll", "RLGL", "Sets the doll position for the rlgl game" )]
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
		}

		public void TurnAround()
		{
			Log.Info( "RlGlDoll::TurnAround" );
			Rotation = Rotation.RotateAroundAxis( Vector3.Up, 180f );
			PlaySound( "doll_sound_ending" );
		}

		public void TurnBack()
		{
			Log.Info( "RlGlDoll::TurnBack" );
			Rotation = Rotation.RotateAroundAxis( Vector3.Up, 180f );
		}

		/// <summary>
		/// Checks if the doll sees the player
		/// </summary>
		public bool CheckPlayer( SquidGamePlayer player )
		{
			// trace 2000 units in eye direction, ignore entity
			TraceResult tr = Trace.Ray( Position + new Vector3( 0, 0, 250 ), player.EyePos )
							.Run();

			// If we hit, draw a 2 inch sphere for 10 seconds
			if ( tr.Hit && tr.Entity.Equals( player ) )
			{
				// DebugOverlay.Line( tr.StartPos, tr.EndPos );
				// DebugOverlay.Sphere( tr.EndPos, 2.0f, Color.Red, duration: 10.0f );
				return true;
			}
			// else if ( tr.Hit )
			// {
			// 	DebugOverlay.Line( tr.StartPos, tr.EndPos );
			// 	DebugOverlay.Sphere( tr.EndPos, 2.0f, Color.Green, duration: 10.0f );
			// }
			return false;
		}
	}
}
