using Gnosronpa.ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class CharacterInfo : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text label;

		[SerializeField]
		private Image avatar;

		[SerializeField]
		private List<CharacterData> charactersList;

		public void SetCharacter(CharacterData character)
		{
			label.enabled = character is not null;
			avatar.enabled = character is not null;

			label.text = character?.characterName;
			avatar.sprite = character?.avatar;
		}
	}
}
