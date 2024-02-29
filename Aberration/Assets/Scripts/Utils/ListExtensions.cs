using System.Collections.Generic;

namespace Aberration.Assets.Scripts.Utils
{
	public static class ListExtensions
	{
		/// <summary>
		/// Get the count with a null check.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">Target list to try to get the length of.</param>
		/// <returns>The count if the list exists, 0 if the list is null.</returns>
		public static int SafeCount<T>(this List<T> list)
		{
			if (list == null)
				return 0;

			return list.Count;
		}

		/// <summary>
		/// Only clear if the List actually exists.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">Potential target list.</param>
		public static void SafeClear<T>(this List<T> list)
		{
			if (list != null)
				list.Clear();
		}
	}
}
