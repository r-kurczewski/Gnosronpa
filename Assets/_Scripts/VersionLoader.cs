using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	[ExecuteInEditMode]
	public class VersionLoader : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text text;

		[SerializeField]
		private string versionPrefix;

		void Start()
		{
			text.text = versionPrefix + Application.version;
		}
	}
}