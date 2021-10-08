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
		Label = Add.Label( "100", "value" );
	}

	public override void Tick()
	{
		base.Tick();
		int diff = maxTime - (int)timeSinceStarted;
		if ( diff < 0 ) diff = 0;
		Label.Text = $"{((int)diff/60).ToString().PadLeft(2, '0')}" + $":{((int)(diff%60)).ToString().PadLeft(2, '0')}";
	}
}
