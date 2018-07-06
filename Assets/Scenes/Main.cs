// bugs
// android stop without network connections !!!!!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class Main : MonoBehaviour {
	private Text priceText;
	private Text overlayText;
	private string curMovie;
	private float defaultTime=3f;
	private float workTime;
	private bool isTimer; 
	private Image imgTimedGaze;
	private string curfocusObj="";
	private bool show_debug = false;
    public GameObject camFade;
    public GameObject camTimedPointer;
    private Material timedPointer; 

    void Start () {
        //showOevrlayInfo ("Starting app!");
        timedPointer = camTimedPointer.GetComponent<Renderer>().material;     
        startFade (1,"start"); 
	    startMain ();
    }
		
	void startFade(float transparent, string scene){
        
        //GameObject fadeCanvas = new GameObject("fadeCanvas");
        //fadeCanvas.transform.SetParent(Camera.main.transform);
        //Canvas c = fadeCanvas.AddComponent<Canvas>();
        ////		c.renderMode = RenderMode.ScreenSpaceOverlay;
        //c.renderMode = RenderMode.WorldSpace;
        //fadeCanvas.AddComponent<CanvasScaler>();
        //GameObject fadeObj = new GameObject("FadeObj");
        ////		GameObject fadeObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //Image img = fadeObj.AddComponent<Image>();
        //img.color = new Color(0f, 0f, 0f, transparent );
        //RectTransform NewCanvasRect = img.GetComponent<RectTransform> ();
        //NewCanvasRect.transform.localScale = new Vector3 (100, 100, 1);
        //fadeObj.transform.SetParent(fadeCanvas.transform);
        //fadeObj.transform.localPosition = new Vector3 (0,0,2);
        camFade.GetComponent<Renderer>().enabled = true;
        Color colorStart = camFade.GetComponent<Renderer>().materials[0].color;
        Debug.Log("Start scene " + colorStart);
        colorStart = new Color(colorStart.r, colorStart.g, colorStart.b, 1f);
        camFade.GetComponent<Renderer>().materials[0].color = colorStart;
        StartCoroutine (FadeCamera (1.0f, false, colorStart,"Main"));
	}

    IEnumerator FadeCamera(float duration, bool alfaSetTo1, Color colorStart, string scene){
        float smoothness = 0.005f; float progress = 0; float increment = smoothness / duration; //The amount of change to apply.
        float newTransparent = 0; if (alfaSetTo1 == true) { newTransparent = 1; }         
        Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, newTransparent);
        while (progress < 1){
            progress += increment;
            camFade.GetComponent<Renderer>().materials[0].color = Color.Lerp(colorStart, colorEnd, progress);
            yield return new WaitForSeconds(smoothness);
        };
        yield return null;
        if (alfaSetTo1 == false)
        {
            camFade.GetComponent<Renderer>().enabled = false;
        }
        else {
            Debug.Log("Start scene" + scene);
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        }
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
			// ScreenCapture.CaptureScreenshot ("Screenshot.png");
		} else {
			Debug.Log ("Start scene"+scene);
			UnityEngine.SceneManagement.SceneManager.LoadScene (scene);
		}
	}



	void startMain(){				
		//Debug.Log("Start color test tr=" + globalData.trColorTest1);
		isTimer = false;
		workTime = defaultTime;
		checkDevice();
		Debug.Log("Hello from Main");
//		List<int> selNames = new List<int>(){1,0,3,4,5,6,7};
//		globalData.addcTest(selNames);
		//if (globalData.trColorTest1 == 0) {	 
		string msg = "This app allows to choose \nyour future profession.\n\nPath the psychological colors test\nfor measure your character.\n\nYou should select colors \nwhich you more like one by one.";
		startMessage(msg);			
		//}else {	movieSelect();}			
		//string jStr = globalData.getLocalTutorialString ("tut1.json");
		//Debug.Log ("jStr="+jStr);
		StartCoroutine(GetRequestJson("vprof1.json"));
		//jsonToObj(jStr);
	}
		
	private void checkDevice(){
		string serverTestString="";
		try{ 
			System.TimeSpan travelTime = System.DateTime.Now-System.DateTime.UtcNow;
			//PlayerPrefs.SetString("zone",  travelTime.Hours.ToString());
			serverTestString+="\"zone\":\""+travelTime.Hours+"\"";
		}catch(System.Exception e){	
			Debug.Log ("Error net ="+e);
			globalData.server.putDataString (false,"\"deviceZone\":\"error\"");
			serverTestString+="zone:error";}
		try{
			serverTestString+=",\"ip\":\""+Network.player.ipAddress+"\""+
			",\"model\":\""+SystemInfo.deviceModel+"\""+
			//serverTestString+=","type":""+SystemInfo.deviceType+"\"";
				",\"os\":\""+SystemInfo.operatingSystem+"\"";
		}catch(System.Exception e){	
			Debug.Log ("deviceid error"+e);
			globalData.server.putDataString (false,"\"deviceData\":\"error\"");
		}
		//Debug.Log("serverTestString="+serverTestString);
		addOevrlayInfo ("DeviceInfo:"+serverTestString);
		globalData.addDeviceData (serverTestString);
		//ServerConnection serv = new ServerConnection ();
		globalData.server.putDataString (true,serverTestString);
	}	

	private void movieSelect(){
		Debug.Log("movie select");
		GameObject newCanvas = new GameObject("CanvasMovie");
		Canvas c = newCanvas.AddComponent<Canvas>();
		c.renderMode = RenderMode.WorldSpace;
		newCanvas.AddComponent<CanvasScaler>();
		//newCanvas.AddComponent<GraphicRaycaster>();
        newCanvas.AddComponent<GvrPointerGraphicRaycaster>();
        //		newCanvas.AddComponent<Physics2DRaycaster>();
        RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform> ();
		NewCanvasRect.sizeDelta = new Vector2 (1200,700);
		NewCanvasRect.localScale = new Vector3 (0.1f, 0.1f, 1f);
		NewCanvasRect.localPosition = new Vector3 (0, 0, -22);
		GameObject panelMain = new GameObject("PanelMain");
		panelMain.AddComponent<CanvasRenderer>();
		Image i = panelMain.AddComponent<Image>();
		i.color = new Vector4(1, 1, 1, 0.7f);
		RectTransform mainTransform = panelMain.GetComponent<RectTransform> ();
		panelMain.transform.SetParent(NewCanvasRect, true);
		mainTransform.localScale = new Vector3 (1, 1, 1);
		mainTransform.localPosition = new Vector3 (0, 3, 0);			
		mainTransform.sizeDelta = new Vector2 (240,120);
		GameObject panelName = new GameObject("PanelTextName");
		panelName.transform.SetParent(NewCanvasRect, true);
		//panelName.AddComponent<CanvasRenderer>();
		Image i2 = panelName.AddComponent<Image>();
		i2.color = new Vector4(1, 1, 1, 0.7f);
		RectTransform rt = panelName.GetComponent<RectTransform>();
		rt.localScale = new Vector3 (0.2f, 0.2f, 1);
		rt.localPosition = new Vector3 (0, 78, 0);
		rt.localRotation = Quaternion.AngleAxis(180, Vector3.up);
		rt.sizeDelta = new Vector2 (1200,100);
		Material mat = new Material (Shader.Find(" Diffuse"));
		//Material mat0 = new Material (Shader.Find(" Diffuse"));
		CreateText(rt, 0, 0,1200, 100, "The movies list", 50, 1, TextAnchor.MiddleCenter);
		Texture2D tex1 = (Texture2D)Resources.Load("Textures/dayModel");
		createMovieButton ("testBtn1",60,30,tex1,mainTransform,mat,true);
		Texture2D tex2 = (Texture2D)Resources.Load("Textures/dayCoach");
		createMovieButton ("testBtn2",-60,30,tex2,mainTransform,mat,false);
		Texture2D tex3 = (Texture2D)Resources.Load("Textures/dayDeveloper");
		createMovieButton ("testBtn3",60,-30,tex3,mainTransform,mat,false);
		Texture2D tex4 = (Texture2D)Resources.Load("Textures/daySales");
		createMovieButton ("testBtn4",-60,-30,tex4,mainTransform,mat,false);

	}

	private void createMovieButton(string name, float x, float y, Texture2D tex, RectTransform root, Material mat, bool active){
		Debug.Log("movie btn "+name);
		GameObject button = new GameObject(name);
		RawImage img = button.AddComponent<RawImage>();
		img.texture = tex;
		if (!active) {
			img.color = new Color (1,1,1,0.5f);
		}
		//img.material = mat;
		Transform br = button.GetComponent<RectTransform>();
		br.parent = root;
		br.localRotation = Quaternion.AngleAxis(180, Vector3.up);
		br.localPosition = new Vector3 (x,y,1);
		br.localScale = new Vector3 (1,0.5f,1);
		if (active) {
			EventTrigger be = button.AddComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener ((eventData) => {
				clickSelectEvent (name);
			});
			be.triggers.Add (entry);
			EventTrigger.Entry entryEnterGaze = new EventTrigger.Entry ();
			entryEnterGaze.eventID = EventTriggerType.PointerEnter;
			entryEnterGaze.callback.AddListener ((eventData) => {
				onEnterTimed (name);
			});
			be.triggers.Add (entryEnterGaze);
			EventTrigger.Entry entryExitGaze = new EventTrigger.Entry ();
			entryExitGaze.eventID = EventTriggerType.PointerExit;
			entryExitGaze.callback.AddListener ((eventData) => {
				onExitTimed ();
			});
			be.triggers.Add (entryExitGaze);
		}
	}		

	private void addOevrlayInfo(string txt){
		if (show_debug) {
			string[] lines = overlayText.text.Split('\n');
			string newTxt = "";	int len = lines.Length;
			if (len > 2) {len = 2;}
			for (int i = len; i > 0 ; i--) {newTxt += lines [lines.Length - i]+"\n";}
			overlayText.text = newTxt+txt;
		}
	}

	private void showOevrlayInfo( string txt){
		if (show_debug) {
			GameObject newCanvas = new GameObject ("InfoCanvas");
			Canvas c = newCanvas.AddComponent<Canvas> ();
			c.renderMode = RenderMode.WorldSpace;
			newCanvas.AddComponent<CanvasScaler> ();
			RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform> ();
			//NewCanvasRect.Rotate(120,40,0);
			NewCanvasRect.localRotation = Quaternion.Euler (25, 180, 0);
			newCanvas.transform.SetParent (Camera.main.transform, true);
			NewCanvasRect.localPosition = new Vector3 (0, -9f, 20);
			NewCanvasRect.sizeDelta = new Vector2 (1200, 20);
			NewCanvasRect.localScale = new Vector3 (0.1f, 0.1f, 1f);
			GameObject panel = new GameObject ("InfoPanel");
			panel.AddComponent<CanvasRenderer> ();
			Image i = panel.AddComponent<Image> ();
			i.color = new Vector4 (0.3f, 0.3f, 0.7f, 0.3f);
			RectTransform panelTransform = panel.GetComponent<RectTransform> ();
			panel.transform.SetParent (NewCanvasRect, true);
			panelTransform.localScale = new Vector3 (0.3f, 0.3f, 1f);
			panelTransform.localRotation = Quaternion.AngleAxis (0, Vector3.right);
			panelTransform.localPosition = new Vector3 (0, 0, 0);			
			panelTransform.sizeDelta = new Vector2 (500, 50);
			GameObject txtObj = CreateText (panelTransform, 6, 4, 500, 50, txt, 12, 0, TextAnchor.LowerLeft);
			txtObj.transform.localRotation = Quaternion.AngleAxis (0, Vector3.right);
			overlayText = txtObj.GetComponent<Text> ();
			overlayText.color = new Color (0, 0.8f, 0);
		}
	}

	private void startMessage(string msg){
		GameObject newCanvas = new GameObject("StartCanvas");
		Canvas c = newCanvas.AddComponent<Canvas>();
		c.renderMode = RenderMode.WorldSpace;
		newCanvas.AddComponent<CanvasScaler>();
		//newCanvas.AddComponent<GraphicRaycaster>();
        newCanvas.AddComponent<GvrPointerGraphicRaycaster>();
        RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform> ();
		NewCanvasRect.sizeDelta = new Vector2 (1200,700);
		NewCanvasRect.localScale = new Vector3 (0.1f, 0.1f, 1f);
		NewCanvasRect.localPosition = new Vector3 (0, 0, -10);
		GameObject panel = new GameObject("Panel");
		panel.AddComponent<CanvasRenderer>();
		Image i = panel.AddComponent<Image>();
		i.color = new Vector4(1, 1, 1, 0.7f);
		RectTransform panelTransform = panel.GetComponent<RectTransform> ();
		panel.transform.SetParent(NewCanvasRect, true);
		panelTransform.localScale = new Vector3 (0.2f, 0.2f, 1f);
		panelTransform.localPosition = new Vector3 (0, 4, 0);			
		panelTransform.sizeDelta = new Vector2 (1200,700);
		//panelTransform.rotation = Quaternion.AngleAxis(-180, Vector3.up);

		CreateText(panelTransform, 0, 80, 1200, 600, msg, 50, 0, TextAnchor.MiddleCenter);
		GameObject bt0 = new GameObject("btnTest");
		RectTransform br = bt0.AddComponent<RectTransform>();
		br.sizeDelta= new Vector2 (300,60);
		Image img = bt0.AddComponent<Image>();
		img.color = new Vector4(0.5f, 0.5f, 0.8f, 1);
		img.material.color = new Vector4(1f, 1f, 1f, 0.7f);
		Button bt = bt0.AddComponent<Button>();
		bt0.transform.SetParent(panelTransform, true);
		bt0.transform.localPosition = new Vector3 (-200, -250, 0);
		//bt0.transform.rotation = Quaternion.AngleAxis(-180, Vector3.up);
		bt0.transform.localScale = new Vector3 (1, 1, 1);
		CreateText(bt.transform, 0, 0,300, 50, "Color test", 40, 1, TextAnchor.MiddleCenter);
		bt.onClick.AddListener(HandleClickStartColorTest);
		EventTrigger be = bt0.AddComponent<EventTrigger>();
		EventTrigger.Entry entryEnterGaze = new EventTrigger.Entry();
		entryEnterGaze.eventID = EventTriggerType.PointerEnter;
		entryEnterGaze.callback.AddListener((eventData) => { onEnterTimed("test"); });
		be.triggers.Add(entryEnterGaze);
		EventTrigger.Entry entryExitGaze = new EventTrigger.Entry();
		entryExitGaze.eventID = EventTriggerType.PointerExit;
		entryExitGaze.callback.AddListener((eventData) => { onExitTimed(); });
		be.triggers.Add(entryExitGaze);

		GameObject bt1_0 = new GameObject("btnVideo");
		RectTransform br2 = bt1_0.AddComponent<RectTransform>();
		br2.sizeDelta= new Vector2 (300,60);
		Image img2 = bt1_0.AddComponent<Image>();
		img2.color = new Vector4(0.5f, 0.5f, 0.8f, 1);
		img2.material.color = new Vector4(1f, 1f, 1f, 0.7f);
		Button bt2 = bt1_0.AddComponent<Button>();
		bt1_0.transform.SetParent(panelTransform, true);
		bt1_0.transform.localPosition = new Vector3 (200, -250, 0);
		bt1_0.transform.rotation = Quaternion.AngleAxis(-180, Vector3.up);
		bt1_0.transform.localScale = new Vector3 (1, 1, 1);
		CreateText(bt2.transform, 0, 0,300, 50, "Video", 40, 1, TextAnchor.MiddleCenter);
		bt2.onClick.AddListener(HandleClickStartVideo);
		EventTrigger be2 = bt1_0.AddComponent<EventTrigger>();
		EventTrigger.Entry entryEnterGaze2 = new EventTrigger.Entry();
		entryEnterGaze2.eventID = EventTriggerType.PointerEnter;
		entryEnterGaze2.callback.AddListener((eventData) => { onEnterTimed("video"); });
		be2.triggers.Add(entryEnterGaze2);
		EventTrigger.Entry entryExitGaze2 = new EventTrigger.Entry();
		entryExitGaze2.eventID = EventTriggerType.PointerExit;
		entryExitGaze2.callback.AddListener((eventData) => { onExitTimed(); });
		be2.triggers.Add(entryExitGaze2);
	}
		
	void Update () {if (isTimer) {workTime -= Time.deltaTime;
			if (workTime <= 0) {onClickTimed ();
			} else {
                //imgTimedGaze.fillAmount = 1f - workTime/defaultTime;
                float percent = (1f - workTime / defaultTime) * 360;
                timedPointer.SetFloat("_Angle", percent);
                //Debug.Log("Start scene shader update " + timedPointer.GetFloat("_Angle"));
            }
        }
    }

	private GameObject CreateText(Transform parent, float x, float y, float w, float h, string message, int fontSize, int fontStyle, TextAnchor achor) {
		GameObject textObject = new GameObject("Text");
		textObject.transform.SetParent(parent);
		RectTransform trans = textObject.AddComponent<RectTransform>();
		trans.sizeDelta= new Vector2(w, h);
		trans.anchoredPosition3D = new Vector3(0, 0, 0);
		trans.anchoredPosition = new Vector2(x, y);
		trans.transform.rotation = Quaternion.AngleAxis(-180, Vector3.up);
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

	public void HandleClickStartColorTest(){onExitTimed ();startFade (0, "colorTest");}
	public void HandleClickStartVideo(){onExitTimed (); tryStartVideo ();}
	public void onClickTimed (){clickSelectEvent(curfocusObj);}
	public void onEnterTimed (string name){isTimer = true;	workTime = defaultTime;
        timedPointer.SetFloat("_Angle", 0);
        //Debug.Log("Start scene shader 3 " + timedPointer.GetFloat("_Angle"));
        //imgTimedGaze.fillAmount = 0;
        curfocusObj = name;}
	public void onExitTimed (){isTimer = false; curfocusObj = ""; workTime = defaultTime;
        //imgTimedGaze.fillAmount = 0;
        timedPointer.SetFloat("_Angle", 360);
        //Debug.Log("Start scene shader 3 " + timedPointer.GetFloat("_Angle"));
    }

	private void clickSelectEvent(string name){	
		onExitTimed ();	
		addOevrlayInfo ("clickSelectEvent" + name);
		Debug.Log ("clickSelectEvent " + name);
		switch (name) {
		case "test" : startFade (0, "colorTest");break;
		case "video" :	tryStartVideo (); break;
		default: break;	}			
	}

	void tryStartVideo(){
		if (globalData.getTrReadyVideo ()) {startFade (0, "Description");
		} else {
			Debug.Log ("Error play video. Videos are not downloaded");
			GameObject rootCanvas = GameObject.Find ("CanvasMovie");
			if (rootCanvas == null) {
				addOevrlayInfo ("Videos are not downloaded. Please wait.");
				//showProgressBar ();		
			}
		}
	}

	void showProgressBar (){
		Debug.Log ("start progress bar");
		if(!globalData.getTrReadyVideo()){
			GameObject canvasProgressBar = new GameObject("CanvasProgress");
			//canvasProgressBar.transform.SetParent (Camera.main.transform, true);
			Canvas c = canvasProgressBar.AddComponent<Canvas>();
			c.renderMode = RenderMode.WorldSpace;
			canvasProgressBar.AddComponent<CanvasScaler>();

			RectTransform NewCanvasRect = canvasProgressBar.GetComponent<RectTransform> ();
			NewCanvasRect.sizeDelta = new Vector2 (20,5);
			NewCanvasRect.localScale = new Vector3 (1f, 1f, 1f);
			NewCanvasRect.localPosition = new Vector3 (0, 12, -22);
			GameObject panel = new GameObject("Panel");

			panel.AddComponent<CanvasRenderer>();
			Image i = panel.AddComponent<Image>();
			i.color = new Vector4(0.5f, 0.5f, 0.5f, 1f);
			RectTransform panelTransform = panel.GetComponent<RectTransform> ();
			panel.transform.SetParent(NewCanvasRect, true);
			panelTransform.localScale = new Vector3 (1f, 1f, 1f);
			panelTransform.localPosition = new Vector3 (0, 0, 0);			
			panelTransform.sizeDelta = new Vector2 (200,50);

			GameObject sliderBack = new GameObject("sliderBack");
			sliderBack.transform.SetParent (panelTransform, true);
			Image back = sliderBack.AddComponent<Image> ();
			back.color = new Vector4 (0.2f, 0.2f, 0.2f, 1);
			RectTransform trBack = sliderBack.GetComponent<RectTransform> ();
			trBack.sizeDelta = new Vector2 (180,10);
			trBack.localPosition = new Vector3 (0, -8, 0);

			GameObject sliderFrwd = new GameObject("sliderFrwd");
			sliderFrwd.transform.SetParent (panelTransform, true);
			Image Frwd = sliderFrwd.AddComponent<Image> ();
			Frwd.color = new Vector4 (0.2f, 0.8f, 0.2f, 1);
			RectTransform progressBar = sliderFrwd.GetComponent<RectTransform> ();
			progressBar.pivot = new Vector2 (0,0.5f);
			progressBar.sizeDelta = new Vector2 (0,10);
			progressBar.rotation = Quaternion.AngleAxis(-180, Vector3.up);
			progressBar.localPosition = new Vector3 (90, -8, 0);

			string txt = "Downloading videos...";
			CreateText(panelTransform, -40, 20, 260, 30, txt, 12, 0, TextAnchor.LowerLeft);
			RectTransform txtObjTransform = panel.GetComponent<RectTransform> ();
			txtObjTransform.localScale = new Vector3 (0.1f, 0.1f, 1f);
			StartCoroutine (updateProgress(canvasProgressBar,progressBar));
		}else{
			Debug.Log("Video already downloaded");
		}
	}

	IEnumerator updateProgress(GameObject canvasProgressBar, RectTransform progressBar){ 
		bool progress = false;
		while (!progress) { 
			progress = globalData.getTrReadyVideo ();	
			//Debug.Log ("main progress: ="+globalData.getDownloadingProgress ());
			progressBar.sizeDelta = new Vector2 (globalData.getDownloadingProgress () * 1.8f, 10);
			yield return new WaitForSeconds (2);
		}
		progressBar.sizeDelta = new Vector2 (100 * 1.8f, 10);
		for (int i = 0; i < 2; i++) {
				yield return new WaitForSeconds (0.1f);
		}
		foreach (Transform child in canvasProgressBar.transform) { Destroy (child.gameObject);}
		Destroy (canvasProgressBar);
		yield return null; 
	}

	void jsonToObj(string jStr){
		bool readyJson = false;
		try{
			Debug.Log ("json: start");
			CurDataTutorial loadedData = JsonUtility.FromJson<CurDataTutorial>(jStr);
			Debug.Log ("json: 0"+jStr+"="+loadedData.name);
			globalData.addTutorial(loadedData);
			globalData.setCurrentTutorial(loadedData.name);
			Debug.Log ("json: 001");
			readyJson = true;
		}catch(System.Exception e){
			Debug.Log ("error parse json ="+jStr +" error: "+e);
			globalData.server.putDataString (false,"\"json\":\"Error parse json \"");
			addOevrlayInfo ("error parse json");
		}
		if (readyJson) {
			Debug.Log ("json 1: ");
			//Debug.Log ("json 1: " + loadedData.parts[0].name+" part2 "+loadedData.parts[0].answers[1].name+" part3 "+loadedData.parts[0].videos[0].fileName);						
			addOevrlayInfo ("Establish connection to server.");
			Debug.Log ("json 2:");
			globalData.server.getAllVideo("tut1");
			Debug.Log ("json 3:");
			showProgressBar ();	
			Debug.Log ("json: 4");
		}
	}

		IEnumerator GetRequestJson(string name){
		string urlServer = globalData.getServerAddress ();
		UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(urlServer + name);        
			yield return www.SendWebRequest(); 
			if (www.isNetworkError || www.isHttpError) {
				Debug.Log ("network error!!! "+www.error +" responseCode:"+www.responseCode +" request: "+ www.url);
				addOevrlayInfo ("Error connection to server. Error code: "+www.responseCode);
			} else {
				Debug.Log ("net ansver: " + www.responseCode+" size="+www.downloadedBytes +" progress="+www.downloadProgress);
				jsonToObj(www.downloadHandler.text);
			}
		}
}
