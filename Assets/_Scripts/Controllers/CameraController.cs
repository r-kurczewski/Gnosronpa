using DG.Tweening;
using Gnosronpa.Common;
using UnityEngine;

namespace Gnosronpa.Controllers
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private float speed;

		private const float defaultTransitionDuration = 0.2f;

		public void ApplyCameraAnimation(Animation3DData animationData, GameObject target = null, bool defaultTransition = true)
		{
			Vector3 targetLookAtRotation = target ? Quaternion.LookRotation(target.transform.position).eulerAngles : Vector3.zero;

			var seq = DOTween.Sequence();

			if (defaultTransition)
			{
				seq.Append(transform.DOLocalMove(animationData.startPosition, defaultTransitionDuration))
					.Join(transform.DOLocalRotate(targetLookAtRotation + animationData.startRotation, defaultTransitionDuration));
			}
			else
			{
				transform.localPosition = animationData.startPosition;
				transform.localRotation = Quaternion.Euler(targetLookAtRotation + animationData.startRotation);
			}

			if (animationData.moveDuration > 0)
			{
				seq.Append(transform.DOLocalMove(animationData.endPosition, animationData.moveDuration));
			}

			if (animationData.rotationDuration > 0)
			{
				seq.Join(transform.DOLocalRotate(targetLookAtRotation + animationData.endRotation, animationData.rotationDuration));
			}
		}

	}
}
