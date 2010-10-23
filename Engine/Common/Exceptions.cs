
using System;

namespace Engine
{
	
	/// <summary>
	/// Probably not needed, use InvalidArgumentException instead in most cases.
	/// </summary>
	public class InvalidValueException : Exception
	{
		public InvalidValueException() : base()
		{
		}
		public InvalidValueException(string what) : base(what)
		{
		}
		public InvalidValueException(string what, Exception innerException) : base(what, innerException)
		{
		}
	}
	
	/// <summary>
	/// Thrown when there is an issue with a key (for instance in a dictionary).
	/// </summary>
	public class KeyException : Exception
	{
		public KeyException() : base()
		{
		}
		public KeyException(string what) : base(what)
		{
		}
		public KeyException(string what, Exception innerException) : base(what, innerException)
		{
		}
	}
	
	/// <summary>
	/// Thrown when something is not found when it was expected
	/// </summary>
	public class NotFoundException : Exception
	{
		public NotFoundException() : base()
		{
		}
		public NotFoundException(string what) : base(what)
		{
		}
		public NotFoundException(string what, Exception innerException) : base(what, innerException)
		{
		}
	}
	
	public class TextureException : Exception
	{
		public TextureException() : base()
		{
		}
		public TextureException(string what) : base(what)
		{
		}
		public TextureException(string what, Exception innerException) : base(what, innerException)
		{
		}
	}
}
