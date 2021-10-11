using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalExample;

namespace SquidGame.Entities
{

	[Hammer.AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
	[Hammer.Solid]
	[Library( "sg_zone" )]
	partial class Zone : BaseTrigger
	{
		public override void Spawn()
		{
			base.Spawn();

			OnStartTouch.Listen( HandleOnStartTouch );
			OnEndTouch.Listen( HandleOnEndTouch );
		}

		private ValueTask HandleOnStartTouch( Entity activator, float delay )
		{
			if ( activator is MinimalPlayer player )
			{
				player.currentGameModeClient.HasWon = true;
			}

			return new ValueTask();
		}

		private ValueTask HandleOnEndTouch( Entity activator, float delay )
		{
			if ( activator is MinimalPlayer player )
			{
				player.currentGameModeClient.HasWon = false;
			}

			return new ValueTask();
		}
	}
}
