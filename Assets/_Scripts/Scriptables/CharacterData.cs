using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(CharacterData), menuName = "")]
	public class CharacterData : ScriptableObject
	{
		public string characterName;

		public Sprite avatar;
	}
}
