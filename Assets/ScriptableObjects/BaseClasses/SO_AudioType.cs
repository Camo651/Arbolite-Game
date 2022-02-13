using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_AudioType : ScriptableObject
{
	public AudioClip audioClip;
	[Range(0f,3f)]public float volumeModifier;
	public string clipCallbackID;
	public AudioManager.AudioClipType clipType;
}
