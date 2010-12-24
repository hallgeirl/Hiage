using System;
using System.Collections.Generic;

namespace Engine
{
	public class Edge : BoundingPolygon, ICloneable
	{
		/// <summary>
		/// Create a new edge.
		/// </summary>
		/// <param name="p1">
		/// Defines the first point of the edge
		/// </param>
		/// <param name="p2">
		/// Defines the second point of the edge
		/// </param>
		/// <param name="normal">
		/// The edge normal vector. If this parameter is null, a vector perpendicular to the one defined by the two points p1 and p2 will be calculated.
		/// </param>
		public Edge(Vector p1, Vector p2, Vector normal) : base()
		{
			//this.P1 = p1;
			//this.P2 = p2;
			AddVertex(p1); AddVertex(p2);
			
			/*if (normal == null)
			{
				Vector v = p2 - p1;
				Normal = new Vector(v.Y, -v.X).Normalize();
			}
			else
				Normal = normal.Normalize();*/
		}
		
		public object Clone()
		{
			return new Edge((Vector)P1.Clone(), (Vector)P2.Clone(), (Vector)Normal.Clone());
		} 
		
		public Edge Copy()
		{
			return (Edge)Clone();
		}
		/*
		public void CalculateAbsolutePositions(int offsetX, int offsetY)
		{
			if (!PositionsAreAbsolute)
			{
				PositionsAreAbsolute = true;
				P1.X += offsetX;
				P1.Y += offsetY;
				P2.X += offsetX;
				P2.Y += offsetY;
			}
		}*/
		
		/// <value>
		/// The edge normal
		/// </value>
		public Vector Normal
		{
			get { return EdgeNormals[0]; }
		}
		
		public Vector P1
		{
			get { return Vertices[0]; }
		}

		public Vector P2
		{
			get { return Vertices[1]; }
		}
		
		//Return the edge in vector form
		public Vector AsVector
		{
			get
			{
				return P2-P1;
			}
		}
		
		public double Length
		{
			get { return new Vector(P2.X-P1.X, P2.Y-P1.Y).Length; }
		}
		
		/*
		public bool PositionsAreAbsolute
		{
			get;
			private set;
		}*/
		
		public override string ToString ()
		{
			return "[Edge: P1=" + P1 + ", P2=" + P2 + ",Normal=" + Normal + "]";
		}


	}
	
	//Class representing a tile with edges and texture
	public class Tile
	{
		/// <summary>
		/// Create a new tile
		/// </summary>
		public Tile(int id, Texture texture, BoundingPolygon boundingPolygon)
		{
			this.Id = id;
			this.BoundingPolygon = boundingPolygon;
			this.Texture = texture;
			
		}
		
		public object Clone()
		{
			//Copy bounding polygon
			BoundingPolygon polygon = BoundingPolygon != null ? (BoundingPolygon)BoundingPolygon.Clone() : null;
						
			return new Tile(Id, Texture, polygon);
		}
		
		public Tile Copy()
		{
			return (Tile)Clone();
		}
		
		public override string ToString()
		{
			return "Tile(" + Id +")";
		}
		
		public int Id
		{
			get;
			private set;
		}
		
		public Texture Texture
		{
			get;
			private set;
		}
		
		public BoundingPolygon BoundingPolygon
		{
			get;
			private set;			
		}
	}
	
	/// <summary>
	/// Class representing a set of tiles
	/// </summary>
	public class Tileset
	{
		Dictionary<int, Tile> 	tiles = new Dictionary<int, Tile>();
		Dictionary<int, string> fileNames = new Dictionary<int, string>();
		
		public Tileset()
		{
		}
		
		#region Member functions
		public void AddTile(Tile t)
		{
			tiles.Add(t.Id, t);
		}
		
		public void AddTile(Tile t, string filename)
		{
			AddTile(t);
			fileNames.Add(t.Id, filename);
		}
		
		public Tile GetTile(int id)
		{
			if (!tiles.ContainsKey(id))
				throw new NotFoundException("Tile with id " + id + " not found in tileset.");
			
			return tiles[id];
		}
		
