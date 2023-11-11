using Cysharp.Threading.Tasks;
using Gnosronpa.Assets._Scripts.Scriptables;
using Gnosronpa.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Gnosronpa.Controllers
{
	public class GameController : SingletonBase<GameController>
	{
		private const string NonstopDebateActionMapKey = "Nonstop Debate";
		private const string MenuActionMapKey = "Menu";

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
		private GameObject bulletsParent;

		[SerializeField]
		private bool paused = false;

		[SerializeField]
		private float previousTimeScale = 1;

		[SerializeField]
		private GameplaySegmentData startMechanic;

		[SerializeField]
		private GameplaySegmentData currentMechanic;

		private Dictionary<InputAction, bool> previousInputState = new();

		public bool Paused => paused;

		private void Start()
		{
			_ = Init();
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
			currentMechanic = startMechanic;
			await RunGameLoop();
		}

		public async UniTask RunGameLoop()
		{
			do
			{
				currentMechanic = await LoadGameMechanic(currentMechanic);
			}
			while (currentMechanic != null);
		}

		private async UniTask<GameplaySegmentData> LoadGameMechanic(GameplaySegmentData mechanic)
		{
			AudioController.instance.PlayMusic(mechanic.segmentMusic);
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
				DialogController.instance.LoadNextMessage(playSound: false);

				await UniTask.WaitUntil(() => DialogController.instance.MessagesEnded);
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
