using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Engine
{
	public class TilesetLoader : IResourceLoader<Tileset>
	{
		/// <summary>
		/// Load the tiles and tileset from an XML
		/// </summary>
		/// <param name="filename">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Tileset"/>
		/// </returns>
		public Tileset LoadResource(string filename, string name)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			
			//Get the directory containing the XML (this will be prepended on each tile filename)
			string directory = Path.GetDirectoryName(filename);
			
			bool valid = false;
			
			//Create a tileset and a texture loader to load each tile
			Tileset tileset = new Tileset();
			TextureLoader textureLoader = new TextureLoader();
			
			//Second node is tileset node (or at least it should be)
			foreach (XmlNode c in doc.ChildNodes)
			{
				if (c.Name == "tileset")
				{
					valid = true;
					
					//Traverse the list of tiles
					foreach (XmlNode tileNode in c.ChildNodes)
					{
						if (tileNode.Name == "tile")
						{
							int tileID = -1;
							string textureFileName = "";
							BoundingPolygon boundingPolygon = null;
							
							//Extract tile attributes
							foreach (XmlAttribute a in tileNode.Attributes)
							{
								switch (a.Name)
								{
								case "texture":
									textureFileName = Path.Combine(directory, a.Value);
									break;
								case "id":
									tileID = int.Parse(a.Value);
									break;
								default:
									Log.Write("Unknown tile attribute in XML: " + a, Log.WARNING);
									break;
								}
							}
							
							//And bounding polygon
							foreach (XmlNode boundsNode in tileNode.ChildNodes)
							{
								if (boundsNode.Name == "bounds")
								{
									List<Vector> points = new List<Vector>();
									
									foreach (XmlNode edgeProps in boundsNode.ChildNodes)
									{
										switch (edgeProps.Name)
										{
										case "point":
											points.Add(new Vector());
											
											foreach (XmlAttribute a in edgeProps.Attributes)
											{
												if (a.Name == "x")
													points[points.Count - 1].X = int.Parse(a.Value);
												else if (a.Name == "y")
													points[points.Count - 1].Y = int.Parse(a.Value);
												else
													Log.Write("Unknown attribute in tile polygon point: " + a.Name, Log.WARNING);
											}
											break;
										}
									}
									if (points.Count > 1)
										boundingPolygon = new BoundingPolygon(points);
									else
										Log.Write("Only one or zero points was found in tile bounding polygon definition for tile with id " + tileID + ". Ignored.", Log.WARNING);
								}
								else
								{
									Log.Write("Unknown child node of tile in XML: " + boundsNode.Name, Log.WARNING);
								}
							}
								
							//Check if we got valid data
							if (tileID < 0 || textureFileName.Trim().Length == 0)
								throw new XmlException("Could not load tileset " + filename + ": TileID is < 0, or empty tile texture filename. TileID: " + tileID + ", filename: " + filename);
							
							//Add the tile to the tileset
							tileset.AddTile(new Tile(tileID, textureLoader.LoadResource(textureFileName, ""), boundingPolygon), textureFileName);
						}
					}
				}
			}
			
			if (!valid)
				throw new XmlException("Could not load tileset: No tileset node in XML " + filename);
			
			Log.Write("" + tileset.TileCount + " tiles loaded.", Log.INFO);
			
			return tileset;
		}
		
	}
}
