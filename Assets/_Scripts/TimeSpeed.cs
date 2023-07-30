using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa
{
    public class TimeSpeed : MonoBehaviour
    {
		[SerializeField]
		private float speed;

		private void Awake()
		{
			Time.timeScale = speed;
		}
	}
}
