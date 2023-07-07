using DG.Tweening;
using Gnosronpa.ScriptableObjects;
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
		private GameObject debateGUI;

		[SerializeField]
		private BulletGUI bulletGUI;

		[Header("State")]

		[SerializeField]
		private bool isPlaying = false;

		[SerializeField]
		private bool isStartLoopAnimation;

		[SerializeField]
		private float time;

		private List<BulletLabel> bulletLabels;

		// Other

		private Sequence loop;

		private Queue<DebateSequenceData> statementsQueue;

		private void Awake()
		{
			statementsQueue = new Queue<DebateSequenceData>();
			if (data) LoadDebate(data);
		}

		private void Update()
		{
			if (!isPlaying || statementsQueue.Count == 0) return;

			var statement = statementsQueue.Peek();
			if (statement.delay < time)
			{
				if (isStartLoopAnimation)
				{
					loop?.Kill();
					debateGUI.SetActive(true);
					isStartLoopAnimation = false;
				}

				PlayAnimation(statement);
				statementsQueue.Dequeue();
				time = 0;
			}

			time += Time.deltaTime;
		}

		private void PlayDebateStartAnimation()
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

				.AppendCallback(() => EnableBulletPick())
				.Join(ct.DOLocalRotate(-skew, instant))

				.AppendCallback(() => PlayLoadingBulletAnimation());

			loop = DOTween.Sequence()
			.Join(cp.DOLocalRotate(spin, 30, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(int.MaxValue));
		}

		public void EnableBulletPick()
		{
			mouseScroll.action.performed += OnBulletChange;
			buttonConfirm.action.performed += OnBulletPick;
		}

		private void PlayLoadingBulletAnimation()
		{
			debateGUI.SetActive(true);

			bulletGUI.Init(data.bullets);
			bulletGUI.StartAnimation();
		}

		private void OnBulletChange(CallbackContext context)
		{
			var direction = context.ReadValue<float>();
			var seq = DOTween.Sequence();

			if (direction > 0)
			{
				bulletGUI.MoveUpAnimation();
			}
			else if (direction < 0)
			{
				bulletGUI.MoveDownAnimation();
			}
		}

		private void OnBulletPick(CallbackContext context)
		{
			isPlaying = true;
			EnableShooting();
			buttonConfirm.action.performed -= OnBulletPick;
		}

		private void EnableShooting()
		{
			shootScript.gameObject.SetActive(true);
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

		public void LoadDebate(DebateData debate)
		{
			data = debate;
			bulletLabels?.ForEach(b => Destroy(b));
			bulletLabels = new();

			debate.debateSequence.ForEach(statement => statementsQueue.Enqueue(statement));

			debateGUI.SetActive(false);

			isStartLoopAnimation = true;
			PlayDebateStartAnimation();
		}
	}
}
