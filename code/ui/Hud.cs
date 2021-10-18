using Sandbox;
using Sandbox.UI;
using SquidGame.UI;

[Library]
public partial class Hud : HudEntity<RootPanel>
{
	public Hud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/Hud.scss" );

		RootPanel.AddChild<GameStatePanel>();
		RootPanel.AddChild<Health>();
		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<HoneyComb>();

		RootPanel.Style.Dirty();
	}
}
