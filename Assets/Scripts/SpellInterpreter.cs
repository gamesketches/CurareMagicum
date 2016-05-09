using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public enum Element {fire, water, wind, earth, metal, wood};

public class SpellInterpreter : MonoBehaviour {

	Element firstSlot, secondSlot, thirdSlot;
	string[] spellNames;
	public string[] cureRatings;
	string[] symptoms;
	string[] areaCures;
	string[] levels;
	public int level = -1;
	public int maxLevel;

	// Use this for initialization
	void Start () {
		StreamReader sr = File.OpenText("Assets/Resources/spellNames.csv");
		spellNames = sr.ReadToEnd().Split('\n').ToArray();
		sr.Close();

		sr = File.OpenText("Assets/Resources/symptomChart.csv");
		symptoms = sr.ReadToEnd().Split('\n');
		sr.Close();

		sr = File.OpenText("Assets/Resources/cureRatings.csv");
		cureRatings = sr.ReadToEnd().Split('\n');
		sr.Close();

		sr = File.OpenText("Assets/Resources/areaCures.csv");
		areaCures = sr.ReadToEnd().Split('\n');
		sr.Close();

		sr = File.OpenText("Assets/Resources/levels.csv");
		levels = sr.ReadToEnd().Split('\n');
		sr.Close();
		maxLevel = levels.Length - 1;
	}

	public void genNewAnswer() {
		firstSlot = (Element)Random.Range(0,6);
		secondSlot = (Element)Random.Range(0,6);
		thirdSlot = (Element)Random.Range(0,6);
		Debug.Log(string.Concat("New answer= ", firstSlot.ToString(), secondSlot.ToString(), thirdSlot.ToString()));
	}

	public void nextLevel() {
		if(level < maxLevel) {
			level++;
			firstSlot = Parse(levels[level].Split(',')[0]);
			secondSlot = Parse(levels[level].Split(',')[1]);
			thirdSlot = Parse(levels[level].Split(',')[2]);
		}else{
			level++;
			genNewAnswer();
		}
	}

	public string getSymptoms() {
		return string.Concat(symptoms[(int)firstSlot].Split(',')[0],
			symptoms[(int)secondSlot].Split(',')[1],
							symptoms[(int)thirdSlot].Split(',')[2]);
	}

	public string getSpellName(string firstElement, string secondElement, string thirdElement) {
		Element first = Parse(firstElement);
		Element second = Parse(secondElement);
		Element third = Parse(thirdElement);
		string coreSpell = string.Concat(spellNames[(int)first].Split(',')[(int)second],
			areaCures[(int)third].Split(',')[0]);
		if(firstElement == secondElement && thirdElement == secondElement) { 
			coreSpell = string.Concat(spellNames[(int)first].Split(',')[6], coreSpell);
		}

		return coreSpell;
	}

	public string getCurrentSlotsChain(){
		string currentSlots = firstSlot.ToString() + "+" + secondSlot.ToString() + "+" + thirdSlot.ToString();
		return currentSlots;
	}

	public int checkAnswer(string firstElement, string secondElement, string thirdElement) {
		Element first = Parse(firstElement);
		Element second = Parse(secondElement);
		Element third = Parse(thirdElement);

		int score = 0;

		if(first == firstSlot) {
			score += 2;
		}
		else if( symptoms[(int)first].Split(',')[0] == symptoms[(int)firstSlot].Split(',')[0]) {
			score += 1;
		}

		if(second == secondSlot) {
			score += 1;
		}

		if(third == thirdSlot || areaCures[(int)thirdSlot].Split(',')[2] == areaCures[(int)third].Split(',')[1]) {
			score += 4;
		}
		return score;
	}

	public string getDiseaseSolved() {
		return string.Concat(getSeverity(), symptoms[(int)firstSlot].Split(',')[0],
			symptoms[(int)secondSlot].Split(',')[1], symptoms[(int)thirdSlot].Split(',')[2]);
	}

	public string getSeverity() {
		string[] curSpell = new string[3] {firstSlot.ToString(), secondSlot.ToString(), thirdSlot.ToString()};
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

	Element Parse(string elementName) {
		int numElements = System.Enum.GetValues(typeof(Element)).Length;
			for(Element elementType = Element.fire; (int)elementType < numElements; elementType++){
				if(elementType.ToString() == elementName) {
					return elementType;
				}
			}
			Debug.LogError("Invalid type sent");
		return Element.fire;
		}
}
