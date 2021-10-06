using Sandbox;
using Sandbox.UI;

[Library]
public partial class MinimalHudEntity : HudEntity<RootPanel>
{
	public MinimalHudEntity()
	{
		if ( !IsClient )
			return;
		RootPanel.StyleSheet.Load( "/ui/Hud.scss" );
		RootPanel.AddChild<GameStatePanel>();
		RootPanel.AddChild<Health>();
	}
}
