
using System;
using System.Collections.Generic;

namespace Engine
{
	/// <summary>
	/// Describes a tilemap. Used as a template to create maps, and for storage in the resource manager.
	/// </summary>
	public class MapDescriptor
	{
		/// <summary>
		/// Contains information on objects placed around the map
		/// </summary>
		public class MapObject
		{
			public MapObject(double x, double y, string name)
			{
				X = x;
				Y = y;
				Properties = new Dictionary<string, string>();
				Name = name;
			}
			
			public double X
			{
				get;
				private set;
			}
			
			public double Y
			{
				get;
				private set;
			}
			
			public string Name
			{
				get;
				private set;
			}
			
			public Dictionary<string, string> Properties
			{
				get;
				private set;
			}
		}
		int[,,] tiles;
		
		//Create a map descriptor from a finished array of tile data
		public MapDescriptor(string mapID, int[,,] tiledata, int width, int height, int layers, double tileSize, double offsetX, double offsetY, string tileset) 
				: this(mapID, width, height, layers, tileSize, offsetX, offsetY, tileset)
		{
			MapID = mapID;
			tiles = (int[,,])tiledata.Clone();
			Objects = new List<MapDescriptor.MapObject>();
		}
		
		//Create a map descriptor from a byte of tile data (usually extracted from binary data or some form of encoding (like base64))
		public MapDescriptor(string mapID, byte[] tiledata, int width, int height, int layers, double tileSize, double offsetX, double offsetY, string tileset) 
			: this(mapID, width, height, layers, tileSize, offsetX, offsetY, tileset)
		{
			MapID = mapID;
			Objects = new List<MapDescriptor.MapObject>();
			
			//tiles = new int[width, height, layers];
			
			//Convert the byte array to a 3D array of ints
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					for (int z = 0; z < layers; z++)
					{
						for (int i = 0; i < sizeof(int); i++)
						{
							int index = i + z*sizeof(int) + y*sizeof(int)*layers + x*sizeof(int)*layers*height;
							tiles[x,y,z] |= (tiledata[index] << (i*8));
						}
					}
				}
			}
		}
		
		//Create an empty map descriptor
		public MapDescriptor(string mapID, int width, int height, int layers, double tileSize, double offsetX, double offsetY, string tileset)
		{
			MapID = mapID;
			tiles = new int[width,height,layers];
			Width = width;
			Height = height;
			Layers = layers;
			OffsetX = offsetX;
			OffsetY = offsetY;
			Tileset = tileset;
			TileSize = tileSize;
			Objects = new List<MapDescriptor.MapObject>();
			ExtraProperties = new Dictionary<string, string>();
		}
		
		public string MapID
		{
			get;
			private set;
		}
		
		public List<MapObject> Objects
		{
			get;
			private set;
		}
		
		public void SetTile(int x, int y, int z, int tileID)
		{
			tiles[x,y,z] = tileID;
		}
		
		public int GetTile(int x, int y, int z)
		{
			return tiles[x,y,z];
		}
		
		public int Width
		{
			get;
			private set;
		}
		
		public int Height
		{
			get;
			private set;
		}
		
		public int Layers
		{
			get;
			private set;
		}
		
		public double TileSize
		{
			get;
			private set;
		}
		
		public double OffsetX
		{
			get;
			private set;
		}
		
		public double OffsetY
		{
			get;
			private set;
		}
		
		public string Tileset
		{
			get;
			private set;
		}
		
		public string Background
		{
			get;
			set;
		}
		
		public Dictionary<string, string> ExtraProperties
		{
			get;
			private set;
		}
	}
	
}
