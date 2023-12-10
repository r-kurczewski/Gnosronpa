using Cysharp.Threading.Tasks;
using Gnosronpa.Scriptables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using static Gnosronpa.Common.AnimationConsts;
using Gnosronpa.Animations;
using Gnosronpa.Common;

namespace Gnosronpa.Controllers
{
	public class GameController : SingletonBase<GameController>
	{
		private const string NonstopDebateActionMapKey = "Nonstop Debate";
		private const string MenuActionMapKey = "Menu";

		public static GameplaySegmentData startingSegment;

		[SerializeField]
		private PlayerInput playerInput;

		[SerializeField]
		private DebateController debateController;

		[SerializeField]
		private InputActionReference inputShowMenu;

		[SerializeField]
		private InputActionReference inputHideMenu;

		[SerializeField]
		private BulletDescriptionPanel bulletDescriptionPanel;

		[SerializeField]
		private GameObject obtainedAnimationPrefab;

		[SerializeField]
		private DialogSource bulletObtainedDialogSource;

		[SerializeField]
		private Canvas canvas;

		[SerializeField]
		private CustomCursor cursor;

		[SerializeField]
		private GameObject bulletsParent;

		[SerializeField]
		private bool paused = false;

		[SerializeField]
		private float previousTimeScale = 1;

		[SerializeField]
		private GameplaySegmentData startingSegmentFallback;

		[SerializeField]
		private GameplaySegmentData currentSegment;

		private Dictionary<InputAction, bool> previousInputState = new();

		public bool Paused => paused;

		private async void Start()
		{
			await Init();
		}

		private void OnEnable()
		{
			inputShowMenu.action.performed += ToggleMenu;
			inputHideMenu.action.performed += ToggleMenu;
		}

		private void OnDisable()
		{
			inputShowMenu.action.performed -= ToggleMenu;
			inputHideMenu.action.performed -= ToggleMenu;
		}

		public async UniTask Init()
		{
			currentSegment = startingSegment ?? startingSegmentFallback;	
			Cursor.visible = false;

			await RunGameLoop();
		}

		public async UniTask RunGameLoop()
		{
			do
			{
				currentSegment = await LoadGameMechanic(currentSegment);
			}
			while (currentSegment != null);

			cursor.gameObject.SetActive(false);
			await CameraFade.instance.DOFade(show, 1.5f);
			SceneController.instance.LoadMenu();
		}

		private async UniTask<GameplaySegmentData> LoadGameMechanic(GameplaySegmentData mechanic)
		{
			if (mechanic.segmentMusic) AudioController.instance.PlayMusic(mechanic.segmentMusic);
			if (mechanic is NonstopDebate debate)
			{
				await debateController.ResetMachineState();
				debateController.Init(debate);
				await UniTask.WaitUntil(() => debateController.CurrentState == debateController.FinalState);
			}
			else if (mechanic is Discussion discussion)
			{
				DialogController.instance.SetVisibility(true);
				discussion.messages.ForEach(message => DialogController.instance.AddMessage(message));
				DialogController.instance.LoadNextMessage(discussion.playFirstMessageSound);

				await UniTask.WaitUntil(() => DialogController.instance.MessagesEnded);
			}
			else if (mechanic is BulletObtained obtained)
			{
				var animation = Instantiate(obtainedAnimationPrefab, canvas.transform, false)
					.GetComponent<BulletObtainedAnimation>();

				animation.name = obtained.bullet.bulletName;

				var bulletObtainedMessage = new DialogMessage(obtained.GetMessage(obtained.bullet), bulletObtainedDialogSource);
				bulletObtainedMessage.skipAnimation = true;

				DialogController.instance.IgnoreUserInput = true;
				await animation.PlayStartingAnimation(obtained.bullet);
				DialogController.instance.IgnoreUserInput = false;

				DialogController.instance.AddMessage(bulletObtainedMessage);
				DialogController.instance.LoadNextMessage(playSound: false);

				await UniTask.WaitUntil(() => DialogController.instance.MessagesEnded);

				await animation.PlayEndingAnimation();
				Destroy(animation.gameObject);
			}
			return mechanic.nextGameplaySegment;
		}

		private async void ToggleMenu(CallbackContext ctx = default)
		{
			if (!paused)
			{
				PauseGame();
			}
			else
			{
				UnpauseGame();
			}
			await bulletDescriptionPanel.SetVisibility(paused);
		}

		private void PauseGame()
		{
			previousTimeScale = Time.timeScale;
			Time.timeScale = 0;

			bulletsParent.SetActive(false);

			SaveCurrentInputEnabledState();
			playerInput.SwitchCurrentActionMap(MenuActionMapKey);

			paused = true;
		}

		private void UnpauseGame()
		{
			Time.timeScale = previousTimeScale;
			playerInput.SwitchCurrentActionMap(NonstopDebateActionMapKey);
			RestorePreviousInputEnabledState();
			bulletsParent.SetActive(true);
			paused = false;
		}

		private void SaveCurrentInputEnabledState()
		{
			foreach (var action in playerInput.actions)
			{
				previousInputState[action] = action.enabled;
			}
		}

		private void RestorePreviousInputEnabledState()
		{
			foreach (var action in playerInput.actions)
			{
				if (previousInputState[action]) action.Enable();
				else action.Disable();
			}
			previousInputState.Clear();
		}
	}
}
