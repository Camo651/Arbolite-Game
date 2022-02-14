using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public GameObject speakerPrefab;
	public SO_AudioType defaultAudioClip;
	public bool musicIsPlaying;
	public int lastSongIndex;
	public float secondsBetweenMusic;
	public List<AudioSource> emptySpeakers, usedSpeakers;
	public Dictionary<string, SO_AudioType> assetClipLookup;
	public List<string> backgroundMusics;

	private void Awake()
	{
		SO_AudioType[] allAssetClips = Resources.FindObjectsOfTypeAll<SO_AudioType>();
		assetClipLookup = new Dictionary<string, SO_AudioType>();
		backgroundMusics = new List<string>();
		foreach (SO_AudioType item in allAssetClips)
		{
			assetClipLookup.Add(item.clipCallbackID, item);
			if (item.clipType == AudioClipType.Music)
				backgroundMusics.Add(item.clipCallbackID);
		}

		PlayMusic();
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
		if (clip.clipType == AudioClipType.Unused || clip.volumeModifier == 0f)
		{ //guard clause to prevent using unused clips
			return;
		}

		if (emptySpeakers.Count == 0)
		{
			emptySpeakers.Add(Instantiate(speakerPrefab, transform).GetComponent<AudioSource>());
		}
		int clipIndex = Random.Range(0, clip.audioClips.Length);
		emptySpeakers[0].clip = clip.audioClips[clipIndex];
		switch (clip.clipType)
		{
			case AudioClipType.Music: emptySpeakers[0].volume = clip.volumeModifier * globalRefManager.settingsManager.musicVolume; break;
			case AudioClipType.Ambient: emptySpeakers[0].volume = clip.volumeModifier * globalRefManager.settingsManager.ambientVolume; break;
			case AudioClipType.Interface: emptySpeakers[0].volume = clip.volumeModifier * globalRefManager.settingsManager.interfaceVolume; break;
		}
		StartCoroutine(ClipTimer(clip.audioClips[clipIndex].length, emptySpeakers[0], clip));
	}

	/// <summary>
	/// Waits till the clip is done playing, then stops it
	/// </summary>
	/// <param name="time"></param>
	/// <param name="speaker"></param>
	/// <returns></returns>
	private IEnumerator ClipTimer(float time, AudioSource speaker, SO_AudioType type)
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

		if (type.clipType == AudioClipType.Music)
		{
			musicIsPlaying = false;
			yield return new WaitForSeconds(secondsBetweenMusic);
			PlayMusic();
		}
	}

	/// <summary>
	/// Get the clip
	/// </summary>
	/// <param name="callbackID">The callback ID for the clip</param>
	/// <returns>The clip if it exists</returns>
	public SO_AudioType GetClip(string callbackID)
	{
		if (assetClipLookup.ContainsKey(callbackID))
			return assetClipLookup[callbackID];
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
	/// keeps the music going
	/// </summary>
	public void PlayMusic()
	{
		musicIsPlaying = true;
		int songIndex = Random.Range(0, backgroundMusics.Count);
		lastSongIndex = songIndex;
		Play(backgroundMusics[songIndex]);
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
