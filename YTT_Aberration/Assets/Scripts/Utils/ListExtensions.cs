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
			return (list != null) ? list.Count : 0;
		}

		/// <summary>
		/// Remove the object with a null check.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="instance"></param>
		/// <returns>True if the list exists and the instance was removed from it, null otherwise.</returns>
		public static bool SafeRemove<T>(this List<T> list, T instance)
		{
			return (list != null) ? list.Remove(instance) : false;
		}

		/// <summary>
		/// Check if List Contains the object with a null check.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="instance"></param>
		/// <returns>True if the list exists and the instance exists in it, null otherwise.</returns>
		public static bool SafeContains<T>(this List<T> list, T instance)
		{
			return (list != null) ? list.Contains(instance) : false;
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
