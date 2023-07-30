using Gnosronpa.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		private void Start()
		{
			//StartCoroutine(SetRandomCharacters());
		}

		public void SetCharacter(CharacterData character)
		{
			label.enabled = character is not null;
			avatar.enabled = character is not null;

			label.text = character?.characterName;
			avatar.sprite = character?.avatar;
		}

		private IEnumerator SetRandomCharacters()
		{
			var wait = new WaitForSeconds(4);
			var list = charactersList.ToList();
			while (true)
			{


				var pick = list[Random.Range(0, list.Count)];
				list.Remove(pick);

				SetCharacter(pick);

				if (list.Count is 0) list = charactersList.ToList();

				yield return wait;
			}
		}
	}
}
