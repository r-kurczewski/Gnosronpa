using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(CutsceneData), menuName = "Dialog/Cutscene")]
	public class CutsceneData : ScriptableObject
	{
		public Sprite sprite;
	}
}
