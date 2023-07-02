using DG.Tweening;
using Gnosronpa.Common;
using UnityEngine;

namespace Gnosronpa.Controllers
{
	/// <summary>
	/// Camera must be a child of this controller GameObject to properly apply base rotation.
	/// </summary>
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private Transform cameraTransform;

		[SerializeField]
		private bool transitions;

		[SerializeField]
		private float defaultTransitionDuration = 0.1f;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="animationData">AnimationData applied in relation to target</param>
		/// <param name="target">Target on which the camera will focus</param>
		/// <param name="defaultTransition">Whether the default camera transition between current and start position and rotation should be used</param>
		public void ApplyCameraAnimation(Animation3DData animationData, GameObject target = null)
		{
			var baseRotation = GetBaseRotation(target);

			var seq = DOTween.Sequence();

			if (transitions)
			{
				seq.Append(transform.DOLocalRotate(baseRotation, defaultTransitionDuration))
					.Join(cameraTransform.DOLocalMove(cameraTransform.InverseTransformDirection(animationData.startPosition), defaultTransitionDuration))
					.Join(cameraTransform.DOLocalRotate(animationData.startRotation, defaultTransitionDuration));
			}
			else
			{
				transform.localRotation = Quaternion.Euler(baseRotation);
				cameraTransform.localPosition = animationData.startPosition;
				cameraTransform.localRotation = Quaternion.Euler(animationData.startRotation);
			}

			if (animationData.moveDuration > 0)
			{
				seq.Append(cameraTransform.DOLocalMove(animationData.endPosition, animationData.moveDuration));
			}

			if (animationData.rotationDuration > 0)
			{
				seq.Join(cameraTransform.DOLocalRotate(animationData.endRotation, animationData.rotationDuration));
			}
		}

		public void SetLastStateOfAnimation(Animation3DData animationData, GameObject target = null)
		{
			var baseRotation = GetBaseRotation(target);

			transform.localRotation = Quaternion.Euler(baseRotation);
			cameraTransform.localPosition = animationData?.endPosition ?? Vector3.zero;
			cameraTransform.localRotation = Quaternion.Euler(animationData?.endRotation ?? Vector3.zero);
		}

		private Vector3 GetBaseRotation(GameObject target) => target ? Quaternion.LookRotation(target.transform.position).eulerAngles : Vector3.zero;
	}
}
