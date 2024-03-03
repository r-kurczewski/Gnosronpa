using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{
	[SerializeField]
	private bool customCursorVisible;

	[SerializeField]
	private InputActionReference cursorPosition;

	[SerializeField]
	private GameObject cursorImageParent;

	[SerializeField]
	private GameObject cursorSpin;

	[SerializeField]
	private float spinSpeed;

	private bool IsMouseOverGameWindow
	{
		get
		{
			var mousePos = cursorPosition.action.ReadValue<Vector2>();
			return mousePos.x > 0 && mousePos.y > 0 && mousePos.x < Screen.width && mousePos.y < Screen.height;
		}
	}

	private void FixedUpdate()
	{
		Cursor.visible = !IsMouseOverGameWindow;
		cursorImageParent.SetActive(customCursorVisible);

		if (cursorImageParent.activeInHierarchy)
		{
			UpdateCursorPosition();
			UpdateCursorRotation();
		}
	}

	public void SetCursorVisible(bool resetPosition = false)
	{
		customCursorVisible = true;
		if (resetPosition) ResetCursorPosition();
	}

	public void HideCursor()
	{
		customCursorVisible = false;
	}

	private void ResetCursorPosition()
	{
		Mouse.current.WarpCursorPosition(new Vector2(Screen.width/2, Screen.height/2));
	}

	private void UpdateCursorPosition()
	{
		Vector3 mouseCords = cursorPosition.action.ReadValue<Vector2>();
		mouseCords.z = GetComponentInParent<Canvas>().planeDistance;
		var mousePosInWorldSpace = Camera.main.ScreenToWorldPoint(mouseCords);

		transform.position = mousePosInWorldSpace;
	}

	private void UpdateCursorRotation()
	{
		cursorSpin.transform.rotation *= Quaternion.Euler(0, 0, -spinSpeed * Time.unscaledDeltaTime);
	}
}
