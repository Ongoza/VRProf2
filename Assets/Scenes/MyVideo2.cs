using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
//using UnityEngine.UI;

public class MyVideo2 : MonoBehaviour {
	public VideoPlayer player;
	public AudioSource audioSource;
	private float rotate;
	// Use this for initialization
	void Start () {
		Debug.Log ("start video2!");
//		rotate = 90;
//		string urlVideo = "C:/Users/del4600/AppData/LocalLow/Ongoza/VRprof/vrprof1_1.mp4";
//		string urlAudio = "C:/Users/del4600/AppData/LocalLow/Ongoza/VRprof/vrprof1_1.mp3";
		globalData.server.putDataString (false,"\"video2\":\"start player! 0\"");
//		if (true) {
			int index = globalData.getCurVideoName ();
			string curIndex = index.ToString();
			string urlVideo = globalData.getVideo (curIndex);
			string urlAudio = globalData.getAudio (curIndex);
			CurDataVideo video = globalData.getVideoData (index);		 
			rotate = video.rotate;
//		} 
		Debug.Log ("start player! rotate="+rotate+" urls="+urlVideo+"="+urlAudio);
		globalData.server.putDataString (false,"\"video2\":\"start player! 1 rotate="+rotate+" urls="+urlVideo+"="+urlAudio+"\"");
		if (urlVideo != null) {
			if (urlAudio != null) {
				StartCoroutine(LoadFile (urlAudio, urlVideo));
			}
		} else {
			Debug.Log ("can not start play! " + urlVideo);
			globalData.server.putDataString (false,"\"video2\":\"start player! error urlVideo\"");
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	IEnumerator LoadFile(string path, string urlVideo){
		//globalData.server.putDataString (false,"\"video2\":\"open audio! 0\"");
		string fullPath = "file://" + path;
		WWW www = new WWW(fullPath); int counter = 1000;
				yield return www;
		if (www.error != null) {
			globalData.server.putDataString (false,"\"video2\":\"error open audio! 0\"");
			Debug.Log ("www error="+www.error);
		}		
		AudioClip clip = www.GetAudioClip(false,false);
		clip.name = Path.GetFileNameWithoutExtension (fullPath);
		clip.LoadAudioData();
//		Debug.Log ("audio 1 state="+clip.loadState+"="+clip.loadState.Equals("Unloaded"));
		while ((clip.loadState == AudioDataLoadState.Unloaded) && (counter>0)) { counter--;
			yield return clip;
			if (!string.IsNullOrEmpty(www.error))
				Debug.Log("error load clip = "+www.error);
				globalData.server.putDataString (false,"\"video2\":\"error loaded audio! e="+www.error+"\"");
		}
		//globalData.server.putDataString (false,"\"video2\":\"loaded audio! 1 status="+clip.loadState+"\"");
		//Debug.Log ("Counter="+counter);
		if (counter < 1) {	Debug.Log ("Can Not open Audio");
			globalData.server.putDataString (false,"\"video2\":\"error loaded audio! 2 can not open=\"");
		} else {
			Debug.Log ("audio 2 state=" + clip.loadState);
			clip.name = Path.GetFileName (path);
			audioSource.clip = clip;
			Debug.Log ("audio loaded");
			//globalData.server.putDataString (false,"\"video2\":\"loaded audio! 3 ok\"");
			player.url = urlVideo;
			player.prepareCompleted += Prepared;
			player.loopPointReached += EndReached;
			player.Prepare ();
		}
	}

	void Prepared(UnityEngine.Video.VideoPlayer vPlayer) {
		Debug.Log("start prepared 2");
		//globalData.server.putDataString (false,"\"video2\":\"prepared video! 0\"");
		Material mat = RenderSettings.skybox;
		if (rotate != 0) {
			Debug.Log ("Rotate camera video "+rotate);
			mat.SetFloat ("_Rotation", rotate);}
		//globalData.server.putDataString (false,"\"video2\":\"prepared video!1\"");
		player.Play();
		audioSource.Play();
	}

	void EndReached(UnityEngine.Video.VideoPlayer vp){
		//		vp.playbackSpeed = vp.playbackSpeed / 10.0F;
		Debug.Log("End reached!!!!!");
		int next = globalData.getCurVideoName () + 1;
		int max = globalData.getVideosCount ();
		if (next < max){
			globalData.setCurVideoName (next);
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Description");
		}else{
			globalData.setCurVideoName (0);
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
		}

	}
}
