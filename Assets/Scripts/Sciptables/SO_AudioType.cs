using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_AudioType : ScriptableObject
{
	public AudioClip[] audioClips;
	[Range(0f,3f)]public float volumeModifier;
	[Range(0f, .3f)] public float pitchVariance;
	public string clipCallbackID;
	public AudioManager.AudioClipType clipType;
}
