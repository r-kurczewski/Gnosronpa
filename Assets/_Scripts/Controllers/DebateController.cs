using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Common;
using Gnosronpa.ScriptableObjects;
using Gnosronpa.StateMachines.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Gnosronpa.StateMachines;
using static UnityEngine.InputSystem.InputAction;

namespace Gnosronpa.Controllers
{
	public class DebateController : StateMachine<DebateController, DebateState>
	{
		#region consts

		private const int fadeOn = 1;
		private const int fadeOff = 0;
		private const int instant = 0;

		private const float rewindSpeed = 3;
		private const float rewindPitch = 1.3f;

		private const float defaultSpeed = 1;
		private const float defaultPitch = 1;

		private const float slowdownSpeed = 0.65f;
		private const float slowdownPitch = 0.9f;

		private const float showHintDelay = 0f;

		#endregion

		#region references

		[Header("References", order = 1)]

		[SerializeField]
		private DebateData data;

		[SerializeField]
		private CameraController cameraController;

		[SerializeField]
		private InputActionReference inputShoot;

		[SerializeField]
		private InputActionReference inputPickBullet;

		[SerializeField]
		private InputActionReference inputChangeBullet;

		[SerializeField]
		private InputActionReference inputRewind;

		[SerializeField]
		private InputActionReference inputSlowmode;

		[SerializeField]
		private InputActionReference inputNextDialog;

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
		private ScreenshotScript screenshotScript;

		[SerializeField]
		private LieAnimation lieAnimation;

		[SerializeField]
		private CustomCursor customCursor;

		[SerializeField]
		private DialogBox dialogBox;

		#endregion

		[Header("Other", order = 1)]

		[SerializeField]
		private float time;

		private bool Paused => Time.timeScale == 0;

		[SerializeField]
		private bool developerMode;

		private Tween loop;

		private TruthBullet hitBullet;

		private DebateStatement hitStatement;

		private readonly Queue<DebateSequenceData> statementsQueue = new();

		#region State Machine

		private readonly DebateState startAnimation = new(nameof(startAnimation));
		private readonly DebateState loadingBullets = new(nameof(loadingBullets));
		private readonly DebateState choosingBullet = new(nameof(choosingBullet));
		private readonly DebateState debate = new(nameof(debate));
		private readonly DebateState preDebateLoop = new(nameof(preDebateLoop));
		private readonly DebateState debateHint = new(nameof(debateHint));
		private readonly DebateState correctBulletHit = new(nameof(correctBulletHit));
		private readonly DebateState incorrectBulletHit = new(nameof(incorrectBulletHit));
		private readonly DebateState showBulletDescriptionState = new(nameof(showBulletDescriptionState));

		protected override DebateState StartingState => startAnimation;

