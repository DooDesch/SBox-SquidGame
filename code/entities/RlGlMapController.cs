using Sandbox;

[Library( "logic_rlgl", Description = "Logic entity for the RlGl Gamemode" )]
[Hammer.EntityTool( "Logic RlGl", "Logic", "Get event calls to change states on the map" )]
[Hammer.AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
public partial class RlGlMapController : Entity
{
	protected Output OpenRoof { get; set; }

	/// <summary>
	/// This will open the roof on the RlGl Map, usually before the beginning of the Game
	/// </summary>
	[Input( Name = "OpenTheRoof" )]
	public virtual void OpenTheRoof( Entity activator )
	{
		OpenRoof.Fire( activator );
	}

	protected Output CloseRoof { get; set; }
	/// <summary>
	/// This will close the roof on the RlGl Map, usually at the end of the Game
	/// </summary>
	[Input( Name = "CloseTheRoof" )]
	public virtual void CloseTheRoof( Entity activator )
	{
		CloseRoof.Fire( activator );
	}
}
