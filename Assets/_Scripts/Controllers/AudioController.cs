using System;
using UnityEngine;

namespace Gnosronpa.Controllers
{
   public class AudioController : SingletonBase<AudioController>
    {
      [SerializeField]
      private AudioSource audioSource;

      public void PlaySound(AudioClip clip)
      {
         audioSource.PlayOneShot(clip);
      }

		public void SetPitch(float pitch)
		{
         audioSource.pitch = pitch;
		}
	}
}
