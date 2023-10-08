using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class BulletLabel : MonoBehaviour
	{
		[SerializeField]
		private TruthBulletData data;

		[SerializeField]
		private Color defaultColor;

		[SerializeField]
		private Color selectedColor;

		[SerializeField]
		private float selectedScale;

		[SerializeField]
		private TMP_Text label;

		[SerializeField]
		private Image labelBg;

		public TruthBulletData Data => data;

		public void Init(TruthBulletData bulletData, string name = null)
		{
			data = bulletData;

			this.name = name ?? data.bulletName;
			label.text = data.bulletName;
		}

		public Tween ChangeSelectedStateAnimation(bool selected, float duration)
		{
			var newColor = selected ? selectedColor : defaultColor;
			var newScale = selected ? selectedScale : 1;

			var seq = DOTween.Sequence(transform);
			seq.Join(DOTween.To(() => labelBg.color, (c) => labelBg.color = c, newColor, duration));
			seq.Join(transform.DOScale(newScale, duration));
			return seq;
		}

		public void ChangeSelectedState(bool selected)
		{
			var newColor = selected ? selectedColor : defaultColor;
			var newScale = selected ? selectedScale : 1;

			labelBg.color = newColor;
			transform.localScale = Vector3.one * newScale;
		}
	}
}
