// bugs
// android stop without network connections !!!!!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class Main : MonoBehaviour
{
    private Text priceText;
    private Text overlayText;
    private string curMovie;
    private float defaultTime = 3f;
    private float workTime;
    private bool isTimer;
    private Image imgTimedGaze;
    private string curfocusObj = "";
    private bool show_debug = false;
    public GameObject camFade;
    public GameObject camTimedPointer;
    private Material timedPointer;
    private GameObject rootMenu;

    void Start()
    {
        timedPointer = camTimedPointer.GetComponent<Renderer>().material;
        showOevrlayInfo("Starting app!");
        Color color = new Color(0.2f, 0.2f, 0.2f, 1);
        //StartCoroutine(fadeScene(1.0f, true, color, "Main"));
        startMain();
        //showTutorialContent("btnModel");
        //showProgressBar();


    }

    IEnumerator fadeScene(float duration, bool startNewScene, Color color, string sceneName)
    {
        camFade.GetComponent<Renderer>().enabled = true;
        Debug.Log("Start fade scene " + color);
        float startTransparent = 0f;
        float endTransparent = 1f;
        float smoothness = 0.005f;
        float progress = 0;
        float increment = smoothness / duration; //The amount of change to apply.
        if (startNewScene == true)
        {
            startTransparent = 1f;
            endTransparent = 0f;
        }
        Color colorStart = new Color(color.r, color.g, color.b, startTransparent);
        camFade.GetComponent<Renderer>().materials[0].color = colorStart;
        Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, endTransparent);
        while (progress < 1)
        {
            progress += increment;
            camFade.GetComponent<Renderer>().materials[0].color = Color.Lerp(colorStart, colorEnd, progress);
            yield return new WaitForSeconds(smoothness);
        };
        yield return null;
        if (startNewScene == true)
        {
            camFade.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            Debug.Log("Start scene " + sceneName);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }

    IEnumerator FadeCanvas(float duration, float transparent, GameObject fadeCanvas, GameObject fadeObj, Color colorStart, string scene)
    {
        float smoothness = 0.005f; float progress = 0; float increment = smoothness / duration; //The amount of change to apply.
        float newTransparent = 0; if (transparent == 0) { newTransparent = 1; }
        Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, newTransparent);
        while (progress < 1)
        {
            progress += increment;
            fadeObj.GetComponent<Image>().color = Color.Lerp(colorStart, colorEnd, progress);
            yield return new WaitForSeconds(smoothness);
        };
        yield return null;
        if (transparent == 1)
        {
            Destroy(fadeObj);
            Destroy(fadeCanvas);
            // ScreenCapture.CaptureScreenshot ("Screenshot.png");
        }
        else
        {
            Debug.Log("Start scene" + scene);
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        }
    }



    void startMain()
    {
        //Debug.Log("Start color test tr=" + globalData.trColorTest1);
        isTimer = false;
        workTime = defaultTime;
        //checkDevice();
        Debug.Log("Hello from Main");
        addOevrlayInfo(": Start Main");
        //		List<int> selNames = new List<int>(){1,0,3,4,5,6,7};
        //		globalData.addcTest(selNames);
        //if (globalData.trColorTest1 == 0) {	     
        showMainMenu();
        addOevrlayInfo(": Main Created 1");
        //}else {	movieSelect();}			
        //string jStr = globalData.getLocalTutorialString ("tut1.json");
        //Debug.Log ("jStr="+jStr);
        StartCoroutine(GetRequestJson("vprof1_0.json"));
        //jsonToObj(jStr);
        addOevrlayInfo(": Start Network");
        //globalData.server.putDataString(false, "\"deviceZone\":\"error\"");
    }

    private void checkDevice()
    {

        string serverTestString = "";
        addOevrlayInfo("DeviceInfo: Start");
        try
        {
            System.TimeSpan travelTime = System.DateTime.Now - System.DateTime.UtcNow;
            //PlayerPrefs.SetString("zone",  travelTime.Hours.ToString());
            serverTestString += "\"zone\":\"" + travelTime.Hours + "\"";
        }
        catch (System.Exception e)
        {
            Debug.Log("Error net =" + e);
            globalData.server.putDataString(false, "\"deviceZone\":\"error\"");
            serverTestString += "zone:error";
        }
        addOevrlayInfo(": Start Device 2");
        Debug.Log("Start device");
        try
        {
            serverTestString += ",\"ip\":\"" + Network.player.ipAddress + "\"" +
            ",\"model\":\"" + SystemInfo.deviceModel + "\"" +
                //serverTestString+=","type":""+SystemInfo.deviceType+"\"";
                ",\"os\":\"" + SystemInfo.operatingSystem + "\"";
        }
        catch (System.Exception e)
        {
            Debug.Log("deviceid error" + e);
            globalData.server.putDataString(false, "\"deviceData\":\"error\"");
        }
        //Debug.Log("serverTestString="+serverTestString);
        addOevrlayInfo("DeviceInfo:" + serverTestString);
        globalData.addDeviceData(serverTestString);
        //ServerConnection serv = new ServerConnection ();
        globalData.server.putDataString(true, serverTestString);
        addOevrlayInfo("DeviceInfo: Sended");
    }

    private void createMovieButton(string name, float x, float y, Texture2D tex, RectTransform root, bool active)
    {
        Debug.Log("movie btn " + name);
        GameObject button = new GameObject(name);
        RawImage img = button.AddComponent<RawImage>();
        img.texture = tex;
        if (!active)
        {
            img.color = new Color(1, 1, 1, 0.5f);
        }
        //img.material = mat;
        Transform br = button.GetComponent<RectTransform>();
        br.SetParent(root, true);
        br.localRotation = Quaternion.AngleAxis(180, Vector3.up);
        br.localPosition = new Vector3(x, y, 1);
        br.localScale = new Vector3(1, 0.5f, 1);
        if (active)
        {
            EventTrigger be = button.AddComponent<EventTrigger>();
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
        else {
            //Debug.Log(name);           
            Material myNewMaterial = new Material(Shader.Find("GUI/Text Shader"));            
            GameObject txtObj = CreateText(br, 0, 0, 100, 20, "Available soon", 14, 0, TextAnchor.MiddleCenter);
            txtObj.GetComponent<Text>().material = myNewMaterial;
        }
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
            NewCanvasRect.localRotation = Quaternion.Euler(0, 180, 0);
            newCanvas.transform.SetParent(Camera.main.transform, true);
            NewCanvasRect.localPosition = new Vector3(0, -5f, 10);
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

    private void showMainMenu()
    {
        rootMenu = new GameObject("rootMenu");
        string msg = "This app allows to choose your future profession.\nPlease path the psychological test before.\n";
        GameObject newCanvas = new GameObject("CnvsMainMenu");
        newCanvas.transform.SetParent(rootMenu.transform);
        Canvas c = newCanvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        newCanvas.AddComponent<CanvasScaler>();
        newCanvas.AddComponent<GvrPointerGraphicRaycaster>();
        RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform>();
        NewCanvasRect.sizeDelta = new Vector2(1200, 300);
        NewCanvasRect.localScale = new Vector3(0.07f, 0.07f, 1f);
        NewCanvasRect.localPosition = new Vector3(0, 9, -10);
        GameObject panel = new GameObject("Panel");
        panel.AddComponent<CanvasRenderer>();
        Image i = panel.AddComponent<Image>();
        i.color = new Vector4(1, 1, 1, 0.7f);
        RectTransform panelTransform = panel.GetComponent<RectTransform>();
        panel.transform.SetParent(NewCanvasRect, true);
        panelTransform.localScale = new Vector3(0.2f, 0.2f, 1f);
        panelTransform.localPosition = new Vector3(0, 0, 0);
        panelTransform.sizeDelta = new Vector2(1200, 300);
        //panelTransform.rotation = Quaternion.AngleAxis(-180, Vector3.up);

        CreateText(panelTransform, 0, 16, 1200, 300, msg, 50, 0, TextAnchor.MiddleCenter);
        GameObject bt0 = new GameObject("btnTest");
        RectTransform br = bt0.AddComponent<RectTransform>();
        br.sizeDelta = new Vector2(300, 60);
        Image img = bt0.AddComponent<Image>();
        img.color = new Vector4(0.5f, 0.5f, 0.8f, 1);
        img.material.color = new Vector4(1f, 1f, 1f, 0.7f);
        Button bt = bt0.AddComponent<Button>();
        bt0.transform.SetParent(panelTransform, true);
        bt0.transform.localPosition = new Vector3(33, -80, 0);
        //bt0.transform.rotation = Quaternion.AngleAxis(-180, Vector3.up);
        bt0.transform.localScale = new Vector3(1, 1, 1);
        CreateText(bt.transform, 0, 0, 300, 50, "Start test", 40, 1, TextAnchor.MiddleCenter);
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

        showTutorialsMenu(rootMenu);
    }

    void Update()
    {
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

    public void HandleClickStartColorTest()
    {
        onExitTimed();

        Color color = new Color(0.2f, 0.2f, 0.2f, 0f);
        StartCoroutine(fadeScene(1.0f, false, color, "colorTest"));
        //startFade(0, "colorTest");
    }
    public void HandleClickStartVideo()
    {
        onExitTimed();
        //tryStartVideo();
    }
    public void onClickTimed()
    {
        clickSelectEvent(curfocusObj);
    }
    public void onEnterTimed(string name)
    {
        isTimer = true;
        workTime = defaultTime;
        timedPointer.SetFloat("_Angle", 0);
        curfocusObj = name;
    }
    public void onExitTimed()
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
        switch (name)
        {
            case "test":
                Color color = new Color(0.2f, 0.2f, 0.2f, 0f);
                StartCoroutine(fadeScene(1.0f, false, color, "colorTest"));
                break;
            case "btnModel":
                Debug.Log("clickSelectEvent 2 =" + name + "=");
                Destroy(rootMenu);
                showTutorialContent(name);
                break;
            case "btnToMain":
                Debug.Log("clickSelectEvent 3 =" + name + "=");
                GameObject progressVideo = GameObject.Find("CnvsProgressVideo");
                if (progressVideo != null)
                {
                    Transform trans = progressVideo.GetComponent<Transform>();
                    Debug.Log(trans.localPosition); // +"_"+ progressVideo.transform.parent.name
                    trans.parent = null;
                    trans.localPosition = new Vector3(0, -2.5f, 0);
                    trans.localRotation = Quaternion.Euler(0, 0, 0);
                    //showProgressBar ();		
                }
                Destroy(rootMenu);
                showMainMenu();
                break;
            case "Part_1":
                globalData.setCurVideoName(0);
                tryStartVideo();
                break;
            case "Part_2":
                globalData.setCurVideoName(1);
                tryStartVideo();
                break;
            case "Part_3":
                globalData.setCurVideoName(2);
                tryStartVideo();
                break;
            default:
                Debug.Log("clickSelectEvent not found action for " + name);
                break;
        }
    }

    void tryStartVideo()
    {
        //globalData.setTrReadyVideo(true);

        Debug.Log("tryStartVideo start");
        if (globalData.getTrReadyVideo())
        {
            Debug.Log("tryStartVideo start fade");
            StartCoroutine(fadeScene(1.0f, false, new Color(0.2f, 0.2f, 0.2f, 0), "Video2"));

            //startFade(0, "Description");
        }
        else
        {
            Debug.Log("Error play video. Videos are not downloaded");
            GameObject rootCanvas = GameObject.Find("CanvasMovie");
            if (rootCanvas == null)
            {
                addOevrlayInfo("Videos are not downloaded. Please wait.");
                GameObject progressVideo = GameObject.Find("CnvsProgressVideo");
                if (progressVideo != null) { 
                    Transform trans = progressVideo.GetComponent<Transform>();
                    Debug.Log(trans.localPosition); // +"_"+ progressVideo.transform.parent.name
                    if (trans.localPosition.z < 1)
                    {
                        trans.SetParent(Camera.main.transform);
                        trans.localPosition = new Vector3(0, -1, 3);
                        trans.localRotation = Quaternion.Euler(0, 180, 0);
                    }
                
                    //trans.localPosition = new Vector3(0,0,0);
                    //showProgressBar ();		
                }
            }
        }
    }

    void showProgressBar()
    {
        Debug.Log("start progress bar");
        addOevrlayInfo("start progress bar");
        if (!globalData.getTrReadyVideo())
        {
            GameObject canvasProgressBar = new GameObject("CnvsProgressVideo");
            //canvasProgressBar.transform.SetParent (Camera.main.transform, true);
            Canvas c = canvasProgressBar.AddComponent<Canvas>();
            c.renderMode = RenderMode.WorldSpace;
            canvasProgressBar.AddComponent<CanvasScaler>();

            RectTransform NewCanvasRect = canvasProgressBar.GetComponent<RectTransform>();            
            NewCanvasRect.sizeDelta = new Vector2(20, 5);
            NewCanvasRect.localScale = new Vector3(0.1f, 0.1f, 1f);
            NewCanvasRect.localPosition = new Vector3(0, -2.5f, 0);
            GameObject panel = new GameObject("Panel");

            panel.AddComponent<CanvasRenderer>();
            Image i = panel.AddComponent<Image>();
            i.color = new Vector4(0.5f, 0.5f, 0.5f, 1f);
            RectTransform panelTransform = panel.GetComponent<RectTransform>();
            panel.transform.SetParent(NewCanvasRect, true);
            panelTransform.localScale = new Vector3(1f, 1f, 1f);
            panelTransform.localPosition = new Vector3(0, 0, 0);
            panelTransform.sizeDelta = new Vector2(200, 50);

            GameObject sliderBack = new GameObject("sliderBack");
            sliderBack.transform.SetParent(panelTransform, true);
            Image back = sliderBack.AddComponent<Image>();
            back.color = new Vector4(0.2f, 0.2f, 0.2f, 1);
            RectTransform trBack = sliderBack.GetComponent<RectTransform>();
            trBack.sizeDelta = new Vector2(180, 10);
            trBack.localPosition = new Vector3(0, -8, 0);
            trBack.localScale = new Vector3(1, 1, 1);

            GameObject sliderFrwd = new GameObject("sliderFrwd");
            sliderFrwd.transform.SetParent(panelTransform, true);
            Image Frwd = sliderFrwd.AddComponent<Image>();
            Frwd.color = new Vector4(0.2f, 0.8f, 0.2f, 1);
            RectTransform progressBar = sliderFrwd.GetComponent<RectTransform>();
            progressBar.pivot = new Vector2(0, 0.5f);
            progressBar.sizeDelta = new Vector2(0, 10);
            progressBar.rotation = Quaternion.AngleAxis(-180, Vector3.up);
            progressBar.localScale = new Vector3(1, 1, 1);
            progressBar.localPosition = new Vector3(90, -8, 0);            
            string txt = "Downloading video: "+ globalData.getDownloadingProgress().ToString()+"%";            
            GameObject txtObj = CreateText(panelTransform, -40, 20, 260, 30, txt, 12, 0, TextAnchor.LowerLeft);
            Text progressBarText = txtObj.GetComponent<Text>();
            RectTransform txtObjTransform = panel.GetComponent<RectTransform>();
            txtObjTransform.localScale = new Vector3(0.1f, 0.1f, 1f);
            addOevrlayInfo("start progress bar coroutine");
            StartCoroutine(updateProgress(canvasProgressBar, progressBar, progressBarText));
        }
        else
        {
            Debug.Log("Video already downloaded");
            addOevrlayInfo("Video alredy exist");
        }
    }

    IEnumerator updateProgress(GameObject canvasProgressBar, RectTransform progressBar, Text progressBarText)
    {
        addOevrlayInfo("progress bar 2");
        bool progress = false;
        while (!progress)
        {
            progress = globalData.getTrReadyVideo();
            progressBarText.text = "Downloading video: " + globalData.getDownloadingProgress() + "%";
            Debug.Log ("main progress: ="+ progressBarText.text);
            progressBar.sizeDelta = new Vector2(globalData.getDownloadingProgress() * 1.8f, 10);
            yield return new WaitForSeconds(2);
        }
        addOevrlayInfo("progress bar 2");
        progressBar.sizeDelta = new Vector2(100 * 1.8f, 10);
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.1f);
        }
        addOevrlayInfo("Progress bar 3");
        foreach (Transform child in canvasProgressBar.transform) { Destroy(child.gameObject); }
        Destroy(canvasProgressBar);
        yield return null;
        addOevrlayInfo("Progress bar end!");
    }

    void jsonToObj(string jStr)
    {
        bool readyJson = false;
        try
        {
            Debug.Log("json: start");
            addOevrlayInfo("Get json start");
            CurDataTutorial loadedData = JsonUtility.FromJson<CurDataTutorial>(jStr);
            Debug.Log("json: 0" + jStr + "=" + loadedData.name);
            globalData.addTutorial(loadedData);
            globalData.setCurrentTutorial(loadedData.name);
            Debug.Log("json: 001");
            readyJson = true;
            addOevrlayInfo("json ready");
        }
        catch (System.Exception e)
        {
            Debug.Log("error parse json =" + jStr + " error: " + e);
            globalData.server.putDataString(false, "\"json\":\"Error parse json \"");
            addOevrlayInfo("error parse json");
        }
        if (readyJson)
        {
            Debug.Log("json 1: ");
            //Debug.Log ("json 1: " + loadedData.parts[0].name+" part2 "+loadedData.parts[0].answers[1].name+" part3 "+loadedData.parts[0].videos[0].fileName);						
            addOevrlayInfo("Establish connection to server.");
            Debug.Log("json 2:");
            globalData.server.getAllVideo("tut1");
            Debug.Log("json 3:");
            showProgressBar();
            Debug.Log("json: 4");
            addOevrlayInfo("Establish connection to server. end");
        }
    }

    IEnumerator GetRequestJson(string name)
    {
        string urlServer = globalData.getServerAddress();
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(urlServer + name);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("network error!!! " + www.error + " responseCode:" + www.responseCode + " request: " + www.url);
            addOevrlayInfo("Error connection to server. Error code: " + www.responseCode);
        }
        else
        {
            Debug.Log("net ansver: " + www.responseCode + " size=" + www.downloadedBytes + " progress=" + www.downloadProgress);
            jsonToObj(www.downloadHandler.text);
            addOevrlayInfo("connection to server ok. Responce code: " + www.responseCode);
        }
    }
    private void showTutorialsMenu(GameObject rootMenu)
    {
        Debug.Log("movie select");
        addOevrlayInfo("Show movies");
        GameObject newCanvas = new GameObject("CnvsMovie");
        newCanvas.transform.SetParent(rootMenu.transform);
        Canvas c = newCanvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        newCanvas.AddComponent<CanvasScaler>();
        newCanvas.AddComponent<GvrPointerGraphicRaycaster>();
        RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform>();
        NewCanvasRect.sizeDelta = new Vector2(1200, 700);
        NewCanvasRect.localScale = new Vector3(0.07f, 0.07f, 1f);
        NewCanvasRect.localPosition = new Vector3(0, 0, -10);
        GameObject panelMain = new GameObject("PanelMain");
        panelMain.AddComponent<CanvasRenderer>();
        Image i = panelMain.AddComponent<Image>();
        i.color = new Vector4(1, 1, 1, 0.7f);
        RectTransform mainTransform = panelMain.GetComponent<RectTransform>();
        panelMain.transform.SetParent(NewCanvasRect, true);
        mainTransform.localScale = new Vector3(1, 1, 1);
        mainTransform.localPosition = new Vector3(0, 0, 0);
        mainTransform.sizeDelta = new Vector2(240, 120);
        GameObject panelName = new GameObject("PanelTextName");
        panelName.transform.SetParent(NewCanvasRect, true);
        //panelName.AddComponent<CanvasRenderer>();
        Image i2 = panelName.AddComponent<Image>();
        i2.color = new Vector4(1, 1, 1, 0.7f);
        RectTransform rt = panelName.GetComponent<RectTransform>();
        rt.localScale = new Vector3(0.2f, 0.2f, 1);
        rt.localPosition = new Vector3(0, 78, 0);
        rt.localRotation = Quaternion.AngleAxis(180, Vector3.up);
        rt.sizeDelta = new Vector2(1200, 100);
        //Material mat = new Material(Shader.Find("Diffuse"));
        //Material mat0 = new Material (Shader.Find(" Diffuse"));
        CreateText(rt, 0, 0, 1200, 100, "The movies list", 50, 1, TextAnchor.MiddleCenter);
        Texture2D tex1 = (Texture2D)Resources.Load("Textures/dayModel");
        createMovieButton("btnModel", 60, 30, tex1, mainTransform, true);
        Texture2D tex2 = (Texture2D)Resources.Load("Textures/dayCoach");
        createMovieButton("btnCach", -60, 30, tex2, mainTransform, false);
        Texture2D tex3 = (Texture2D)Resources.Load("Textures/dayDeveloper");
        createMovieButton("btnDeveloper", 60, -30, tex3, mainTransform, false);
        Texture2D tex4 = (Texture2D)Resources.Load("Textures/daySales");
        createMovieButton("btnSales", -60, -30, tex4, mainTransform, false);
        addOevrlayInfo("Show movies ok");

    }

    private void showTutorialContent(string name)
    {
        Debug.Log("Show content for " + name);
        rootMenu = new GameObject("rootMenu");
        GameObject newCanvas = new GameObject("CnvsTutC_" + name);
        newCanvas.transform.SetParent(rootMenu.transform);
        Canvas c = newCanvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        newCanvas.AddComponent<CanvasScaler>();
        newCanvas.AddComponent<GvrPointerGraphicRaycaster>();
        RectTransform NewCanvasRect = newCanvas.GetComponent<RectTransform>();
        NewCanvasRect.sizeDelta = new Vector2(1200, 700);
        NewCanvasRect.localScale = new Vector3(0.07f, 0.07f, 1f);
        NewCanvasRect.localPosition = new Vector3(0, 1, -10);
        GameObject panelMain = new GameObject("PanelMain");
        panelMain.AddComponent<CanvasRenderer>();
        Image i = panelMain.AddComponent<Image>();
        i.color = new Vector4(1, 1, 1, 0.7f);
        RectTransform mainTransform = panelMain.GetComponent<RectTransform>();
        panelMain.transform.SetParent(NewCanvasRect, true);
        mainTransform.localScale = new Vector3(1, 1, 1);
        mainTransform.localPosition = new Vector3(0, 3, 0);
        mainTransform.sizeDelta = new Vector2(240, 120);

        GameObject bt1_0 = new GameObject("btnBackMain");
        RectTransform br2 = bt1_0.AddComponent<RectTransform>();
        br2.sizeDelta = new Vector2(80, 20);
        Image img2 = bt1_0.AddComponent<Image>();
        img2.material.color = new Vector4(1f, 1f, 1f, 0.9f);
        Button bt2 = bt1_0.AddComponent<Button>();
        bt1_0.transform.SetParent(NewCanvasRect, true);
        bt1_0.transform.localPosition = new Vector3(85, -70, 0);
        bt1_0.transform.rotation = Quaternion.AngleAxis(-180, Vector3.up);
        bt1_0.transform.localScale = new Vector3(1, 1, 1);
        CreateText(bt2.transform, 0, 0, 70, 15, "<< Main menu", 8, 1, TextAnchor.MiddleCenter);
        bt2.onClick.AddListener(HandleClickStartVideo);

        EventTrigger be2 = bt1_0.AddComponent<EventTrigger>();
        EventTrigger.Entry entryBack = new EventTrigger.Entry();
        entryBack.eventID = EventTriggerType.PointerClick;
        entryBack.callback.AddListener((eventData) => { clickSelectEvent("btnToMain"); });
        be2.triggers.Add(entryBack);
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { onEnterTimed("btnToMain"); });
        be2.triggers.Add(entryEnter);
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { onExitTimed(); });
        be2.triggers.Add(entryExit);

        GameObject panelName = new GameObject("PanelTextName");
        panelName.transform.SetParent(NewCanvasRect, true);
        //panelName.AddComponent<CanvasRenderer>();
        Image i2 = panelName.AddComponent<Image>();
        i2.color = new Vector4(1, 1, 1, 0.9f);
        RectTransform rt = panelName.GetComponent<RectTransform>();
        rt.localScale = new Vector3(0.2f, 0.2f, 1);
        rt.localPosition = new Vector3(0, 78, 0);
        rt.localRotation = Quaternion.AngleAxis(180, Vector3.up);
        rt.sizeDelta = new Vector2(1200, 100);
        //Material mat = new Material(Shader.Find(" Diffuse"));
        //Material mat0 = new Material (Shader.Find(" Diffuse"));
        CreateText(rt, 0, 0, 500, 70, "How it be a model", 50, 1, TextAnchor.MiddleCenter);
        Texture2D tex1 = (Texture2D)Resources.Load("Textures/part_1");
        createItemButton("Part_1", 80, 0, tex1, mainTransform,  true, "Part 1\nThe first meeting");
        Texture2D tex2 = (Texture2D)Resources.Load("Textures/Part_2");
        createItemButton("Part_2", 0, 0, tex2, mainTransform,  true, "Part 2\nThe photo session");
        Texture2D tex3 = (Texture2D)Resources.Load("Textures/part_1");
        createItemButton("Part_3", -80, 0, tex3, mainTransform,  true, "Part 3\nInformation\nabout a carrier");

    }
    private void createItemButton(string name, float x, float y, Texture2D tex, RectTransform root, bool active, string Description)
    {
        Debug.Log("movie btn name=" + name);
        GameObject panelMain = new GameObject("Panel_" + name);
        panelMain.AddComponent<CanvasRenderer>();
        Image i = panelMain.AddComponent<Image>();
        //i.color = new Vector4(1, 1, 1, 0.7f);
        RectTransform mainTransform = panelMain.GetComponent<RectTransform>();
        mainTransform.SetParent(root, true);
        mainTransform.localScale = new Vector3(1, 1, 1);
        mainTransform.sizeDelta = new Vector2(65, 110);
        mainTransform.localPosition = new Vector3(x, y, 0);
        CreateText(mainTransform, 0, -26, 100, 30, Description, 8, 0, TextAnchor.MiddleCenter);
        GameObject button = new GameObject("btn" + name);
        RawImage img = button.AddComponent<RawImage>();
        img.texture = tex;
        if (!active)
        {
            img.color = new Color(1, 1, 1, 0.5f);
        }
        //img.material = mat;
        Transform br = button.GetComponent<RectTransform>();
        br.SetParent(mainTransform, true);
        br.localScale = new Vector3(0.5f, 0.5f, 1);
        br.localRotation = Quaternion.AngleAxis(180, Vector3.up);
        br.localPosition = new Vector3(0, 20, 0.2f);

        if (active)
        {
            EventTrigger be = button.AddComponent<EventTrigger>();

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

    }
}
