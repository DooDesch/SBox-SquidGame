using Sandbox;
class RedLightGreenLightClient : AbstractGameModeClient
{
	public override void Init()
	{
		if (minimalPlayer.IsClient)
		{
			minimalPlayer.ClientSpawn();
		}
	}
}
