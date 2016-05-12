using UnityEngine;
using System.Collections;
using Vuforia;
using UnityEngine.UI;
using System.Threading;

public class GameManager : MonoBehaviour {
	public enum GameState{
		menu,
		level1,
		level2,
		level3
	}

	public VuforiaBehaviour ARCamera;
	public Text feedbackText;
	public GameObject magicCircle;
	public UnityEngine.UI.Image magicImage0;
	public UnityEngine.UI.Image magicImage1;
	public UnityEngine.UI.Image magicImage2;
	public UnityEngine.UI.Image progressLine;

	public Sprite fire;
	public Sprite water;
	public Sprite wood;
	public Sprite wind;
	public Sprite earth;
	public Sprite metal;

	public float smooth = 0.5f;
	float currentProgress = 0;
	float targetProgress = 0;
	float velocity;

	public bool startCount;
	public int MagicNum = 0;
	public bool successfulCast;

	public GameState gameState;

	public GameObject menuPanel;
	public GameObject scanPanel;
	public Text narratorText;
	public Text menuText;
	public UnityEngine.UI.Image castButton;

	bool justBegin;
	int triedTimes = 0;
	int highestScoreOfThreeTries = 0;
	int totalScore = 0;
	int caseNum = 0;

	bool startCountDownTime = false;

	AudioSource uiSounds;
	AudioSource spellSounds;

	string stringChain;
	SpellInterpreter interpreter;

	// Use this for initialization
	void Start () {
		VuforiaBehaviour.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
		VuforiaBehaviour.Instance.RegisterOnPauseCallback(OnPaused);
		StartCoroutine(DisableARCamera());
		gameState = GameState.menu;
		interpreter = GetComponent<SpellInterpreter>();
		justBegin = true;
		uiSounds = GetComponents<AudioSource>()[0];
		spellSounds = GetComponents<AudioSource>()[1];
	}
	
	// Update is called once per frame
	void Update () {
		if(gameState == GameState.menu){
			if(justBegin){
				menuPanel.GetComponent<Animator>().Play("begin");
				justBegin = false;
			}
		}else{
			currentProgress = Mathf.SmoothDamp(currentProgress, targetProgress, ref velocity, smooth);
			progressLine.fillAmount = currentProgress;

		}
	}


	void OnVuforiaStarted(){
		CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
	}

