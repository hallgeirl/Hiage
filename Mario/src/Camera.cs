using System;
using Engine;

namespace Mario
{
	public class Camera
	{
		private Display display;
		
		public Camera(Display display, double xMin, double xMax, double yMin, double yMax)
		{
			this.display = display;
			X = display.CameraX;
			Y = display.CameraY;
			
			XMin = xMin;
			XMax = xMax;
			YMin = yMin;
			YMax = yMax;
		}
		
		public double XMin
		{
			get;
			private set;
		}
		
		public double XMax
		{
			get;
			private set;
		}
		
		public double YMin
		{
			get;
			private set;
		}
		
		public double YMax
		{
			get;
			private set;
		}
		
		double x;
		public double X
		{
			get { return x;}
			set 
			{
				double val = Math.Max(value, XMin + display.ViewportWidth / 2);
				val = Math.Min(val, XMax - display.ViewportWidth / 2);
				x = val;
				display.CameraX = val;
			}
		}
		
		double y;
		public double Y
		{
			get { return y; }
			set
			{
				double val = Math.Max(value, YMin + display.ViewportHeight / 2);
				val = Math.Min(val, YMax - display.ViewportHeight / 2);
				y = val;
				display.CameraY = val;
			}
		}
	}
}

