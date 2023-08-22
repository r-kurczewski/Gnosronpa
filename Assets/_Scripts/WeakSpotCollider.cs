using System;
using UnityEngine;

namespace Gnosronpa
{
	public class WeakSpotCollider : MonoBehaviour
	{
		public event Action<TruthBullet> OnWeakSpotHit;

		[SerializeField]
		private BoxCollider weakspotCollider;

		private void OnTriggerEnter(Collider other)
		{
			// Block invoking statement trigger
			other.enabled = false;

			var bullet = other.GetComponent<TruthBullet>();
			OnWeakSpotHit?.Invoke(bullet);
		}

		public void SetColliderSize(Vector3 center, Vector3 size)
		{
			weakspotCollider.center = center;
			weakspotCollider.size = size;
		}
	}
}
