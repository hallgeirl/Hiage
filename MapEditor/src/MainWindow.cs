using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Gtk;
using Engine;
using MapEditor;

public partial class MainWindow: Gtk.Window, IEditorListener
{	
	EditorModel model;
	TileChooser tileChooserWindow;
	ObjectChooser objectChooserWindow;
	const string WINDOW_TITLE = "Hiage Map Editor";
	
	delegate void VoidDelegate();
	
	public MainWindow (EditorModel model): base (Gtk.WindowType.Toplevel)
	{
		//Set the model, and add this window as a listener to the model
		this.model = model;
		model.AddListener(this);
		tileChooserWindow = new TileChooser(model);
		objectChooserWindow = new ObjectChooser(model);
		tileChooserWindow.Visible = false;
		objectChooserWindow.Visible = false;
		
		//Build window from design
		Build();
		
		Title = WINDOW_TITLE;

		#region Event handlers
		#region Radio buttons
		//Set up event handlers
		radioDrawTiles.Toggled += delegate {
			if (radioDrawTiles.Active) 
			{
				model.CurrentTool = EditorModel.Tool.DrawTile;
				tileChooserWindow.Visible = true;
			}
			else
				tileChooserWindow.Visible = false;
		};
		
		radioCreateObjects.Toggled += delegate {
			if (radioCreateObjects.Active)
			{
				model.CurrentTool = EditorModel.Tool.CreateObject;
				objectChooserWindow.Visible = true;
			}
			else
				objectChooserWindow.Visible = false;
		};
		
		radioSelectObjects.Toggled += delegate {
			if (radioSelectObjects.Active)
			{
				model.CurrentTool = EditorModel.Tool.SelectObject;
			}
		};
		
		radioDrawTiles.Toggle();
		#endregion
		
		#region Background and draw-to-layer spin
		
		comboBackgrounds.Changed += delegate(object sender, EventArgs e) {
			model.Background = ((ComboBox)sender).ActiveText;
			Log.Write("Changed background to " + ((ComboBox)sender).ActiveText);
		};
		
		spinDrawToLayer.Changed += delegate {
			model.DrawToLayer = spinDrawToLayer.ValueAsInt;
		};
		
		#endregion
		
		//Set up event handlers for menu actions
		#region File->New
		NewAction.Activated += delegate {
			bool continueNew = false; //Used to stop the new map action if the user presses "cancel" on the question on wether or not to save
			//Check if the map has been changed. If so, ask the user if he/she would like to save.
			if (model.Changed)
				QuerySave(delegate { continueNew = true; SaveAction.Activate(); }, delegate {continueNew = true;});
			else continueNew = true;
			
			//If continueNew is false, the user pressed cancel and we should therefor stop creating a new map.
			if (continueNew)
			{
				NewMapDialog newMapDialog = new NewMapDialog(model);
				
				newMapDialog.Response += delegate(object o, ResponseArgs args) {
					switch (args.ResponseId)
					{
					case ResponseType.Ok:
						NewMapDialog dlg = (NewMapDialog)o;
						
						MapEditorState.Schedule(delegate { 
							model.CurrentTileset = model.ResourceManager.GetTileset(dlg.Tileset);
							model.TileMap = new TileMap(model.Display, model.CurrentTileset, dlg.MapWidth, dlg.MapHeight, dlg.MapDepth, dlg.XOffset, dlg.YOffset, dlg.TileSize);
							model.Display.CameraX = 0;
							model.Display.CameraY = 0;
							model.Display.Zoom = 100;
							model.Changed = false;
							model.Filename = "";
							model.Objects.Clear();
							model.DrawToLayer = 1;
							model.Background = "";
						});
						break;
						
					case ResponseType.Cancel:
						//Do nothing
						break;
					}
				};
				
				newMapDialog.Run();
				newMapDialog.Destroy();
			}
		};
		#endregion
		
		#region File->Open
		OpenAction.Activated += delegate {
			bool continueOpen = false; //Used to stop the new map action if the user presses "cancel" on the question on wether or not to save
			//Check if the map has been changed. If so, ask the user if he/she would like to save.
			if (model.Changed)
				QuerySave(delegate { continueOpen = true; SaveAction.Activate(); }, delegate {continueOpen = true;});
			else continueOpen = true;
			
			//If continueOpen is false, the user pressed cancel and we should therefor stop opening another map.
			if (continueOpen)
			{
				FileChooserDialog dlg = new FileChooserDialog("Open map", this, FileChooserAction.Open, "Open", ResponseType.Ok, "Cancel", ResponseType.Cancel);
				FileFilter f = new FileFilter();
				f.AddPattern("*.xml");
				dlg.Filter = f;
				
				dlg.Response += delegate(object o, ResponseArgs args) {
					switch (args.ResponseId)
					{
					case ResponseType.Ok:
						string filename = dlg.Filename;
						
						MapEditorState.Schedule(delegate{
							try
							{
								MapDescriptor mapDescriptor = new MapLoader().LoadResource(filename, "");
								TileMap newTm = new TileMap(model.Display, model.ResourceManager, mapDescriptor); 
								model.CurrentTileset = newTm.Tileset;
								model.TileMap = newTm;
								model.Objects.Clear();
								foreach (MapDescriptor.MapObject obj in mapDescriptor.Objects)
								{
									Log.Write("Spawning object " + obj.Name);
									MapObject newObj = new MapObject(model.ResourceManager.GetObjectDescriptor(obj.Name), model.ResourceManager, model.Display.Renderer, new Vector(obj.X, obj.Y));
									model.Objects.Add(newObj);
								}
								model.Filename = filename;
								model.Changed = false;
								model.DrawToLayer = 1;
								model.Background = mapDescriptor.Background;
								foreach (KeyValuePair<string, string> p in mapDescriptor.ExtraProperties)
									model.ExtraProperties.Add(p.Key, p.Value);
							}
							catch (Exception e)
							{
								Application.Invoke(delegate {
									Log.Write("Unable to open map " + filename + ": " + e.Message,Log.ERROR);
									MessageDialog mdlg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false, "Unable to open map " + filename + ": " + e.Message);
									mdlg.Run();
									mdlg.Destroy();
								});
							}
						});
						break;
					case ResponseType.Cancel:
						break;
					}
				};
						
				dlg.Run();
				dlg.Destroy();
				
			}
		};
		#endregion
		
		
		#region File->Save
		SaveAction.Activated += delegate {
			if (!string.IsNullOrEmpty(model.Filename))
			{
				SaveMap(model.Filename);
				model.Changed = false;
			}
			else
			{
				ShowSaveAs();
			}
		};
		#endregion
		
