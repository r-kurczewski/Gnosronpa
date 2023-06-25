using UnityEngine;

namespace Gnosronpa
{
	public class CharacterPositioning : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] characters;

		[SerializeField]
		private float distance;

		public void SetCharacterPositions()
		{
			for (int i = 0; i < characters.Length; i++)
			{
				var pos = new Vector3(0, 0, 15);
				pos.x = distance * Mathf.Cos(2 * Mathf.PI / characters.Length * i);
				pos.z = distance * Mathf.Sin(2 * Mathf.PI / characters.Length * i);

				characters[i].transform.localPosition = new Vector3(pos.x, characters[i].transform.localPosition.y, pos.z);
			}
		}
	}
}