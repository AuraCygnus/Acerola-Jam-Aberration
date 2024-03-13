using System;
using UnityEditor;
using UnityEngine;

namespace Aberration
{
	/// <summary>
	/// Handy script for instantiating ScriptableObjects.
	/// From: https://gist.github.com/ParanoidBigfoot/f9e2285962fcce18fbbfc49103c1405f
	/// </summary>
	public class CreateSingleScriptableObjectAssetContextMenu
	{
		[MenuItem("Assets/Create Asset From ScriptableObject")]
		public static void CreateAsset()
		{
			MonoScript scriptFile = Selection.activeObject as MonoScript;
			Type t = scriptFile.GetClass();
			string fileType = ObjectNames.NicifyVariableName(t.Name);
			string path = EditorUtility.SaveFilePanelInProject($"Create new {fileType}", fileType, "asset", "");
			if (path.Length > 0)
			{
				AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(t), path);
				UnityEngine.Object o = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
				ProjectWindowUtil.ShowCreatedAsset(o);
			}
		}

		[MenuItem("Assets/Create Asset From ScriptableObject", validate = true)]
		public static bool CreateAssetValidation()
		{
			UnityEngine.Object selected = Selection.activeObject;
			if (selected != null)
			{
				if (selected.GetType() == typeof(MonoScript))
				{
					MonoScript scriptFile = selected as MonoScript;
					Type t = scriptFile.GetClass();
					if (t != null)
					{
						if (!t.IsAbstract)
						{
							if (t.IsSubclassOf(typeof(ScriptableObject)))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}
}
