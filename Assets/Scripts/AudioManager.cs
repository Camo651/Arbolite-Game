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
	public AudioSource backgroundMusicSource;
	public Dictionary<AudioClipType, Dictionary<string, SO_AudioType>> assetClipLookup;

	private void Start()
	{
		SO_AudioType[] allAssetClips = Resources.LoadAll<SO_AudioType>("");
		assetClipLookup = new Dictionary<AudioClipType, Dictionary<string, SO_AudioType>>();
		foreach (SO_AudioType item in allAssetClips)
		{
			if (!assetClipLookup.ContainsKey(item.clipType))
			{
				assetClipLookup.Add(item.clipType, new Dictionary<string, SO_AudioType>());
			}
			assetClipLookup[item.clipType].Add(item.clipCallbackID, item);
		}

		PlayMusic();
	}
	/// <summary>
	/// play an audio clip given its name
	/// </summary>
	/// <param name="callbackID"></param>
	public void Play(AudioClipType type, string callbackID)
	{
		Play(GetClip(type, callbackID));
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
		AudioSource s = emptySpeakers[0];
		s.clip = clip.audioClips[clipIndex];
		switch (clip.clipType)
		{
			case AudioClipType.BackgroundMusic: s.volume = clip.volumeModifier * globalRefManager.settingsManager.musicVolume; break;
			case AudioClipType.Ambient: s.volume = clip.volumeModifier * globalRefManager.settingsManager.ambientVolume; break;
			case AudioClipType.Interface: s.volume = clip.volumeModifier * globalRefManager.settingsManager.interfaceVolume; break;
		}
		s.Play();
		usedSpeakers.Add(s);
		emptySpeakers.Remove(s);
		StartCoroutine(ClipTimer(clip.audioClips[clipIndex].length, s, clip));
	}

	/// <summary>
	/// Waits till the clip is done playing, then stops it
	/// </summary>
	/// <param name="time"></param>
	/// <param name="speaker"></param>
	/// <returns></returns>
	private IEnumerator ClipTimer(float time, AudioSource speaker, SO_AudioType type)
	{
		yield return new WaitForSeconds(time);
		speaker.Stop();
		if (type.clipType == AudioClipType.BackgroundMusic)
		{
			musicIsPlaying = false;
			yield return new WaitForSeconds(secondsBetweenMusic);
			PlayMusic();
		}
		else
		{
			usedSpeakers.Remove(speaker);
			emptySpeakers.Add(speaker);
			speaker.clip = null;
		}
	}

	/// <summary>
	/// Get the clip
	/// </summary>
	/// <param name="callbackID">The callback ID for the clip</param>
	/// <returns>The clip if it exists</returns>
	public SO_AudioType GetClip(AudioClipType type, string callbackID)
	{
		if (assetClipLookup.ContainsKey(type) && assetClipLookup[type].ContainsKey(callbackID))
			return assetClipLookup[type][callbackID];
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
		List<SO_AudioType> backgroundMusic = new List<SO_AudioType>(assetClipLookup[AudioClipType.BackgroundMusic].Values);
		int songIndex = Random.Range(0, backgroundMusic.Count);
		if (backgroundMusic.Count > 0)
		{
			if(backgroundMusicSource == null)
			{
				backgroundMusicSource = Instantiate(speakerPrefab, transform).GetComponent<AudioSource>();
				backgroundMusicSource.gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency = 7000;
			}
			backgroundMusicSource.clip = backgroundMusic[songIndex].audioClips[Random.Range(0, backgroundMusic[songIndex].audioClips.Length)];
			backgroundMusicSource.volume = backgroundMusic[songIndex].volumeModifier  * globalRefManager.settingsManager.musicVolume;
			backgroundMusicSource.Play();
			StartCoroutine(ClipTimer(backgroundMusicSource.clip.length, backgroundMusicSource, backgroundMusic[songIndex]));
		}
	}

	/// <summary>
	/// Toggles the low-pass filter on the back ground music clip
	/// </summary>
	/// <param name="state"></param>
	public void SetBackgroundMusicLowPass(bool state)
	{
		if (backgroundMusicSource)
			StartCoroutine(FadeLowPass(state));
	}

	/// <summary>
	/// Fades the low pass filter in and out
	/// </summary>
	/// <param name="state">The targeted state of the low pass filter</param>
	/// <returns>Nothing</returns>
	IEnumerator FadeLowPass(bool state)
	{
		float current = backgroundMusicSource.GetComponent<AudioLowPassFilter>().cutoffFrequency;
		float target = state ? 700 : 7000;
		float time = 100;
		for (float i = time/20f; i < time; i++)
		{
			yield return null;
			backgroundMusicSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = Mathf.Lerp(current, target, Mathf.Clamp01(i / time));
		}
		backgroundMusicSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = target;
	}

	/// <summary>
	/// The various types of audio clips that can be used
	/// </summary>
	public enum AudioClipType
	{
		Unused,
		BackgroundMusic,
		Interface,
		Ambient
	}

}
