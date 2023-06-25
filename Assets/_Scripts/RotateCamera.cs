using DG.Tweening;
using UnityEngine;

namespace Gnosronpa
{
	public class RotateCamera : MonoBehaviour
    {
		[SerializeField]
		private float speed;

		//private void Start()
		//{
		//	var camera = Camera.main;
		//	camera.transform.DORotate(new Vector3(0, -180, 0), 30);
		//}

		private void Update()
		{
			Camera.main.transform.rotation *= Quaternion.Euler(0, -speed * Time.deltaTime, 0);
		}
	}
}
