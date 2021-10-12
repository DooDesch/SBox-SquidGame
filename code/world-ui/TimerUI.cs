using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using MinimalExample;

class TimerUI : WorldPanel
{
	public Label Label;
	public MinimalPlayer MinimalPlayer { get; }

	private int maxTime = 300;
	private TimeSince timeSinceStarted = 0;

	public TimerUI() : base()
	{
		StyleSheet.Load( "/world-ui/World-UI.scss" );
		Label = Add.Label( "100" );
	}

	public override void Tick()
	{
		base.Tick();
		int diff = maxTime - (int)timeSinceStarted;
		if ( diff < 0 ) diff = 0;
		string minutes = ((int)diff / 60).ToString().PadLeft( 2, '0' );
		string seconds = ((int)(diff % 60)).ToString().PadLeft( 2, '0' );
		Label.Text = $"{minutes}:{seconds}";
	}

	public void UpdateTimer( int time )
	{
		maxTime = time;
		timeSinceStarted = 0;
	}
}
