using System.Collections.Generic;

namespace Aberration.Assets.Scripts.Utils
{
	public static class ListUtils
	{
		/// <summary>
		/// Ensure that the list exists before trying to add a <typeparamref name="T"/> instance to it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">Reference to list/list location</param>
		/// <param name="instance">Instance of <typeparamref name="T"/> to add to List.</param>
		public static void SafeAdd<T>(ref List<T> list, T instance)
		{
			if (list == null)
				list = new List<T>();

			list.Add(instance);
		}
	}
}
