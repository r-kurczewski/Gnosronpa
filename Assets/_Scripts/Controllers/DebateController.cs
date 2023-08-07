using DG.Tweening;
using Gnosronpa.Common;
using Gnosronpa.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace Gnosronpa.Controllers
{
	public class DebateController : MonoBehaviour
	{
		#region consts

		private const int fadeOn = 1;
		private const int fadeOff = 0;
		private const int instant = 0;

		private const float rewindSpeed = 4;
		private const float rewindPitch = 1.3f;

		private const float defaultSpeed = 1;
		private const float defaultPitch = 1;

		private const float slowdownSpeed = 0.5f;
		private const float slowdownPitch = 0.9f;

		#endregion

		#region references

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
		private InputActionReference leftCtrl;

		[SerializeField]
		private InputActionReference space;

		[SerializeField]
		private Transform statementsParent;

		[SerializeField]
		private Transform bulletsParent;

		[SerializeField]
		private GameObject statementPrefab;

		[SerializeField]
		private GameObject bulletLabelPrefab;

		[SerializeField]
		private CounterAnimation counterAnimationPrefab;

		[SerializeField]
		private Transform counterAnimationParent;

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

		[SerializeField]
		private TimeSlider timeSlider;

		[SerializeField]
		private CharacterData playableCharacter;

		[SerializeField]
		private ScreenshotScript screenshotScript;

		[SerializeField]
		private LieAnimation lieAnimation;

		[SerializeField]
		private CustomCursor customCursor;

		#endregion

		[Header("State")]

		[SerializeField]
		private bool isPlaying = false;

		[SerializeField]
		private bool isStartLoopAnimation;

		[SerializeField]
		private float time;

		[SerializeField]
		private bool developerMode;

		// Other

		[SerializeField]
		private Animation3DData endLoopCameraPosition;

		private Sequence loop;

		private Queue<DebateSequenceData> statementsQueue;

		private void Awake()
		{
			if (data) Init(data);
		}

		private void OnDisable()
		{
			ClearAllUserInputEvents();
		}

		public void Init(DebateData debate)
		{
			data = debate;
			statementsQueue = new Queue<DebateSequenceData>();
			debate.debateSequence.ForEach(statement => statementsQueue.Enqueue(statement));

			debateGUI.SetActive(false);

			isStartLoopAnimation = true;
			StartCoroutine(Debate());
			bulletController.OnBulletMenuHidingEnd.AddListener(() => OnBulletPick());
		}

		public void PlayAnimation(DebateSequenceData statementData)
		{
			var speakingCharacter = TryGetCharacter(statementData.statement.speakingCharacter);

			cameraController.PlayCameraAnimation(statementData.cameraAnimation, speakingCharacter?.gameObject);

			if (statementData.statement)
			{
				LoadStatement(statementData);
				characterInfo.SetCharacter(statementData.statement.speakingCharacter);
			}
		}

		private static Character TryGetCharacter(CharacterData characterData)
		{
			return GameObject.FindGameObjectsWithTag("Character")
				.Select(x => x.GetComponent<Character>())
				.FirstOrDefault(x => x.Data == characterData);
		}

		private IEnumerator Debate()
		{
			Debug.Log("Starting debate...");

			if (!developerMode)
			{
				var animation = PlayDebateStartAnimation();
				yield return new WaitWhile(animation.IsActive);
			}

			Debug.Log("Loading bullets...");
			debateGUI.SetActive(true);
			bulletController.Init(data.bullets);

			if (!developerMode)
			{
				var animation = PlayLoadingBulletAnimation();
				yield return new WaitWhile(animation.IsActive);
			}

			EnableBulletPick();
			Debug.Log("Waiting for bullet pick...");

			if (!developerMode)
			{
				yield return new WaitUntil(() => isPlaying);
			}
			else
			{
				OnBulletPick();
				isPlaying = true;
			}

			EnableDebateRewind();
			EnableDebateSlowdown();

			while (isPlaying)
			{
				while (isPlaying && statementsQueue.Any())
				{
					var sentence = TryLoadNewSentence();
					if (sentence != null)
					{
						var sliderPosition = (float)(data.debateSequence.IndexOf(sentence) + 1) / (data.debateSequence.Count);
						timeSlider.SetPosition(sliderPosition);
					}
					yield return null;
				}

				if (!isPlaying) yield break;

				DebateSequenceData lastLoadedSequence = data.debateSequence.Last();
				yield return new WaitForSeconds(lastLoadedSequence.SequenceDuration);

				if (!developerMode) ResetDebateLoop();

				Debug.Log("Loop finished");
			}
		}

		private void ResetDebateLoop()
		{
			timeSlider.SetPosition(0);
			characterInfo.SetCharacter(playableCharacter);
			cameraController.PlayCameraAnimation(endLoopCameraPosition, TryGetCharacter(playableCharacter)?.gameObject);
			data.debateSequence.ForEach(statement => statementsQueue.Enqueue(statement));
		}

		private DebateSequenceData TryLoadNewSentence()
		{
			var statement = statementsQueue.Peek();
			if (statement.delay < time)
			{
				if (isStartLoopAnimation)
				{
					loop?.Kill();
					rightPanel.SetActive(true);
					characterInfo.SetCharacter(null);
					isStartLoopAnimation = false;
				}

				PlayAnimation(statement);
				statementsQueue.Dequeue();
				time = 0;
				return statement;
			}

			time += Time.deltaTime;
			return default;
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
			return bulletController.StartAnimation();
		}

		private void OnBulletChange(CallbackContext context = default)
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

		private void OnBulletPick(CallbackContext context = default)
		{
			bulletController.HideBulletPickMenu();
			bulletController.ShowSelectedBulletPanel();

			Debug.Log($"Picked bullet: {bulletController.SelectedBullet.bulletName}");

			isPlaying = true;
			buttonConfirm.action.performed -= OnBulletPick;
			buttonConfirm.action.performed += OnShoot;
		}

		private void OnShoot(CallbackContext context = default)
		{
			shootScript.Shoot(bulletController.SelectedBullet);
		}

		private DebateStatement LoadStatement(DebateSequenceData statementData)
		{
			var statement = Instantiate(statementPrefab, statementsParent).GetComponent<DebateStatement>();
			statement.Init(statementData);
			statement.OnCorrectBulletHit += PlayCounterAnimation;
			return statement;
		}

		private void PlayCounterAnimation(TruthBullet bullet)
		{
			StartCoroutine(IPlayCounterAnimation());

			IEnumerator IPlayCounterAnimation()
			{
				isPlaying = false;
				debateGUI.SetActive(false);
				customCursor.gameObject.SetActive(false);
				PauseDebate();
				ClearAllUserInputEvents();
				SetDebateNormalSpeed();

				screenshotScript.TakeScreenshot();
				yield return null; // wait for finishing the screenshot

				var counterAnimation = Instantiate(counterAnimationPrefab, counterAnimationParent).GetComponent<CounterAnimation>();

				counterAnimation.OnAnimationEnd += () =>
				{
					lieAnimation.OnAnimationEnd += () =>
					{
						cameraFade.DOFade(fadeOn, 0.75f)
						.SetEase(Ease.InOutFlash)
						.SetUpdate(true)
						.onComplete += () =>
						{
							RestartDebateScene();
						};
					};
					lieAnimation.PlayAnimation();

					Destroy(counterAnimation.gameObject);
				};
			}
		}

		private void RestartDebateScene()
		{
			DOTween.Clear();
			Time.timeScale = 1;
			SceneManager.LoadScene(0);
		}

		private void PauseDebate()
		{
			Time.timeScale = 0;
		}
		private void EnableDebateRewind()
		{
			leftCtrl.action.started += SetDebateRewindSpeed;
			leftCtrl.action.canceled += SetDebateNormalSpeed;
		}

		private void EnableDebateSlowdown()
		{
			space.action.started += SetDebateSlowdownSpeed;
			space.action.canceled += SetDebateNormalSpeed;
		}

		private void SetDebateNormalSpeed(CallbackContext context = default)
		{
			Time.timeScale = defaultSpeed;
			AudioController.instance.SetPitch(defaultPitch);
		}

		private void SetDebateRewindSpeed(CallbackContext context = default)
		{
			Time.timeScale = rewindSpeed;
			AudioController.instance.SetPitch(rewindPitch);
		}

		private void SetDebateSlowdownSpeed(CallbackContext context = default)
		{
			Time.timeScale = slowdownSpeed;
			AudioController.instance.SetPitch(slowdownPitch);
		}

		private void ClearAllUserInputEvents()
		{
			leftCtrl.action.started -= SetDebateRewindSpeed;
			leftCtrl.action.canceled -= SetDebateNormalSpeed;

			space.action.started -= SetDebateSlowdownSpeed;
			space.action.canceled -= SetDebateNormalSpeed;

			buttonConfirm.action.performed -= OnBulletPick;
			buttonConfirm.action.performed -= OnShoot;

			mouseScroll.action.performed -= OnBulletChange;
		}
	}
}
