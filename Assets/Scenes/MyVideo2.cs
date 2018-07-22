// TODO Add video control panel and show video current time 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyVideo2 : MonoBehaviour {
	public VideoPlayer player;
	public AudioSource audioSource;
	public GameObject stopIcon;
	private float rotate;
	private bool show_debug = false;
	private Text overlayText;
	private bool trAudio = false;
	private string curfocusObj = "";
    public GameObject camTimedPointer;
    private Material timedPointer;
	private float defaultTime = 3f;
    private float workTime;
    private bool isTimer;
    private Image imgTimedGaze;
	// Use this for initialization
	void Start () {
		showOevrlayInfo("Starting app!");
		Debug.Log ("start video!");
		timedPointer = camTimedPointer.GetComponent<Renderer>().material;
		trAudio = false;
		showStartMsg();
		showControlBar();
//		rotate = 90;
//		string urlVideo = "C:/Users/del4600/AppData/LocalLow/Ongoza/VRprof/vrprof1_1.mp4";
//		string urlAudio = "C:/Users/del4600/AppData/LocalLow/Ongoza/VRprof/vrprof1_1.mp3";
		globalData.server.putDataString (false,"\"video2\":\"start player! 0\"");
        Debug.Log("start video! 2");
		try{
        	int index = globalData.getCurVideoName ();
        	Debug.Log("start video! 3 index = "+index);
			addOevrlayInfo("start video! 3 index =" + index);		
        	string curIndex = index.ToString();
			string urlVideo = globalData.getVideo (curIndex);
			string urlAudio = globalData.getAudio (curIndex);
			CurDataVideo video = globalData.getVideoData (index);		 
			rotate = video.rotate;
			Debug.Log ("start video player! rotate="+rotate+" urls="+urlVideo+"="+urlAudio);
			addOevrlayInfo("startvideo player! rotate="+rotate+" urls="+urlVideo+"="+urlAudio);
			globalData.server.putDataString (false,"\"video2\":\"start player! 1 rotate="+rotate+" urls="+urlVideo+"="+urlAudio+"\"");
			if (urlVideo != null) 
			{
				if (urlAudio != null) 
				{
					try{
						StartCoroutine(LoadFile (urlAudio));
					}catch (System.Exception e)
					{
						Debug.Log ("can not start play audio! error " + e);
						addOevrlayInfo("start audio error! can not start audio");
					}		
					player.url = urlVideo;
					addOevrlayInfo("start local video open");
					player.prepareCompleted += Prepared;
					player.loopPointReached += EndReached;
					player.Prepare ();
				}
			} else {
				Debug.Log ("error start play! " + urlVideo);
				addOevrlayInfo("error start video video="+urlVideo);
				globalData.server.putDataString (false,"\"video2\":\"start player! error urlVideo\"");
			}
		}
		catch (System.Exception e)
		{
			Debug.Log ("can not start play! error " + e);
			addOevrlayInfo("start video error! can not start video");
		}		
	}
	
	private void showStartMsg(){
		GameObject newCanvas = new GameObject("MsgCanvas");
        Canvas c = newCanvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        newCanvas.AddComponent<CanvasScaler>();
        RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform>();
        //NewCanvasRect.Rotate(120,40,0);
        NewCanvasRect.localRotation = Quaternion.Euler(0, 0, 0);
        newCanvas.transform.SetParent(Camera.main.transform, true);
        NewCanvasRect.localPosition = new Vector3(0, -0.8f, 7);
        NewCanvasRect.sizeDelta = new Vector2(1200, 200);
        NewCanvasRect.localScale = new Vector3(0.1f, 0.1f, 1f);
        GameObject panel = new GameObject("msgPanel");
        panel.AddComponent<CanvasRenderer>();
        Image i = panel.AddComponent<Image>();
        i.color = new Vector4(1f, 1f, 1f, 1f);
        RectTransform panelTransform = panel.GetComponent<RectTransform>();
        panel.transform.SetParent(NewCanvasRect, true);
        panelTransform.localScale = new Vector3(0.3f, 0.3f, 1f);
        panelTransform.localRotation = Quaternion.AngleAxis(0, Vector3.right);
        panelTransform.localPosition = new Vector3(0, 0, 0);
        panelTransform.sizeDelta = new Vector2(100, 12);
        GameObject txtObj = CreateText(panelTransform, 5, -1, 100, 12, "For exit push button \"Stop\"", 8, 0, TextAnchor.UpperLeft);
        txtObj.transform.localRotation = Quaternion.AngleAxis(0, Vector3.right);

		GameObject button = new GameObject("array");
		Texture2D tex = (Texture2D)Resources.Load("Textures/arroy");
        RawImage img = button.AddComponent<RawImage>();
        img.texture = tex;
		Transform br = button.GetComponent<RectTransform>();
        br.SetParent(panelTransform, true);
        br.localScale = new Vector3(0.3f, 0.3f, 1);
        br.localRotation = Quaternion.AngleAxis(180, Vector3.up);
        br.localPosition = new Vector3(0, -30, 0.2f);
		StartCoroutine(fadeMsg(1.5f, newCanvas, panel, button));
	}

	IEnumerator fadeMsg(float duration, GameObject rootMsg, GameObject panel, GameObject array)
    {           
        float smoothness = 0.05f;
        float progress = 0;
        float increment = smoothness / duration; //The amount of change to apply.        
        Color colorStart = new Color(1, 1, 1, 1);       
        Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0);
        while (progress < 1)
        {
            progress += increment;
            //Debug.Log(progress);
            panel.GetComponent<Image>().color = Color.Lerp(colorStart, colorEnd, progress);
			array.GetComponent<RawImage>().color = Color.Lerp(colorStart, colorEnd, progress);
            yield return new WaitForSeconds(smoothness);
        }
        yield return null;
		// delete root obj.
		Destroy(rootMsg);
    }

	private void showControlBar()
	{		
		EventTrigger be = stopIcon.AddComponent<EventTrigger>();
		string name = "exit";
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { clickSelectEvent(name); });
        be.triggers.Add(entry);
        EventTrigger.Entry entryEnterGaze = new EventTrigger.Entry();
        entryEnterGaze.eventID = EventTriggerType.PointerEnter;
        entryEnterGaze.callback.AddListener((eventData) => { onEnterTimed(name); });
        be.triggers.Add(entryEnterGaze);
        EventTrigger.Entry entryExitGaze = new EventTrigger.Entry();
        entryExitGaze.eventID = EventTriggerType.PointerExit;
        entryExitGaze.callback.AddListener((eventData) => { onExitTimed(); });
        be.triggers.Add(entryExitGaze);
	}

	private void onClickTimed()
    {
        clickSelectEvent(curfocusObj);
    }
    private void onEnterTimed(string name)
    {
        isTimer = true;
        workTime = defaultTime;
        timedPointer.SetFloat("_Angle", 0);
        curfocusObj = name;
    }
    private void onExitTimed()
    {
        isTimer = false;
        curfocusObj = "";
        workTime = defaultTime;
        timedPointer.SetFloat("_Angle", 360);
    }

    private void clickSelectEvent(string name)
    {
        onExitTimed();
        addOevrlayInfo("clickSelectEvent" + name);
        Debug.Log("clickSelectEvent=" + name + "=");
		globalData.setCurVideoName (1);
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
	}
	private void addOevrlayInfo(string txt)
    {
        if (show_debug)
        {
            string[] lines = overlayText.text.Split('\n');
            string newTxt = ""; int len = lines.Length;
            if (len > 13) { len = 13; }
            for (int i = len; i > 0; i--) { newTxt += "\n" + lines[lines.Length - i]; }
            overlayText.text = txt + newTxt;
        }
    }

	private void showOevrlayInfo(string txt)
    {
        if (show_debug)
        {
            GameObject newCanvas = new GameObject("InfoCanvas");
            Canvas c = newCanvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.WorldSpace;
            newCanvas.AddComponent<CanvasScaler>();
            RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform>();
            //NewCanvasRect.Rotate(120,40,0);
            NewCanvasRect.localRotation = Quaternion.Euler(0, 0, 0);
            newCanvas.transform.SetParent(Camera.main.transform, true);
            NewCanvasRect.localPosition = new Vector3(0, 0f, 7);
            NewCanvasRect.sizeDelta = new Vector2(1200, 200);
            NewCanvasRect.localScale = new Vector3(0.1f, 0.1f, 1f);
            GameObject panel = new GameObject("InfoPanel");
            panel.AddComponent<CanvasRenderer>();
            Image i = panel.AddComponent<Image>();
            i.color = new Vector4(1f, 1f, 1f, 1f);
            RectTransform panelTransform = panel.GetComponent<RectTransform>();
            panel.transform.SetParent(NewCanvasRect, true);
            panelTransform.localScale = new Vector3(0.3f, 0.3f, 1f);
            panelTransform.localRotation = Quaternion.AngleAxis(0, Vector3.right);
            panelTransform.localPosition = new Vector3(0, 0, 0);
            panelTransform.sizeDelta = new Vector2(300, 200);
            GameObject txtObj = CreateText(panelTransform, 20, -5, 300, 200, txt, 8, 0, TextAnchor.UpperLeft);
            txtObj.transform.localRotation = Quaternion.AngleAxis(0, Vector3.right);
            overlayText = txtObj.GetComponent<Text>();
            //overlayText.color = new Color(0, 0.8f, 0);
        }
    }
	// Update is called once per frame
	void Update () {
		        if (isTimer)
        {
            workTime -= Time.deltaTime;
            if (workTime <= 0)
            {
                onClickTimed();
                addOevrlayInfo("Click!");
            }
            else
            {
                float percent = (1f - workTime / defaultTime) * 360;
                timedPointer.SetFloat("_Angle", percent);
                //Debug.Log("onUpdate =" + percent);
            }
        }
	}

