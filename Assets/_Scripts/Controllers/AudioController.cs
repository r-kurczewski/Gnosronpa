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
    }
}
