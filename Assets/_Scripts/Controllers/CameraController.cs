using DG.Tweening;
using Gnosronpa.Common;
using UnityEngine;

namespace Gnosronpa.Controllers
{
	/// <summary>
	/// Camera must be a child of this controller GameObject to properly apply base rotation.
	/// </summary>
	public class CameraController : SingletonBase<CameraController>
	{
		[SerializeField]
		private Transform cameraTransform;

		[SerializeField]
		private float defaultDistanceFromCharacter;

		[SerializeField]
		private float defaultCameraHeight;

		[SerializeField]
		private float defaultCameraRotationX;


		public void PlayCameraAnimation(CameraAnimationData animationData, GameObject target)
		{
			StopCurrentAnimations();

			var baseRotation = GetBaseRotation(target);
			var seq = DOTween.Sequence(transform);

			transform.SetLocalPositionAndRotation(
				new Vector3(transform.localPosition.x, defaultCameraHeight, transform.position.z),
				Quaternion.Euler(baseRotation));

			var startPosition = GetCameraPosition(target, animationData.startPosition);

			cameraTransform.position = startPosition;
			cameraTransform.transform.localRotation = Quaternion.Euler(animationData.startRotation);

			if (animationData.moveDuration > 0)
			{
				seq.Append(cameraTransform.DOLocalMove(animationData.endPosition, animationData.moveDuration));
			}

			if (animationData.rotationDuration > 0)
			{
				seq.Join(cameraTransform.DOLocalRotate(animationData.endRotation, animationData.rotationDuration));
			}
		}

		public void SetLastStateOfAnimation(CameraAnimationData animationData, GameObject target = null)
		{
			var baseRotation = GetBaseRotation(target);

			Vector3 cameraLastPosition;
			if (target != null && animationData != null)
			{
				cameraLastPosition = GetCameraPosition(target, animationData.startPosition) + animationData.endPosition;
			}
			else
			{
				cameraLastPosition = Vector3.zero;
			}

			transform.localRotation = Quaternion.Euler(baseRotation);
			cameraTransform.transform.position = cameraLastPosition;
			cameraTransform.transform.localRotation = Quaternion.Euler(animationData?.endRotation ?? Vector3.zero);
		}

		private Vector3 GetCameraPosition(GameObject target, Vector3 offset)
		{
			var distanceFromTarget = target.transform.forward * defaultDistanceFromCharacter;
			var cameraOffset = transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;

			var defaultCameraPosition = target.transform.position + distanceFromTarget;
			defaultCameraPosition.y = defaultCameraHeight;

			return defaultCameraPosition + cameraOffset;
		}

		private void StopCurrentAnimations()
		{
			var killCount = DOTween.Kill(transform);

			if (killCount == 0) return;
		}

		private Vector3 GetBaseRotation(GameObject target)
		{
			if (!target) return Vector3.zero;

			var rotation = Quaternion.LookRotation(target.transform.position).eulerAngles;
			rotation.x = defaultCameraRotationX;
			return rotation;
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawCube(transform.position, Vector3.one);
		}


	}
}
