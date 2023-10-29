using DG.Tweening;
using UnityEngine;

namespace Gnosronpa.Common
{
	public static class TweenExtensions
	{
		public static Tween BlendableShake(this Transform transform, Vector3 shake, float duration, float vibrato, bool fadeout)
		{
			var seq = DOTween.Sequence(transform);

			float shakeDuration = duration / vibrato / 2;
			float fading = shakeDuration / duration * 2;

			float shakePower = 1;
			while (shakePower > 0)
			{
				for (int direction = 1; direction >= -1; direction -= 2)
				{
					seq.Append(transform.DOBlendableLocalMoveBy(direction * (fadeout ? shakePower : 1) * shake, shakeDuration));
				}
				shakePower -= fading;
			}
			return seq;
		}
	}
}
