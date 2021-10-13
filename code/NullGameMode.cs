using MinimalExample;
using Sandbox;

public class NullGameMode : AbstractGameMode
{
	public NullGameMode()
	{
		Log.Info( "NullGameMode::Constructor" );
		GameState = GAME_STATE.NULL;
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
		// base.Init();
	}

	public override void OnTick()
	{

	}
}
