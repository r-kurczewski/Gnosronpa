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

			return result;
		}

		private void Update()
		{
			UpdateRotation();
		}

		public void Load(CharacterData data)
		{
			throw new NotImplementedException();
		}

		public void UpdateRotation()
		{
			var cameraRotation = Camera.main.transform.rotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(0, cameraRotation.y, 0);
		}
	}
}
