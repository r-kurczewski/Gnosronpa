using UnityEngine;

namespace Fangan
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
