using Cysharp.Threading.Tasks;
using DG.Tweening;
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
			var zoom = new Vector3(0, 7, 20);

			var fadeTime = 0.5f;
			var spin1Time = 2.5f;

			await UniTask.Delay(100);

			// First spin
			cp.transform.localRotation = Quaternion.identity;
			ct.transform.localRotation = Quaternion.Euler(2 * skew);

			_ = cameraFade.FadeIn(fadeTime);
			_ = ct.DOLocalRotate(spin + 2 * skew, spin1Time, RotateMode.FastBeyond360).SetEase(Ease.Linear);
			await UniTask.Delay((int)((spin1Time - fadeTime) * 1000));
			await cameraFade.FadeOut(fadeTime);

			// Second spin
			ct.transform.SetLocalPositionAndRotation(zoom, Quaternion.Euler(-skew));

			_ = cameraFade.FadeIn(fadeTime);
			await ct.DOLocalRotate(-skew, instant);

			// infinite spin until bullet is chosen - transform changed to avoid killing by CameraController
			loop = cp.DOLocalRotate(spin, 30, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(int.MaxValue).SetTarget(transform);
		}

		public void KillLoopAnimation()
		{
			loop?.Kill();
		}
	}
}
