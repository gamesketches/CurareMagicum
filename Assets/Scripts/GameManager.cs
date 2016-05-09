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

	bool justBegin;
	int triedTimes = 0;

	string stringChain;
	SpellInterpreter interpreter;

	// Use this for initialization
	void Start () {
		VuforiaBehaviour.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
		VuforiaBehaviour.Instance.RegisterOnPauseCallback(OnPaused);
		Invoke("DisableARCamera",0.5f);
		gameState = GameState.menu;
		interpreter = GetComponent<SpellInterpreter>();
		justBegin = true;
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

	void DisableARCamera(){
		Thread.Sleep(500);
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
			magicImage0.sprite = tmpSprit;
			magicImage0.color = Color.white;
			targetProgress = 0.367f;
			stringChain += element;
		}else if(MagicNum == 2){
			magicImage1.sprite = tmpSprit;
			magicImage1.color = Color.white;
			targetProgress = 0.631f;
			stringChain += "+" + element;
		}else if(MagicNum == 3){
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
		feedbackText.text = "You casted a\n" + interpreter.getSpellName(castedSpell[0],castedSpell[1],castedSpell[2]);
		if(gameState == GameState.level1){
			if(stringChain == interpreter.getCurrentSlotsChain()){
				interpreter.nextLevel();
				currentSlots = interpreter.getCurrentSlotsChain().Split('+');
				narratorText.text = "Now give us a\n" + interpreter.getSpellName(currentSlots[0],currentSlots[1],currentSlots[2]);
				gameState = GameState.level2;
			}
		}else if(gameState == GameState.level2){
			if(stringChain == interpreter.getCurrentSlotsChain()){
				feedbackText.text += "\nThis is a treatment for\n" +interpreter.getSeverity()+" "+ interpreter.getSymptoms();
				if(interpreter.level < 2){
					interpreter.nextLevel();
					currentSlots = interpreter.getCurrentSlotsChain().Split('+');
					narratorText.text = "Now give us a\n" + interpreter.getSpellName(currentSlots[0],currentSlots[1],currentSlots[2]);
				}else{
					interpreter.nextLevel();
					narratorText.text = "Now try to cure\n" + interpreter.getSeverity()+" "+ interpreter.getSymptoms();
					gameState = GameState.level3;
				}
			}
		}else if(gameState == GameState.level3){
			int score = interpreter.checkAnswer(castedSpell[0],castedSpell[1],castedSpell[2]);
			feedbackText.text += "\n" + interpreter.cureRatings[score];
			if(stringChain == interpreter.getCurrentSlotsChain()){
				interpreter.nextLevel();
				narratorText.text = "Now try to cure\n" + interpreter.getSeverity()+" "+ interpreter.getSymptoms();
				triedTimes = 0;
			}else{
				triedTimes += 1;
				if(triedTimes >= 3){
					interpreter.nextLevel();
					narratorText.text = "Now try to cure\n" + interpreter.getSeverity()+" "+ interpreter.getSymptoms();
					triedTimes = 0;
				}
			}
		}
	}

	public void EnterGame(){
		if(gameState == GameState.menu){
			gameState = GameState.level1;
			interpreter.nextLevel();
			menuPanel.GetComponent<Animator>().Play("gameStart");
			narratorText.text = "Give us the following\nspell\n" + interpreter.getCurrentSlotsChain();
		}
	}

	public void ButtonDown(){
		Debug.Log("ButtonDown");
		stringChain = "";
		feedbackText.text = stringChain;
		narratorText.CrossFadeAlpha(0,0.5f,true);
		ARCamera.enabled = true;
		startCount = true;
		magicCircle.ScaleTo(Vector3.one,1f,0,EaseType.easeInOutQuad);
	}

	public void ButtonUp(){
		Debug.Log("ButtonUp");
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
