using Gnosronpa.Scriptables;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gnosronpa.Controllers
{
	public class CharactersController : SingletonBase<CharactersController>
	{

		[SerializeField]
		private List<Character> characters;

		[SerializeField]
		private float _distance;

		public List<Character> Characters => characters;
		
		private float _defaultDistance;

		public float Distance { get => _distance; set => _distance = value; }

		public float DefaultDistance => _defaultDistance;

		/// <summary>
		/// Ratio of CharacterDistance to DefaultCharacterDistance
		/// </summary>
		public float DistanceMultiplier => Distance / DefaultDistance;

		private new void Awake()
		{
			base.Awake();
			_defaultDistance = Distance;
		}

		public void SetCharacters(List<CharacterData> charactersData)
		{
			var visibleCharacters = charactersData.Select(cd => Character.TryGet(cd)).ToList();

			SetVisibleCharacters(visibleCharacters);
			SetCharactersPositions(visibleCharacters);
		}

		public void SetVisibleCharacters(List<Character> visibleCharacters)
		{
			characters.ForEach(c => c.gameObject.SetActive(visibleCharacters.Contains(c)));
		}

		public void SetCharactersPositions(List<Character> characters)
		{
			for (int i = 0; i < characters.Count; i++)
			{
				var pos = new Vector3
				{
					x = transform.position.x + Distance * Mathf.Cos(-2 * Mathf.PI / characters.Count * i),
					z = transform.position.z + Distance * Mathf.Sin(-2 * Mathf.PI / characters.Count * i),
					y = transform.position.y
				};

				characters[i].transform.position = new Vector3(pos.x, pos.y, pos.z);
				characters[i].transform.LookAt(transform);
			}
		}
	}
}