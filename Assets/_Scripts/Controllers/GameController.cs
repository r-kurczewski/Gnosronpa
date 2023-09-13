using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Gnosronpa.Assets._Scripts.Controllers
{
	public class GameController : SingletonBase<GameController>
	{
		[SerializeField]
		private InputActionReference menuButton;

		[SerializeField]
		private BulletDescriptionPanel bulletDescriptionPanel;

		[SerializeField]
		private bool paused = false;

		[SerializeField]
		private float previousTimeScale = 1;

		private void OnEnable()
		{
			menuButton.action.performed += ToggleMenu;
		}

		private void OnDisable()
		{
			menuButton.action.performed -= ToggleMenu;
		}

		private async void ToggleMenu(CallbackContext ctx = default)
		{
			if (!paused)
			{
				previousTimeScale = Time.timeScale;
				Time.timeScale = 0;
				paused = true;
			}
			else
			{
				Time.timeScale = previousTimeScale;
				paused = false;
			}
			await bulletDescriptionPanel.SetVisibility(paused);
		}
	}
}
