
using System;
using System.Drawing;
using SdlDotNet.Input;

namespace Engine
{
	/// <summary>
	/// Abstracting the key definitions
	/// </summary>
	public enum HKey
	{
		LeftArrow = Key.LeftArrow,
		RightArrow = Key.RightArrow,
		UpArrow = Key.UpArrow,
		DownArrow = Key.DownArrow,
		A = Key.A, B = Key.B, C = Key.C, D = Key.D,
		E = Key.E, F = Key.F, G = Key.G, H = Key.H,
		I = Key.I, J = Key.J, K = Key.K, L = Key.L,
		M = Key.M, N = Key.N, O = Key.O, P = Key.P,
		Q = Key.Q, R = Key.R, S = Key.S, T = Key.T,
		U = Key.U, V = Key.V, W = Key.W, X = Key.X,
		Y = Key.Y, Z = Key.Z,
		One = Key.One, Two = Key.Two, Three = Key.Three,
		Four = Key.Four, Five = Key.Five, Six = Key.Six,
		Seven = Key.Seven, Eight = Key.Eight, Nine = Key.Nine,
		Zero = Key.Zero,
		
		Delete = Key.Delete,
		Escape = Key.Escape,
		LeftControl = Key.LeftControl,
		LeftAlt = Key.LeftAlt,
		Return = Key.Return,
		RightControl = Key.RightControl,
		RightAlt = Key.RightAlt,
		Space = Key.Space,
		Tab = Key.Tab
		
		
	};
	
	public enum HMouseButton
	{
		LeftButton = MouseButton.PrimaryButton,
		RightButton = MouseButton.SecondaryButton,
		MiddleButton = MouseButton.MiddleButton,
		WheelUp = MouseButton.WheelUp,
		WheelDown = MouseButton.WheelDown,
		None = MouseButton.None
	}
	
	public class InputManager
	{

		
		public InputManager()
		{
		}
		
		public bool KeyPressed(HKey key)
		{
			return Keyboard.IsKeyPressed((Key)key);
		}
		


		//Convert window coordinates to world coordinates
		public Vector WindowToWorldPosition(Vector windowPos, Display display, bool relative)
		{
			double x = ((double)(windowPos.X * display.ViewportWidth)) / (double)display.WindowWidth;
			
			if (relative)
				return new Vector(x, -((double)(windowPos.Y * display.ViewportHeight)) / (double)display.WindowHeight);
			else 
				return new 
					Vector(x - (double)display.ViewportWidth / 2.0 + display.CameraX, 
					       (double)display.ViewportHeight / 2.0 - ((double)(windowPos.Y * display.ViewportHeight)) / (double)display.WindowHeight+display.CameraY);
		}
		
		/// <value>
		/// Window coordinates for mouse cursor
		/// </value>
		public Vector MouseWindowPosition
		{
			get { return new Vector(Mouse.MousePosition.X, Mouse.MousePosition.Y); }
		}
		
		/// <value>
		/// Change in mouse position in window coordinates
		/// </value>
		public Vector MouseWindowPositionChange
		{
			get 
			{
				Point p = Mouse.MousePositionChange;
				return new Vector(p.X, p.Y); 
			}
		}
		
		public bool MouseButtonPressed(HMouseButton button)
		{
			return (Mouse.IsButtonPressed((MouseButton)button));
		}
		
		public bool ShowCursor
		{
			get { return Mouse.ShowCursor; }
			set { Mouse.ShowCursor = value; }
		}
		
	}
}
