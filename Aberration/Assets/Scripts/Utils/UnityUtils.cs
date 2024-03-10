using System.Collections.Generic;
using UnityEngine;

namespace Aberration.Assets.Scripts.Utils
{
	public static class UnityUtils
	{
		public static void DestroyMonoBehaviorGOs<T>(List<T> monoBehaviours) where T : MonoBehaviour
		{
			if (monoBehaviours != null)
			{
				foreach (T instance in monoBehaviours)
				{
					SafeDestroyGO(instance);
				}

				monoBehaviours.Clear();
			}
		}

		public static void SafeDestroyGO<T>(T monoBehaviour) where T : MonoBehaviour
		{
			if (monoBehaviour != null)
				GameObject.Destroy(monoBehaviour.gameObject);
		}
	}
}
