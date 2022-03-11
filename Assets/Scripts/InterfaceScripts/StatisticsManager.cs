using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the tracking of statistics
/// </summary>
public class StatisticsManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public Dictionary<string, StatTrack> statTrackLookup;
	public List<RectTransform> barGraph;
	public StatTrack selectedStat;
	public TMPro.TextMeshProUGUI hoveredStatDisplay;
	public Color barGraphbaseColour, barGraphHoverColour;
	private GameObject hoveredBarGraph;
	public Dictionary<string, int> timesRoomsPlaced = new Dictionary<string, int>();
	public Dictionary<string, int> timesRoomsDestoyed = new Dictionary<string, int>();
	public List<DataNode> advancements;

	private void Awake()
	{
		advancements = new List<DataNode>(FindObjectsOfType<DataNode>());
	}
	private void Start()
	{
		GetStat("sessions_played").AddStatValue(1);
	}
	public void CheckAdvancementLeaves()
	{
		for (int i = 0; i < advancements.Count;i++)
		{
			if (advancements[i].nodeState == DataNode.NodeState.Unlocked && advancements[i].CheckConditionsMet())
			{
				advancements[i].nodeState = DataNode.NodeState.Obtained;
				globalRefManager.interfaceManager.EnqueueNotification("advancement_unlocked", "advancement_unlocked", new string[] { advancements[i].GetNodeName() });
				foreach (DataNode node in advancements[i].childedDataNodes)
				{
					if (node.nodeState == DataNode.NodeState.Locked)
						node.nodeState = DataNode.NodeState.Unlocked;
				}
			}
		}
	}

	#region Times Placed
	public int IncrementTimesPlaced(ContainedRoom room, int amount)
	{
		if (!room)
			return 0;
		return IncrementTimesPlaced(room.tileNameInfoID, amount);
	}
	public int IncrementTimesPlaced(string callbackID, int amount)
	{
		if (!timesRoomsPlaced.ContainsKey(callbackID))
			timesRoomsPlaced.Add(callbackID, 0);
		timesRoomsPlaced[callbackID] += amount;
		return timesRoomsPlaced[callbackID];
	}

	public int GetTimesPlaced(string callbackID)
	{
		if (timesRoomsPlaced.ContainsKey(callbackID))
			return timesRoomsPlaced[callbackID];
		return 0;
	}
	public int GetTimesPlaced(ContainedRoom room)
	{
		if (!room)
			return 0;
		return GetTimesPlaced(room.tileNameInfoID);
	}
	#endregion
	#region Times Destroyed
	public int IncrementTimesDestroyed(ContainedRoom room, int amount)
	{
		if (!room)
			return 0;
		return IncrementTimesDestroyed(room.tileNameInfoID, amount);
	}
	public int IncrementTimesDestroyed(string callbackID, int amount)
	{
		if (!timesRoomsDestoyed.ContainsKey(callbackID))
			timesRoomsDestoyed.Add(callbackID, 0);
		timesRoomsDestoyed[callbackID] += amount;
		return timesRoomsDestoyed[callbackID];
	}

	public int GetTimesDestroyed(string callbackID)
	{
		if (timesRoomsDestoyed.ContainsKey(callbackID))
			return timesRoomsDestoyed[callbackID];
		return 0;
	}
	public int GetTimesDestroyed(ContainedRoom room)
	{
		if (!room)
			return 0;
		return GetTimesDestroyed(room.tileNameInfoID);
	}
	#endregion
	public void SetBarGraphInit()
	{
		SetGraph("days_played");
		CloseHoverStatDisplay();
	}

	/// <summary>
	/// Logs a new entry to the item histories for each stat
	/// </summary>
	public void UpdateStats()
	{
		//Need to manually add the buttons in for the list
		//AND the translations list
		GetStat("tiles_placed").IncrementHistory();
		GetStat("tiles_destroyed").IncrementHistory();
		GetStat("days_played").IncrementHistory();
		GetStat("sessions_played").IncrementHistory();
		GetStat("plants_harvested").IncrementHistory();
		GetStat("advancements_unlocked").IncrementHistory();
	}

	/// <summary>
	/// Sets the stat value display text to nothing
	/// </summary>
	public void CloseHoverStatDisplay()
	{
		hoveredStatDisplay.text = selectedStat.GetStatName() + " : ";
		if(hoveredBarGraph)
			hoveredBarGraph.GetComponent<UnityEngine.UI.Image>().color = barGraphbaseColour;
		hoveredBarGraph = null;
	}

	/// <summary>
	/// Sets the stat value display text to the value of the entry at the sibling index of i
	/// </summary>
	/// <param name="i">The bar graph bar game object</param>
	public void SetHoverStatDisplay(GameObject i)
	{
		if (selectedStat == null)
			return;
		int index = i.transform.GetSiblingIndex();
		if (i && index < selectedStat.history.Count)
		{
			hoveredStatDisplay.text = selectedStat.GetStatName() +" : "+selectedStat.history[index];
			i.GetComponent<UnityEngine.UI.Image>().color = barGraphHoverColour;
			hoveredBarGraph = i;
		}
	}

	/// <summary>
	/// Sets the bars of the bar graph to show the history of stat
	/// </summary>
	/// <param name="stat">The stat tracker to display the values of</param>
	public void SetBarGraphStat(StatTrack stat)
	{
		selectedStat = stat;
		if (stat == null)
			return;
		float relMax = stat.value;
		foreach (float f in stat.history)
		{
			if (f > relMax)
				relMax = f;
		}
		for (int i = 0; i < barGraph.Count; i++)
		{
			if(i < stat.history.Count && relMax!=0f)
			{
				barGraph[i].sizeDelta = new Vector2(5f, Mathf.Lerp(0,300,(i==stat.history.Count-1?stat.value:stat.history[i])/relMax));
			}
			else
			{
				barGraph[i].sizeDelta = Vector2.right * 5f;
			}
		}
		hoveredStatDisplay.text = selectedStat.GetStatName() + " : ";
	}

	/// <summary>
	/// Set the bargraph display to highlight the stat tracker from its callback id
	/// </summary>
	/// <param name="callbackID">The callback ID of the stat tracker</param>
	public void SetGraph(string callbackID)
	{
		SetBarGraphStat(GetStat(callbackID));
	}

	/// <summary>
	/// Get a stat tracker from the lookup. Makes a new one if not yet available
	/// </summary>
	/// <param name="callbackID">The stat tracker callback ID</param>
	/// <returns>The stat tracker</returns>
	public StatTrack GetStat(string callbackID)
	{
		if (statTrackLookup == null)
			statTrackLookup = new Dictionary<string, StatTrack>();
		if (!statTrackLookup.ContainsKey(callbackID))
			statTrackLookup.Add(callbackID, new StatTrack(this, callbackID, 0f));
		return statTrackLookup[callbackID];
	}

}

