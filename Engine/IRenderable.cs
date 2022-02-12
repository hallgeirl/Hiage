
using System;

namespace Engine
{
	
	/*
	 * Interface for objects that can be rendered.
	 * */
	public interface IRenderable
	{

		//// <value>
		/// Left edge
		/// </value>
		double Left
		{
			get;
		}
		
		//// <value>
		/// Right edge
		/// </value>
		double Right
		{
			get;
		}
		
		//// <value>
		/// Top edge
		/// </value>
		double Top
		{
			get;
		}
		
		//// <value>
		/// Bottom edge
		/// </value>
		double Bottom
		{
			get;
		}
		
		//// <value>
		/// Texture to use for rendering this object
		/// </value>
		Texture Texture
		{
			get;
		}
	
		//// <value>
		/// Left edge of texture
		/// </value>
		int TextureLeft
		{
			get;
		}
		
		//// <value>
		/// Right edge of texture
		/// </value>
		int TextureRight
		{
			get;
		}
		
		//// <value>
		/// Top edge of texture
		/// </value>
		int TextureTop
		{
			get;
		}
		
		//// <value>
		/// Bottom edge of texture
		/// </value>
		int TextureBottom
		{
			get;
		}
		
		double Rotation
		{
			get;
		}
		
		//// <value>
		/// If this returns true, the object will be deleted on the next pass in the renderer
		/// </value>
		/*bool DeleteFromRenderer
		{
			get;
		}*/
	}
}
