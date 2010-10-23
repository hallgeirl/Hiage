
using System;

namespace MapEditor
{
	public partial class NewMapDialog : Gtk.Dialog
	{
		public NewMapDialog(EditorModel model)
		{
			this.Build ();

			int counter = 0;
			foreach (string t in model.ResourceManager.Tilesets)
				comboTileset.InsertText(counter++, t);
			comboTileset.Active = 0;
			
			spinXOffset.Value = 0;
			spinYOffset.Value = 0;
			
		}
		
		public int MapWidth
		{
			get { return (int)spinWidth.ValueAsInt; }
		}

		public int MapHeight
		{
			get { return (int)spinHeight.ValueAsInt; }
		}

		public int MapDepth
		{
			get { return spinLayers.ValueAsInt; }
		}
		
		public string Tileset
		{
			get { return comboTileset.ActiveText; }
		}
		
		public int TileSize
		{
			get { return spinTilesize.ValueAsInt; }
		}
		
		public int XOffset
		{
			get { return spinXOffset.ValueAsInt; }
		}

		public int YOffset
		{
			get { return spinYOffset.ValueAsInt; }
		}

	}
}
