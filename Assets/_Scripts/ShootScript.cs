using Gnosronpa.Controllers;
using Gnosronpa.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{
	private const float defaultShootCooldown = 1.25f;

	[SerializeField]
	private InputActionReference mousePosition;

	[SerializeField]
	private GameObject truthBulletPrefab;

	[SerializeField]
	private Transform truthBulletParent;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private AudioClip shootSound;

	[SerializeField]
	private float bulletSpeed;

	[SerializeField]
	private float shootCooldown;

	private float AdditionalBulletSpawnOffsetX => canvas.planeDistance / 10;

	private void Update()
	{
		if (shootCooldown > 0)
		{
			shootCooldown -= Time.timeScale is 0 ? 0 : Time.unscaledDeltaTime;
		}
	}

	public bool TryShoot(TruthBulletData bulletData)
	{
		if (shootCooldown <= 0)
		{
			Shoot(bulletData);
			shootCooldown = defaultShootCooldown;
			return true;
		}
		else return false;
	}

	private void Shoot(TruthBulletData bulletData)
	{
		Vector3 mousePos = mousePosition.action.ReadValue<Vector2>();
		mousePos.z = canvas.planeDistance;
		var dstWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

		var srcPos = new Vector3(Screen.width, 0, Camera.main.nearClipPlane);
		var srcWorldPos = Camera.main.ScreenToWorldPoint(srcPos);
		srcWorldPos += Camera.main.transform.right * AdditionalBulletSpawnOffsetX;

		var bullet = Instantiate(truthBulletPrefab, truthBulletParent).GetComponent<TruthBullet>();
		bullet.Init(bulletData);
		var dynamicSpeed = bulletSpeed * canvas.planeDistance;
		bullet.Shoot(srcWorldPos, dstWorldPos, dynamicSpeed);
		AudioController.instance.PlaySound(shootSound);
	}
}
