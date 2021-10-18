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

			Style.Dirty();
		}

		[Event( "honeycomb.open" )]
		public static void OnOpenHoneyComb()
		{
			Log.Warning( "HoneyComb::OnOpenHoneyComb" );
			Current?.Open();
		}

		[Event( "honeycomb.close" )]
		public static void OnCloseHoneyComb()
		{
			Log.Warning( "HoneyComb::OnCloseHoneyComb" );
			Current?.Close();
		}

		public void Open()
		{
			Log.Warning( "HoneyComb::Open" );
			AddClass( "open" );
		}

		public void Close()
		{
			Log.Warning( "HoneyComb::Close" );
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
