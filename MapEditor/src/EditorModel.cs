
using System;
using Gtk;
using Engine;
using System.Collections.Generic;

namespace MapEditor
{
	/// <summary>
	/// Listener to provide two-way communication with the GUI
	/// </summary>
	public interface IEditorListener
	{
		void ModelChanged(EditorModel.VariableName v, object oldValue, object newValue);
	}
	
	/// <summary>
	/// This is the shared data model between the GUI and game engine.
	/// </summary>
	public class EditorModel
	{
		public enum VariableName
		{
			Background,
			Changed,
			CurrentTile,
			CurrentTileset,
			CurrentObject,
			DrawToLayer,
			Filename,
			MapID,
			MousePosition,
			Running,
			SelectedObject,
			TileMap,
			Tool,
			Zoom
		}
		
		public enum Tool 
		{
			CreateObject,
			DrawTile,
			SelectObject
		}
		
		//Model attributes
		string 	background = "";
		bool 	changed = false;
		Tile 	currentTile;
		Tileset currentTileset;
		string  currentObject;
		int 	drawToLayer = 1;
		string 	filename = "";
		string  mapID = "";
		Vector 	mousePosition = new Vector();
		bool 	running = true;
		MapObject selectedObject;
		TileMap tileMap;
		Tool 	tool;
		double 	zoom;
		
		
		List<IEditorListener> listeners = new List<IEditorListener>(); // Listeners
		
		public EditorModel()
		{
			Running = true;
			MousePosition = new Vector();
			Objects = new List<MapObject>();
			ExtraProperties = new Dictionary<string, string>();
		}
		
		/// <summary>
		/// Add a new listener
		/// </summary>
		public void AddListener(IEditorListener l)
		{
			lock(listeners)
			{
				listeners.Add(l);
			}
		}
				
		/// <summary>
		/// Notify all listeners that a variable in the model has been changed.
		/// </summary>
		private void NotifyListeners(VariableName v, object oldValue, object newValue)
		{
			lock(this)
			{
				lock(listeners)
				{
					foreach (IEditorListener l in listeners)
					{
						l.ModelChanged(v, oldValue, newValue);
					}
				}
			}

		}
		
		#region Model properties
		public string MapID
		{
			get { return mapID; }
			set 
			{
				NotifyListeners(VariableName.MapID, mapID, value);
				mapID = value;
			}
		}
				                
		
		public string Background
		{
			get { return background; }
			set
			{
				NotifyListeners(VariableName.Background, background, value);
				background = value;
			}
		}
		
		/// <value>
		/// Indicates wether or not the map has been changed since last save
		/// </value>
		public bool Changed
		{
			get { return changed; }
			set 
			{ 
				NotifyListeners(VariableName.Changed, changed, value);
				changed = value; 
			}
		}
		
		/// <summary>
		/// Current tile used for drawing
		/// </summary>
		public Tile CurrentTile
		{
			get { return currentTile; }
			set 
			{ 
				NotifyListeners(VariableName.CurrentTile, currentTile, value);
				currentTile = value; 
			}
		}
		
		/// <summary>
		/// Current object used for drawing
		/// </summary>
		public string CurrentObject
		{
			get { return currentObject; }
			set 
			{ 
				NotifyListeners(VariableName.CurrentObject, currentObject, value);
				currentObject = value; 
			}
		}
		
		/// <summary>
		/// Current tileset
		/// </summary>
		public Tileset CurrentTileset
		{
			get { return currentTileset; }
			set 
			{ 
				NotifyListeners(VariableName.CurrentTileset, currentTileset, value);
				currentTileset = value; 
			}
		}
		
		/// <value>
		/// The current tool used to manipulate the map
		/// </value>
		public Tool CurrentTool
		{
			get { return tool; }
			set 
			{ 
				NotifyListeners(VariableName.Tool, tool, value);
				tool = value; 
			}
		}
		
		public Dictionary<string, string> ExtraProperties
		{
			get;
			private set;
		}
			
		
		public Display Display
		{
			get;
			set;
		}
		
		public int DrawToLayer
		{
			get { return drawToLayer; }
			set
			{
				NotifyListeners(VariableName.DrawToLayer, drawToLayer, value);
				drawToLayer = value;
			}
		}
		
		public string Filename
		{
			get { return filename; }
			set 
			{
				NotifyListeners(VariableName.Filename, filename, value);
				filename = value; 
			}
		}
		
		/// <value>
		/// Last recorded mouse position
		/// </value>
		public Vector MousePosition
		{
			get { return mousePosition; }
			set 
			{ 
				Vector old = mousePosition;
				if (old != value)
				{
					mousePosition = value; 
					NotifyListeners(VariableName.MousePosition, old, value);
				}
			}
		}
		
		/// <value>
		/// Indicates wether or not the application should be running (for passing on shutdown-messages)
		/// </value>
		public bool Running
		{
			get { return running; }
			set 
			{ 
				NotifyListeners(VariableName.Running, running, value);
				running = value; 
			}	
		}
		
		/// <value>
		/// Currently loaded tilemap
		/// </value>
		public TileMap TileMap
		{
			get { return tileMap; }
			set 
			{ 
				NotifyListeners(VariableName.TileMap, tileMap, value);
				tileMap = value;
			}
		}
		
		public List<MapObject> Objects
		{
			get;
			private set;
		}
		
		public ResourceManager ResourceManager
		{
			get;
			set;
		}
		
		public MapObject SelectedObject
		{
			get { return selectedObject; }
			set 
			{
				NotifyListeners(VariableName.SelectedObject, selectedObject, value);
				selectedObject = value;
			}
		}
		
		/// <value>
		/// The current zoom value
		/// </value>
		public double Zoom
		{
			get { return zoom; }
			set 
			{ 
				NotifyListeners(VariableName.Zoom, zoom, value);
				zoom = value; 
			}
		}
		#endregion
	}
}
