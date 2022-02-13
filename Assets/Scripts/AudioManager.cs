using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public GameObject speakerPrefab;
	public SO_AudioType defaultAudioClip;
	public List<AudioSource> emptySpeakers, usedSpeakers;
	public List<SO_AudioType> allAssetClips;
	public List<string> allAssetClipsCallbacks;

	private void Awake()
	{
		allAssetClips.AddRange(Resources.FindObjectsOfTypeAll<SO_AudioType>());
		foreach (SO_AudioType item in allAssetClips)
		{
			allAssetClipsCallbacks.Add(item.clipCallbackID);
		}
	}
	/// <summary>
	/// play an audio clip given its name
	/// </summary>
	/// <param name="callbackID"></param>
	public void Play(string callbackID)
	{
		Play(GetClip(callbackID));
	}

	/// <summary>
	/// Play an audio clip given the clip
	/// </summary>
	/// <param name="clip"></param>
	public void Play(SO_AudioType clip)
	{
		if(clip.clipType == AudioClipType.Unused || clip.volumeModifier == 0f)
		{ //guard clause to prevent using unused clips
			return;
		}

		if(emptySpeakers.Count == 0)
		{
			emptySpeakers.Add(Instantiate(speakerPrefab, transform).GetComponent<AudioSource>());
		}
		emptySpeakers[0].clip = clip.audioClip;
		switch (clip.clipType)
		{
			case AudioClipType.Music: emptySpeakers[0].volume = clip.volumeModifier * globalRefManager.settingsManager.musicVolume;break;
			case AudioClipType.Ambient: emptySpeakers[0].volume = clip.volumeModifier * globalRefManager.settingsManager.ambientVolume;break;
			case AudioClipType.Interface: emptySpeakers[0].volume = clip.volumeModifier * globalRefManager.settingsManager.interfaceVolume;break;
		}
		StartCoroutine(ClipTimer(clip.audioClip.length, emptySpeakers[0]));
	}

	/// <summary>
	/// Waits till the clip is done playing, then stops it
	/// </summary>
	/// <param name="time"></param>
	/// <param name="speaker"></param>
	/// <returns></returns>
	private IEnumerator ClipTimer(float time, AudioSource speaker)
	{
		usedSpeakers.Add(speaker);
		emptySpeakers.Remove(speaker);
		speaker.Play();
		yield return new WaitForSeconds(time);
		speaker.Stop();
		usedSpeakers.Remove(speaker);
		emptySpeakers.Add(speaker);
		speaker.volume = 0f;
		speaker.clip = null;
	}

	/// <summary>
	/// Get the clip
	/// </summary>
	/// <param name="callbackID">The callback ID for the clip</param>
	/// <returns>The clip if it exists</returns>
	public SO_AudioType GetClip(string callbackID)
	{
		int i = allAssetClipsCallbacks.IndexOf(callbackID);
		if (i >= 0)
			return allAssetClips[i];
		return defaultAudioClip;
	}

	/// <summary>
	/// stops all currently played audio
	/// </summary>
	public void StopAllAudioSources()
	{
		for (int i = 0; i < usedSpeakers.Count; i++)
		{
			emptySpeakers.Add(usedSpeakers[i]);
			usedSpeakers[i].Stop();
			usedSpeakers[i].volume = 0f;
			usedSpeakers[i].clip = null;
			usedSpeakers.RemoveAt(i);
		}
	}

	/// <summary>
	/// The various types of audio clips that can be used
	/// </summary>
	public enum AudioClipType
	{
		Unused,
		Music,
		Interface,
		Ambient
	}
}
