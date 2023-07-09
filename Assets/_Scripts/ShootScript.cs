using Gnosronpa.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{
	[SerializeField]
	private InputActionReference mousePosition;

	[SerializeField]
	private GameObject truthBullet;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private float bulletSpeed;

	private float AdditionalBulletSpawnOffsetX => canvas.planeDistance / 10;

	public void Shoot(TruthBulletData bulletData)
	{
		Vector3 mousePos = mousePosition.action.ReadValue<Vector2>();
		mousePos.z = canvas.planeDistance;
		var dstWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

		var srcPos = new Vector3(Screen.width, 0, Camera.main.nearClipPlane);
		var srcWorldPos = Camera.main.ScreenToWorldPoint(srcPos);
		srcWorldPos += Camera.main.transform.right * AdditionalBulletSpawnOffsetX;

		var bullet = Instantiate(truthBullet, canvas.transform).GetComponent<TruthBullet>();
		bullet.Init(bulletData);
		var dynamicSpeed = bulletSpeed * canvas.planeDistance;
		bullet.Shoot(srcWorldPos, dstWorldPos, dynamicSpeed);
	}
}
