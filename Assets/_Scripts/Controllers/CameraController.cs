using DG.Tweening;
using Gnosronpa.Common;
using System;
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
		public void PlayCameraAnimation(Animation3DData animationData, GameObject target = null)
		{
			StopCurrentAnimations();

			var baseRotation = GetBaseRotation(target);
			var seq = DOTween.Sequence(transform);

			if (transitions)
			{
				seq.Append(transform.DOLocalRotate(baseRotation, defaultTransitionDuration))
					.Join(cameraTransform.DOLocalMove(cameraTransform.InverseTransformDirection(animationData.startPosition), defaultTransitionDuration))
					.Join(cameraTransform.DOLocalRotate(animationData.startRotation, defaultTransitionDuration));
			}
			else
			{
				transform.localRotation = Quaternion.Euler(baseRotation);
				cameraTransform.SetLocalPositionAndRotation(animationData.startPosition, Quaternion.Euler(animationData.startRotation));
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
			cameraTransform.SetLocalPositionAndRotation(animationData?.endPosition ?? Vector3.zero, Quaternion.Euler(animationData?.endRotation ?? Vector3.zero));
		}

		public void ResetCamera()
		{

		}

		private void StopCurrentAnimations()
		{
			var killCount = DOTween.Kill(transform);

			if (killCount == 0) return;
			Debug.Log($"Stopped {killCount} active camera animations");
		}

		private Vector3 GetBaseRotation(GameObject target) => target ? Quaternion.LookRotation(target.transform.position).eulerAngles : Vector3.zero;

		private void OnDrawGizmos()
		{
			Gizmos.DrawCube(transform.position, Vector3.one);
		}
	}
}
