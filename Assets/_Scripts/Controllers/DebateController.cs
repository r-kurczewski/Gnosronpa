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
		[SerializeField]
		private DebateData data;

		[SerializeField]
		private Transform debateParent;

		[SerializeField]
		private GameObject statementPrefab;

		[SerializeField]
		private CameraController cameraController;

		[SerializeField]
		private CameraFade cameraFade;

		[SerializeField]
		private ShootScript shootScript;

		[SerializeField]
		private CharacterInfo characterInfo;

		[SerializeField]
		private InputActionReference mouseClick;

		private Sequence seq;

		private Queue<StatementConfigurationData> statementsQueue;

		private bool isPlaying = false;

		private bool isStartLoopAnimation;

		private float time;

		private void Awake()
		{
			statementsQueue = new Queue<StatementConfigurationData>();
			if (data) LoadDebate(data);
		}

		private void Update()
		{
			if (statementsQueue.Count == 0) return;

			if (isPlaying)
			{
				var statement = statementsQueue.Peek();
				if (statement.delay < time)
				{
					if (isStartLoopAnimation)
					{
						seq.Kill();
						isStartLoopAnimation = false;
					}

					LoadAnimation(statement);
					statementsQueue.Dequeue();
					time = 0;
				}

				time += Time.deltaTime;
			}
		}

		private void StartDebateAnimation()
		{
			var camTransform = Camera.main.transform;
			var spin = new Vector3(0, -360, 0);
			var skew = new Vector3(0, 0, 5);
			var zoom = new Vector3(0, 7, 20);

			const int fadeOn = 1;
			const int fadeOff = 0;
			float duration = 3;
			var fadeTime = 0.5f;
			var fadeDelay = 2;

			camTransform.rotation = Quaternion.Euler(skew);

			seq = DOTween.Sequence();

			seq.Append(camTransform.DOLocalRotate(spin + 2*skew, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
				.Join(cameraFade.DOFade(fadeOn, fadeTime).SetEase(Ease.InOutFlash).SetDelay(fadeDelay))
				.Append(camTransform.DOLocalMove(zoom, 0))
				.Join(camTransform.DOLocalRotate(-skew, 0))
				.Append(cameraFade.DOFade(fadeOff, fadeTime).SetEase(Ease.InOutFlash))
				.AppendCallback(() => mouseClick.action.performed += OnBulletPick)
				.Append(camTransform.parent.DOLocalRotate(spin - skew, 30, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(int.MaxValue));
		}

		private void OnBulletPick(CallbackContext context)
		{
			isPlaying = true;
			shootScript.enabled = true;
			mouseClick.action.performed -= OnBulletPick;
		}

		public void LoadAnimation(StatementConfigurationData statementData)
		{
			var speakingCharacter = GameObject.FindGameObjectsWithTag("Character")
								.Select(x => x.GetComponent<Character>())
								.FirstOrDefault(x => x.Data == statementData.statement.speakingCharacter);

			cameraController.ApplyCameraAnimation(statementData.cameraOffset, speakingCharacter?.gameObject);

			if (statementData.statement)
			{
				LoadStatement(statementData);
				characterInfo.SetCharacter(statementData.statement.speakingCharacter);
			}
		}

		private void LoadStatement(StatementConfigurationData statementData)
		{
			var statement = Instantiate(statementPrefab, debateParent).GetComponent<DebateStatement>();
			statement.Init(statementData);

		}

		public void LoadDebate(DebateData debate)
		{
			data = debate;
			debate.data.ForEach(statement => statementsQueue.Enqueue(statement));

			isStartLoopAnimation = true;
			StartDebateAnimation();
		}

		public void Stop()
		{
			Time.timeScale = 0;
		}

		public void SetSpeed(float speed)
		{
			Time.timeScale = speed;
		}

		public void Restart()
		{
			time = 0;
		}
	}
}
