using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using MinimalExample;

class TimerUI : WorldPanel
{
	public Label Label;
	public MinimalPlayer MinimalPlayer { get; }

	private AbstractGameMode assignedGamemode;
	private int maxTime = 300;

	public TimerUI( MinimalPlayer minimalPlayer ) : base()
	{
		StyleSheet.Load( "/world-ui/World-UI.scss" );
		Label = Add.Label( "100", "value" );
		MinimalPlayer = minimalPlayer;
		assignedGamemode = MinimalPlayer.currentGameMode;
	}

	public override void Tick()
	{
		base.Tick();
		int diff = maxTime - (int)assignedGamemode.timeSinceStarted;
		if ( diff < 0 ) diff = 0;
		Label.Text = $"{((int)diff/60).ToString().PadLeft(2, '0')}" + $":{((int)(diff%60)).ToString().PadLeft(2, '0')}";
	}
}
