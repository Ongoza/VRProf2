using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimedGaze : MonoBehaviour {
	private float defaultTime=3f;
	private float workTime;
	private bool isTimer; 
	private Camera cam;
	private Image img;
	// Use this for initialization
	void Start () {
		isTimer = false;
		workTime = defaultTime;
		cam = Camera.main;
		img  = GameObject.Find("TimedGazePoint").GetComponent<Image>();

		Debug.Log ("Start Click cam"+cam.name);
	}
	
	// Update is called once per frame
	void Update () {
		if (isTimer) {
			workTime -= Time.deltaTime;

			if (workTime <= 0) {
				onClickTimed ();
			} else { Debug.Log ("Start Click time="+workTime+"=="+ (1f - workTime/defaultTime));
				img.fillAmount = 1f - workTime/defaultTime;}
		}
	}

	public void onClickTimed (){
		Debug.Log ("Start Click timed");
		isTimer = false;
		workTime = defaultTime;
		//img.fillAmount = 0;
	}

	public void onEnterTimed (){
		Debug.Log ("Start enter");
		isTimer = true;
		workTime = defaultTime;
		img.fillAmount = 0;
	}

	public void onExitTimed (){
		Debug.Log ("Start exit");
		isTimer = false;
		workTime = defaultTime;
		img.fillAmount = 0;
	}

}