	void OnPaused(bool paused){
		if(!paused){
			CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);	
		}
	}

	IEnumerator DisableARCamera(){
		yield return new WaitForSeconds(1);
		ARCamera.enabled = false;
	}

	public void ShowElementInOrder(string element){
		Sprite tmpSprit;
		switch(element){
		default:
			tmpSprit = fire;
			break;
		case "fire":
			tmpSprit = fire;
			break;
		case "water":
			tmpSprit = water;
			break;
		case "wood":
			tmpSprit = wood;
			break;
		case "wind":
			tmpSprit = wind;
			break;
		case "earth":
			tmpSprit = earth;
			break;
		case "metal":
			tmpSprit = metal;
			break;
		}
		if(MagicNum == 1){
			spellSounds.clip = Resources.Load<AudioClip>(string.Concat("Sounds/", element, "Sound"));
			uiSounds.clip = Resources.Load<AudioClip>("Sounds/SpellSelect1");
			uiSounds.Play();
			magicImage0.sprite = tmpSprit;
			magicImage0.color = Color.white;
			targetProgress = 0.367f;
			stringChain += element;
		}else if(MagicNum == 2){
			uiSounds.clip = Resources.Load<AudioClip>("Sounds/SpellSelect2");
			uiSounds.Play();
			magicImage1.sprite = tmpSprit;
			magicImage1.color = Color.white;
			targetProgress = 0.631f;
			stringChain += "+" + element;
		}else if(MagicNum == 3){
			uiSounds.clip = Resources.Load<AudioClip>("Sounds/SpellSelect2");
			uiSounds.Play();
			magicImage2.sprite = tmpSprit;
			magicImage2.color = Color.white;
			targetProgress = 1f;
			stringChain += "+" + element;
		}
	}

	public void ShowEffect(){
		string[] castedSpell = stringChain.Split('+');
		successfulCast = false;
		string[] currentSlots = interpreter.getCurrentSlotsChain().Split('+');
		scanPanel.GetComponent<Animator>().Play("castSpell");
		feedbackText.text = "You cast a\n" + interpreter.getSpellName(castedSpell[0],castedSpell[1],castedSpell[2]);
		spellSounds.Play();
		if(gameState == GameState.level1){
			if(stringChain == interpreter.getCurrentSlotsChain()){
				uiSounds.clip = Resources.Load<AudioClip>("Sounds/Correct");
				uiSounds.Play();
				narratorText.CrossFadeAlpha(0,0.1f,true);
				narratorText.CrossFadeAlpha(1,2f,true);
				narratorText.text = "Now that you've learned how the elements work, let's cast some real spells.";
				StartCoroutine(EnterLevel2());
			}else{
				uiSounds.clip = Resources.Load<AudioClip>("Sounds/Incorrect");
				uiSounds.Play();
				feedbackText.text = "Incorrect\nYou cast\n" + stringChain;
				magicCircle.ScaleTo(Vector3.one * 0.1f,1,0,EaseType.easeInOutQuad);
				narratorText.CrossFadeAlpha(1,2f,true);
			}
		}else if(gameState == GameState.level2){
			if(stringChain == interpreter.getCurrentSlotsChain()){
				uiSounds.clip = Resources.Load<AudioClip>("Sounds/Correct");
				uiSounds.Play();
				feedbackText.text += "\nThis spell treats \n" +interpreter.getSeverity()+" "+ interpreter.getSymptoms();
				if(interpreter.level < 2){
					interpreter.nextLevel();
					currentSlots = interpreter.getCurrentSlotsChain().Split('+');
					narratorText.text = "Now cast a\n" + interpreter.getSpellName(currentSlots[0],currentSlots[1],currentSlots[2]);
				}else{
					narratorText.CrossFadeAlpha(0,0.1f,true);
					narratorText.CrossFadeAlpha(1,2f,true);
					narratorText.text = "You've gained a practical understanding of magic. Let's tackle some case files. You'll have 3 chances to get a Full Cure.";
					StartCoroutine(EnterLevel3());
				}
			}else{
				uiSounds.clip = Resources.Load<AudioClip>("Sounds/Incorrect");
				uiSounds.Play();
				feedbackText.text = "Incorrect\nYou cast a\n" + interpreter.getSpellName(castedSpell[0],castedSpell[1],castedSpell[2]);
				magicCircle.ScaleTo(Vector3.one * 0.1f,1,0,EaseType.easeInOutQuad);
				narratorText.CrossFadeAlpha(1,2f,true);
			}
		}else if(gameState == GameState.level3){
			int score = interpreter.checkAnswer(castedSpell[0],castedSpell[1],castedSpell[2]);
			if(score >= 16) {
				uiSounds.clip = Resources.Load<AudioClip>("Sounds/FullCure");
			}
			else if(score > 8) {
				uiSounds.clip = Resources.Load<AudioClip>("Sounds/Correct");
			}
			else {
				uiSounds.clip = Resources.Load<AudioClip>("Sounds/Incorrect");
			}
			uiSounds.Play();
			feedbackText.text += "\n" + interpreter.cureRatings[score];
			if(score > highestScoreOfThreeTries){
				highestScoreOfThreeTries = score;
			}

			if(stringChain == interpreter.getCurrentSlotsChain()){
				interpreter.nextLevel();
				totalScore += highestScoreOfThreeTries;
				highestScoreOfThreeTries = 0;
				caseNum = Random.Range(1000,9999);
				currentSlots = interpreter.getCurrentSlotsChain().Split('+');
				narratorText.text = "Case file:" + caseNum.ToString() + "\nPatient suffers from\n"+ interpreter.getSeverity()+" "+ interpreter.getSymptoms() + "\nCast a cure!";
				triedTimes = 0;
				if(interpreter.level == interpreter.maxLevel+1){
					StartCoroutine(BackToMenuAndShowScore());
				}
			}else{
				magicCircle.ScaleTo(Vector3.one * 0.1f,1,0,EaseType.easeInOutQuad);
				narratorText.CrossFadeAlpha(1,2f,true);
				triedTimes += 1;
				if(triedTimes >= 3){
					interpreter.nextLevel();
					totalScore += highestScoreOfThreeTries;
					highestScoreOfThreeTries = 0;
					caseNum = Random.Range(1000,9999);
					currentSlots = interpreter.getCurrentSlotsChain().Split('+');
					narratorText.text = "Case file:" + caseNum.ToString() + "\nPatient suffers from\n"+ interpreter.getSeverity()+" "+ interpreter.getSymptoms() + "\nCast a cure!";
					triedTimes = 0;
					if(interpreter.level == interpreter.maxLevel+1){
						StartCoroutine(BackToMenuAndShowScore());
					}
				}
			}
		}
	}

	IEnumerator EnterLevel2(){
		while(!startCountDownTime){
			yield return null;
		}
		yield return new WaitForSeconds(6);
		interpreter.nextLevel();
		string[] currentSlots = interpreter.getCurrentSlotsChain().Split('+');
		narratorText.CrossFadeAlpha(0,0.1f,true);
		narratorText.CrossFadeAlpha(1,2f,true);
		narratorText.text = "Now cast a\n" + interpreter.getSpellName(currentSlots[0],currentSlots[1],currentSlots[2]);
		gameState = GameState.level2;
	}

	IEnumerator EnterLevel3(){
		while(!startCountDownTime){
			yield return null;
		}
		yield return new WaitForSeconds(6);
		interpreter.nextLevel();
		caseNum = Random.Range(1000,9999);
		string[] currentSlots = interpreter.getCurrentSlotsChain().Split('+');
		narratorText.CrossFadeAlpha(0,0.1f,true);
		narratorText.CrossFadeAlpha(1,2f,true);
		narratorText.text = "Case file:" + caseNum.ToString() + "\nPatient suffers from\n"+ interpreter.getSeverity()+" "+ interpreter.getSymptoms() + "\nCast a cure!";
		gameState = GameState.level3;
	}

	public void EnterGame(){
		if(gameState == GameState.menu){
			uiSounds.clip = Resources.Load<AudioClip>("Sounds/ButtonClick");
			uiSounds.Play();
			gameState = GameState.level1;
			interpreter.nextLevel();
			menuPanel.GetComponent<Animator>().Play("gameStart");
			narratorText.text = "Cast a spell composed of\n" + interpreter.getCurrentSlotsChain();
		}else if(gameState == GameState.level3){
			menuPanel.GetComponent<Animator>().Play("gameStart");
		}
	}

	IEnumerator BackToMenuAndShowScore(){
		while(!startCountDownTime){
			yield return null;
		}
		yield return new WaitForSeconds(3);
		menuPanel.GetComponent<Animator>().Play("gameEnd");
		menuText.text = "Total Score: "+totalScore.ToString();
		string ranking = "";
		feedbackText.text = "";
		if(totalScore > 60) {
			ranking = "Fully Accredited\nStay sharp, keep curing!";
		}
		else if(totalScore > 50) {
			ranking = "Accredited\nStay sharp, keep curing!";
		}
		else if(totalScore > 32) {
			ranking = "Barely Accredited\nStay sharp, keep curing!";
		}
		else if(totalScore > 16) {
			ranking = "Unlicensed\nPlease try again";
		}
		else {
			ranking = "Dropout\nPlease try again";
		}

		if(totalScore <= 32){
			totalScore = 0;
			interpreter.level = -1;
			gameState = GameState.menu;
		}
		menuText.text += "\nCongratulations!" + "\nRanking: " + ranking;
	}

	public void ButtonDown(){
		Debug.Log("ButtonDown");
		startCountDownTime = false;
		uiSounds.clip = Resources.Load<AudioClip>("Sounds/ButtonClick");
		uiSounds.Play();
		castButton.CrossFadeAlpha(0.5f,0.1f,true);
		stringChain = "";
		feedbackText.text = stringChain;
		narratorText.CrossFadeAlpha(0,0.5f,true);
		ARCamera.enabled = true;
		startCount = true;
		magicCircle.ScaleTo(Vector3.one,1f,0,EaseType.easeInOutQuad);
	}

	public void ButtonUp(){
		Debug.Log("ButtonUp");
		startCountDownTime = true;
		castButton.CrossFadeAlpha(1,0.1f,true);
		narratorText.CrossFadeAlpha(1,2f,true);
		ARCamera.enabled = false;
		startCount = false;
		MagicNum = 0;
		targetProgress = 0;
		magicImage0.color = new Color(1,1,1,0);
		magicImage1.color = new Color(1,1,1,0);
		magicImage2.color = new Color(1,1,1,0);
		if(!successfulCast)
			magicCircle.ScaleTo(Vector3.one * 0.1f,1,0,EaseType.easeInOutQuad);
	}
}
