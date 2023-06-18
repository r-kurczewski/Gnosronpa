using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{
	[SerializeField]
	private Texture2D cursorTexture;

	[SerializeField]
	private InputActionReference cursorPosition;

	private bool IsMouseOverGameWindow
	{
		get
		{
			var mousePos = cursorPosition.action.ReadValue<Vector2>();
			return mousePos.x > 0 && mousePos.y > 0 && mousePos.x < Screen.width && mousePos.y < Screen.height;
		}
	}

	private void Update()
	{
		if (IsMouseOverGameWindow) SetCursor();
		else ResetCursor();
	}

	private void OnDisable()
	{
		ResetCursor();
	}
	private void SetCursor()
	{
		Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.Auto);
	}

	private void ResetCursor()
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
}
