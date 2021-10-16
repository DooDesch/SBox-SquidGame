using Sandbox;

[Library( "info_sg_spawn", Description = "Spawnpoint for squid game games" )]
[Hammer.DrawAngles]
[Hammer.EntityTool( "Squid Game Spawnpoint", "Player", "Defines a point where a defined entity can (re)spawn" )]
public partial class SgSp : Entity
{
	[Property( Title = "SpawnPoint Type" )]
	public SgSpEnum Type { get; set; } = SgSpEnum.PLAYER;
}
