using UnityEngine;
using System.Collections.Generic;

public class LanguangeManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public List<TextAsset> textAssetLanguageOptions;
	public List<LanguageOption> allLanguageOptions;
	public LanguageOption currentLanguage;
	public char textFormattingDelimiter;

	private void Awake()
	{
		InitializeLanguage();
		SetLanguage("en_us");
	}

	public void InitializeLanguage()
	{
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
	public string GetTranslation(string callbackID)
	{
		int index = currentLanguage.callbackIDs.IndexOf(callbackID.ToLower());
		if(index >= 0)
			return currentLanguage.texts[index];
		return "'" + callbackID + "' "+GetTranslation("no_translation_found")+" "+currentLanguage.langNativeName;
	}
}
public class LanguageOption
{
	public string langID, langNativeName;
	public List<string> callbackIDs, texts;
}