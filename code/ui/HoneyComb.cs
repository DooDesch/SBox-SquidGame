using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SquidGame.UI
{
	public class HoneyComb : Panel
	{
		public static HoneyComb Current { get; private set; }

		public HoneyComb()
		{
			Current = this;

			StyleSheet.Load( "/ui/HoneyComb.scss" );
		}



		public void Open()
		{
			AddClass( "open" );
		}

		public void Close()
		{
			RemoveClass( "open" );
		}

		public override void Tick()
		{
			base.Tick();

			var player = Local.Pawn;
			if ( player == null ) return;
		}
	}
}
