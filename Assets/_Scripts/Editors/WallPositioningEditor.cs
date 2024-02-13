using Gnosronpa.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gnosronpa.Editors
{
	[CustomEditor(typeof(WallsPositioning))]
	public class WallPositioningEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var script = target as WallsPositioning;

			if (GUILayout.Button("Set Positions"))
			{
				script.SetPositions();
			}
		}
	}
}
