using Sandbox;

[Library( "info_gametimer", Description = "Spawnpoint for Timer" )]
[Hammer.DrawAngles]
[Hammer.EditorSprite( "editor/snd_event.vmat" )]
[Hammer.EntityTool( "Game Timer", "Effects", "Spawns a Panel with a Timer/Countdown" )]
public partial class GameTimer : Entity
{
	TimerUI TimerUI;

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
	}

	public void UpdateTimer( int time )
	{
		TimerUI.UpdateTimer( time );
	}
}
