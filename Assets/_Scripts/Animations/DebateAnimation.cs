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

			var seq = DOTween.Sequence()
				.Append(ct.DOLocalRotate(2 * skew, instant))

				.Append(ct.DOLocalRotate(1.25f * spin + 2 * skew, 3, RotateMode.FastBeyond360).SetEase(Ease.Linear))
				.Join(cameraFade.DOFade(fadeOn, fadeTime).SetEase(Ease.InOutFlash).SetDelay(2))
			.Append(ct.DOLocalMove(zoom, instant))
				.Join(ct.DOLocalRotate(-skew, instant))

				.Append(cameraFade.DOFade(fadeOff, fadeTime).SetEase(Ease.InOutFlash))
				.Join(ct.DOLocalRotate(-skew, instant));

			// infinite spin until bullet is chosen - transform changed to avoid killing by CameraController
			loop = cp.DOLocalRotate(spin, 45, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(int.MaxValue).SetTarget(transform);

			await seq.AwaitForComplete();
		}

		public void KillLoopAnimation()
		{
			loop?.Kill();
		}
	}
}
