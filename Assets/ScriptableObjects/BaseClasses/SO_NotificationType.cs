using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_NotificationType : ScriptableObject
{
	public string callbackID;
	public string notificationName;
	public string notificationDescription;
	public bool shouldBeSaved;
	public Sprite notificationIcon;
}
