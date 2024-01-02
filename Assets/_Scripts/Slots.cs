using Gnosronpa.Controllers;
using Gnosronpa.Scriptables;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gnosronpa
{
	public class Slots : MonoBehaviour
	{

		[SerializeField]
		private AudioClip votingLoopSound;

		[SerializeField]
		private AudioClip votingFinishedSound;

		[SerializeField]
		private List<Slot> slots;

		public CharacterData stopValue;

		[SerializeField]
		private InputActionReference stopSlotsAction;

		[SerializeField]
		private bool stop;

		[SerializeField]
		private List<CharacterData> slotCharacters;

		private void Start()
		{
			StartSlotsRotating();
		}

		private void StartSlotsRotating()
		{
			slots.ForEach(x => x.SetImages(slotCharacters));
			slots.ForEach(x => _ = x.RotateTo(slotCharacters.Last()));
		}

		private void OnEnable()
		{
			stopSlotsAction.action.Enable();
		}

		private void OnDisable()
		{
			stopSlotsAction.action.Disable();
		}

		private void Update()
		{
			if (stopSlotsAction.action.IsPressed() && !stop)
			{
				slots.ForEach(x => x.Stop = true);
				AudioController.instance.StopLoopSound();
				AudioController.instance.PlaySound(votingFinishedSound);
				stop = true;
			}
		}
	}
}