private GameObject CreateText(Transform parent, float x, float y, float w, float h, string message, int fontSize, int fontStyle, TextAnchor achor)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(parent);
        RectTransform trans = textObject.AddComponent<RectTransform>();
        trans.sizeDelta = new Vector2(w, h);
        trans.anchoredPosition3D = new Vector3(0, 0, 0);
        trans.anchoredPosition = new Vector2(x, y);
        trans.transform.rotation = Quaternion.AngleAxis(-180, Vector3.up);
        trans.localScale = new Vector3(1.0f, 1.0f, 0f);
        trans.localPosition.Set(0, 0, 0);
        textObject.AddComponent<CanvasRenderer>();
        Text text = textObject.AddComponent<Text>();
        text.supportRichText = true;
        text.text = message;
        text.fontSize = fontSize;
        if (fontStyle == 1) { text.fontStyle = FontStyle.Bold; }
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.alignment = achor;
        //text.alignment = TextAnchor.MiddleCenter;
        //text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.color = new Color(0, 0, 0);
        return textObject;
    }

	IEnumerator LoadFile(string path){
			//globalData.server.putDataString (false,"\"video2\":\"open audio! 0\"");
			addOevrlayInfo("start load local audio file ="+path);
			string fullPath = "file://" + path;
			WWW www = new WWW(fullPath); 			
			yield return www;
			if (www.error != null) {
				addOevrlayInfo("error load local audio file ! 0");
				globalData.server.putDataString (false,"\"video2\":\"error load local audio file ! 0\"");
				Debug.Log ("www error="+www.error);
			}
			addOevrlayInfo("local audio file loaded!");		
			AudioClip clip = www.GetAudioClip(false,false);
			clip.name = Path.GetFileNameWithoutExtension (fullPath);
			clip.LoadAudioData();
	//		Debug.Log ("audio 1 state="+clip.loadState+"="+clip.loadState.Equals("Unloaded"));
			addOevrlayInfo("start load local audio!");
			int counter = 1000;
			while ((clip.loadState == AudioDataLoadState.Unloaded) && (counter>0)) { 
				counter--;
				//Debug.Log(counter);
				yield return clip;
				if (!string.IsNullOrEmpty(www.error))
					Debug.Log("error load local audio = "+www.error);
					addOevrlayInfo("error load local audio ");
					globalData.server.putDataString (false,"\"video2\":\"error loaded audio! e="+www.error+"\"");
			}
			//globalData.server.putDataString (false,"\"video2\":\"loaded audio! 1 status="+clip.loadState+"\"");
			//Debug.Log ("Counter="+counter);
			addOevrlayInfo("local audio loading is finished!");
			if (counter < 1) {	
				Debug.Log ("Can Not load local audio");
				globalData.server.putDataString (false,"\"video2\":\"error loaded audio! 2 can not open=\"");
				addOevrlayInfo("can not open local audio");
			} else {
				Debug.Log ("local audio 2 state=" + clip.loadState);
				addOevrlayInfo("local audio loaded");
				clip.name = Path.GetFileName (path);
				audioSource.clip = clip;
				Debug.Log ("local audio opened");
				addOevrlayInfo("local audio opened");
				//globalData.server.putDataString (false,"\"video2\":\"loaded audio! 3 ok\"");
				trAudio = true;
			}
		
	}

	void Prepared(UnityEngine.Video.VideoPlayer vPlayer) {
		try {		
			Debug.Log("start video prepare");
			addOevrlayInfo("start video prepare");
			//globalData.server.putDataString (false,"\"video2\":\"prepared video! 0\"");
			Material mat = RenderSettings.skybox;
			if (rotate != 0) {
				Debug.Log ("Rotate camera video "+rotate);
				mat.SetFloat ("_Rotation", rotate);}
			//globalData.server.putDataString (false,"\"video2\":\"prepared video!1\"");			
			player.Play();
			addOevrlayInfo("video started");
			if(trAudio==true){			
				audioSource.Play();
				addOevrlayInfo("audio started");
			}else{
				addOevrlayInfo("audio did not start");
			}
		} catch(System.Exception e){
			addOevrlayInfo("error prepered local files");
			Debug.LogError ("error prepered local files "+e);
			//globalData.setCurVideoName (1);
			//UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
		}
	}

	void EndReached(UnityEngine.Video.VideoPlayer vp){
		//		vp.playbackSpeed = vp.playbackSpeed / 10.0F;
		Debug.Log("Video End reached");
		addOevrlayInfo("Video End reached");
		//int next = globalData.getCurVideoName () + 1;
		//int max = globalData.getVideosCount ();
		//if (next < max){
		//	globalData.setCurVideoName (next);
		//	UnityEngine.SceneManagement.SceneManager.LoadScene ("Description");
		//}else{
		globalData.setCurVideoName (1);
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
		//}
	}

}
