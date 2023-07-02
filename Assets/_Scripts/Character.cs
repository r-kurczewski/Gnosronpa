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
			var cameraRotation = Camera.main.transform.rotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(0, cameraRotation.y, 0);
		}
	}
}
