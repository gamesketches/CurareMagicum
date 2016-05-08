using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class SpellInterpreter : MonoBehaviour {

	string[] spellNames;
	string[] cureRatings;

	// Use this for initialization
	void Start () {
		StreamReader sr = File.OpenText("Assets/Resources/spellNames.csv");
		spellNames = sr.ReadToEnd().Split('\n').ToArray();

		Debug.Log(spellNames[2].Split(',')[3]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
