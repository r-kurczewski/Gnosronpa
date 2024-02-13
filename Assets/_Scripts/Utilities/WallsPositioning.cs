using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Utilities
{
	public class WallsPositioning : MonoBehaviour
	{
		public float distance;
		public List<GameObject> walls;

		public void SetPositions()
		{
			for (int i = 0; i < walls.Count; i++)
			{
				var pos = new Vector3
				{
					x = distance * Mathf.Cos(-2 * Mathf.PI / walls.Count * i),
					z = distance * Mathf.Sin(-2 * Mathf.PI / walls.Count * i),
					y = transform.position.y
				};

				walls[i].transform.position = new Vector3(pos.x, pos.y, pos.z);
				walls[i].transform.LookAt(transform);
			}
		}
	}
}
