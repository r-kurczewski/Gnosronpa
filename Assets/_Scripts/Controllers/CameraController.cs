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
		private float speed;

		private Transform cameraTransform;

		private const float defaultTransitionDuration = 0.1f;

		private void Awake()
		{
			cameraTransform = GetComponentInChildren<Camera>().transform;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="animationData">AnimationData applied in relation to target</param>
		/// <param name="target">Target on which the camera will focus</param>
		/// <param name="defaultTransition">Whether the default camera transition between current and start position and rotation should be used</param>
		public void ApplyCameraAnimation(Animation3DData animationData, GameObject target = null, bool defaultTransition = true)
		{
			Vector3 baseRotation = target ? Quaternion.LookRotation(target.transform.position).eulerAngles : Vector3.zero;

			var seq = DOTween.Sequence();

			if (defaultTransition)
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
	}
}
