using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SquidGame;

partial class TimerUI : WorldPanel
{
	public Label Label;
	public SquidGamePlayer Player { get; set; }

	public int maxTime { get; set; } = 70;
	public int timeSinceStarted { get; set; } = 0;

	public TimerUI() : base()
	{
		SetPanelBounds();

		if ( Local.Pawn is SquidGamePlayer player )
		{
			Player = player;
		}

		StyleSheet.Load( "/world-ui/World-UI.scss" );
		Label = Add.Label( "100" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Player is not null && Player.CurrentGameMode is not null )
		{
			timeSinceStarted = (int)Player.CurrentGameMode.GameStateTimer;
			maxTime = Player.CurrentGameMode.NextGameStateTime;
		}

		int diff = maxTime - timeSinceStarted;
		if ( diff < 0 ) diff = 0;
		string minutes = (diff / 60).ToString().PadLeft( 2, '0' );
		string seconds = (diff % 60).ToString().PadLeft( 2, '0' );
		Label.Text = $"{minutes}:{seconds}";

		SetPanelBounds();

		Style.Dirty();
	}

	private void SetPanelBounds()
	{
		var w = 2300;
		var h = 1160;
		PanelBounds = new Rect( -(w / 2), -110 - (h / 2), w, h );
	}
}