		protected override void DefineMachineStates()
		{
			startAnimation.OnEnter = () =>
			{
				statementsQueue.Clear();
				data.debateSequence.ForEach(statement => statementsQueue.Enqueue(statement));

				dialogBox.OnMessageChanged += OnMessageChanged;

				debateGUI.SetActive(false);
				rightPanel.SetActive(false);
				customCursor.gameObject.SetActive(true);

				return UniTask.CompletedTask;
			};

			startAnimation.OnExecute = async () =>
			{
				if (!developerMode)
				{
					await PlayDebateStartAnimation();
				}
				return loadingBullets;
			};

			loadingBullets.OnEnter = () =>
			{
				customCursor.gameObject.SetActive(true);
				debateGUI.SetActive(true);
				bulletController.Init(data.bullets);

				return UniTask.CompletedTask;
			};

			loadingBullets.OnExecute = async () =>
			{
				await UniTask.WaitWhile(() => bulletController.CurrentState == bulletController.Loading);

				return choosingBullet;
			};

			choosingBullet.OnEnter = () =>
			{
				inputPickBullet.action.Enable();
				inputChangeBullet.action.Enable();
				return UniTask.CompletedTask;
			};

			choosingBullet.OnExecute = async () =>
			{
				if (!developerMode)
				{
					await UniTask.WaitUntil(() => bulletController.CurrentState == bulletController.Closed);
				}
				return preDebateLoop;
			};

			choosingBullet.OnExit = async () =>
			{
				inputShoot.action.Disable();
				await UniTask.Yield();
			};

			preDebateLoop.OnEnter = () =>
			{
				rightPanel.SetActive(false);

				inputChangeBullet.action.Enable();
				inputPickBullet.action.Enable();

				return UniTask.CompletedTask;
			};

			preDebateLoop.OnExecute = async () =>
			{
				var firstStatement = statementsQueue.Peek();
				while (time < firstStatement.delay)
				{
					time += Time.deltaTime;
					await UniTask.Yield();
				}
				return debate;
			};

			preDebateLoop.OnExit = () =>
			{
				loop?.Kill();
				return UniTask.CompletedTask;
			};

			debate.OnEnter = () =>
			{
				inputShoot.action.Enable();
				inputChangeBullet.action.Enable();
				inputPickBullet.action.Enable();

				rightPanel.SetActive(true);
				characterInfo.SetCharacter(null);

				return UniTask.CompletedTask;
			};

			debate.OnExecute = async () =>
			{
				while (statementsQueue.Any() || statementsParent.childCount > 0)
				{
					if (Paused)
					{
						Debug.Log("Pause");
						await UniTask.WaitWhile(() => Paused);
						continue;
					}

					CheckStateChangeRequest();

					var statement = TryLoadNewStatement();
					if (statement != null)
					{
						var sliderPosition = (float)(data.debateSequence.IndexOf(statement) + 1) / (data.debateSequence.Count);
						timeSlider.SetPosition(sliderPosition);
					}

					if (inputSlowmode.action.IsPressed())
					{
						SetDebateSlowdownSpeed();
					}
					else if (inputRewind.action.IsPressed())
					{
						SetDebateRewindSpeed();
					}
					else
					{
						SetDebateNormalSpeed();
					}

					time += Time.deltaTime;
					await UniTask.Yield();
				}

				return developerMode ? FinalState : debateHint;
			};

			debate.OnExit = async () =>
			{
				SetDebateNormalSpeed();
				inputShoot.action.Disable();
				inputChangeBullet.action.Disable();

				await bulletController.RequestStateChange(bulletController.Closing);
				await UniTask.WaitUntil(() => bulletController.CurrentState == bulletController.Closed);
			};

			debateHint.OnEnter = async () =>
			{
				await UniTask.Delay(TimeSpan.FromSeconds(showHintDelay), true);

				debateGUI.SetActive(false);
				dialogBox.SetVisibility(true);

				data.debateHints.ForEach((msg) => dialogBox.AddMessage(msg));

				dialogBox.LoadNextMessage(playSound: false);
				inputNextDialog.action.Enable();
			};

			debateHint.OnExecute = async () =>
			{
				await UniTask.WaitUntil(() => dialogBox.MessagesEnded);
				return preDebateLoop;
			};

			debateHint.OnExit = () =>
			{
				debateGUI.SetActive(true);
				dialogBox.SetVisibility(false);

				inputNextDialog.action.Disable();
				ResetDebateLoop();
				return UniTask.CompletedTask;
			};

			correctBulletHit.OnEnter = () =>
			{
				debateGUI.SetActive(false);
				customCursor.gameObject.SetActive(false);
				PauseDebate();
				RemoveUserInputEvents();
				SetDebateNormalSpeed();
				return UniTask.CompletedTask;
			};

			correctBulletHit.OnExecute = async () =>
			{
				var speakingCharacter = data.debateSequence
				.Single(x => x.statement.correctBullet == hitBullet.Data)
				.statement.speakingCharacter;

				var speakingCharacterLieAnimation = GameObject.FindGameObjectsWithTag("Character")
					.Select(x => x.GetComponent<Character>())
					.Single(x => x.Data == speakingCharacter)
					.GetComponent<LieAnimation>();

				var counterAnimation = Instantiate(counterAnimationPrefab, counterAnimationParent).GetComponent<CounterAnimation>();

				await counterAnimation.PlayAnimation();
				await speakingCharacterLieAnimation.PlayAnimation();

				await cameraFade.DOFade(fadeOn, 0.75f)
					.SetEase(Ease.InOutFlash)
					.SetUpdate(true);

				return FinalState;
			};

			incorrectBulletHit.OnEnter = () =>
			{

				debateGUI.SetActive(false);
				dialogBox.SetVisibility(true);

				hitStatement.Data.hitDialogs.ForEach((msg) => dialogBox.AddMessage(msg));
				dialogBox.LoadNextMessage(playSound: false);

				inputNextDialog.action.Enable();
				return UniTask.CompletedTask;
			};

			incorrectBulletHit.OnExecute = async () =>
			{
				await UniTask.WaitUntil(() => dialogBox.MessagesEnded);
				return preDebateLoop;
			};

			incorrectBulletHit.OnExit = () =>
			{
				debateGUI.SetActive(true);
				dialogBox.SetVisibility(false);

				inputNextDialog.action.Disable();

				ResetDebateLoop();

				return UniTask.CompletedTask;
			};

			FinalState.OnEnter = () =>
			{
				RestartDebateScene();
				return UniTask.CompletedTask;
			};
		}

