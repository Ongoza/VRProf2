using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalData
{
    //	private static string urlServer = "http://192.168.1.64:8000/static/";
    //	private static string urlDataServer = "http://192.168.1.64:27080/local/vrprof";
    private static string urlServer = "http://91.212.177.22:8008/video/";
    private static string urlDataServer = "http://91.212.177.22:27080/local/vrprof";
    private static List<int> colorTest1 = new List<int>();
    private static List<int> colorTest2 = new List<int>();
    public static ServerConnection server = new ServerConnection();
    public static int trColorTest1 = 0;
    private static string localPath = Application.persistentDataPath;
    private static bool trVideoReady = false;
    private static int downloadingProgress = 0;
    private static List<CurDataTutorial> tutorials = new List<CurDataTutorial>();
    private static Dictionary<string, string> videos = new Dictionary<string, string>();
    private static Dictionary<string, string> audios = new Dictionary<string, string>();
    //private static Dictionary<string, CurDataVideo> curDataVideo = new Dictionary<string,CurDataVideo>();
    private static CurDataTutorial curTutorial;
    private static string deviceUUID = SystemInfo.deviceUniqueIdentifier.ToString();
    private static string serverTestString;
    private static int curVideoName = 0;

    public static void addcTest(List<int> value)
    {
        if (trColorTest1 == 0)
        {
            colorTest1.AddRange(value); trColorTest1++;
        }
        else { colorTest2.AddRange(value); }
        string str = "{\"uuid\":\"" + deviceUUID + "\",\"time\":\"" + System.DateTime.UtcNow.ToString("yyyy/MM/dd/HH:mm") + "\",\"colors\":[" + string.Join(", ", value.ConvertAll(i => i.ToString()).ToArray()) + "]}";
        Debug.Log("globalData color test" + str);
        server.putDataString(true, str);
    }


    public static CurDataVideo getVideoData(int index)
    {
        CurDataVideo curVideo = null;
        CurDataVideo[] videos = getVideosList("tut1");
        if (index < videos.Length) { curVideo = videos[index]; }
        return curVideo;
    }

    public static CurDataVideo[] getVideosList(string name) { return tutorials[0].parts[0].videos; }

    public static int getVideosCount() { return videos.Count; }
    public static int getCurVideoName() { return curVideoName; }
    public static void setCurVideoName(int name) { curVideoName = name; }
    public static string getLocalPath() { return localPath; }
    public static string getDeviceUUID() { return deviceUUID; }
    public static int getDownloadingProgress() { return downloadingProgress; }
    public static void setDownloadingProgress(int pro) { downloadingProgress = pro; }
    public static void setTrReadyVideo(bool data) { trVideoReady = data; }
    public static bool getTrReadyVideo() { return trVideoReady; }
    public static void addDeviceData(string data) { serverTestString = data; Debug.Log("serverTestString" + serverTestString); }
    public static void addTutorial(CurDataTutorial value) { tutorials.Add(value); }
    public static string getServerAddress() { return urlServer; }
    public static string getDataServerAddress() { return urlDataServer; }
    public static CurDataTutorial getTutorial(string name)
    {
        CurDataTutorial selTut = null;
        foreach (CurDataTutorial tut in tutorials) { if (tut.name == name) { selTut = tut; } }
        return selTut;
    }

    public static bool setCurrentTutorial(string name)
    {
        bool sel = false;
        foreach (CurDataTutorial tut in tutorials)
        {
            if (tut.name == name) { curTutorial = tut; sel = true; }
        }
        Debug.Log("curTutorial" + curTutorial);
        return sel;
    }

    public static void addVideo(string name, string value)
    {
        //Debug.Log ("name="+name+" val="+value);
        if (!videos.ContainsKey(name))
        {
            videos.Add(name, value);
        }
        else
        {
            Debug.Log("!!!!!!!!!!Error key already exist. name=" + name + " nameVideo=" + videos[name]);
        }
    }

    public static string getVideo(string name)
    {
        string result = null;
        if (videos.ContainsKey(name)) { result = videos[name]; }
        return result;
    }

    public static void addAudio(string name, string value)
    {
        //Debug.Log ("name="+name+" val="+value);
        if (!audios.ContainsKey(name))
        {
            audios.Add(name, value);
        }
        else
        {
            Debug.Log("!!!!!!!!!!Error key already exist. name=" + name + " nameVideo=" + audios[name]);
        }
    }

    public static string getAudio(string name)
    {
        string result = null;
        if (audios.ContainsKey(name)) { result = audios[name]; }
        return result;
    }

    public static string getLocalTutorialString(string name)
    {
        string jStr = "{\"name\":\"tutorial_1\",\"text\":\"text in start tutorial\",\"author\":\"Grag Lowrance\", \"version\":1,\"date\":151749557,\"parts\"";
        jStr += ":[{\"name\":\"Part N 1\", \"order\":1,\"question\": \"What color do you prefer?\",\"answers\":[{\"name\":\"red\", \"text\":\"Red\", \"order\":1, \"type\":\"text\", \"fileName\":\"none\"}";
        jStr += ",{\"name\":\"blue\", \"text\":\"Blue\", \"order\":2, \"type\":\"text\", \"fileName\":\"none\"}],\"videos\":[";
        jStr += "{\"name\":\"Briefing\",\"desc\":\"The first meeting\",\"fileName\":\"vrprof1_1.mp4\",\"audio\":\"vrprof1_1.mp3\"" +
            ",\"sizeVideo\":53905,\"sizeAudio\":1664 ,\"rotate\":90,\"date\":1517495579 ,\"order\":1 ,\"md5_audio\":\"BB\",\"md5_video\":\"BB\"}";
        jStr += ",{\"name\":\"Photo\",\"order\":2,\"desc\":\"The photo session\",\"fileName\":\"vrprof1_2.mp4\",\"rotate\":270" +
            ",\"audio\":\"vrprof1_2.mp3\",\"md5_audio\":\"BB\",\"md5_video\":\"BB\",\"sizeVideo\":69479,\"sizeAudio\":2104,\"date\":1517495579}";
        jStr += ",{\"name\":\"Introduction\",\"order\":3,\"desc\":\"Information about a model carrier\",\"fileName\":\"vrprof1_3.mp4\"" +
            ",\"rotate\":90,\"audio\":\"vrprof1_3.mp3\",\"md5_audio\":\"BB\",\"md5_video\":\"BB\",\"sizeVideo\":131486,\"sizeAudio\":4062,\"date\":1517495579}";
        jStr += "]}]}";
        return jStr;
    }
}


[System.Serializable]
public class CurDataTutorial
{
    public string name; public string text; public string author; public int version; public int date; public CurDataPart[] parts;
}

[System.Serializable]
public class CurDataPart
{
    public string name; public int order; public string question; public CurDataAnswer[] answers; public CurDataVideo[] videos;
}

[System.Serializable]
public class CurDataVideo
{
    public string name; public string desc; public string fileName; public string audio; public long sizeVideo;
    public long sizeAudio; public int date; public int rotate; public string md5_video; public string md5_audio; public int order;
}

[System.Serializable]
public class CurDataAnswer
{
    public string name; public string text; public int order; public string type; public string fileName;
}

//[System.Serializable] public class CurDataVideoLocal{
//	public string name; public int order; public string desc; public string fileName; public string audio; 
//}