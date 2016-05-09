using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class SpellInterpreter : MonoBehaviour {

	string firstSlot, secondSlot, thirdSlot;
	string[] spellNames;
	string[] cureRatings;
	string[] symptoms;
	string[] areaCures;
	string[] levels;
	string[] spells = new string[6] {"fire", "water", "wind", "earth", "metal", "wood"};
	public int level = 0;
	private int maxLevel;

	// Use this for initialization
	void Start () {
		StreamReader sr = File.OpenText("Assets/Resources/spellNames.csv");
		spellNames = sr.ReadToEnd().Split('\n').ToArray();
		sr.Close();

		sr = File.OpenText("Assets/Resources/symptomChart");
		symptoms = sr.ReadToEnd().Split('\n');
		sr.Close();

		sr = File.OpenText("Assets/Resources/cureRatings");
		cureRatings = sr.ReadToEnd().Split('\n');
		sr.Close();

		sr = File.OpenText("Assets/Resources/areaCures");
		areaCures = sr.ReadToEnd().Split('\n');
		sr.Close();

		sr = File.OpenText("Assets/Resources/levels");
		levels = sr.ReadToEnd().Split('\n');
		sr.Close();

		maxLevel = levels.Length - 1;
	}

	void genNewAnswer() {
		firstSlot = spells[Random.Range(0, 6)];
		secondSlot = spells[Random.Range(0, 6)];
		thirdSlot = spells[Random.Range(0, 6)];
	}

	void nextLevel() {
		if(level < maxLevel) {
			level++;
			firstSlot = levels[level].Split(',')[0];
			secondSlot = levels[level].Split(',')[1];
			thirdSlot = levels[level].Split(',')[2];
		}
	}

	string getSymptoms() {
		return string.Concat(symptoms[getSpellIndex(firstSlot)].Split(',')[0],
							symptoms[getSpellIndex(secondSlot)].Split(',')[1],
							symptoms[getSpellIndex(thirdSlot)].Split(',')[2]);
	}

	string getSpellName(string firstElement, string secondElement, string thirdElement) {
		return firstElement == secondElement && thirdElement == secondElement ? 
			string.Concat(spellNames[getSpellIndex(firstSlot)].Split(',')[6], spellNames[getSpellIndex(firstSlot)].Split(',')[getSpellIndex(secondSlot)],
			areaCures[getSpellIndex(thirdSlot)].Split(',')[0]) : string.Concat(spellNames[getSpellIndex(firstSlot)].Split(',')[getSpellIndex(secondSlot)],
													areaCures[getSpellIndex(thirdSlot)].Split(',')[0]);
	}

	int checkAnswer(string firstElement, string secondElement, string thirdElement) {
		int score = 0;

		if(firstElement == firstSlot) {
			score += 2;
		}
		else if( symptoms[getSpellIndex(firstElement)].Split(',')[0] == symptoms[getSpellIndex(firstSlot)].Split(',')[0]) {
			score += 1;
		}

		if(secondElement == secondSlot) {
			score += 1;
		}

		if(thirdElement == thirdSlot || areaCures[getSpellIndex(thirdSlot)].Split(',')[2] == areaCures[getSpellIndex(thirdElement)].Split(',')[1]) {
			score += 4;
		}
		return score;
	}

	string getDiseaseSolved() {
		return string.Concat(getSeverity(), symptoms[getSpellIndex(firstSlot)].Split(',')[0],
			symptoms[getSpellIndex(secondSlot)].Split(',')[1], symptoms[getSpellIndex(thirdSlot)].Split(',')[2]);
	}

	string getSeverity() {
		string[] curSpell = new string[3] {firstSlot, secondSlot, thirdSlot};
		if(levels.ToArray().Distinct().Count() == 1) {
			return "Lethal";
		}
		else if(levels.ToArray().Distinct().Count() == 3) {
			return "Mild";
		}
		else if(curSpell[1] == curSpell[2]) {
			return "Moderate";
		}
		else if(curSpell[0] == curSpell[1]) {
			return "Severe";
		}
		else if(curSpell[0] == curSpell[2]) {
			return "Acute";
		}

		return "";
	}

	int getSpellIndex(string elementName) {
		for(int i = 0; i < spells.Length; i++) {
			if(elementName == spells[i]) {
				return i;
			}
		}
		Debug.LogError("illegal element " + elementName + " found");
		return -1;
	}
}
