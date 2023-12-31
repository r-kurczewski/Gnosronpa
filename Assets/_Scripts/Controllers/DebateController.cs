using Cysharp.Threading.Tasks;
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
using static Gnosronpa.Common.AnimationConsts;
using DG.Tweening;
using Gnosronpa.Animations;

namespace Gnosronpa.Controllers
{
	public class DebateController : StateMachine<DebateController, DebateState>
	{
		#region consts

		private const float rewindSpeed = 4;
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

		[SerializeField]
		private DebateAnimation debateAnimation;

		#endregion

		[Header("Other", order = 1)]

		[SerializeField]
		private DebateData data;

		[SerializeField]
		private float time;

		private bool Paused => GameController.instance.Paused;

		[SerializeField]
		private bool developerMode;

		private TruthBullet hitBullet;

		private DebateStatement hitStatement;

		private readonly Queue<DebateSequenceData> statementsQueue = new();

		#region State Machine

		private DebateState startAnimation;
		private DebateState loadingBullets;
		private DebateState choosingBullet;
		private DebateState debate;
		private DebateState preDebateLoop;
		private DebateState debateHint;
		private DebateState correctBulletHit;
		private DebateState incorrectBulletHit;

		protected override DebateState StartingState => startAnimation;

		protected override void DefineMachineStates()
		{
			startAnimation = new(nameof(startAnimation))
			{
				OnEnter = () =>
				{
					statementsQueue.Clear();
					data.debateSequence.ForEach(statement => statementsQueue.Enqueue(statement));

					dialogBox.OnMessageChanged += OnMessageChanged;

					debateGUI.SetActive(false);
					rightPanel.SetActive(false);
					customCursor.gameObject.SetActive(true);

					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					if (!developerMode)
					{
						await debateAnimation.PlayDebateStartAnimation();
					}
					return loadingBullets;
				}
			};

			loadingBullets = new(nameof(loadingBullets))
			{
				OnEnter = () =>
				{
					customCursor.gameObject.SetActive(true);
					debateGUI.SetActive(true);
					bulletController.Init(data);

					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					await UniTask.WaitWhile(() => bulletController.CurrentState == bulletController.Loading);

					return choosingBullet;
				},
			};

			choosingBullet = new(nameof(choosingBullet))
			{
				OnEnter = () =>
				{
					inputPickBullet.action.Enable();
					inputChangeBullet.action.Enable();
					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					if (!developerMode)
					{
						await UniTask.WaitUntil(() => bulletController.CurrentState == bulletController.Closed);
					}
					return preDebateLoop;
				},

				OnExit = async () =>
				{
					inputShoot.action.Disable();
					await UniTask.Yield();
				},
			};

			preDebateLoop = new(nameof(preDebateLoop))
			{
				OnEnter = () =>
				{
					rightPanel.SetActive(false);

					inputChangeBullet.action.Enable();
					inputPickBullet.action.Enable();

					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					var firstStatement = statementsQueue.Peek();
					while (time < firstStatement.delay)
					{
						time += Time.deltaTime;
						await UniTask.Yield();
					}
					return debate;
				},

				OnExit = () =>
				{
					debateAnimation.KillLoopAnimation();
					return UniTask.CompletedTask;
				},
			};

			debate = new(nameof(debate))
			{
				OnEnter = () =>
				{
					inputShoot.action.Enable();
					inputChangeBullet.action.Enable();
					inputPickBullet.action.Enable();

					rightPanel.SetActive(true);
					characterInfo.SetCharacter(null);

					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					while (developerMode)
					{
						await UniTask.Yield();
					}

					while (statementsQueue.Any() || statementsParent.childCount > 0)
					{
						if (Paused)
						{
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

					return debateHint;
				},

				OnExit = async () =>
				{
					SetDebateNormalSpeed();
					inputShoot.action.Disable();
					inputChangeBullet.action.Disable();

					await bulletController.CloseBulletMenu();
				},
			};

			debateHint = new(nameof(debateHint))
			{
				OnEnter = async () =>
					{
						await UniTask.Delay(TimeSpan.FromSeconds(showHintDelay), true);

						debateGUI.SetActive(false);
						dialogBox.SetVisibility(true);

						data.debateHints.ForEach((msg) => dialogBox.AddMessage(msg));

						dialogBox.LoadNextMessage(playSound: false);
						inputNextDialog.action.Enable();
					},

				OnExecute = async () =>
						{
							await UniTask.WaitUntil(() => dialogBox.MessagesEnded);
							return preDebateLoop;
						},

				OnExit = () =>
				{
					debateGUI.SetActive(true);
					dialogBox.SetVisibility(false);

					inputNextDialog.action.Disable();
					ResetDebateLoop();
					return UniTask.CompletedTask;
				},
			};

			correctBulletHit = new(nameof(correctBulletHit))
			{
				OnEnter = () =>
				{
					debateGUI.SetActive(false);
					PauseDebate();
					RemoveUserInputEvents();
					SetDebateNormalSpeed();
					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
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
				},
			};

			incorrectBulletHit = new(nameof(correctBulletHit))
			{
				OnEnter = () =>
				{

					debateGUI.SetActive(false);
					dialogBox.SetVisibility(true);

					hitStatement.Data.statement.hitDialogs.ForEach((msg) => dialogBox.AddMessage(msg));
					dialogBox.LoadNextMessage(playSound: false);

					inputNextDialog.action.Enable();
					return UniTask.CompletedTask;
				},

				OnExecute = async () =>
				{
					await UniTask.WaitUntil(() => dialogBox.MessagesEnded);
					return preDebateLoop;
				},

				OnExit = () =>
				{
					debateGUI.SetActive(true);
					dialogBox.SetVisibility(false);

					inputNextDialog.action.Disable();

					ResetDebateLoop();

					return UniTask.CompletedTask;
				},
			};
		}

		protected override Func<UniTask> DefineFinalStateBehaviour()
		{
			return () =>
			{
				RestartDebateScene();
				return UniTask.CompletedTask;
			};
		}

		#endregion

		public void Init(DebateData data)
		{
			this.data = data;
			AddUserInputEvents(disabled: true);
			_ = InitStateMachine();
		}

		private void OnDestroy()
		{
			RemoveUserInputEvents();
		}

		public void PlayAnimation(DebateSequenceData statementData)
		{
			var speakingCharacter = TryGetCharacter(statementData.statement.speakingCharacter);

			if (speakingCharacter == null)
			{
				Debug.LogError($"Could not get character from {statementData.statement.name}", this);
			}

			cameraController.PlayCameraAnimation(statementData.cameraAnimation, speakingCharacter.gameObject);

			if (statementData.statement)
			{
				LoadStatement(statementData);
				characterInfo.SetCharacter(statementData.statement.speakingCharacter);
			}
		}

		private Character TryGetCharacter(CharacterData characterData)
		{
			var result = GameObject.FindGameObjectsWithTag("Character")
				.Select(x => x.GetComponent<Character>())
				.FirstOrDefault(x => x.Data == characterData);

			return result;
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
			ClearSpawnedBullets();
			ClearSpawnedStatements();
			time = 0;

			data.debateSequence.ForEach(statement => statementsQueue.Enqueue(statement));
		}

		private void ClearSpawnedBullets()
		{
			foreach (Transform statement in bulletsParent.transform)
			{
				statement.gameObject.SetActive(false);
				Destroy(statement.gameObject);
			}
		}

		private void ClearSpawnedStatements()
		{
			foreach (Transform statement in statementsParent.transform)
			{
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

		private async void OnBulletChange(CallbackContext context = default)
		{
			if (bulletController.CurrentState == bulletController.Closed)
			{
				await bulletController.OpenBulletMenu();
				return;
			}

			if (bulletController.CurrentState != bulletController.Opened
				&& bulletController.CurrentState != bulletController.Loaded) return;

			var direction = context.ReadValue<float>();

			if (direction > 0)
			{
				await bulletController.ChangeBulletUp();
			}
			else if (direction < 0)
			{
				await bulletController.ChangeBulletDown();
			}
		}

		private async void OnPickBullet(CallbackContext context = default)
		{
			if (bulletController.CurrentState != bulletController.Closed)
				await bulletController.CloseBulletMenu();
		}

		private void OnShoot(CallbackContext context = default)
		{
			if (bulletController.CurrentState != bulletController.Closed) return;

			shootScript.TryShoot(bulletController.SelectedBullet);
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

		private void AddUserInputEvents(bool disabled)
		{
			inputPickBullet.action.performed += OnPickBullet;
			inputShoot.action.performed += OnShoot;
			inputChangeBullet.action.performed += OnBulletChange;
			inputNextDialog.action.performed += OnNextDialogMessage;

			if (disabled)
			{
				inputPickBullet.action.Disable();
				inputShoot.action.Disable();
				inputChangeBullet.action.Disable();
				inputNextDialog.action.Disable();
			}
		}

		private void RemoveUserInputEvents()
		{
			inputShoot.action.performed -= OnPickBullet;
			inputShoot.action.performed -= OnShoot;
			inputChangeBullet.action.performed -= OnBulletChange;
			inputNextDialog.action.performed -= OnNextDialogMessage;
		}
	}
}
