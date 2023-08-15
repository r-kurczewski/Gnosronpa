using Gnosronpa.Common;
using Gnosronpa.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	public class DialogBox : MonoBehaviour
	{
		public event Action<DialogMessage> OnMessageChanged;

		public event Action<DialogMessage> OnMessageContentDisplayed;

		public event Action OnMessagesEnded;

		[SerializeField]
		private TMP_Text title;

		[SerializeField]
		private TMP_Text message;

		[SerializeField]
		private float characterLoadDelay;

		[SerializeField]
		private AudioClip messageLoadSound;
		
		[SerializeField]
		private bool messageContentDisplayed;

		private Queue<DialogMessage> messages = new();

		private Coroutine revealTextCoroutine;

		public bool MessageContentDisplayed => messageContentDisplayed;


		[SerializeField]
		private DialogMessage currentMessage;

		public void AddMessage(DialogMessage message)
		{
			messages.Enqueue(message);
		}

		public void SetVisibility(bool visibility)
		{
			gameObject.SetActive(visibility);
		}

		public void LoadNextMessage(bool playSound = true)
		{
			if(playSound) AudioController.instance.PlaySound(messageLoadSound);

			if (!messages.Any())
			{
				OnMessagesEnded?.Invoke();
				return;
			}

			currentMessage = messages.Dequeue();

			messageContentDisplayed = false;
			SetTitle(currentMessage.speakingCharacter.characterName);
			SetMessage(currentMessage.messageText);
			RevealText();

			OnMessageChanged?.Invoke(currentMessage);
		}

		public void ForceDisplayMessage()
		{
			StopCoroutine(revealTextCoroutine);
			
			message.maxVisibleCharacters = message.textInfo.characterCount;
			
			messageContentDisplayed = true;
			OnMessageContentDisplayed?.Invoke(currentMessage);
		}

		private void RevealText()
		{
			revealTextCoroutine = StartCoroutine(IRevealText());

			IEnumerator IRevealText()
			{
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
				OnMessageContentDisplayed?.Invoke(currentMessage);
				messageContentDisplayed = true;
			}
		}

		private void SetTitle(string title)
		{
			this.title.text = title;
		}

		private void SetMessage(string message)
		{
			this.message.text = message;
		}
	}
}
