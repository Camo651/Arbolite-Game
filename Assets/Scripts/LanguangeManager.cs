using UnityEngine;
using System.Collections.Generic;

public class LanguangeManager : MonoBehaviour
{
	//controls all the text translations in the game
	public GlobalRefManager globalRefManager;
	public List<TextAsset> textAssetLanguageOptions;
	public List<LanguageOption> allLanguageOptions;
	public LanguageOption currentLanguage;
	public char textFormattingDelimiter;

	private void Awake()
	{
		//default the language to english for now
		InitializeLanguage();
		SetLanguage("en_us");
	}

	public void InitializeLanguage()
	{
		// initialize and index all the possible translations into an indexer
		allLanguageOptions = new List<LanguageOption>();
		foreach (TextAsset textAsset in textAssetLanguageOptions)
		{
			LanguageOption langOpt = new LanguageOption();
			langOpt.texts = new List<string>();
			langOpt.callbackIDs = new List<string>();
			string text = textAsset.text;
			string[] parse = text.Split(textFormattingDelimiter);
			langOpt.langID = parse[0].Replace("\n", "").Replace(" ", "");
			langOpt.langNativeName = parse[1].Replace("\n", "");
			for (int i = 2; i < parse.Length; i++)
			{
				parse[i] = parse[i].Replace("\n", "").Replace(" ", i % 2 == 0 ? "" : " ");
				if (parse[i] != "")
				{
					if (i % 2 == 0)
						langOpt.callbackIDs.Add(parse[i].ToLower());
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
		return "'" + callbackID + "' "+GetTranslation("no_translation_found")+" "+currentLanguage.langNativeName;
	}
}

//a seperate class to index all the language options
public class LanguageOption
{
	public string langID, langNativeName;
	public List<string> callbackIDs, texts;
}