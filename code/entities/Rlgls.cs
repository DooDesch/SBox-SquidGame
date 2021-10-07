using Sandbox;

[Library( "info_rlgl_spawn", Description = "Spawnpoint for red light green light game" )]
[Hammer.EntityTool( "RlGl Spawnpoint", "Player", "Defines a point where a defined entity can (re)spawn" )]
public partial class Rlgls : Entity
{
	[Property( Title = "SpawnPoint Type" )]
	public RlGlsEnum Type { get; set; } = RlGlsEnum.PLAYER;
}
