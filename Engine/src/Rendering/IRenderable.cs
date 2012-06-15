
using System;

namespace Engine
{
	
	/*
	 * Interface for objects that can be rendered.
	 * */
	public interface IRenderable
	{

		//// <value>
		/// X position of center
		/// </value>
		double X
		{
			get;
		}
		
		//// <value>
		/// Y position of center
		/// </value>
		double Y
		{
			get;
		}
		
		//// <value>
		/// Height
		/// </value>
		double Height
		{
			get;
		}
		
		//// <value>
		/// Width
		/// </value>
		double Width
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
		
		double Scaling
		{
			get;
		}
		
		bool Flipped
		{
			get;set;
		}
		
		double AnimationSpeedFactor
		{
			get;set;
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
