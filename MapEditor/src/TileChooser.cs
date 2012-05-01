using Engine;
using Gtk;
using Gdk;
using System;
using System.Collections.Generic;

namespace MapEditor
{
	public partial class TileChooser : Gtk.Window, IEditorListener
	{
		EditorModel  model;
		List<EventBox> tileButtons = new List<EventBox>();
		const int TILE_WIDTH = 32;
		const int TILE_HEIGHT = 32;
		Gtk.Image lastSelection = null;
		Pixbuf 	  lastSelectionPixels = null;
		
		public TileChooser (EditorModel model) : base(Gtk.WindowType.Toplevel)
		{
			//Store the model reference and add the window to the list of listeners
			this.model = model;
			model.AddListener(this);
			
			this.Build ();
			
			Deletable = false; //Why doesn't this work?
			
			//Prevent this window from closing
			DeleteEvent += delegate(object o, Gtk.DeleteEventArgs args) {
				args.RetVal = true;
			};
			
			//Handle resizing / rearranging of tiles
			fixed3.SizeAllocated += delegate {
				if (tileButtons.Count > 0)
				{
					int width = Math.Min(Math.Min((fixed3.Allocation.Width), this.Allocation.Width) / TILE_WIDTH, tileButtons.Count);
					if (width == 0) width = 1;
					int height = (int)Math.Ceiling((double)tileButtons.Count / (double)width) ;
					if (height == 0) height = 1;
					
					if (width != tableTileGrid.NColumns || height != tableTileGrid.NRows)
					{
						ClearTileGrid();
						tableTileGrid.Resize((uint)height, (uint)width);
						FillTileGrid();
					}
				}
			};
			
			
		}
		
		protected void TileClicked(object o, ButtonPressEventArgs args) 
		{
			EventBox b = (EventBox)o;
			Gtk.Image img  = (Gtk.Image)b.Child;
			
			if (lastSelection != null)
			{
				//Restore pixbuf
				lastSelection.Pixbuf = lastSelectionPixels;
			}
			
			
			//Backup pixbuf and modify the old one
			lastSelection = img;
			lastSelectionPixels = img.Pixbuf;
			img.Pixbuf = img.Pixbuf.Copy();
			
			//Some unsafe code was needed to modify the pixbuf
			unsafe
			{
				Pixbuf p = img.Pixbuf;
				
				byte* ptr = (byte*)(p.Pixels);
				int borderWidth = 2;
				for (int x = 0; x < p.Width; x++)
				{
					for (int y = 0; y < p.Height; y++)
					{
						if (x < borderWidth || x >= p.Width - borderWidth || y < borderWidth || y >= p.Height - borderWidth)
						{
							int pixelIndex = x * p.NChannels + y * p.Width * p.NChannels;
							*(ptr + pixelIndex) = 255;
							for (int c = 1; c < p.NChannels; c++)
							{
								*(ptr + pixelIndex + c) = 0;
							}
							
							//Make the border opaque
							if (p.HasAlpha)
							{
								*(ptr + pixelIndex + p.NChannels - 1) = 255;
							}
						}
					}
				}
			}
			
			model.CurrentTile = model.CurrentTileset.GetTile((int)b.Data["tileid"]);
		}
		
		private void FillTileGrid()
		{
			int width = (int)tableTileGrid.NColumns;
			int counter = 0;
			
			foreach (EventBox b in tileButtons)
			{
				tableTileGrid.Attach(b, (uint)(counter % width), (uint)(counter % width+1), (uint)(counter / width), (uint)(counter / width + 1));
				counter++;
			}
		}
		
		private void ClearTileGrid()
		{
			foreach (Widget w in tableTileGrid.Children)
			{
				tableTileGrid.Remove(w);
			}
		}
		
		/// <summary>
		/// Event handler for when the model is changed
		/// </summary>
		public void ModelChanged(EditorModel.VariableName v, object oldValue, object newValue)
		{
			Application.Invoke(delegate { 
				
				//Update the tile grid if the tileset is changed
				if (v == EditorModel.VariableName.CurrentTileset)
				{
					Tileset ts = (Tileset)newValue;

					ClearTileGrid();
					tileButtons.Clear();
					foreach (KeyValuePair<int, Tile> t in ts.Tiles)
					{
						EventBox b = new EventBox();
						b.Data.Add("tileid", t.Key);
						Pixbuf p = new Pixbuf(ts.GetTileFilename(t.Key),TILE_WIDTH,TILE_HEIGHT);

						Gtk.Image i = new Gtk.Image(p);

						i.Visible = true;
						b.Visible = true;
						b.ButtonPressEvent += TileClicked;
						b.Add(i);
						tileButtons.Add(b);
					}
					FillTileGrid();
				}
			});
		}
	}
}
