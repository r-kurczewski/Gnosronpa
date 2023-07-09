using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gnosronpa
{
	public class BulletGUI : MonoBehaviour
	{
		private const int maxVisibleBullets = 3;
		private const float moveY = 80;
		private const float moveYDuration = 0.3f;
		private const int instant = 0;

		[SerializeField]
		private GameObject bulletLabelPrefab;

		[SerializeField]
		private Vector3 labelSpawnPos;

		[SerializeField]
		private BulletLabel selectedBulletLabel;

		[SerializeField]
		private Transform bulletLabelsParent;

		[SerializeField]
		private int selectedIndex;

		[SerializeField]
		private List<BulletLabel> bulletLabels;

		[SerializeField]
		private bool isAnimating;

		public bool IsAnimating => isAnimating;

		public void Init(IEnumerable<TruthBulletData> bulletsData)
		{
			bulletLabels = new List<BulletLabel>();
			foreach (var bulletData in bulletsData)
			{
				var bulletLabel = LoadBulletLabel(bulletData);
				bulletLabels.Add(bulletLabel);
			}
			selectedIndex = bulletLabels.Count - maxVisibleBullets / 2 - 1;
		}

		public void StartAnimation()
		{
			isAnimating = true;

			var moveLeftTo = -462;
			var moveLeftDuration = 0.3f;

			var outOfScreenRight = new Vector3(550, -150, 0);

			var loadedBullets = new List<BulletLabel>();
			var seq = DOTween.Sequence();

			// for all bullets
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				seq.Append(bulletLabels[i].transform.DOLocalMoveX(moveLeftTo, moveLeftDuration));
				loadedBullets.Add(bulletLabels[i]);

				// for already visible bullets
				for (int j = 0; j <= i; j++)
				{
					var level = i - j + 1;
					AddMoveYBulletAnimation(seq, bulletLabels[j], level, j == 0);
				}
			}

			seq.onComplete += () => isAnimating = false;
		}

		private void AddMoveYBulletAnimation(Sequence seq, BulletLabel bullet, int level, bool append = false)
		{
			var bt = bullet.transform;
			var cg = bullet.GetComponent<CanvasGroup>();

			var movement = bt.DOLocalMoveY(GetLevelHeight(level), moveYDuration);

			int? startAnimationIndex = null;
			Tween fadeEffect = null;

			if (level < 0)
			{
				Debug.LogError($"Invalid level [{level}] on [{bullet.name}]", this);
			}
			// fade out to bottom
			else if (level is 0 && bullet.gameObject.activeSelf)
			{
				startAnimationIndex = 1;
				fadeEffect = DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0f, moveYDuration);
				fadeEffect.onComplete += () => bullet.gameObject.SetActive(false);
			}
			//fade in from bottom
			else if (level is 1 && !bullet.gameObject.activeSelf)
			{
				startAnimationIndex = 0;
				bullet.gameObject.SetActive(true);
				fadeEffect = DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 1, moveYDuration);
			}
			//fade in from top
			else if (level is maxVisibleBullets && !bullet.gameObject.activeSelf)
			{
				startAnimationIndex = maxVisibleBullets + 1;
				bullet.gameObject.SetActive(true);
				fadeEffect = DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 1, moveYDuration);
			}
			else if (level is maxVisibleBullets + 1 && bullet.gameObject.activeSelf)
			{
				//fade out to top
				startAnimationIndex = maxVisibleBullets;
				fadeEffect = DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0f, moveYDuration);
				fadeEffect.onComplete += () => bullet.gameObject.SetActive(false);
			}

			if (startAnimationIndex is not null && fadeEffect is not null)
			{
				seq.Join(bt.DOLocalMoveY(GetLevelHeight(startAnimationIndex.Value), instant));
				seq.Join(fadeEffect);
			}

			if (append) seq.Append(movement);
			else seq.Join(movement);
		}

		private float GetLevelHeight(int level)
		{
			return labelSpawnPos.y + level * moveY;
		}

		private BulletLabel LoadBulletLabel(TruthBulletData bulletData)
		{
			var bulletLabel = Instantiate(bulletLabelPrefab, bulletLabelsParent).GetComponent<BulletLabel>();
			bulletLabel.transform.localPosition = labelSpawnPos;
			bulletLabel.Init(bulletData);
			return bulletLabel;
		}

		public void MoveUpAnimation()
		{
			if (isAnimating) return;

			isAnimating = true;

			var seq = DOTween.Sequence();
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var newLevel = (selectedIndex - i + 3 + bulletLabels.Count) % bulletLabels.Count;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel);
			}
			selectedIndex = (selectedIndex + 1) % bulletLabels.Count;

			seq.onComplete += () => isAnimating = false;
		}

		public void MoveDownAnimation()
		{
			if (isAnimating) return;

			isAnimating = true;

			var seq = DOTween.Sequence();
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var newLevel = (selectedIndex - i + 1 + bulletLabels.Count) % bulletLabels.Count;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel);
			}
			selectedIndex = (selectedIndex - 1 + bulletLabels.Count) % bulletLabels.Count;

			seq.onComplete += () => isAnimating = false;
		}
	}
}
