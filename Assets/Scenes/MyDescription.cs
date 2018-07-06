using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyDescription : MonoBehaviour {

	// Use this for initialization
	void Start () {
		startFade (1,"start");
		int index = globalData.getCurVideoName ();
		if(globalData.getTrReadyVideo ()){
		CurDataVideo video = globalData.getVideoData (index);
			showMessage(video.desc);
		}else{
			Debug.Log ("video is not ready");
		}
	}

	void startFade(float transparent, string scene){
		GameObject fadeCanvas = new GameObject("fadeCanvas");
		fadeCanvas.transform.SetParent(Camera.main.transform);
		Canvas c = fadeCanvas.AddComponent<Canvas>();
		//		c.renderMode = RenderMode.ScreenSpaceOverlay;
		c.renderMode = RenderMode.WorldSpace;
		fadeCanvas.AddComponent<CanvasScaler>();
		GameObject fadeObj = new GameObject("FadeObj");
//		GameObject fadeObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Image img = fadeObj.AddComponent<Image>();
		img.color = new Color(0f, 0f, 0f, transparent );
		RectTransform NewCanvasRect = img.GetComponent<RectTransform> ();
		NewCanvasRect.transform.localScale = new Vector3 (100, 100, 1);
		fadeObj.transform.SetParent(fadeCanvas.transform);
		fadeObj.transform.localPosition = new Vector3 (0,0,2);
		StartCoroutine (FadeCanvas (1.0f,transparent,fadeCanvas,fadeObj,img.color, scene));
	}

	IEnumerator FadeCanvas( float duration, float transparent, GameObject fadeCanvas, GameObject fadeObj, Color  colorStart, string scene){
		float smoothness = 0.005f; float progress = 0; float increment = smoothness/duration; //The amount of change to apply.
		float newTransparent = 0; if (transparent == 0) {newTransparent = 1;}
		Color  colorEnd = new Color (colorStart.r,colorStart.g,colorStart.b, newTransparent);
		while(progress < 1){ progress += increment;	
			fadeObj.GetComponent<Image>().color = Color.Lerp(colorStart,colorEnd,progress);	
			yield return new WaitForSeconds (smoothness);};
		yield return null;
		if (transparent == 1) {
			Destroy (fadeObj);
			Destroy (fadeCanvas);
		} else {
			Debug.Log ("Start video");
			UnityEngine.SceneManagement.SceneManager.LoadScene (scene);
		}
	}

	private void showMessage(string msg){
		GameObject newCanvas = new GameObject("MsgCanvas");
		Canvas c = newCanvas.AddComponent<Canvas>();
		c.renderMode = RenderMode.WorldSpace;
		newCanvas.AddComponent<CanvasScaler>();
		newCanvas.AddComponent<GraphicRaycaster>();
		RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform> ();
		NewCanvasRect.sizeDelta = new Vector2 (1200,700);
		NewCanvasRect.localScale = new Vector3 (0.1f, 0.1f, 1f);
		NewCanvasRect.localPosition = new Vector3 (0, 0, 40);
		GameObject panel = new GameObject("Panel");
		panel.AddComponent<CanvasRenderer>();
		Image i = panel.AddComponent<Image>();
		i.color = new Vector4(1, 1, 1, 0.7f);
		RectTransform panelTransform = panel.GetComponent<RectTransform> ();
		panel.transform.SetParent(NewCanvasRect, true);
		panelTransform.localScale = new Vector3 (0.5f, 0.5f, 1f);
		panelTransform.localPosition = new Vector3 (0, 4, 0);			
		panelTransform.sizeDelta = new Vector2 (1200,700);
		CreateText(panelTransform, 0, 80, 1200, 600, msg, 50, 0, TextAnchor.MiddleCenter);
		Invoke("startVideo", 2);
	}

	private void startVideo(){startFade (0,"Video2");}

	private GameObject CreateText(Transform parent, float x, float y, float w, float h, string message, int fontSize, int fontStyle, TextAnchor achor) {
		GameObject textObject = new GameObject("Text");
		textObject.transform.SetParent(parent);
		RectTransform trans = textObject.AddComponent<RectTransform>();
		trans.sizeDelta= new Vector2(w, h);
		trans.anchoredPosition3D = new Vector3(0, 0, 0);
		trans.anchoredPosition = new Vector2(x, y);
		//trans.transform.rotation = Quaternion.AngleAxis(-180, Vector3.up);
		trans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		trans.localPosition.Set(0, 0, 0);
		textObject.AddComponent<CanvasRenderer>();
		Text text = textObject.AddComponent<Text>();
		text.supportRichText = true;
		text.text = message;
		text.fontSize = fontSize;
		if(fontStyle == 1) {text.fontStyle = FontStyle.Bold;}
		text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		text.alignment = achor;
		//text.alignment = TextAnchor.MiddleCenter;
		//text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.color = new Color(0, 0, 0);
		return textObject;
	}
	// Update is called once per frame
	void Update () {

	}
}
