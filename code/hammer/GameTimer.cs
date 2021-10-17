using Sandbox;
using SquidGame;

[Library( "info_gametimer", Description = "Spawnpoint for Timer" )]
[Hammer.DrawAngles]
[Hammer.EditorSprite( "editor/snd_event.vmat" )]
[Hammer.EntityTool( "Game Timer", "Effects", "Spawns a Panel with a Timer/Countdown" )]
public partial class GameTimer : Entity
{
	private TimerUI TimerUI;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		TimerUI = new();
		TimerUI.Transform = Transform;
		TimerUI.Transform = TimerUI.Transform.WithPosition( TimerUI.Transform.Position + TimerUI.Transform.Rotation.Forward * 0.05f );

		// if ( Local.Client.Pawn is SquidGamePlayer player )
		// {
		// 	player.GameTimers.Add( this );
		// }
	}
}
