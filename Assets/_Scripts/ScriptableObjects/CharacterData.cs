using UnityEngine;

namespace Gnosronpa.ScriptableObjects
{
	[CreateAssetMenu(fileName ="Character", menuName ="NonstopDebate/Character")]
	public class CharacterData : ScriptableObject
	{
		public string characterName;
		public Sprite avatar;
	}
}
