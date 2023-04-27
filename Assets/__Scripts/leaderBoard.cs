using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class leaderBoard : MonoBehaviour {

	public string path = "Assets/Resources/scores.txt";
	public List<Text> Names;
	public List<Text> Score;
	public Dictionary<string, int> data = new Dictionary<string, int>();
	public Button submit;
	public GameObject UsernameInput;
	public MissionDemolition game;

	// Use this for initialization
	void Start () {
		Button btn = submit.GetComponent<Button>();
		btn.onClick.AddListener(submitfunction);

		update_table(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void update_scores(int score, string name)
    {
		data.Add(name, score);
		StreamWriter writer = new StreamWriter(path);
		data.OrderByDescending(pair => pair.Value);

		foreach (var item in data)
		{
			writer.WriteLine(item.Key + " " + item.Value.ToString());
        }

		writer.Close();
		update_table();
    }

	public void submitfunction()
    {
		Debug.Log(game.Totalscore);
		update_scores(game.Totalscore, UsernameInput.GetComponent<InputField>().text);
    }

	public void update_table(bool read = false)
    {
		if (read)
        {
			StreamReader reader = new StreamReader(path);
			for (int i = 0; i < Names.Count; i++)
            {
				string text = reader.ReadLine();
				string[] name = text.Split(' ');
				data.Add(name[0], int.Parse(name[1]));
            }

			reader.Close();
        }

		var new_data = data.OrderByDescending(pair => pair.Value);
		int j = 0;
		foreach (var item in new_data)
        {
			if (j < 7)
            {
				Names[j].text = item.Key;
				Score[j].text = item.Value.ToString();
				j++;
            }
        }
    }
}
