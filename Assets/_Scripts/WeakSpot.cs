using Fangan.ScriptableObjects;
using System.Collections;
using TMPro;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
	public WeakSpotData data;

	private TMP_Text text;

	new private BoxCollider collider;

	private Time time;

	private string Gradient(string text) => $"<gradient=\"Weak spot\">{text}</gradient>";

	private void Awake()
	{
		text = GetComponent<TMP_Text>();
		collider = GetComponent<BoxCollider>();
	}

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		name = data.name;
		text.text = string.Format(data.textTemplate, Gradient(data.weakSpotText));
		collider.center = data.collider.center;
		collider.size = data.collider.extents; // correct

		transform.rotation = Quaternion.Euler(0, 0, data.startRotation);
	}

	private void OnTriggerEnter(Collider other)
	{
		var bullet = other.GetComponent<TruthBullet>();
		Debug.Log($"[{name}] hit with [{bullet.name}], correct: [{bullet.data == data.correctBullet}]");
	}
}
