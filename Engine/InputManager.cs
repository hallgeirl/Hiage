
using System;
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
		Y = Key.Y, Z = Key.Z
	};
	
	public class InputManager
	{

		
		
		public InputManager()
		{
		}
		
		public bool KeyPressed(HKey key)
		{
			return Keyboard.IsKeyPressed((Key)key);
		}
	}
}
