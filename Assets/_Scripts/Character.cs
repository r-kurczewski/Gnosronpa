using Gnosronpa.Scriptables;
using System;
using System.Linq;
using UnityEngine;

namespace Gnosronpa
{
	public class Character : MonoBehaviour
	{
		[SerializeField]
		private CharacterData _data;

		public CharacterData Data { get => _data; set => _data = value; }

		public static Character TryGet(CharacterData characterData)
		{
			var result = GameObject.FindGameObjectsWithTag("Character")
				.Select(x => x.GetComponent<Character>())
				.FirstOrDefault(x => x.Data == characterData);

			if (result == null) Debug.LogWarning($"Could not get Character script for {characterData.name}");
			return result;
		}
	}
}
