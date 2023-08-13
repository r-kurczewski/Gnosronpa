using System.Collections;
using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	public class DialogBox : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text title;

		[SerializeField]
		private TMP_Text message;

		[SerializeField]
		private float characterLoadDelay;

		[SerializeField]
		private bool textLoaded;

		public bool TextLoaded => textLoaded;

		public void RevealText()
		{
			StartCoroutine(IRevealText());

			IEnumerator IRevealText()
			{
				textLoaded = false;
				message.ForceMeshUpdate();

				var totalCharacters = message.textInfo.characterCount;
				var visibleCharacters = 0;
				var characterLoadDelay = new WaitForSeconds(this.characterLoadDelay);

				while (visibleCharacters <= totalCharacters)
				{
					message.maxVisibleCharacters = visibleCharacters;
					visibleCharacters++;
					yield return characterLoadDelay;
				}

				textLoaded = true;
			}
		}
		
		public void SetTitle(string title)
		{
			this.title.text = title;
		}

		public void SetMessage(string message)
		{
			this.message.text = message;
		}

		public void SetVisibility(bool visibility)
		{
			gameObject.SetActive(visibility);
		}
	}
}
