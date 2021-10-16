using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Health : Panel
{
	public Label HealthText;
	public Panel HealthBar;

	public Health()
	{
		Panel healthIconBack = Add.Panel( "HealthIconBack" );
		healthIconBack.Add.Label( "favorite", "healthicon" );

		Panel healthBarBack = Add.Panel( "HealthBarBack" );
		HealthBar = healthBarBack.Add.Panel( "HealthBar" );

		HealthText = Add.Label( "100", "healthtext" );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn;
		if ( player == null ) return;

		HealthText.Text = $"{player.Health.CeilToInt()}";
		HealthBar.Style.Dirty();
		HealthBar.Style.Width = Length.Percent( player.Health );
	}
}
