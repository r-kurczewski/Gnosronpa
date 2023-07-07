using Gnosronpa.ScriptableObjects;
using System;
using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	public class BulletLabel : MonoBehaviour
	{
		[SerializeField]
		private TruthBulletData data;

		[SerializeField]
		private TMP_Text label;

		public void Init(TruthBulletData bulletData)
		{
			data = bulletData;

			name = data.bulletName;
			label.text = data.bulletName;
		}
	}
}
