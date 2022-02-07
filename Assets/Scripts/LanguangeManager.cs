using UnityEngine;
using System.Collections.Generic;

public class LanguangeManager : MonoBehaviour
{
	//controls all the text translations in the game
	public GlobalRefManager globalRefManager;
	public List<TextAsset> textAssetLanguageOptions;
	public List<LanguageOption> allLanguageOptions;
	public List<TranslationKey> allTranslationKeys;
	public LanguageOption currentLanguage;
	public char textFormattingDelimiter;

	public static string allowedAsciiChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_,.;:<>?/'+=`~!@#$%^&*()[]{}|";

	private void Awake()
	{
		//default the language to english for now
		InitializeLanguage();
		SetLanguage("en_us");
	}

	public void InitializeLanguage()
	{

		//index all the translation keys (UGUIs)
		allTranslationKeys = new List<TranslationKey>();
		allTranslationKeys.AddRange(Resources.FindObjectsOfTypeAll<TranslationKey>());

		// initialize and index all the possible translations into an indexer
		allLanguageOptions = new List<LanguageOption>();
		foreach (TextAsset textAsset in textAssetLanguageOptions)
		{
			LanguageOption langOpt = new LanguageOption();
			langOpt.texts = new List<string>();
			langOpt.callbackIDs = new List<string>();
			string text = textAsset.text;
			string[] parse = text.Split(textFormattingDelimiter);
			langOpt.langID = CleanString(parse[0].Replace("\n", "").Replace(" ", ""));
			langOpt.langNativeName = parse[1].Replace("\n", "").Replace(System.Environment.NewLine, "");
			for (int i = 2; i < parse.Length; i++)
			{
				parse[i] = parse[i].Replace("\n", "").Replace(" ", i % 2 == 0 ? "" : " ");
				if (parse[i] != "")
				{
					if (i % 2 == 0)
						langOpt.callbackIDs.Add(CleanString(parse[i].Replace("\n", "").Replace(" ", "").
							ToLower()));
					else
						langOpt.texts.Add(parse[i]);
				}
			}
			allLanguageOptions.Add(langOpt);
		}
	}

	//sets the current language to the langID and (eventually) sets all the text in the game to that language
	public void SetLanguage(string langID)
	{
		foreach (LanguageOption lang in allLanguageOptions)
		{
			if (lang.langID == langID)
			{
				currentLanguage = lang;
			}
		}
	}

	//returns the text in the current language based on the id of the text box
	public string GetTranslation(string callbackID)
	{
		int index = currentLanguage.callbackIDs.IndexOf(callbackID.ToLower());
		if(index >= 0)
			return currentLanguage.texts[index];
		return "ERROR: '" + callbackID + "' FOR "+currentLanguage.langID +" NOT FOUND";
	}


	//returns a string with only the characters that are allowed ascii. Don't use this for translated text yet, it wont accept anything but en-us
	public string CleanString(string a)
	{
		string b = "";
		foreach (char c in a)
			if (allowedAsciiChars.Contains(c+""))
				b += c;
		return b;
	}
}

//a seperate class to index all the language options
public class LanguageOption
{
	public string langID, langNativeName;
	public List<string> callbackIDs, texts;
}