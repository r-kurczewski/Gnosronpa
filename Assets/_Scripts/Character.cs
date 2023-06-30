using Gnosronpa.ScriptableObjects;
using System;
using UnityEngine;

namespace Gnosronpa
{
	public class Character : MonoBehaviour
    {
		[SerializeField]
		private CharacterData _data;

		public CharacterData Data { get => _data; set => _data = value; }

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
			transform.rotation = Camera.main.transform.rotation;
		}
	}
}
