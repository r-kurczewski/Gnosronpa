using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Scriptables;
using Gnosronpa.StateMachines;
using Gnosronpa.StateMachines.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gnosronpa.Controllers
{
	public class BulletController : StateMachine<BulletController, BulletMenuState>, IRefreshable
	{
		private const float startHideMenuTime = 10f;
		private const float defaultHideMenuTime = 1.25f;

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
		private int visibleBullets;

		[SerializeField]
		private int selectedIndex;

		[SerializeField]
		private float hideMenuTimer = 0;

		[SerializeField]
		private List<BulletLabel> bulletLabels = new();

		public TruthBulletData SelectedBullet => bulletLabels[selectedIndex].Data;

		#region State Machine

		public BulletMenuState Loading { get; private set; }
		public BulletMenuState Loaded { get; private set; }
		public BulletMenuState Opened { get; private set; }
		public BulletMenuState Closing { get; private set; }
		public BulletMenuState Closed { get; private set; }
		public BulletMenuState Opening { get; private set; }
		public BulletMenuState BulletUp { get; private set; }
		public BulletMenuState BulletDown { get; private set; }
		protected override BulletMenuState StartingState => Loading;

		protected override void DefineMachineStates()
		{
			Loading = new(nameof(Loading))
			{
				OnExecute = async () =>
				{
					await StartAnimation();
					return Loaded;
				}
			};

			Loaded = new(nameof(Loaded))
			{
				OnEnter = () =>
				{
					hideMenuTimer = startHideMenuTime;
					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					CheckStateChangeRequest();

					if (hideMenuTimer < 0)
					{
						return Closing;
					}
					UpdateHideMenuTimeCounter();

					await UniTask.Yield();
					return Loaded;
				}
			};

			Closing = new(nameof(Closing))
			{
				OnEnter = () =>
				{
					hideMenuTimer = 0;
					ShowSelectedBulletPanel();
					Refresh();
					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					await HideBulletPickMenu();
					return Closed;
				}
			};

			Opening = new(nameof(Opening))
			{
				OnEnter = () =>
				{
					HideSelectedBulletPanel();
					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					await ShowBulletPickMenu();
					return Opened;
				}
			};

			BulletUp = new(nameof(BulletUp))
			{
				OnExecute = async () =>
				{
					await ChangeBulletUpAnimation();
					return Opened;
				}
			};

			BulletDown = new(nameof(BulletDown))
			{
				OnExecute = async () =>
				{
					await ChangeBulletDownAnimation();
					return Opened;
				}
			};

			Opened = new(nameof(Opened))
			{
				OnEnter = () =>
				{
					hideMenuTimer = defaultHideMenuTime;
					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					CheckStateChangeRequest();

					if (hideMenuTimer < 0)
					{
						return Closing;
					}
					UpdateHideMenuTimeCounter();

					await UniTask.Yield();
					return Opened;
				},
			};

			Closed = new(nameof(Closed))
			{
				OnExecute = async () =>
				{
					CheckStateChangeRequest();
					await UniTask.Yield();
					return Closed;
				},
			};
		}

		#endregion

		private void UpdateHideMenuTimeCounter()
		{
			if (Time.timeScale != 0) hideMenuTimer -= Time.unscaledDeltaTime;
		}

		public void Init(NonstopDebate debateData)
		{
			visibleBullets = debateData.visibleBullets;
			hideMenuTimer = 0;

			var bulletsData = debateData.bullets;

			foreach (Transform bullet in bulletLabelsParent)
			{
				Destroy(bullet.gameObject);
			}
			bulletLabels.Clear();
			bulletLabelsParent.gameObject.SetActive(true);
			selectedBulletLabel.gameObject.SetActive(false);

			foreach (var bulletData in bulletsData)
			{
				var bulletLabel = LoadBulletLabel(bulletData);
				bulletLabels.Add(bulletLabel);
			}
			selectedIndex = bulletLabels.Count - visibleBullets / 2 - 1;

			InitStateMachine();
		}

		public async UniTask OpenBulletMenu()
		{
			await RequestStateChange(Opening);
			await UniTask.WaitUntil(() => CurrentState == Opened);
		}

		public async UniTask CloseBulletMenu()
		{
			await RequestStateChange(Closing);
			await UniTask.WaitUntil(() => CurrentState == Closed);
		}

		public async UniTask ChangeBulletUp()
		{
			await RequestStateChange(BulletUp);
			await UniTask.WaitWhile(() => CurrentState == BulletUp);
		}

		public async UniTask ChangeBulletDown()
		{
			await RequestStateChange(BulletDown);
			await UniTask.WaitWhile(() => CurrentState == BulletDown);
		}

		private void ShowSelectedBulletPanel()
		{
			selectedBulletLabel.gameObject.SetActive(true);
		}

		private void HideSelectedBulletPanel()
		{
			selectedBulletLabel.gameObject.SetActive(false);
		}

		public void Refresh()
		{
			selectedBulletLabel.Init(bulletLabels[selectedIndex].Data, "Selected");
			selectedBulletLabel.ChangeSelectedState(true);
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
				_ = AudioController.Instance.PlaySound(bulletLoadSound);

				var tasks = new List<UniTask>();

				// for already visible bullets
				for (int j = 0; j <= i; j++)
				{
					var label = bulletLabels[j].transform;

					var seq = DOTween.Sequence(label)
						.Append(label.DOBlendableLocalMoveBy(Vector2.up * moveY, moveYDuration));

					// hide bullet over limit 
					if (j > visibleBullets - 1)
					{
						var cg = bulletLabels[j - visibleBullets].GetComponent<CanvasGroup>();
						_ = seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0f, moveYDuration))
							.onComplete = () => cg.gameObject.SetActive(false);
					}
					tasks.Add(seq.ToUniTask());
				}
				await UniTask.WhenAll(tasks);
			}

			var selectAnimation = DOTween.Sequence();
			JoinLabelSelectionAnimation(selectAnimation);
			await selectAnimation.AwaitForComplete();
		}

		private async UniTask ChangeBulletUpAnimation()
		{
			var seq = DOTween.Sequence(transform).SetUpdate(true);

			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var level = (selectedIndex - i + 2 + bulletLabels.Count) % bulletLabels.Count;
				var newLevel = (level + 1) % bulletLabels.Count;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel, true);


			}
			selectedIndex = (selectedIndex + 1 + bulletLabels.Count) % bulletLabels.Count;
			JoinLabelSelectionAnimation(seq);

			await seq.AwaitForComplete();
		}

		private async UniTask ChangeBulletDownAnimation()
		{
			var seq = DOTween.Sequence(transform).SetUpdate(true);

			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var level = (selectedIndex - i + 2 + bulletLabels.Count) % bulletLabels.Count;
				var newLevel = (level - 1 + bulletLabels.Count) % bulletLabels.Count;
				AddMoveYBulletAnimation(seq, bulletLabels[i], newLevel, false);
			}
			selectedIndex = (selectedIndex - 1 + bulletLabels.Count) % bulletLabels.Count;
			JoinLabelSelectionAnimation(seq);

			await seq.AwaitForComplete();
		}

		private async UniTask ShowBulletPickMenu()
		{
			bulletLabelsParent.gameObject.SetActive(true);

			var seq = DOTween.Sequence(transform).SetUpdate(true);

			var firstVisibleIndexUnclamped = selectedIndex - visibleBullets / 2;

			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var currentIndex = (firstVisibleIndexUnclamped + i + bulletLabels.Count) % bulletLabels.Count;
				var delay = i < visibleBullets ? i * nextBulletAnimationDelay : 0;
				_ = seq.Join(bulletLabels[currentIndex].transform.DOLocalMoveX(bulletsPosX, 0.3f).SetDelay(delay));
			}

			await seq.AwaitForComplete();
		}

		private async UniTask HideBulletPickMenu()
		{
			var outOfScreenRight = -950;
			var duration = 0.3f;

			var seq = DOTween.Sequence(transform).SetUpdate(true);

			var animationStartingBullet = selectedIndex - visibleBullets / 2;
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				var currentIndex = (animationStartingBullet + i + bulletLabels.Count) % bulletLabels.Count;
				var delay = i < visibleBullets ? i * nextBulletAnimationDelay : 0;
				_ = seq.Join(bulletLabels[currentIndex].transform.DOLocalMoveX(outOfScreenRight, duration).SetDelay(delay));
			}

			await seq.AwaitForComplete();

			bulletLabelsParent.gameObject.SetActive(false);
		}

		private void JoinLabelSelectionAnimation(Sequence seq)
		{
			for (int i = 0; i < bulletLabels.Count; i++)
			{
				_ = seq.Join(bulletLabels[i].ChangeSelectedStateAnimation(i == selectedIndex, moveYDuration));
			}
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

				// fade out
				if (newLevel > visibleBullets)
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

				// fade out + move to top
				if (newLevel is 0)
				{
					seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, moveYDuration));
					seq.onComplete += () =>
					{
						var newPos = bullet.transform.localPosition;
						newPos.y = GetLevelHeight(visibleBullets + 1);
						bullet.transform.localPosition = newPos;
						bullet.gameObject.SetActive(false);
					};
				}
				// fade in + move to top
				else if (newLevel == visibleBullets)
				{
					var newPos = bullet.transform.localPosition;
					newPos.y = GetLevelHeight(visibleBullets + 1);
					bullet.transform.localPosition = newPos;

					bullet.gameObject.SetActive(true);
					seq.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 1, moveYDuration));
				}
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
	}
}