		#endregion

		private void Start()
		{
			InitStateMachine();
		}

		private void OnEnable()
		{
			AddUserInputEvents();
		}

		private void OnDisable()
		{
			RemoveUserInputEvents();
		}

		public void PlayAnimation(DebateSequenceData statementData)
		{
			var speakingCharacter = TryGetCharacter(statementData.statement.speakingCharacter);

			cameraController.PlayCameraAnimation(statementData.cameraAnimation, speakingCharacter.gameObject);

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

		private void OnNextDialogMessage(CallbackContext context = default)
		{
			if (dialogBox.MessageContentDisplayed)
			{
				dialogBox.LoadNextMessage();
			}
			else
			{
				dialogBox.ForceDisplayMessage();
			}
		}

		private void OnMessageChanged(DialogMessage currentMessage)
		{
			characterInfo.SetCharacter(currentMessage.speakingCharacter);
			cameraController.PlayCameraAnimation(currentMessage.cameraAnimation, TryGetCharacter(currentMessage.speakingCharacter).gameObject);
		}

		private void ResetDebateLoop()
		{
			timeSlider.SetPosition(0);
			statementsQueue.Clear();
			ClearSpawnedStatements();
			ClearSpawnedBullets();
			time = 0;

			data.debateSequence.ForEach(statement => statementsQueue.Enqueue(statement));
		}

		private void ClearSpawnedStatements()
		{
			foreach (Transform statement in bulletsParent.transform)
			{
				Debug.Log(statement.name);
				statement.gameObject.SetActive(false);
				Destroy(statement.gameObject);
			}
		}

		private void ClearSpawnedBullets()
		{
			foreach (Transform statement in statementsParent.transform)
			{
				Debug.Log(statement.name);
				statement.gameObject.SetActive(false);
				Destroy(statement.gameObject);
			}
		}

		private DebateSequenceData TryLoadNewStatement()
		{
			if (!statementsQueue.Any()) return null;

			var statement = statementsQueue.Peek();
			if (statement.delay < time)
			{
				PlayAnimation(statement);
				statementsQueue.Dequeue();
				time = 0;
				return statement;
			}
			return null;
		}

		private async UniTask PlayDebateStartAnimation()
		{
			var ct = Camera.main.transform;
			var cp = ct.parent;

			var spin = new Vector3(0, -360, 0);
			var skew = new Vector3(0, 0, 5);
			var zoom = new Vector3(0, 7, 20);

			var fadeTime = 0.5f;

			var seq = DOTween.Sequence()
				.Append(ct.DOLocalRotate(2 * skew, instant))

				.Append(ct.DOLocalRotate(1.25f * spin + 2 * skew, 3, RotateMode.FastBeyond360).SetEase(Ease.Linear))
				.Join(cameraFade.DOFade(fadeOn, fadeTime).SetEase(Ease.InOutFlash).SetDelay(2))

				.Append(ct.DOLocalMove(zoom, instant))
				.Join(ct.DOLocalRotate(-skew, instant))

				.Append(cameraFade.DOFade(fadeOff, fadeTime).SetEase(Ease.InOutFlash))
				.Join(ct.DOLocalRotate(-skew, instant));

			await seq.AwaitForComplete();

			// infinite spin until bullet is chosen - transform changed to avoid killing by CameraController
			loop = cp.DOLocalRotate(spin, 30, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(int.MaxValue).SetTarget(transform);
		}

		private async void OnBulletChange(CallbackContext context = default)
		{
			if (bulletController.CurrentState == bulletController.Closed)
			{
				await bulletController.RequestStateChange(bulletController.Opening);
			}

			if (bulletController.CurrentState != bulletController.Opened
				&& bulletController.CurrentState != bulletController.Loaded) return;

			var direction = context.ReadValue<float>();

			if (direction > 0)
			{
				await bulletController.RequestStateChange(bulletController.BulletUp);
			}
			else if (direction < 0)
			{
				await bulletController.RequestStateChange(bulletController.BulletDown);
			}
		}

		private async void OnPickBullet(CallbackContext context = default)
		{
			if (bulletController.CurrentState != bulletController.Closed)
				await bulletController.RequestStateChange(bulletController.Closing);
		}

		private void OnShoot(CallbackContext context = default)
		{
			if (bulletController.CurrentState != bulletController.Closed) return;

			shootScript.Shoot(bulletController.SelectedBullet);
		}

		private DebateStatement LoadStatement(DebateSequenceData statementData)
		{
			var statement = Instantiate(statementPrefab, statementsParent).GetComponent<DebateStatement>();
			statement.Init(statementData);
			statement.OnCorrectBulletHit += PlayCounterAnimation;
			statement.OnIncorrectBulletHitAnimationEnded += LoadIncorrectHitMessages;
			return statement;
		}

		private async UniTask PlayCounterAnimation(TruthBullet bullet, DebateStatement statement)
		{
			hitBullet = bullet;
			hitStatement = statement;

			await RequestStateChange(correctBulletHit);
		}

		private async UniTask LoadIncorrectHitMessages(TruthBullet bullet, DebateStatement statement)
		{
			hitBullet = bullet;
			hitStatement = statement;

			await RequestStateChange(incorrectBulletHit);
		}

		private void RestartDebateScene()
		{
			DOTween.Clear(true);
			Time.timeScale = 1;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void SetDebateNormalSpeed()
		{
			Time.timeScale = defaultSpeed;
			AudioController.instance.SetSoundPitch(defaultPitch);
		}

		private void SetDebateRewindSpeed()
		{
			Time.timeScale = rewindSpeed;
			AudioController.instance.SetSoundPitch(rewindPitch);
		}

		private void SetDebateSlowdownSpeed()
		{
			Time.timeScale = slowdownSpeed;
			AudioController.instance.SetSoundPitch(slowdownPitch);
		}

		private void PauseDebate()
		{
			Time.timeScale = 0;
		}

		private void AddUserInputEvents(bool disabled = true)
		{
			inputPickBullet.action.performed += OnPickBullet;
			inputShoot.action.performed += OnShoot;
			inputChangeBullet.action.performed += OnBulletChange;
			inputNextDialog.action.performed += OnNextDialogMessage;
			//inputBulletDescription.action.performed += OnBulletDescription;

			if (disabled)
			{
				inputPickBullet.action.Disable();
				inputShoot.action.Disable();
				inputChangeBullet.action.Disable();
				inputNextDialog.action.Disable();
				//inputBulletDescription.action.Disable();
			}
		}

		private void RemoveUserInputEvents()
		{
			inputShoot.action.performed -= OnPickBullet;
			inputShoot.action.performed -= OnShoot;
			inputChangeBullet.action.performed -= OnBulletChange;
			inputNextDialog.action.performed -= OnNextDialogMessage;
			//inputBulletDescription.action.performed -= OnBulletDescription;
		}
	}
}
