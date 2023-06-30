using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{
	[SerializeField]
	private InputActionReference shoot, mousePosition;

	[SerializeField]
	private GameObject truthBullet;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private float bulletSpeed;

	private float AdditionalBulletSpawnOffsetX => canvas.planeDistance / 10;

	private void OnEnable()
	{
		shoot.action.performed += OnShoot;
	}

	private void OnDisable()
	{
		shoot.action.performed -= OnShoot;
	}

	private void Update()
	{
		//OnShoot(default);
	}

	private void OnShoot(InputAction.CallbackContext ctx)
	{
		Vector3 mousePos = mousePosition.action.ReadValue<Vector2>();
		mousePos.z = canvas.planeDistance;
		var dstWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

		var srcPos = new Vector3(Screen.width, 0, Camera.main.nearClipPlane);
		var srcWorldPos = Camera.main.ScreenToWorldPoint(srcPos);
		srcWorldPos += Camera.main.transform.right * AdditionalBulletSpawnOffsetX;

		var bullet = Instantiate(truthBullet, canvas.transform).GetComponent<TruthBullet>();
		var dynamicSpeed = bulletSpeed * canvas.planeDistance;
		bullet.Shoot(srcWorldPos, dstWorldPos, dynamicSpeed);
	}
}
