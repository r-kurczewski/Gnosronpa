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
					AddMoveYBulletAnimation(seq, bulletLabels[j], i - j + 1, j == 0);
				}
			}

			seq.onComplete += () => isAnimating = false;
		}

		
		private void AddMoveYBulletAnimation(Sequence seq, BulletLabel bullet, int level, bool append)
		{
			if (level == 0) return;

			//Debug.Log($"Moved {bullet.name} to level {level}");
			var bt = bullet.transform;

			var movement = bt.DOLocalMoveY(GetLevelHeight(level), moveYDuration);

			if (append) seq.Append(movement);
			else seq.Join(movement);

			if (level > maxVisibleBullets && bullet.gameObject.activeSelf)
			{
				var startAnimationPos = bullet.transform.localPosition;
				startAnimationPos.y = GetLevelHeight(maxVisibleBullets);
				bullet.transform.localPosition = startAnimationPos;

				var cg = bullet.GetComponent<CanvasGroup>();
				var fadeOut = DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration);
				fadeOut.onStepComplete += () =>
				{
					cg.gameObject.SetActive(false);
				};
				seq.Join(fadeOut);
			}
			else if (level <= maxVisibleBullets && !bullet.gameObject.activeSelf)
			{
				var startAnimationPos = bullet.transform.localPosition;
				startAnimationPos.y = GetLevelHeight(0);
				bullet.transform.localPosition = startAnimationPos;

				var cg = bullet.GetComponent<CanvasGroup>();
				cg.gameObject.SetActive(true);
				var fadeIn = DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 1, moveYDuration);
				seq.Join(fadeIn);
			}
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
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel, i == 0);
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
				var newLevel = (selectedIndex - i + bulletLabels.Count) % bulletLabels.Count + 1;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel, i == 0);
			}
			selectedIndex = (selectedIndex - 1 + bulletLabels.Count) % bulletLabels.Count;

			seq.onComplete += () => isAnimating = false;
		}
	}
}
