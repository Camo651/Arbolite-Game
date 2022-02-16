using UnityEngine;
using System.Text.RegularExpressions;
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

	private void Start()
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
			langOpt.lookup = new Dictionary<string, string>();
			string text = textAsset.text;
			text = Regex.Replace(text, "(<.*>)", "");
			string[] parse = text.Split(textFormattingDelimiter);
			langOpt.langID = CleanString(parse[0].Replace("\n", "").Replace(" ", ""));
			langOpt.langNativeName = parse[1].Replace("\n", "").Replace(System.Environment.NewLine, "");
			for (int i = 2; i < parse.Length-1; i++)
			{
				string key = CleanString(parse[i].Replace("\n", "").Replace(" ", ""));
				i++;
				string val = parse[i].Replace("\n", "");

				//check to see if this line is a comment
				if (!key.StartsWith("<"))
				{
					//choose to add new key or mod old
					if (!langOpt.lookup.ContainsKey(key))
					{
						langOpt.lookup.Add(key, val);
					}
					else
					{
						langOpt.lookup[key] = val;
					}
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
		//guard clause for empty IDs
		if (callbackID == "" || callbackID == " ")
		{
			return "";
		}

		if (currentLanguage.lookup.ContainsKey(callbackID))
			return currentLanguage.lookup[callbackID];
		return callbackID + "' not found for " + currentLanguage.langID;
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
	public Dictionary<string, string> lookup;
}