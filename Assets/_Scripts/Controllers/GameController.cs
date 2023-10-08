using Gnosronpa.Controllers;
using Gnosronpa.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Gnosronpa.Assets._Scripts.Controllers
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
		private BulletController bulletController;

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
		private DebateData startDebate;

		private void Start()
		{
			debateController.Init(startDebate);
			bulletDescriptionPanel.Init(startDebate.bullets);
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
			playerInput.SwitchCurrentActionMap(MenuActionMapKey);
			bulletsParent.SetActive(false);
			paused = true;
		}

		private void UnpauseGame()
		{
			Time.timeScale = previousTimeScale;
			playerInput.SwitchCurrentActionMap(NonstopDebateActionMapKey);
			bulletsParent.SetActive(true);
			paused = false;
		}
	}
}
