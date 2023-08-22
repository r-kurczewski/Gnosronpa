using System;
using UnityEngine;

namespace Gnosronpa
{
	public class StatementCollider : MonoBehaviour
    {
		public event Action<TruthBullet> OnStatementHit;

		[SerializeField]
		private BoxCollider statementCollider;

		private void OnTriggerEnter(Collider other)
		{
			// Block invoking weakspot trigger
			other.enabled = false;

			var bullet = other.GetComponent<TruthBullet>();
			OnStatementHit?.Invoke(bullet);
		}

		public void SetColliderSize(Vector3 center, Vector3 size)
		{
			statementCollider.center = center;
			statementCollider.size = size;
		}
	}
}
