using UnityEngine;

namespace Gnosronpa
{
	public class CharacterRotation : MonoBehaviour
    {
		private void Update()
		{
			SetRotation();
		}

		public void SetRotation()
		{
			transform.rotation = Camera.main.transform.rotation;
		}
	}
}
