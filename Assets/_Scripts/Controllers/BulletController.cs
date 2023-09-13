using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using Gnosronpa.StateMachines;
using Gnosronpa.StateMachines.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gnosronpa.Controllers
{
	public class BulletController : StateMachine<BulletController, BulletMenuState>, IRefreshable
	{
		private const int maxVisibleBullets = 3;
		private const float startHideMenuTime = 10f;
		private const float defaultHideMenuTime = 2.5f;

		private const float moveY = 80;
		private const float moveYDuration = 0.3f;
		private const int bulletsPosX = -462;
		private const float nextBulletAnimationDelay = 0.04f;

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
		private float hideMenuTimer = 0;

		[SerializeField]
		private List<BulletLabel> bulletLabels;

		private CancellationTokenSource hideMenuAferTimeCancellationTokenSource;

		public TruthBulletData SelectedBullet => bulletLabels[selectedIndex].Data;

		#region State Machine

		protected override BulletMenuState StartingState => Loading;

		public readonly BulletMenuState Loading = new(nameof(Loading));
		public readonly BulletMenuState Loaded = new(nameof(Loaded));
		public readonly BulletMenuState Opened = new(nameof(Opened));
		public readonly BulletMenuState Closing = new(nameof(Closing));
		public readonly BulletMenuState Closed = new(nameof(Closed));
		public readonly BulletMenuState Opening = new(nameof(Opening));
		public readonly BulletMenuState BulletUp = new(nameof(BulletUp));
		public readonly BulletMenuState BulletDown = new(nameof(BulletDown));

		protected override void DefineMachineStates()
		{
			Loading.OnExecute = async () =>
			{
				await StartAnimation();
				return Loaded;
			};

			Loaded.OnEnter = () =>
			{
				hideMenuTimer = startHideMenuTime;
				return UniTask.CompletedTask;
			};

			Loaded.OnExecute = async () =>
			{
				CheckStateChangeRequest();

				if (hideMenuTimer < 0)
				{
					return Closing;
				}
				hideMenuTimer -= Time.unscaledDeltaTime;

				await UniTask.Yield();
				return Loaded;
			};

			Closing.OnExecute = async () =>
			{
				await HideBulletPickMenu();
				return Closed;
			};

			Opening.OnExecute = async () =>
			{
				await ShowBulletPickMenu();
				return Opened;
			};

			BulletUp.OnExecute = async () =>
			{
				await ChangeBulletUp();
				return Opened;
			};

			BulletDown.OnExecute = async () =>
			{
				await ChangeBulletDown();
				return Opened;
			};

			Opened.OnEnter = () =>
			{
				hideMenuTimer = defaultHideMenuTime;
				return UniTask.CompletedTask;
			};

			Opened.OnExecute = async () =>
			{
				CheckStateChangeRequest();

				if (hideMenuTimer < 0)
				{
					return Closing;
				}
				hideMenuTimer -= Time.unscaledDeltaTime;

				await UniTask.Yield();
				return Opened;
			};

			Closed.OnEnter = () =>
			{
				hideMenuTimer = 0;
				return UniTask.CompletedTask;
			};

			Closed.OnExecute = async () =>
			{
				CheckStateChangeRequest();
				await UniTask.Yield();
				return Closed;
			};
		}

		#endregion

		public void Init(IEnumerable<TruthBulletData> bulletsData)
		{
			bulletLabels = new List<BulletLabel>();
			foreach (var bulletData in bulletsData)
			{
				var bulletLabel = LoadBulletLabel(bulletData);
				bulletLabels.Add(bulletLabel);
			}
			selectedIndex = bulletLabels.Count - maxVisibleBullets / 2 - 1;

			InitStateMachine();
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

		private async UniTask StartAnimation()
		{
			var moveLeftTo = bulletsPosX;
			var moveLeftDuration = 0.3f;

			var outOfScreenRight = new Vector3(550, -150, 0);

			// for all bullets
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				await bulletLabels[i].transform.DOLocalMoveX(moveLeftTo, moveLeftDuration);
				AudioController.instance.PlaySound(bulletLoadSound);

				var tasks = new List<UniTask>();

				// for already visible bullets
				for (int j = 0; j <= i; j++)
				{
					var label = bulletLabels[j].transform;

					var seq = DOTween.Sequence(label)
						.Append(label.DOBlendableLocalMoveBy(Vector2.up * moveY, moveYDuration));

					// hide bullet over limit 
					if (j > maxVisibleBullets - 1)
					{
						var cg = bulletLabels[j - maxVisibleBullets].GetComponent<CanvasGroup>();
						_ = seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0f, moveYDuration))
							.onComplete = () => cg.gameObject.SetActive(false);
					}
					tasks.Add(seq.ToUniTask());
				}
				await UniTask.WhenAll(tasks);
			}
		}

		private async UniTask ChangeBulletUp()
		{
			var seq = DOTween.Sequence(transform).SetUpdate(true);

			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var level = (selectedIndex - i + 2 + bulletLabels.Count) % bulletLabels.Count;
				var newLevel = (level + 1) % bulletLabels.Count;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel, true);
			}
			selectedIndex = (selectedIndex + 1 + bulletLabels.Count) % bulletLabels.Count;
			await seq.AwaitForComplete();
		}

		private async UniTask ChangeBulletDown()
		{
			var seq = DOTween.Sequence(transform).SetUpdate(true);

			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var level = (selectedIndex - i + 2 + bulletLabels.Count) % bulletLabels.Count;
				var newLevel = (level - 1 + bulletLabels.Count) % bulletLabels.Count;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel, false);
			}
			selectedIndex = (selectedIndex - 1 + bulletLabels.Count) % bulletLabels.Count;

			await seq.AwaitForComplete();
		}

		private async UniTask ShowBulletPickMenu()
		{
			bulletLabelsParent.gameObject.SetActive(true);

			var seq = DOTween.Sequence(transform)
				.SetUpdate(true);

			var firstVisibleIndexUnclamped = selectedIndex - maxVisibleBullets / 2;
			//var lastVisibleIndex = (firstVisibleIndexUnclamped + maxVisibleBullets + bulletLabels.Count) % bulletLabels.Count;

			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var currentIndex = (firstVisibleIndexUnclamped + i + bulletLabels.Count) % bulletLabels.Count;
				var delay = i < maxVisibleBullets ? i * nextBulletAnimationDelay : 0;
				_ = seq.Join(bulletLabels[currentIndex].transform.DOLocalMoveX(bulletsPosX, 0.3f).SetDelay(delay));
			}

			await seq.AwaitForComplete();
		}

		private async UniTask HideBulletPickMenu()
		{
			var outOfScreenRight = -950;
			var duration = 0.3f;

			hideMenuAferTimeCancellationTokenSource?.Cancel();

			ShowSelectedBulletPanel();
			Refresh();

			var seq = DOTween.Sequence(transform)
				.SetUpdate(true);

			var animationStartingBullet = selectedIndex - maxVisibleBullets / 2;
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var currentIndex = (animationStartingBullet + i + bulletLabels.Count) % bulletLabels.Count;
				var delay = i < maxVisibleBullets ? i * nextBulletAnimationDelay : 0;
				_ = seq.Join(bulletLabels[currentIndex].transform.DOLocalMoveX(outOfScreenRight, duration).SetDelay(delay));
			}

			await seq.AwaitForComplete();

			bulletLabelsParent.gameObject.SetActive(false);
		}

		private void AddMoveYBulletAnimation(Sequence seq, BulletLabel bullet, int newLevel, bool upwards)
		{
			var bt = bullet.transform;
			var cg = bullet.GetComponent<CanvasGroup>();

			if (newLevel < 0)
			{
				Debug.LogError($"Invalid level [{newLevel}] on [{bullet.name}]", this);
				return;
			}

			if (upwards)
			{
				var moveUp = new Vector3(0, moveY, 0);
				seq.Join(bt.DOBlendableLocalMoveBy(moveUp, moveYDuration));

				//fade out
				if (newLevel > maxVisibleBullets)
				{
					seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration));
					seq.onComplete += () => bullet.gameObject.SetActive(false);
				}
				// fade out + move to bottom
				else if (newLevel is 0)
				{
					seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration));
					seq.onComplete += () =>
					{
						var newPos = bullet.transform.localPosition;
						newPos.y = GetLevelHeight(0);
						bullet.transform.localPosition = newPos;
						bullet.gameObject.SetActive(false);
					};
				}
				// fade in + move to bottom
				else if (newLevel is 1)
				{
					var newPos = bullet.transform.localPosition;
					newPos.y = GetLevelHeight(0);
					bullet.transform.localPosition = newPos;

					bullet.gameObject.SetActive(true);
					seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 1, moveYDuration));
				}
			}
			else
			{
				var moveDown = new Vector3(0, -moveY, 0);
				seq.Join(bt.DOBlendableLocalMoveBy(moveDown, moveYDuration));

				////fade out
				//if (newLevel > maxVisibleBullets)
				//{
				//	seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration));
				//	seq.onComplete += () => bullet.gameObject.SetActive(false);
				//}

				// fade out + move to top
				if (newLevel is 0)
				{
					seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration));
					seq.onComplete += () =>
					{
						var newPos = bullet.transform.localPosition;
						newPos.y = GetLevelHeight(maxVisibleBullets + 1);
						bullet.transform.localPosition = newPos;
						bullet.gameObject.SetActive(false);
					};
				}
				// fade in + move to top
				else if (newLevel is maxVisibleBullets)
				{
					var newPos = bullet.transform.localPosition;
					newPos.y = GetLevelHeight(maxVisibleBullets + 1);
					bullet.transform.localPosition = newPos;

					bullet.gameObject.SetActive(true);
					seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 1, moveYDuration));
				}
			}
		}

		//private void AddMoveYBulletAnimation(Sequence seq, BulletLabel bullet, int level, int newLevel, bool upwards)
		//{
		//	var bt = bullet.transform;
		//	var cg = bullet.GetComponent<CanvasGroup>();

		//	if (newLevel < 0)
		//	{
		//		Debug.LogError($"Invalid level [{newLevel}] on [{bullet.name}]", this);
		//		return;
		//	}

		//	// move top one to bottom
		//	if (level is maxVisibleBullets && newLevel is 0)
		//	{
		//		seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration));
		//		seq.onComplete += () =>
		//		{
		//			var bottomPos = bullet.transform.localPosition;
		//			bottomPos.y = GetLevelHeight(0);
		//			bullet.transform.localPosition = bottomPos;
		//			bullet.gameObject.SetActive(false);
		//		};
		//	}
		//	// move bottom one to top
		//	else if (newLevel - level > 1)
		//	{
		//		Debug.Log(2 + bullet.name);
		//	}
		//	// fade out to bottom
		//	if (level is 1 && newLevel is 0)
		//	{
		//		seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration));
		//		seq.onComplete += () => bullet.gameObject.SetActive(false);
		//	}
		//	//fade in from bottom
		//	else if (level is 0 && newLevel is 1)
		//	{
		//		bullet.gameObject.SetActive(true);
		//		seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 1, moveYDuration));
		//	}
		//	//fade in from top
		//	else if (level is (maxVisibleBullets + 1) && newLevel is maxVisibleBullets)
		//	{
		//		bullet.gameObject.SetActive(true);
		//		seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 1, moveYDuration));
		//	}
		//	//fade out to top
		//	else if (level is maxVisibleBullets && newLevel is (maxVisibleBullets + 1))
		//	{
		//		seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration));
		//		seq.onComplete += () => bullet.gameObject.SetActive(false);
		//	}

		//	if (newLevel > level || newLevel - level != 1)
		//	{
		//		var moveUp = new Vector3(0, moveY, 0);
		//		seq.Join(bt.DOBlendableLocalMoveBy(moveUp, moveYDuration));
		//	}
		//	else
		//	{
		//		var moveDown = new Vector3(0, -moveY, 0);
		//		seq.Join(bt.DOBlendableLocalMoveBy(moveDown, moveYDuration));
		//	}
		//}

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

		public void HideAndReset()
		{
			hideMenuAferTimeCancellationTokenSource?.Cancel();
			hideMenuTimer = 0;
			bulletLabelsParent.gameObject.SetActive(false);
		}
	}
}