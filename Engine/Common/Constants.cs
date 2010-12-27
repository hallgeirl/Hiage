using System;
namespace Engine
{
	public static class Constants
	{
		// This is used as a roundoff point to zero for doubles.
		// So anything below MinDouble may be treated as zero, because of rounding errors.
		public const double MinDouble = 1e-12;
	}
}

