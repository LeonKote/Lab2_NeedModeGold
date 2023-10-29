# АНАЛИЗ ДАННЫХ И ИСКУССТВЕННЫЙ ИНТЕЛЛЕКТ [in GameDev]
Отчет по лабораторной работе #2 выполнил(а):
- Папушев Роман Олегович
- РИ220947
Отметка о выполнении заданий (заполняется студентом):

| Задание | Выполнение | Баллы |
| ------ | ------ | ------ |
| Задание 1 | * | - |
| Задание 2 | * | - |
| Задание 3 | * | - |

знак "*" - задание выполнено; знак "#" - задание не выполнено;

Работу проверили:
- -

[![N|Solid](https://cldup.com/dTxpPi9lDf.thumb.png)](https://nodesource.com/products/nsolid)

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

Структура отчета

- Данные о работе: название работы, фио, группа, выполненные задания.
- Цель работы.
- Задание 1.
- Код реализации выполнения задания. Визуализация результатов выполнения (если применимо).
- Задание 2.
- Код реализации выполнения задания. Визуализация результатов выполнения (если применимо).
- Задание 3.
- Код реализации выполнения задания. Визуализация результатов выполнения (если применимо).
- Выводы.
- ✨Magic ✨

## Цель работы
Научиться передавать в Unity данные из Google Sheets с помощью Python.

## Задание 1
### Выберите одну из компьютерных игр, приведите скриншот её геймплея и краткое описание концепта игры. Выберите одну из игровых переменных в игре (ресурсы, внутри игровая валюта, здоровье персонажей и т.д.), опишите её роль в игре, условия изменения / появления и диапазон допустимых значений. Постройте схему экономической модели в игре и укажите место выбранного ресурса в ней.

![screenshot](https://i.imgur.com/Ed29RMX.png)

- Я выбрал игру CS:GO. Это многопользовательский тактический шутер. Игроки могут присоединиться к одной из двух команд: террористам или контр-террористам, и сражаться друг с другом в режимах соревновательных матчей. Игроки могут покупать оружие во время раундов. Стратегическое взаимодействие с командой является ключом к успеху.
- Я выбрал переменную внутренней валюты в игре. С помощью неё игроки могут приобретать оружие. Основных источников этой валюты два: вознаграждение за победу и вознаграждение за убийство. Дополнительными источниками валюты служат компенсация за поражение и награда за установку/разминирования бомбы. Валюту можно потратить на покупку оружия, а также заплатить в качестве штрафа (например, за ранение заложника). В соревновательном режиме диапазон значений составляет 0 - 16000.

![scheme](https://i.imgur.com/nKddbLs.png)

## Задание 2
### С помощью скрипта на языке Python заполните google-таблицу данными, описывающими выбранную игровую переменную в выбранной игре (в качестве таких переменных может выступать игровая валюта, ресурсы, здоровье и т.д.). Средствами google-sheets визуализируйте данные в google-таблице (постройте график, диаграмму и пр.) для наглядного представления выбранной игровой величины.

- см. [скрипт](https://github.com/LeonKote/Lab2_NeedModeGold/blob/master/Anaconda/UnityDataScience.ipynb).
- см. [таблицу](https://docs.google.com/spreadsheets/d/1Vfur8dln_cwEoD_SDpQ1SyEaGDNr7A1TEVNs_UKmULI/edit?usp=sharing).

```py

import gspread
import numpy as np
gc = gspread.service_account(filename='unitydatascience-400416-98e6ba2ba08f.json')
sh = gc.open("UnityWorkshop2")
money = 800
for i in range(1, 16):
    # На первом раунде всегда $800
    if i == 1:
        sh.sheet1.update(('A' + str(i)), str(i))
        sh.sheet1.update(('B' + str(i)), str(money))
        print(money)
        continue
    win = np.random.randint(2)
    # За победу получаем $3250, за поражение - $1400
    money += 3250 if win else 1400
    # За каждое убийство можем получить по $300
    money += np.random.randint(6) * 300
    # Ограничение денег - $16000
    money = min(money, 16000)
    sh.sheet1.update(('A' + str(i)), str(i))
    sh.sheet1.update(('B' + str(i)), str(money))
    print(money)
    # Закупаем основное оружие
    if not win and money >= 2700:
        money -= 2700
    # Закупаем бронежилет
    if money >= 1000:
        money -= 1000

```

## Задание 3
### Настройте на сцене Unity воспроизведение звуковых файлов, описывающих динамику изменения выбранной переменной. Например, если выбрано здоровье главного персонажа вы можете выводить сообщения, связанные с его состоянием.

- см. [каталог](https://github.com/LeonKote/Lab2_NeedModeGold/tree/master/UnityDataScience) с проектом.
- см. [скрипт](https://github.com/LeonKote/Lab2_NeedModeGold/blob/master/UnityDataScience/Assets/Script/GameManager.cs).

```cs

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

```

## Выводы

В процессе работы я научился передавать в Unity данные из Google Sheets с помощью Python. Я узнал, как описывать роль выбранной игровой переменной (ресурсы, внутри игровая валюта, здоровье персонажей и т.д.) в игре, условия изменения / появления и диапазон допустимых значений, как построить схему экономической модели в игре и указать место выбранного ресурса в ней, как с помощью скрипта на языке Python заполнить google-таблицу данными, описывающими выбранную игровую переменную в выбранной игре, как средствами google-sheets визуализировать данные в google-таблице (построить график, диаграмму и пр.) для наглядного представления выбранной игровой величины, а также как настроить на сцене Unity воспроизведение звуковых файлов, описывающих динамику изменения выбранной переменной.

| Plugin | README |
| ------ | ------ |
| Dropbox | [plugins/dropbox/README.md][PlDb] |
| GitHub | [plugins/github/README.md][PlGh] |
| Google Drive | [plugins/googledrive/README.md][PlGd] |
| OneDrive | [plugins/onedrive/README.md][PlOd] |
| Medium | [plugins/medium/README.md][PlMe] |
| Google Analytics | [plugins/googleanalytics/README.md][PlGa] |

## Powered by

**BigDigital Team: Denisov | Fadeev | Panov**
