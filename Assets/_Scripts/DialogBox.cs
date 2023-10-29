using Cysharp.Threading.Tasks;
using Gnosronpa.Common;
using Gnosronpa.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	public class DialogBox : MonoBehaviour
	{
		public event Action<DialogMessage> OnMessageChanged;

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

		[SerializeField]
		private bool messagesEnded;

		private Queue<DialogMessage> messages = new();

		private CancellationTokenSource revealTextTokenSource;

		public bool MessageContentDisplayed => messageContentDisplayed;

		public bool MessagesEnded => messagesEnded;


		[SerializeField]
		private DialogMessage currentMessage;

		public void AddMessage(DialogMessage message)
		{
			messages.Enqueue(message);
			messagesEnded = false;
		}

		public void SetVisibility(bool visibility)
		{
			gameObject.SetActive(visibility);
		}

		public void LoadNextMessage(bool playSound = true)
		{
			if (playSound) AudioController.instance.PlaySound(messageLoadSound);

			if (!messages.Any())
			{
				messagesEnded = true;
				return;
			}

			currentMessage = messages.Dequeue();

			messageContentDisplayed = false;
			SetTitle(currentMessage.speakingCharacter.characterName);
			SetMessage(currentMessage.messageText);

			revealTextTokenSource = new CancellationTokenSource();
			_ = RevealText(revealTextTokenSource.Token);

			OnMessageChanged?.Invoke(currentMessage);
		}

		public void ForceDisplayMessage()
		{
			revealTextTokenSource.Cancel();

			message.maxVisibleCharacters = message.textInfo.characterCount;

			messageContentDisplayed = true;
		}

		private async UniTask RevealText(CancellationToken cancellationToken)
		{
			message.ForceMeshUpdate();

			var totalCharacters = message.textInfo.characterCount;
			var visibleCharacters = 0;

			while (visibleCharacters <= totalCharacters)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				message.maxVisibleCharacters = visibleCharacters;
				visibleCharacters++;
				await UniTask.Delay(TimeSpan.FromSeconds(characterLoadDelay));
			}
			messageContentDisplayed = true;
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
