using Cysharp.Threading.Tasks;
using Gnosronpa.Animations;
using Gnosronpa.Common;
using Gnosronpa.Scriptables;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static Gnosronpa.Common.AudioConsts;
using static UnityEngine.InputSystem.InputAction;

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
		private GameObject votingAnimationPrefab;

		[SerializeField]
		private Transform votingAnimationParent;

		[SerializeField]
		private DialogSource bulletObtainedDialogSource;

		[SerializeField]
		private Canvas canvas;

		[SerializeField]
		private CustomCursor cursor;

		[SerializeField]
		private CutsceneView cutscene;

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
			while (currentSegment);

			await OnScenarioEnd();
		}

		private async Task OnScenarioEnd()
		{
			cursor.gameObject.SetActive(false);
			await CameraFade.Instance.FadeOut(1.5f);
			SceneController.Instance.LoadMenu();
		}

		private async UniTask<GameplaySegmentData> LoadGameMechanic(GameplaySegmentData mechanic)
		{
			if (mechanic is NonstopDebate debate)
			{
				await debateController.ResetMachineState();
				debateController.Init(debate);
				await UniTask.WaitUntil(() => debateController.CurrentState == debateController.FinalState);
			}
			else if (mechanic is Discussion discussion)
			{
				// enter cutscene
				if (!cutscene.Data && discussion.cutscene)
				{
					await CameraFade.Instance.FadeOut();

					DialogController.Instance.SetVisibility(false);
					if (discussion.segmentMusic) AudioController.Instance.PlayMusic(discussion.segmentMusic);

					cutscene.SetCutscene(discussion.cutscene);
					cutscene.Show();

					await CameraFade.Instance.FadeIn();
				}
				// exit cutscene
				else if (cutscene.Data && !discussion.cutscene)
				{
					await CameraFade.Instance.FadeOut();

					cutscene.SetCutscene(null);
					cutscene.Hide();
					if (discussion.segmentMusic) AudioController.Instance.PlayMusic(discussion.segmentMusic);
				}
				else
				{
					if (discussion.segmentMusic) AudioController.Instance.PlayMusic(discussion.segmentMusic);
				}

				DialogController.Instance.SetVisibility(true);
				discussion.messages.ForEach(message => DialogController.Instance.AddMessage(message));
				DialogController.Instance.LoadNextMessage(discussion.playFirstMessageSound);

				await CameraFade.Instance.FadeIn();

				await UniTask.WaitUntil(() => DialogController.Instance.MessagesEnded);
			}
			else if (mechanic is BulletObtained obtained)
			{
				foreach (var bullet in obtained.bullets)
				{
					var animation = Instantiate(obtainedAnimationPrefab, canvas.transform, false)
						.GetComponent<BulletObtainedAnimation>();

					animation.name = bullet.bulletName;

					var bulletObtainedMessage = new DialogMessage(obtained.GetMessage(bullet), bulletObtainedDialogSource)
					{
						skipAnimation = true
					};

					using (new DialogController.PlayerInputLock())
					{
						await animation.PlayStartingAnimation(bullet);
					}

					DialogController.Instance.AddMessage(bulletObtainedMessage);
					DialogController.Instance.LoadNextMessage(playSound: false);

					await UniTask.WaitUntil(() => DialogController.Instance.MessagesEnded);

					await animation.PlayEndingAnimation();
					Destroy(animation.gameObject);
				}
			}
			else if (mechanic is Voting voting)
			{
				var fadeDuration = 2f;

				var fadeMusic = AudioController.Instance.FadeOutMusic(off, fadeDuration);
				var fadeScreen = CameraFade.Instance.FadeOut(fadeDuration);
				await UniTask.WhenAll(fadeMusic, fadeScreen);

				DialogController.Instance.SetVisibility(false);

				var animation = Instantiate(votingAnimationPrefab, votingAnimationParent, false)
						.GetComponent<VotingAnimation>();

				await UniTask.Delay(1000);

				await animation.PlayAnimation(voting);
				await CameraFade.Instance.FadeOut();

				Destroy(animation.gameObject);
				_ = AudioController.Instance.FadeOutMusic(on);
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
