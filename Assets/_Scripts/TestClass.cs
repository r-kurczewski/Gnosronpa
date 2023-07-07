using Codice.Client.Common.GameUI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	public class TestClass : MonoBehaviour
	{
		public List<Sequence> sequences = new List<Sequence>();
		void Start()
		{
			var seq = DOTween.Sequence();
			if (name == "One")
			{
				seq.Append(transform.DORotate(new Vector3(0, 0, 360), 3, RotateMode.FastBeyond360).SetEase(Ease.Linear));
				InvokeRepeating("AddLoop", 4, 4);
			}
			else
			{
				seq.Append(transform.DORotate(new Vector3(0, 0, 360), 3, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(int.MaxValue));
			}
			sequences.Add(seq);
		}
		private void AddLoop()
		{
			Debug.Log("New loop");
			var seq = DOTween.Sequence();
			sequences.First().Kill();
			seq.Append(transform.DORotate(new Vector3(0, 0, 360), 3, RotateMode.FastBeyond360).SetEase(Ease.Linear));
		}
	}
}
