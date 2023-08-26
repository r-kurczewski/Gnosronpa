using System;
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

		public void SetSoundPitch(float pitch)
		{
         musicSource.pitch = pitch;
         sfxSource.pitch = pitch;
		}
	}
}
