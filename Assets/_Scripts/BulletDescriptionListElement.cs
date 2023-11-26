using Gnosronpa.Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class BulletDescriptionListElement : MonoBehaviour
	{
		[SerializeField]
		private TruthBulletData bulletData;

		[SerializeField]
		private Image background;

		[SerializeField]
		private Color defaultColor;

		[SerializeField]
		private Color selectedColor;

		[SerializeField]
		private TMP_Text bulletName;

		public TruthBulletData BulletData => bulletData;

		public void Init(TruthBulletData data)
		{
			bulletData = data;
			bulletName.text = data.bulletName;
		}

		public void SetSelected(bool active)
		{
			if (active) background.color = selectedColor;
			else background.color = defaultColor;
		}
	}
}