/// <summary>
/// A non-mono class to hold the stats as they get tracked
/// </summary>
public class StatTrack
{
	public StatisticsManager statMan;
	public string statCallbackID;
	public float value;
	public List<float> history;
	public GameObject statBar;

	/// <summary>
	/// Constructs the statistic
	/// </summary>
	/// <param name="s">The singleton stat manager</param>
	/// <param name="callback">The unique callbackID for the stat</param>
	/// <param name="initVal">The initial value of the stat</param>
	public StatTrack(StatisticsManager s, string callback, float initVal)
	{
		statMan = s;
		statCallbackID = callback;
		value = initVal;
		history = new List<float>();
	}

	/// <summary>
	/// Get the translated name of the stat
	/// </summary>
	/// <returns></returns>
	public string GetStatName()
	{
		return statMan.globalRefManager.langManager.GetTranslation("stat_" + statCallbackID);
	}

	/// <summary>
	/// Sets the current value of the stat tracker, overwritting it completely
	/// </summary>
	/// <param name="v">The new value of the stat</param>
	public void SetStatValue(float v)
	{
		value = v;
		if (history.Count == 0)
			history.Add(value);
		history[history.Count - 1] = value;

		statMan.StartCoroutine(StatCheckDelay());
	}

	/// <summary>
	/// Increments the value by v
	/// </summary>
	/// <param name="v">The value to be added to the stat</param>
	public void AddStatValue(float v)
	{
		SetStatValue(value + v);
	}

	/// <summary>
	/// Logs a new entry to the stat trackers history and deletes the overflow
	/// </summary>
	public void IncrementHistory()
	{
		history.Add(value);
		if(history.Count > statMan.barGraph.Count)
		{
			history.RemoveAt(0);
		}
	}

	private IEnumerator StatCheckDelay()
	{
		yield return new WaitForEndOfFrame();
		statMan.CheckAdvancementLeaves();
	}
}
