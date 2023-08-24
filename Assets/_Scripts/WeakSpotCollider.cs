using System;
using UnityEngine;

namespace Gnosronpa
{
	public class WeakSpotCollider : MonoBehaviour
	{
		public event Action<TruthBullet, DebateStatement> OnWeakSpotHit;

		[SerializeField]
		private BoxCollider weakspotCollider;

		private void OnTriggerEnter(Collider other)
		{
			// Block invoking statement trigger
			other.enabled = false;

			var bullet = other.GetComponent<TruthBullet>();
			var statement = GetComponentInParent<DebateStatement>();
			OnWeakSpotHit?.Invoke(bullet, statement);
		}

		public void SetColliderSize(Vector3 center, Vector3 size)
		{
			weakspotCollider.center = center;
			weakspotCollider.size = size;
		}
	}
}
