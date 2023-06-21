using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{
	private const int AdditionalBulletSpawnOffsetX = 10;

	[SerializeField]
	private InputActionReference shoot, mousePosition;

	[SerializeField]
	private GameObject truthBullet;

	[SerializeField]
	private GameObject castPointer;

	[SerializeField]
	private Transform canvas;

	[SerializeField]
	private float bulletSpawnDistance;

	private void OnEnable()
	{
		shoot.action.performed += OnShoot;
	}

	private void OnDisable()
	{
		shoot.action.performed -= OnShoot;
	}

	private void OnShoot(InputAction.CallbackContext ctx)
	{
		Vector3 mousePos = mousePosition.action.ReadValue<Vector2>();
		mousePos.z = FindObjectOfType<Canvas>().planeDistance;
		var dstWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

		var srcPos = new Vector3(Screen.width, 0, Camera.main.nearClipPlane);
		var srcWorldPos = Camera.main.ScreenToWorldPoint(srcPos);
		srcWorldPos.x += AdditionalBulletSpawnOffsetX;

		var bullet = Instantiate(truthBullet, canvas).GetComponent<TruthBullet>();
		bullet.Shoot(srcWorldPos, dstWorldPos);
	}
}
