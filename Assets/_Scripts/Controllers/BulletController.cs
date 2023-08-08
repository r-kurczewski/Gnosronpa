using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Gnosronpa.Controllers
{
	public class BulletController : MonoBehaviour, IRefreshable
	{
		private const int maxVisibleBullets = 3;
		private const int instant = 0;
		private const float hideMenuTime = 1f;

		private const float moveY = 80;
		private const float moveYDuration = 0.3f;
		private const int bulletsPosX = -462;
		private const float nextBulletAnimationDelay = 0.04f;

		public UnityEvent OnBulletMenuHidingStart;

		public UnityEvent OnBulletMenuHidingEnd;

		[Header("References")]

		[SerializeField]
		private GameObject bulletLabelPrefab;

		[SerializeField]
		private Vector3 labelSpawnPos;

		[SerializeField]
		private BulletLabel selectedBulletLabel;

		[SerializeField]
		private Transform bulletLabelsParent;

		[SerializeField]
		private AudioClip bulletLoadSound;

		[Header("State")]

		[SerializeField]
		private int selectedIndex;

		[SerializeField]
		private bool isMenuOpen;

		[SerializeField]
		private float hideMenuTimer;

		[SerializeField]
		private List<BulletLabel> bulletLabels;

		private Sequence seq;

#pragma warning disable IDE0052
		private Coroutine hideMenuCoroutine;
#pragma warning restore IDE0052

		private bool AnimationPlaying => seq?.IsActive() ?? false && seq.IsPlaying();

		public TruthBulletData SelectedBullet => bulletLabels[selectedIndex].Data;

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

		public Sequence StartAnimation()
		{
			var moveLeftTo = bulletsPosX;
			var moveLeftDuration = 0.3f;

			var outOfScreenRight = new Vector3(550, -150, 0);

			seq = DOTween.Sequence();

			// for all bullets
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				seq.Append(bulletLabels[i].transform.DOLocalMoveX(moveLeftTo, moveLeftDuration));
				seq.AppendCallback(() => AudioController.instance.PlaySound(bulletLoadSound));

				// for already visible bullets
				for (int j = 0; j <= i; j++)
				{
					var level = i - j + 1;
					AddMoveYBulletAnimation(seq, bulletLabels[j], level, j == 0);
				}
			}
			seq.onComplete += () => isMenuOpen = true;

			return seq;
		}

		public Sequence ChangeBulletUp()
		{
			if (AnimationPlaying) return default;

			if (!isMenuOpen)
			{
				seq = ShowBulletPickMenu();
				return seq;
			}

			seq = DOTween.Sequence();
			seq.SetUpdate(true);

			seq.AppendCallback(() => HideMenuAfterTime(hideMenuTime));

			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var newLevel = (selectedIndex - i + 3 + bulletLabels.Count) % bulletLabels.Count;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel);
			}
			selectedIndex = (selectedIndex + 1) % bulletLabels.Count;

			return seq;
		}

		public Sequence ChangeBulletDown()
		{
			if (AnimationPlaying) return default;

			if (!isMenuOpen)
			{
				seq = ShowBulletPickMenu();
				return seq;
			}

			seq = DOTween.Sequence();
			seq.SetUpdate(true);

			seq.AppendCallback(() => HideMenuAfterTime(hideMenuTime));

			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var newLevel = (selectedIndex - i + 1 + bulletLabels.Count) % bulletLabels.Count;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel);
			}
			selectedIndex = (selectedIndex - 1 + bulletLabels.Count) % bulletLabels.Count;

			return seq;
		}

		public Sequence ShowBulletPickMenu()
		{
			if (AnimationPlaying) return default;

			if (isMenuOpen) return default;

			bulletLabelsParent.gameObject.SetActive(true);

			seq = DOTween.Sequence();
			seq.SetUpdate(true);

			var firstVisibleIndexUnclamped = selectedIndex - maxVisibleBullets / 2;
			var lastVisibleIndex = (firstVisibleIndexUnclamped + maxVisibleBullets + bulletLabels.Count) % bulletLabels.Count;
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var currentIndex = (firstVisibleIndexUnclamped + i + bulletLabels.Count) % bulletLabels.Count;
				var delay = i < maxVisibleBullets ? i * nextBulletAnimationDelay : 0;
				seq.Join(bulletLabels[currentIndex].transform.DOLocalMoveX(bulletsPosX, 0.3f).SetDelay(delay));
			}
			seq.AppendCallback(() =>
			{
				isMenuOpen = true;
				HideMenuAfterTime(hideMenuTime);
			});

			return seq;
		}

		public Sequence HideBulletPickMenu()
		{
			if (AnimationPlaying) return default;

			if (!isMenuOpen) return default;

			var outOfScreenRight = -950;
			var duration = 0.3f;
			var delay = nextBulletAnimationDelay;

			seq = DOTween.Sequence();
			seq.SetUpdate(true);

			seq.AppendCallback(OnBulletMenuHidingStart.Invoke);

			var animationStartingBullet = selectedIndex - maxVisibleBullets / 2;
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var currentIndex = (animationStartingBullet + i + bulletLabels.Count) % bulletLabels.Count;
				seq.Join(bulletLabels[currentIndex].transform.DOLocalMoveX(outOfScreenRight, duration).SetDelay(i * delay));
			}

			seq.onComplete += () =>
			{
				bulletLabelsParent.gameObject.SetActive(false);
				isMenuOpen = false;
				OnBulletMenuHidingEnd.Invoke();
			};

			Refresh();

			return seq;
		}
		public void ShowSelectedBulletPanel()
		{
			selectedBulletLabel.gameObject.SetActive(true);
		}

		public void HideSelectedBulletPanel()
		{
			selectedBulletLabel.gameObject.SetActive(false);
		}

		public void Refresh()
		{
			selectedBulletLabel.Init(bulletLabels[selectedIndex].Data, "Selected");
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
			if (append) seq.Append(movement);
			else seq.Join(movement);

			if (startAnimationIndex is not null && fadeEffect is not null)
			{
				seq.Join(bt.DOLocalMoveY(GetLevelHeight(startAnimationIndex.Value), instant));
				seq.Join(fadeEffect);
			}
		}

		private BulletLabel LoadBulletLabel(TruthBulletData bulletData)
		{
			var bulletLabel = Instantiate(bulletLabelPrefab, bulletLabelsParent).GetComponent<BulletLabel>();
			bulletLabel.transform.localPosition = labelSpawnPos;
			bulletLabel.Init(bulletData);
			return bulletLabel;
		}

		private float GetLevelHeight(int level)
		{
			return labelSpawnPos.y + level * moveY;
		}

		private void HideMenuAfterTime(float time)
		{
			hideMenuTimer = time;
			hideMenuCoroutine ??= StartCoroutine(IHideMenuAfterTime());
		}

		private IEnumerator IHideMenuAfterTime()
		{
			// This way waiting time counter can be reset after player input ex. bullet change
			while(hideMenuTimer > 0)
			{
				hideMenuTimer -= Time.unscaledDeltaTime;
				yield return null;
			}

			HideBulletPickMenu();
			hideMenuCoroutine = null;
		}

	}
}
