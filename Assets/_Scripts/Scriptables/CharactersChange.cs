using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(CharactersChange), menuName = "Scripts/" + nameof(CharactersChange))]
	public class CharactersChange : GameplaySegmentData
	{
		public List<CharacterData> characters;
	}
}
