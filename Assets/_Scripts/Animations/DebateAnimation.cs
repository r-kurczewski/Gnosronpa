using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Controllers;
using UnityEngine;
using static Gnosronpa.Common.AnimationConsts;

namespace Gnosronpa.Animations
{
	public class DebateAnimation : MonoBehaviour
	{
		[SerializeField]
		private CameraFade cameraFade;

		[SerializeField]
		private Tweener loop;

		public async UniTask PlayDebateStartAnimation()
		{
			var ct = Camera.main.transform;
			var cp = ct.parent;

			var spin = new Vector3(0, -360, 0);
			var skew = new Vector3(0, 0, 5);
			var zoom = new Vector3(0, 4, CharactersController.Instance.Distance - 30);

			var fadeTime = 1f;
			var spin1Time = 4f * CharactersController.Instance.DistanceMultiplier;

			await UniTask.Delay(100);

			// First spin
			cp.localRotation = Quaternion.identity;
			ct.localRotation = Quaternion.Euler(2 * skew);

			_ = cameraFade.FadeIn(fadeTime);
			_ = cp.DOLocalRotate(1.5f * spin + 2 * skew, spin1Time, RotateMode.FastBeyond360).SetEase(Ease.Linear);
			await UniTask.Delay((int)((spin1Time - fadeTime) * 1000));
			await cameraFade.FadeOut(fadeTime);

			var spin2Time = 25f * CharactersController.Instance.DistanceMultiplier;

			// Second spin
			ct.transform.SetLocalPositionAndRotation(zoom, Quaternion.Euler(-skew));

			_ = cameraFade.FadeIn(fadeTime);
		
			cp.localRotation = Quaternion.identity;

			// infinite spin until bullet is chosen - transform changed to avoid killing by CameraController
			loop = cp.DOLocalRotate(spin, spin2Time, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(int.MaxValue).SetTarget(transform);
		}

		public void KillLoopAnimation()
		{
			loop?.Kill();
		}
	}
}
