using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class GameManager : MonoBehaviour
{
	public AudioClip goodSpeak;
	public AudioClip normalSpeak;
	public AudioClip badSpeak;
	private AudioSource selectAudio;
	private Dictionary<string, int> dataSet = new Dictionary<string, int>();
	private bool statusStart = false;
	private int i = 1;

	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine(GoogleSheets());
	}

	// Update is called once per frame
	void Update()
	{
		if (i <= dataSet.Count && dataSet["Mon_" + i.ToString()] < 3700 & statusStart == false)
		{
			StartCoroutine(PlaySelectAudioBad());
			Debug.Log(i + " раунд. Не получится закупить основное оружие + броню: $" + dataSet["Mon_" + i.ToString()]);
		}

		if (i <= dataSet.Count && dataSet["Mon_" + i.ToString()] >= 3700 & dataSet["Mon_" + i.ToString()] < 10000 & statusStart == false)
		{
			StartCoroutine(PlaySelectAudioNormal());
			Debug.Log(i + " раунд. Денег на оружие + броню хватает: $" + dataSet["Mon_" + i.ToString()]);
		}

		if (i <= dataSet.Count && dataSet["Mon_" + i.ToString()] >= 10000 & statusStart == false)
		{
			StartCoroutine(PlaySelectAudioGood());
			Debug.Log(i + " раунд. Денег на оружие + броню хватает, можно закупить оружие тиммейтам: $" + dataSet["Mon_" + i.ToString()]);
		}
	}

	IEnumerator GoogleSheets()
	{
		UnityWebRequest curentResp = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/1Vfur8dln_cwEoD_SDpQ1SyEaGDNr7A1TEVNs_UKmULI/values/Лист1?key=AIzaSyAGzTmYQudoS1LnTraXs-aEXuUKuFl7AA4");
		yield return curentResp.SendWebRequest();
		string rawResp = curentResp.downloadHandler.text;
		var rawJson = JSON.Parse(rawResp);
		foreach (var itemRawJson in rawJson["values"])
		{
			var parseJson = JSON.Parse(itemRawJson.ToString());
			var selectRow = parseJson[0].AsStringList;
			dataSet.Add(("Mon_" + selectRow[0]), int.Parse(selectRow[1]));
		}
	}

	IEnumerator PlaySelectAudioGood()
	{
		statusStart = true;
		selectAudio = GetComponent<AudioSource>();
		selectAudio.clip = goodSpeak;
		selectAudio.Play();
		yield return new WaitForSeconds(3);
		statusStart = false;
		i++;
	}
	IEnumerator PlaySelectAudioNormal()
	{
		statusStart = true;
		selectAudio = GetComponent<AudioSource>();
		selectAudio.clip = normalSpeak;
		selectAudio.Play();
		yield return new WaitForSeconds(3);
		statusStart = false;
		i++;
	}
	IEnumerator PlaySelectAudioBad()
	{
		statusStart = true;
		selectAudio = GetComponent<AudioSource>();
		selectAudio.clip = badSpeak;
		selectAudio.Play();
		yield return new WaitForSeconds(4);
		statusStart = false;
		i++;
	}
}
