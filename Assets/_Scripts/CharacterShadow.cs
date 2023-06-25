using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa
{
    public class CharacterShadow : MonoBehaviour
    {
		private void Start()
		{
			GetComponent<Renderer>().receiveShadows = true;
		}
	}
}
