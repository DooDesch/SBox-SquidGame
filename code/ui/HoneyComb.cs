using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace SquidGame.UI
{
	public partial class IntoTheSquareHole : Panel
	{
		public Label Label { get; private set; }

		public IntoTheSquareHole()
		{
			Log.Warning( "HoneyComb::IntoTheSquareHole" );
			Label = Add.Label( "", "square" );

			// Log.Info( Position );

			Style.Left = Length.Pixels( 0f );
			Style.Top = Length.Pixels( 0f );

			Style.Dirty();
		}

		public void UpdatePosition()
		{
			Log.Warning( "HoneyComb::IntoTheSquareHole::UpdatePosition" );

			// _ = GameTask.DelaySeconds( .5f );

			Vector2 position = Parent.ScreenPositionToPanelPosition( Mouse.Position );
			// Log.Info( Position );

			Style.Left = position.x * ScaleFromScreen;
			Style.Top = position.y * ScaleFromScreen;

			Style.Dirty();
		}

		public override void Tick()
		{
			base.Tick();

			// Position = Parent.ScreenPositionToPanelPosition( Mouse.Position );

			// Style.Left = Position.x * ScaleFromScreen;
			// Style.Top = Position.y * ScaleFromScreen;

			// Style.Dirty();

			// Log.Info( ComputedStyle.Width );
		}
	}

	public partial class HoneyComb : Panel
	{
		public static HoneyComb Current { get; private set; }
		public Action OnClicked { get; set; }

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

			if ( ChildrenCount <= 0 ) return;

			foreach ( var item in Children )
			{
				item.Delete();
			}
		}

		protected override void OnClick( MousePanelEvent e )
		{
			Log.Warning( "HoneyComb::OnClick" );

			var child = AddChild<IntoTheSquareHole>();
			child.UpdatePosition();

			OnClicked?.Invoke();
			base.OnClick( e );
		}

		public override void Tick()
		{
			base.Tick();

			var player = Local.Pawn;
			if ( player == null ) return;
		}
	}
}