		public string GetTileFilename(int id)
		{
			if (!fileNames.ContainsKey(id))
				throw new NotFoundException("Filename for tile with id " + id + " is not available.");
			
			return fileNames[id];
		}
		

		
		#endregion
		
		#region Properties
		public int TileCount
		{
			get { return tiles.Count; }
		}
		
		public Dictionary<int, Tile> Tiles
		{
			get { return tiles; }
		}
		#endregion
	}
	
	/// <summary>
	/// Class representing a grid of tiles
	/// </summary>
	public class TileMap
	{
		Display display;
		
		Tile[,,] map;
		
		/// <summary>
		/// Construct a tilemap.
		/// </summary>
		/// <param name="display">
		/// The main display is required for camera positions and renderer
		/// </param>
		/// <param name="offsetX">
		/// X offset for the tilemap, in case it's not desired that the drawing starts at (0,0)
		/// </param>
		/// <param name="offsetY">
		/// Y offset
		/// </param>
		public TileMap(Display display, Tileset tileset, int width, int height, int layers, double offsetX, double offsetY, double tileSize)
		{
			this.display = display;
			Tileset = tileset;
			Tilesize = tileSize;
			Width = width;
			Height = height;
			Layers = layers;
			OffsetX = offsetX;
			OffsetY = offsetY;
			
			map = new Tile[width, height, layers];
			
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					for (int z = 0; z < Layers; z++)
					{
						map[x,y,z] = (Tile)Tileset.GetTile(0).Clone();
					}
				}
			}
		}
		
		/// <summary>
		/// Create a tilemap from a descriptor.
		/// </summary>
		public TileMap(Display display, ResourceManager resources, MapDescriptor descriptor)
		{
			this.display = display;
			Width = descriptor.Width;
			Height = descriptor.Height;
			Layers = descriptor.Layers;
			Tilesize = descriptor.TileSize;
			OffsetX = descriptor.OffsetX;
			OffsetY = descriptor.OffsetY;
			Tileset = resources.GetTileset(descriptor.Tileset);
			map = new Tile[Width, Height, Layers];
			
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					for (int z = 0; z < Layers; z++)
					{
						SetTile(x,y,z,Tileset.GetTile(descriptor.GetTile(x,y,z)));
					}
				}
			}
		}
		
		/// <summary>
		/// Calculate absolute edge positions for all edges in a specified tile in tilemap so that these don't have to be calculated every time they're needed.
		/// </summary>
		private void CalculateEdgePositions(int x, int y, int z)
		{
			Texture t = map[x,y,z].Texture;
			
			BoundingPolygon bp = map[x,y,z].BoundingPolygon;
			if (bp == null) return;
			
			bp.Scale(Tilesize/(t.Width-1), Tilesize/(t.Height-1));
			bp.Translate(x*Tilesize + OffsetX, y*Tilesize + OffsetY);
			/*e.P1.X = e.P1.X*(Tilesize/(t.Width-1))+x*Tilesize + OffsetX;
			e.P1.Y = e.P1.Y*(Tilesize/(t.Height-1))+y*Tilesize + OffsetY;
			e.P2.X = e.P2.X*(Tilesize/(t.Width-1))+x*Tilesize + OffsetX;
			e.P2.Y = e.P2.Y*(Tilesize/(t.Height-1))+y*Tilesize + OffsetY;*/
		}
			
		
		/// <summary>
		/// Render the tilemap
		/// </summary>
		public void Render(int layer)
		{
			Renderer renderer = display.Renderer;
			//renderer.SetFont("arial", 8);

			//Find out what tiles which actually needs to be rendered (we don't really care about those outside the view of the camera)
			int minTileX = (int)Math.Max((display.RenderedCameraX - display.ViewportWidth/2 - OffsetX) / Tilesize, 0.0);
			int maxTileX = (int)Math.Min((display.RenderedCameraX + display.ViewportWidth/2 - OffsetX) / Tilesize, Width-1);
			int minTileY = (int)Math.Max((display.RenderedCameraY - display.ViewportHeight/2 - OffsetY) / Tilesize, 0);
			int maxTileY = (int)Math.Min((display.RenderedCameraY + display.ViewportHeight/2 - OffsetY) / Tilesize, Height-1);

			for (int xTile = minTileX; xTile <= maxTileX; xTile++)
			{
				for (int yTile = minTileY; yTile <= maxTileY; yTile++)
				{
					//Log.Write("x:" + xTile + " y:" + yTile + "z:" + layer + " minX:" + minTileX + " maxX:" + maxTileX);
					Tile t = map[xTile, yTile, layer];
					
					
					renderer.Render(xTile*Tilesize + OffsetX, yTile*Tilesize + OffsetY,(xTile+1)*Tilesize + OffsetX, (yTile+1)*Tilesize + OffsetY, t.Texture);
					
					/*foreach (Edge e in map[xTile, yTile, 0].Edges)
					{
						Vector p1 = e.P1, p2 = e.P2;
						
						renderer.DrawLine(e.P1.X, e.P1.Y, e.P2.X, e.P2.Y);
						
						//Draw normal
						renderer.DrawLine(p1.X + (p2.X-p1.X)/2, p1.Y + (p2.Y-p1.Y)/2, p1.X + (p2.X-p1.X)/2+e.Normal.X*Tilesize/4, p1.Y + (p2.Y-p1.Y)/2+e.Normal.Y*Tilesize/4);
					}*/
					
					
					
					//renderer.DrawText("(" + xTile + ", " + yTile + ")", xTile*tileSize,yTile*tileSize);
				}
			}
		}
		
		public void SetTile(int x, int y, int layer, Tile t)
		{
			map[x, y, layer] = t.Copy();
			CalculateEdgePositions(x, y, layer);
		}
		
		//Convert world position to tile position
		public Vector WorldToTilePosition(Vector worldPos)
		{
			Vector tilePos = new Vector((int)((worldPos.X-OffsetX)/Tilesize), (int)((worldPos.Y-OffsetY)/Tilesize));
			if (tilePos.X < 0 || tilePos.X >= Width || tilePos.Y < 0 || tilePos.Y >= Height)
				throw new IndexOutOfRangeException("Tile at position (" + worldPos.X + "," + worldPos.Y + ") is outside of map.");
			
			return tilePos;
		}
		
		
		public Tile GetTile(int x, int y, int layer)
		{
			return map[x, y, layer];
		}
		
		/// <summary>
		/// Return all tiles' bounding polygons in a specified region.
		/// </summary>
		public List<BoundingPolygon> GetBoundingPolygonsInRegion(Box boundingBox, int layer)
		{
			List<BoundingPolygon> polygons = new List<BoundingPolygon>();
				
			int minTileX = (int)Math.Max((boundingBox.Left - OffsetX) / Tilesize, 0.0);
			int maxTileX = (int)Math.Min((boundingBox.Right - OffsetX) / Tilesize, Width-1);
			int minTileY = (int)Math.Max((boundingBox.Bottom - OffsetY) / Tilesize, 0);
			int maxTileY = (int)Math.Min((boundingBox.Top - OffsetY) / Tilesize, Height-1);

			for (int xTile = minTileX; xTile <= maxTileX; xTile++)
			{
				for (int yTile = minTileY; yTile <= maxTileY; yTile++)
				{
					BoundingPolygon p = map[xTile, yTile, layer].BoundingPolygon;
					if (p != null && p.Vertices.Count > 1)
						polygons.Add(p);
				}
			}
			
			return polygons;
		}
		
		#region Properties
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
		
		public double Tilesize
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
		
		public double Left
		{
			get { return OffsetX; }
		}
		
		public double Right
		{
			get { return OffsetX + Width*Tilesize; }
		}
		
		public double Top
		{
			get { return OffsetY + Height*Tilesize; }
		}
		
		public double Bottom
		{
			get { return OffsetY; }
		}
		
		public Tileset Tileset
		{
			get;
			private set;
		}
		
		//For testing only
		/*public List<Edge> AllEdges
		{
			get
			{
				List<Edge> edges = new List<Edge>();
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						for (int z = 0; z < Layers; z++)
						{
							foreach (Edge e in map[x,y,z].Edges)
							{
								edges.Add(e);
							}
						}
					}
				}
				return edges;
			}
		}*/
		
		#endregion
	}
}
