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

		public void ApplyCameraAnimation(Animation3DData data, bool defaultTransition = true)
		{
			var seq = DOTween.Sequence();
			
			if (defaultTransition)
			{
				seq.Append(transform.DOLocalMove(data.startPosition, defaultTransitionDuration))
					.Join(transform.DOLocalRotate(data.startRotation, defaultTransitionDuration));
			}
			else
			{
				transform.localPosition = data.startPosition;
				transform.localRotation = Quaternion.Euler(data.startRotation);
			}

			if (data.moveDuration > 0)
			{
				seq.Append(transform.DOLocalMove(data.endPosition, data.moveDuration));
			}

			if (data.rotationDuration > 0)
			{
				seq.Join(transform.DOLocalRotate(data.endRotation, data.rotationDuration));
			}
		}
	}
}
