using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SquidGame;

class TimerUI : WorldPanel
{
	public Label Label;
	public SquidGamePlayer Player { get; set; }

	private int MaxTime { get; set; } = 70;
	private int TimeSinceStarted { get; set; } = 0;

	public TimerUI() : base()
	{
		SetPanelBounds();

		if ( Local.Pawn is SquidGamePlayer player )
		{
			Player = player;
		}

		StyleSheet.Load( "/world-ui/World-UI.scss" );
		Label = Add.Label( "00:00" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Player is not null && Player.CurrentGameMode is not null )
		{
			TimeSinceStarted = (int)Player.CurrentGameMode.GameStateTimer;
			MaxTime = Player.CurrentGameMode.NextGameStateTime;
		}

		int diff = MaxTime - TimeSinceStarted;
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
