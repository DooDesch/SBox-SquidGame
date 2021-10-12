using MinimalExample;
using Sandbox;

public class NullGameMode : AbstractGameMode
{
	public NullGameMode()
	{
		GameState = GAME_STATE.NULL;
		Log.Info( "Fuck me im the null GameMode" );
	}

	public override void AddPlayer( MinimalPlayer player )
	{

	}

	public override string GetGameText()
	{
		return "";
	}

	public override void Init()
	{
		base.Init();
	}

	public override void OnTick()
	{

	}
}
