using Sandbox;
using Sandbox.UI;

[Library]
public partial class SquidGameHudEntity : HudEntity<RootPanel>
{
	public SquidGameHudEntity()
	{
		if ( !IsClient )
			return;
		RootPanel.StyleSheet.Load( "/ui/Hud.scss" );
		RootPanel.AddChild<GameStatePanel>();
		RootPanel.AddChild<Health>();
	}
}
