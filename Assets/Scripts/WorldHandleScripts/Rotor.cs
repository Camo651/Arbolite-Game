using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoomTile))]
public class Rotor : MonoBehaviour
{
	/// <summary>
	/// The types of shafts
	/// </summary>
	public enum RotorType
	{
		Driveshaft,
		Generator,
		Machine
	}

	public RotorType rotorType;
	public float energyDelta;
	public bool rotorIsEnabled;
	public bool systemHasSufficientEnergy;
	public float totalSystemEnergy;
	public List<Rotor> allRotorsInSystem;
	public RoomTile roomTile;
	public SO_Property rotorStateProperty;
	public List<SO_Property> rotorProductionProperties;

	/// <summary>
	/// Updates all the energy values in the current system
	/// </summary>
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
			systemHasSufficientEnergy = totalEnergy >= 0;
		}

	}

	/// <summary>
	/// Recursively pulses the energy to flood the whole system
	/// </summary>
	/// <param name="origin">The origin of the pulse</param>
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

	public bool GetRotorActivity()
	{
		if (rotorType == RotorType.Machine && !systemHasSufficientEnergy)
			return false;
		return rotorIsEnabled;
	}

	/// <summary>
	/// Get a list of all the items that this rotor produces
	/// </summary>
	/// <returns></returns>
	public string GetRotorProductionItems()
	{
		string a = "";
		foreach (SO_Property property in rotorProductionProperties)
		{
			if (property.propertyType == PropertyManager.PropertyType.Resource)
				a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_prop_resource_" + (property.callbackID.ToLower())) + ", ";
		}

		return a.Contains(",") ? a.Remove(a.LastIndexOf(',')) : a;
	}

	public SO_Property GetRotorStatePropType(string callbackID)
	{
		return roomTile.roomContainer.globalRefManager.propertyManager.GetProperty(PropertyManager.PropertyType.MachineState, callbackID);
	}

	public void ToggleRotorState(UnityEngine.UI.Toggle toggle)
	{
		rotorIsEnabled = toggle.isOn;
		SetRotorStateFromValues();
	}

	public void SetRotorStateFromValues()
	{
		if (!rotorIsEnabled)
		{
			rotorStateProperty = GetRotorStatePropType("off");
			return;
		}
		rotorStateProperty = GetRotorStatePropType((rotorType!=RotorType.Machine || systemHasSufficientEnergy)?"working":"broken");
	}
	public List<SO_Property> GetRotorProperties()
	{
		List<SO_Property> toRet = new List<SO_Property>();
		toRet.Add(rotorStateProperty);
		toRet.AddRange(rotorProductionProperties);
		return toRet;
	}
}
