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

		if ( Player != null && Player.CurrentGameMode != null )
		{
			timeSinceStarted = (int)Player.CurrentGameMode.GameStateTimer;
			maxTime = (int)Player.CurrentGameMode.NextGameStateTime;
		}

		int diff = maxTime - (int)timeSinceStarted;
		if ( diff < 0 ) diff = 0;
		string minutes = ((int)diff / 60).ToString().PadLeft( 2, '0' );
		string seconds = ((int)(diff % 60)).ToString().PadLeft( 2, '0' );
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
