using Cysharp.Threading.Tasks;
using Gnosronpa.Common;
using Gnosronpa.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Gnosronpa.Controllers
{
	public class DialogController : SingletonBase<DialogController>
	{
		private const float characterLoadDelay = 0.03f;

		[SerializeField]
		private InputActionReference inputNextDialog;

		[SerializeField]
		private GameObject dialogBox;

		[SerializeField]
		private CharacterInfo characterInfo;

		[SerializeField]
		private TMP_Text title;

		[SerializeField]
		private GameObject titleBox;

		[SerializeField]
		private TMP_Text message;

		[SerializeField]
		private AudioClip messageLoadSound;

		[SerializeField]
		private bool messageContentDisplayed;

		[SerializeField]
		private bool messagesEnded;

		[SerializeField]
		private bool _ignoreUserInput;

		private Queue<DialogMessage> messages = new();

		public bool IgnoreUserInput { get => _ignoreUserInput; set => _ignoreUserInput = value; }

		private CancellationTokenSource revealTextTokenSource;

		public bool MessageContentDisplayed => messageContentDisplayed;

		public bool MessagesEnded => messagesEnded;


		private void OnEnable()
		{
			inputNextDialog.action.performed += OnNextDialogMessage;
		}

		private void OnDisable()
		{
			inputNextDialog.action.performed -= OnNextDialogMessage;
		}

		[SerializeField]
		private DialogMessage currentMessage;

		public void AddMessage(params DialogMessage[] msgs)
		{
			foreach (var msg in msgs)
			{
				messages.Enqueue(msg);
			}
			messagesEnded = false;
		}

		public void SetVisibility(bool visibility)
		{
			dialogBox.SetActive(visibility);

			if (visibility) inputNextDialog.action.Enable();
			else inputNextDialog.action.Disable();
		}

		public void LoadNextMessage(bool playSound = true)
		{
			if (!messages.Any())
			{
				messagesEnded = true;
				return;
			}

			if (playSound) _ = AudioController.instance.PlaySound(messageLoadSound);

			currentMessage = messages.Dequeue();

			if (currentMessage.dialogSource is null)
			{
				Debug.LogError($"DialogSource cannot be null: {currentMessage.messageText}");
			}

			messageContentDisplayed = false;
			SetTitle(currentMessage.dialogSource.SourceName);
			SetMessage(currentMessage.FormatedMessage);
			titleBox.SetActive(currentMessage.dialogSource.ShowTitle);

			if (currentMessage.dialogSource.CameraTarget is GameObject target && !currentMessage.skipAnimation)
			{
				CameraController.instance.PlayCameraAnimation(currentMessage.cameraAnimation, target);
			}


			revealTextTokenSource = new CancellationTokenSource();
			_ = RevealText(revealTextTokenSource.Token);
		}

		public void ForceDisplayMessage()
		{
			revealTextTokenSource.Cancel();

			message.maxVisibleCharacters = message.textInfo.characterCount;

			messageContentDisplayed = true;
		}

		public void SetSpeakingCharacter(CharacterData characterData)
		{
			characterInfo.SetCharacter(characterData);
		}

		public void ClearDialogBox(bool showTitle)
		{
			SetTitle(string.Empty);
			SetMessage(string.Empty);
			titleBox.SetActive(showTitle);
		}

		private void OnNextDialogMessage(CallbackContext context = default)
		{
			if (IgnoreUserInput) return;

			if (MessageContentDisplayed)
			{
				LoadNextMessage(playSound: true);
			}
			else
			{
				ForceDisplayMessage();
			}
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
