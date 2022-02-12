
using System;
using System.Collections.Generic;
using Engine;

namespace Testing
{

	public class GeneratedMap
	{
		private class Node
		{
			public Node(int x, int y)
			{
				X = x;
				Y = y;
			}
			
			public int X
			{
				get;
				private set;
			}
			
			public int Y
			{
				get;
				private set;
			}
			
			public List<Node> Children
			{
				get;
				private set;
			}
		}
				
		Node root;
		Renderer renderer;
		
		public GeneratedMap(int width, int height, Renderer renderer)
		{
			this.renderer = renderer;
			bool[,] visited = new bool[width, height];
			List<Node> nodeStack = new List<Node>();
			root = new Node(0,0);
			
			nodeStack.Add(root);
			
			while (nodeStack.Count > 0)
			{
				Node current = nodeStack[nodeStack.Count-1];
				nodeStack.Remove(current);
				visited[current.X, current.Y] = true;
				
				if (current.X > 0 && !visited[current.X-1, current.Y])
					current.Children.Add(new Node(current.X-1, current.Y));
				if (current.X < width-1 && !visited[current.X+1, current.Y])
					current.Children.Add(new Node(current.X+1, current.Y));
				if (current.Y > 0 && !visited[current.X, current.Y-1])
					current.Children.Add(new Node(current.X, current.Y-1));
				if (current.Y < height-1 && !visited[current.X, current.Y-1])
					current.Children.Add(new Node(current.X, current.Y+1));
				
				nodeStack.AddRange(current.Children);
			}
			
		}
		
		public void Render()
		{
			List<Node> nodeStack = new List<Node>();
			nodeStack.Add(root);
			
			while (nodeStack.Count > 0)
			{
				Node current = nodeStack[nodeStack.Count-1];
				nodeStack.Remove(current);
				
				foreach (Node n in current.Children)
				{
					renderer.DrawLine(current.X, current.Y, n.X, n.Y);
				}
			}
			
			
		}
	}
}
