using UnityEngine;

namespace Gnosronpa.Controllers
{
   public class AudioController : SingletonBase<AudioController>
   {
      [SerializeField]
      private AudioSource musicSource;

      [SerializeField]
      private AudioSource sfxSource;

      public void PlaySound(AudioClip clip)
      {
         sfxSource.PlayOneShot(clip);
      }

      public void PlayLoopSound(AudioClip clip)
      {
         sfxSource.clip = clip;
         sfxSource.Play();
      }

      public void StopLoopSound()
      {
         sfxSource.Stop();
      }

		public void PlayMusic(AudioClip segmentMusic)
		{
         musicSource.clip = segmentMusic;
         musicSource.Play();

		}

		public void SetPitch(float pitch)
		{
			musicSource.pitch = pitch;
			sfxSource.pitch = pitch;
		}

	}
}
