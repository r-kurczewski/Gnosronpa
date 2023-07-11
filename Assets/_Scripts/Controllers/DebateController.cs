using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Gnosronpa.Controllers
{
	public class DebateController : MonoBehaviour
	{
		private const int fadeOn = 1;
		private const int fadeOff = 0;
		private const int instant = 0;

		[Header("References")]

		[SerializeField]
		private DebateData data;

		[SerializeField]
		private CameraController cameraController;

		[SerializeField]
		private InputActionReference buttonConfirm;

		[SerializeField]
		private InputActionReference mouseScroll;

		[SerializeField]
		private InputActionReference shoot;

		[SerializeField]
		private Transform statementsParent;

		[SerializeField]
		private Transform bulletsParent;

		[SerializeField]
		private GameObject statementPrefab;

		[SerializeField]
		private GameObject bulletLabelPrefab;

		[SerializeField]
		private CameraFade cameraFade;

		[SerializeField]
		private ShootScript shootScript;

		[SerializeField]
		private CharacterInfo characterInfo;

		[SerializeField]
		private GameObject rightPanel;

		[SerializeField]
		private GameObject debateGUI;

		[SerializeField]
		private BulletController bulletController;

		[Header("State")]

		[SerializeField]
		private bool isPlaying = false;

		[SerializeField]
		private bool isStartLoopAnimation;

		[SerializeField]
		private float time;

		// Other

		private Sequence loop;

		private Queue<DebateSequenceData> statementsQueue;

		private void Awake()
		{
			if (data) Init(data);
		}

		public void Init(DebateData debate)
		{
			data = debate;
			statementsQueue = new Queue<DebateSequenceData>();
			debate.debateSequence.ForEach(statement => statementsQueue.Enqueue(statement));

			debateGUI.SetActive(false);

			isStartLoopAnimation = true;
			StartCoroutine(Debate());
			bulletController.OnBulletMenuHidingEnd.AddListener(() => OnBulletPick(default));
		}

		private IEnumerator Debate()
		{
			Debug.Log("Starting debate...");
			var animation = PlayDebateStartAnimation();
			yield return new WaitWhile(animation.IsActive);

			Debug.Log("Loading bullets...");
			animation = PlayLoadingBulletAnimation();
			yield return new WaitWhile(animation.IsActive);

			EnableBulletPick();
			Debug.Log("Waiting for bullet pick...");

			yield return new WaitUntil(() => isPlaying);

			while (statementsQueue.Any())
			{
				LoadNewSentences();
				yield return null;
			}
		}

		private void LoadNewSentences()
		{
			var statement = statementsQueue.Peek();
			if (statement.delay < time)
			{
				if (isStartLoopAnimation)
				{
					loop?.Kill();
					rightPanel.gameObject.SetActive(true);
					isStartLoopAnimation = false;
				}

				PlayAnimation(statement);
				statementsQueue.Dequeue();
				time = 0;
			}

			time += Time.deltaTime;
		}

		private Sequence PlayDebateStartAnimation()
		{
			var ct = Camera.main.transform;
			var cp = ct.parent;

			var spin = new Vector3(0, -360, 0);
			var skew = new Vector3(0, 0, 5);
			var zoom = new Vector3(0, 7, 20);

			var fadeTime = 0.5f;

			var seq = DOTween.Sequence()
				.Append(ct.DOLocalRotate(2 * skew, instant))

				.Append(ct.DOLocalRotate(spin + 2 * skew, 3, RotateMode.FastBeyond360).SetEase(Ease.Linear))
				.Join(cameraFade.DOFade(fadeOn, fadeTime).SetEase(Ease.InOutFlash).SetDelay(2))

				.Append(ct.DOLocalMove(zoom, instant))
				.Join(ct.DOLocalRotate(-skew, instant))

				.Append(cameraFade.DOFade(fadeOff, fadeTime).SetEase(Ease.InOutFlash))
				.Join(ct.DOLocalRotate(-skew, instant))

				.AppendCallback(() =>
				{
					loop = DOTween.Sequence()
					.Join(cp.DOLocalRotate(spin, 30, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(int.MaxValue));
				});

			return seq;
		}

		private void EnableBulletPick()
		{
			mouseScroll.action.performed += OnBulletChange;
			buttonConfirm.action.performed += OnBulletPick;
			buttonConfirm.action.performed -= OnShoot;
		}

		private Sequence PlayLoadingBulletAnimation()
		{
			debateGUI.SetActive(true);

			bulletController.Init(data.bullets);
			return bulletController.StartAnimation();
		}

		private void OnBulletChange(CallbackContext context)
		{
			var direction = context.ReadValue<float>();
			EnableBulletPick();

			if (direction > 0)
			{
				bulletController.ChangeBulletUp();
			}
			else if (direction < 0)
			{
				bulletController.ChangeBulletDown();
			}
		}

		private void OnBulletPick(CallbackContext context)
		{
			bulletController.HideBulletPickMenu();
			bulletController.ShowSelectedBulletPanel();

			Debug.Log($"Picked bullet: {bulletController.SelectedBullet.bulletName}");

			isPlaying = true;
			buttonConfirm.action.performed -= OnBulletPick;
			buttonConfirm.action.performed += OnShoot;
		}

		private void OnShoot(CallbackContext context)
		{
			shootScript.Shoot(bulletController.SelectedBullet);
		}

		public void PlayAnimation(DebateSequenceData statementData)
		{
			var speakingCharacter = GameObject.FindGameObjectsWithTag("Character")
								.Select(x => x.GetComponent<Character>())
								.FirstOrDefault(x => x.Data == statementData.statement.speakingCharacter);

			cameraController.PlayCameraAnimation(statementData.characterRelativeCameraAnimation, speakingCharacter?.gameObject);

			if (statementData.statement)
			{
				LoadStatement(statementData);
				characterInfo.SetCharacter(statementData.statement.speakingCharacter);
			}
		}

		private DebateStatement LoadStatement(DebateSequenceData statementData)
		{
			var statement = Instantiate(statementPrefab, statementsParent).GetComponent<DebateStatement>();
			statement.Init(statementData);
			return statement;
		}
	}
}
