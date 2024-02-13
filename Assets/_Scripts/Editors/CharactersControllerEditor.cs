using Gnosronpa.Controllers;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gnosronpa.Editors
{
	[CustomEditor(typeof(CharactersController))]
	public class CharactersControllerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var script = target as CharactersController;

			if (GUILayout.Button("Set Positions"))
			{
				var visibleCharacters = script.Characters.Where(x => x.gameObject.activeInHierarchy).ToList();
				script.SetCharactersPositions(visibleCharacters);
			}
		}
	}
}
