using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoomTile))]
public class Rotor : MonoBehaviour
{
	public enum RotorType
	{
		Driveshaft,
		Generator,
		Machine
	}

	public RotorType rotorType;
	public float energyDelta;
	public float totalSystemEnergy;
	public List<Rotor> allRotorsInSystem;
	public RoomTile roomTile;

	public void UpdateSystemEnergy()
	{
		if (!roomTile)
			roomTile = GetComponent<RoomTile>();

		//pulses when an element is added or removed to the system
		//the changed element should update all the tiles in the system to change its allRotorsInSystem

		allRotorsInSystem = new List<Rotor>();
		allRotorsInSystem.Add(this);
		UpdateSystemEnergyPulse(this);
		float totalEnergy = 0;
		foreach (Rotor rotor in allRotorsInSystem)
		{
			totalEnergy += rotor.energyDelta;
		}
		foreach (Rotor rotor in allRotorsInSystem)
		{
			rotor.totalSystemEnergy = totalEnergy;
			rotor.allRotorsInSystem = allRotorsInSystem;
		}

	}
	public void UpdateSystemEnergyPulse(Rotor origin)
	{
		if (!roomTile)
			roomTile = GetComponent<RoomTile>();
		foreach (RoomTile rt in roomTile.neighborRooms)
		{
			if (rt && rt.thisRoomsRotor && !origin.allRotorsInSystem.Contains(rt.thisRoomsRotor))
			{
				origin.allRotorsInSystem.Add(rt.thisRoomsRotor);
				rt.thisRoomsRotor.UpdateSystemEnergyPulse(origin);
			}
		}
	}
}
