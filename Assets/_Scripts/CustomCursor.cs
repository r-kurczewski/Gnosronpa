using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{
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
		UpdateCursorPosition();
		UpdateCursorRotation();
	}

	private void UpdateCursorPosition()
	{
		if (IsMouseOverGameWindow)
		{
			SetCursor();

			Vector3 mouseCords = cursorPosition.action.ReadValue<Vector2>();
			mouseCords.z = GetComponentInParent<Canvas>().planeDistance;
			var mousePosInWorldSpace = Camera.main.ScreenToWorldPoint(mouseCords);

			transform.position = mousePosInWorldSpace;
		}
		else ResetCursor();
	}

	private void UpdateCursorRotation()
	{
		cursorSpin.transform.rotation *= Quaternion.Euler(0, 0, -spinSpeed * Time.unscaledDeltaTime);
	}

	private void OnDisable()
	{
		ResetCursor();
	}

	private void SetCursor()
	{
		Cursor.visible = false;
		cursorImageParent.SetActive(true);
	}

	private void ResetCursor()
	{
		cursorImageParent.SetActive(false);
		Cursor.visible = true;
	}
}
