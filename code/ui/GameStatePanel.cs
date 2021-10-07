using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using MinimalExample;

public class GameStatePanel : Panel
{
	public Label Label;

	public GameStatePanel()
	{
		Label = Add.Label( "NONE", "value" );
	}

	public override void Tick()
	{
		if (Local.Pawn is MinimalPlayer player)
		{
				Label.Text = "_________" + $"{player.currentGameMode.GetGameText()}";
		}
	}
}