		#region File->Save As
		SaveAsAction.Activated += delegate { ShowSaveAs(); };
		#endregion
		
		#region File->Quit
		QuitAction.Activated += delegate {
			bool continueQuit = false; //Used to stop the new map action if the user presses "cancel" on the question on wether or not to save
			//Check if the map has been changed. If so, ask the user if he/she would like to save.
			if (model.Changed)
				QuerySave(delegate { continueQuit = true; SaveAction.Activate(); }, delegate {continueQuit = true;});
			else continueQuit = true;
			
			if (continueQuit)
			{
				model.Running = false;
	
				Application.Quit();
			}
		};
		#endregion
		
		#endregion
	
		int pos = 0;
		foreach (string texture in model.ResourceManager.Textures)
		{
			comboBackgrounds.InsertText(pos, texture);
			pos++;
		}
	}
	
	#region Private helper functions for saving maps
	
	/// <summary>
	/// Queries the user if he or she wants to save the current map before continuing. Pressing "cancel" invokes no delegates, while answering "yes" or "no" invokes an action.
	/// </summary>
	/// <param name="yes">
	/// The delegate to be invoked if the user answers yes
	/// </param>
	/// <param name="no">
	/// The delegate to be invoked if the user answers no
	/// </para>
	private void QuerySave(VoidDelegate yes, VoidDelegate no)
	{
		MessageDialog dlg = new MessageDialog(this, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, true,"The current map has been modified. Would you like to save before continuing?");
		dlg.AddButton("Cancel", ResponseType.Cancel);
		dlg.Response += delegate(object o, ResponseArgs args) {
			switch (args.ResponseId)
			{
			case ResponseType.Yes:
				yes();
				break;
			case ResponseType.No:
				no();
				break;
			}
		};
		dlg.Run();
		dlg.Destroy();
	}
	
	/// <summary>
	/// Save the map to file.
	/// </summary>
	/// <param name="filename">
	/// A <see cref="System.String"/>
	/// </param>
	private void SaveMap(string filename)
	{
		//XML file
		string xmlFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), System.IO.Path.GetFileNameWithoutExtension(filename) + ".xml");
		//Tile data file
		//string tdFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), System.IO.Path.GetFileNameWithoutExtension(filename) + ".td");
		
		Log.Write("Saving current map to XML " + xmlFile);
		
		XmlWriter xmlWriter = new XmlTextWriter(xmlFile, new System.Text.UTF8Encoding());
		
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteWhitespace("\r\n");
		xmlWriter.WriteStartElement("map");
		
		//Write dimensions
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteStartElement("geometry");		
		
		//Write dimensions
		xmlWriter.WriteWhitespace("\r\n\t\t");
		xmlWriter.WriteStartElement("width");
		xmlWriter.WriteValue(model.TileMap.Width);
		xmlWriter.WriteEndElement();
		
		xmlWriter.WriteWhitespace("\r\n\t\t");
		xmlWriter.WriteStartElement("height");
		xmlWriter.WriteValue(model.TileMap.Height);
		xmlWriter.WriteEndElement();
		
		xmlWriter.WriteWhitespace("\r\n\t\t");
		xmlWriter.WriteStartElement("layers");
		xmlWriter.WriteValue(model.TileMap.Layers);
		xmlWriter.WriteEndElement();
		
		xmlWriter.WriteWhitespace("\r\n\t\t");
		xmlWriter.WriteStartElement("tilesize");
		xmlWriter.WriteValue(model.TileMap.Tilesize);
		xmlWriter.WriteEndElement();
		
		//Offsets
		xmlWriter.WriteWhitespace("\r\n\t\t");
		xmlWriter.WriteStartElement("xoffset");
		xmlWriter.WriteValue(model.TileMap.OffsetX);
		xmlWriter.WriteEndElement();

		xmlWriter.WriteWhitespace("\r\n\t\t");
		xmlWriter.WriteStartElement("yoffset");
		xmlWriter.WriteValue(model.TileMap.OffsetY);
		xmlWriter.WriteEndElement();
		
		//Geometry end
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteEndElement();
		xmlWriter.WriteWhitespace("\r\n");
		
		//Tileset
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteStartElement("tileset");
		{
			
			bool foundTileset = false;
			foreach (string tsName in model.ResourceManager.Tilesets)
			{
				Tileset ts = model.ResourceManager.GetTileset(tsName);
				if (ts == model.CurrentTileset)
				{
					xmlWriter.WriteValue(tsName);
					foundTileset = true;
					break;
				}
			}
			if (!foundTileset)
			{
				throw new NotFoundException("Tileset was not found in resource manager. This should not happen! What did you do?");
			}
		}
		
		xmlWriter.WriteEndElement();
		
		//Background
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteStartElement("background");
		xmlWriter.WriteValue(model.Background);
		xmlWriter.WriteEndElement();
		xmlWriter.WriteWhitespace("\r\n");
		
		//Extra properties
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteStartElement("properties");
		foreach (KeyValuePair<string, string> o in model.ExtraProperties)
		{
			xmlWriter.WriteWhitespace("\r\n\t\t");
			xmlWriter.WriteStartElement(o.Key);
			xmlWriter.WriteValue(o.Value);
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteEndElement();
		xmlWriter.WriteWhitespace("\r\n");
		
		//Write objects
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteStartElement("objects");
		foreach (MapObject o in model.Objects)
		{
			xmlWriter.WriteWhitespace("\r\n\t\t");
			xmlWriter.WriteStartElement("object");
			
			xmlWriter.WriteWhitespace("\r\n\t\t\t");
			xmlWriter.WriteStartElement("name");
			xmlWriter.WriteValue(o.Name);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteWhitespace("\r\n\t\t\t");
			xmlWriter.WriteStartElement("x-pos");
			xmlWriter.WriteValue(o.Position.X);
			xmlWriter.WriteEndElement();
			
			xmlWriter.WriteWhitespace("\r\n\t\t\t");
			xmlWriter.WriteStartElement("y-pos");
			xmlWriter.WriteValue(o.Position.Y);
			xmlWriter.WriteEndElement();
			
			foreach (KeyValuePair<string, string> p in o.ExtraProperties)
			{
				xmlWriter.WriteWhitespace("\r\n\t\t\t");
				xmlWriter.WriteStartElement(p.Key);
				xmlWriter.WriteValue(p.Value);
				xmlWriter.WriteEndElement();
			}
			//Object end
			xmlWriter.WriteWhitespace("\r\n\t\t");
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteEndElement();
		xmlWriter.WriteWhitespace("\r\n");
		
		//Write tiledata
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteStartElement("tiledata");
		xmlWriter.WriteAttributeString("encoding", "base64");
		
		TileMap tm = model.TileMap;
		byte[] tiledata = new byte[tm.Width*tm.Height*tm.Layers*sizeof(int)];
		
		
		//Convert the 3D array to a byte stream for input into the XML file as base64 encoding
		for (int x = 0; x < model.TileMap.Width; x++)
		{
			for (int y = 0; y < model.TileMap.Height; y++)
			{
				for (int z = 0; z < model.TileMap.Layers; z++)
				{
					for (int i = 0; i < sizeof(int); i++)
					{
						int index = i + z*sizeof(int) + y*sizeof(int)*model.TileMap.Layers + x*sizeof(int)*model.TileMap.Layers*model.TileMap.Height;
						tiledata[index] = (byte)((tm.GetTile(x, y, z).Id & (255 << (i*8))) >> (i*8));
					}
				}
			}
		}
		
		string base64Tiledata = Convert.ToBase64String(tiledata);
		int rowLength = 128;
		for (int i = 0; i < base64Tiledata.Length; i+=rowLength)
		{
			xmlWriter.WriteWhitespace("\r\n\t\t");
			xmlWriter.WriteValue(base64Tiledata.Substring(i, Math.Min(rowLength, base64Tiledata.Length-i)));
		}
		
		//tiledata end
		xmlWriter.WriteWhitespace("\r\n\t");
		xmlWriter.WriteEndElement();
			
		//map end
		xmlWriter.WriteWhitespace("\r\n");
		xmlWriter.WriteEndElement();
		
		xmlWriter.Close();
	}
	
	/// <summary>
	/// Show the save as-dialog. Returns false if user presses cancel.
	/// </summary>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	private bool ShowSaveAs()
	{
		bool result = false;
		
		FileChooserDialog dlg = new FileChooserDialog("Save map as", this, FileChooserAction.Save, "Save", ResponseType.Ok, "Cancel", ResponseType.Cancel);
		FileFilter f = new FileFilter();
		f.AddPattern("*.xml");
		dlg.Filter = f;
		
		dlg.Response += delegate(object o, ResponseArgs args) {
			switch (args.ResponseId)
			{
			case ResponseType.Ok:
				model.Filename = dlg.Filename;
				if (System.IO.Path.GetExtension(dlg.Filename).Length == 0)
					model.Filename += ".xml";
				SaveMap(model.Filename);
				model.Changed = false;
				result = true;
				break;
			case ResponseType.Cancel:
				break;
			}
		};
				
		dlg.Run();
		dlg.Destroy();
		
		return result;
	}
	#endregion
	
	//Update the window title based on the flags in model (changed and filename)
	private void UpdateTitle()
	{
		
		Application.Invoke(delegate {
			Title = WINDOW_TITLE + (model.Filename.Trim().Length > 0 ? " (" + System.IO.Path.GetFileName(model.Filename) + ")" : "") + (model.Changed ? " (modified)" : "");
		});
	}
	
	//Handle any updates to the common model
	public void ModelChanged(EditorModel.VariableName var, object oldValue, object newValue)
	{
		switch (var)
		{
		case EditorModel.VariableName.MousePosition:
			Vector v = (Vector)newValue;
			Application.Invoke(delegate { 
				Vector tilePosition = null;
				if (model.TileMap != null)
				{
					try
					{
						tilePosition = model.TileMap.WorldToTilePosition(v);
					}
					catch (IndexOutOfRangeException)
					{
					}
				}
				labelMousePos.Text = "(" + v.X.ToString("N2") + ", " + v.Y.ToString("N2") + ")" + (tilePosition != null ? ", tile (" + tilePosition.X + "," + tilePosition.Y + ")" : ""); 
			});
			break;

		case EditorModel.VariableName.TileMap:
			TileMap tm = (TileMap)newValue;
			if (tm != null)
			{
				Application.Invoke(delegate {
					labelWidth.Text = "" + model.TileMap.Width;
					labelHeight.Text = "" + model.TileMap.Height;
					labelLayers.Text = "" + model.TileMap.Layers;
					spinDrawToLayer.SetRange(1, tm.Layers);
					spinDrawToLayer.Value = 1;
				});
			}
			break;
			
		case EditorModel.VariableName.Changed:
			UpdateTitle();
			break;
			
		case EditorModel.VariableName.Filename:
			UpdateTitle();
			break;
		}
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		bool continueQuit = false; //Used to stop the new map action if the user presses "cancel" on the question on wether or not to save
		//Check if the map has been changed. If so, ask the user if he/she would like to save.
		if (model.Changed)
			QuerySave(delegate { continueQuit = true; SaveAction.Activate(); }, delegate {continueQuit = true;});
		else continueQuit = true;
		
		if (continueQuit)
		{
			a.RetVal = false;
			model.Running = false;

			Application.Quit();
		}
		else 
			a.RetVal = true;
	}
}