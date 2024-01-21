using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gnosronpa.Controllers
{
	public class AudioController : SingletonBase<AudioController>
	{
		[SerializeField]
		private AudioSource musicSource;

		[SerializeField]
		private AudioSource sfxSource;

		[SerializeField]
		private AudioSource ambientSource;

		public async UniTask PlaySound(AudioClip clip, float volume = 1)
		{
			sfxSource.volume = volume;
			sfxSource.PlayOneShot(clip);
			await UniTask.Delay(Mathf.CeilToInt(clip.length * 1000));
		}

		public void PlayMusic(AudioClip segmentMusic, float volume = 1)
		{
			musicSource.clip = segmentMusic;
			musicSource.volume = volume;
			musicSource.Play();

		}

		public void PlayAmbient(AudioClip clip, float volume = 1)
		{
			ambientSource.clip = clip;
			ambientSource.volume = volume;
			ambientSource.Play();
		}

		public void StopAmbient()
		{
			ambientSource.Stop();
		}

		public async UniTask FadeOutMusic(float targetValue, float duration = 1)
		{
			await DOTween.To(() => musicSource.volume, (v) => musicSource.volume = v, targetValue, duration);
		}

		public void SetPitch(float pitch)
		{
			musicSource.pitch = pitch;
			sfxSource.pitch = pitch;
			ambientSource.pitch = pitch;
		}
	}
}
