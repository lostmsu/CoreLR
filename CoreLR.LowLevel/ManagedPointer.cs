namespace CoreLR.LowLevel
{
	using System;

	/// <summary>
	/// Struct, representing managed pointers (e.g. pointers tracked by GC)
	/// </summary>
	/// <typeparam name="T">Type of the item, referenced by this pointer</typeparam>
	public struct ManagedPointer<T> where T : struct
	{
		IntPtr pointer;
	}
}
