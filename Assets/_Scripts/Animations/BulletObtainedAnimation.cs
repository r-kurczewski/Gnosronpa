using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Controllers;
using Gnosronpa.Scriptables;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Gnosronpa.Common.AnimationConsts;

namespace Gnosronpa.Animations
{
	[Serializable]
	public class BulletObtainedAnimation : MonoBehaviour
	{
		const float dimScreenAlpha = 0.2f;
		const float imageAnimationTime = 0.5f;
		const float imageMoveY = 30;

		[SerializeField]
		private AudioClip bulletObtainedSound;

		[SerializeField]
		private CanvasGroup imageGroup;

		[SerializeField]
		private CanvasGroup shadowBox;

		[SerializeField]
		private CanvasGroup dimScreen;

		[SerializeField]
		private Image bulletImage;

		public async UniTask PlayStartingAnimation(TruthBulletData bullet)
		{
			imageGroup.alpha = 0;
			dimScreen.alpha = 0;
			shadowBox.alpha = 1;

			imageGroup.transform.localPosition += Vector3.up * imageMoveY;
			bulletImage.sprite = bullet.bulletIcon;

			_ = AudioController.Instance.PlaySound(bulletObtainedSound);

			var fade = DOTween.To(() => dimScreen.alpha, (v) => dimScreen.alpha = v, dimScreenAlpha, imageAnimationTime).ToUniTask();
			var showImage = DOTween.To(() => imageGroup.alpha, (v) => imageGroup.alpha = v, show, imageAnimationTime).ToUniTask();
			var moveImageDown = imageGroup.transform.DOBlendableLocalMoveBy(Vector2.down * imageMoveY, imageAnimationTime).ToUniTask();

			await UniTask.WhenAll(fade, showImage, moveImageDown);

			await DOTween.To(() => shadowBox.alpha, (v) => shadowBox.alpha = v, hide, 2 * imageAnimationTime).ToUniTask();
		}

		public async UniTask PlayEndingAnimation()
		{
			var fadeOut = DOTween.To(() => dimScreen.alpha, (v) => dimScreen.alpha = v, hide, imageAnimationTime).ToUniTask();
			var hideImage = DOTween.To(() => imageGroup.alpha, (v) => imageGroup.alpha = v, hide, imageAnimationTime).ToUniTask();
			var moveImageDown = imageGroup.transform.DOBlendableLocalMoveBy(Vector2.down * imageMoveY, imageAnimationTime).ToUniTask();

			await UniTask.WhenAll(fadeOut, hideImage, moveImageDown);
		}
	}
}