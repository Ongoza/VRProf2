using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
//using System.IO;

public class MyVideo: MonoBehaviour {
	// RenderTexture rt;
	//VideoPlayer player;
	//public Shader shader1;
	//	public VideoClip videoToPlay;
	//	private VideoSource videoSource;
	public VideoPlayer player;
	private GameObject fadeObj;
	private GameObject fadeCanvas;


	// Use this for initialization
	void Start () {	startMain ();}
	// startFade ();  has to be cylinder


	void startMain(){
//		string url = Application.persistentDataPath + "/podium.mp4";
//		player.url = url;
//		player.prepareCompleted += Prepared;
//		player.loopPointReached += EndReached;
	}

//	IEnumerator playVideo(){
//	//string url = "http://192.168.1.64:8000/static/podium.mp4";
//	//	string url = "/storage/emulated/O/11/podium.mp4";
//		string url =  Application.persistentDataPath + "/podium.mp4";
////		Debug.Log("url = "+url);
////		globalData.server.getString("/myVideo1/"+url);
//		player.url = url;
////		globalData.server.getString("/myVideo3");
//	player.EnableAudioTrack(0, true);
////		player.Prepare();
//		globalData.server.getString("/myVideo4");
//		WaitForSeconds waitTime = new WaitForSeconds(2);
//		globalData.server.getString("/myVideo42");
//		while (!player.isPrepared){
//			Debug.Log("Preparing Video");
//			globalData.server.getString("/myVideo42");
//			//Prepare/Wait for 5 sceonds only
//			yield return waitTime;
////			//Break out of the while loop after 5 seconds wait
//			break;
//		}
//		globalData.server.getString("/myVideo5");
//	//	try{
//			player.Play();
//			globalData.server.getString("/myVideo6");
//			Debug.Log("Playing Video");
//			while (player.isPlaying){
//			//	Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)player.time));
//				//globalData.server.getString("/myVideo6" + Mathf.FloorToInt((float)player.time));
//				yield return null;
//			}
//	//	}catch(System.Exception e){
//		//	globalData.server.getString("/myVideo6 error:"+e.ToString());
//		//}
//		Debug.Log("Done Playing Video");
//	}

//	void Prepared(UnityEngine.Video.VideoPlayer vPlayer) {
//		Debug.Log("start play!");
//		globalData.server.getString("/myVideoPlay");
//		player.Play();
//		globalData.server.getString("/myVideoPlay2");
//	}
//
//	void EndReached(UnityEngine.Video.VideoPlayer vp){
////		vp.playbackSpeed = vp.playbackSpeed / 10.0F;
//		Debug.Log("End reached!!!!!");
//		UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
//	}

	//	IEnumerator loadAndPlay(string url){
	//		player.Prepare();
	//
	//		//Wait until video is prepared
	//		WaitForSeconds waitTime = new WaitForSeconds(1);
	//		while (!player.isPrepared)
	//		{
	//			Debug.Log("Preparing Video");
	//			//Prepare/Wait for 5 sceonds only
	//			yield return waitTime;
	//			//Break out of the while loop after 5 seconds wait
	//			break;
	//		}
	//
	//
	//		player.Play ();
	//	}

	// Update is called once per frame
//	void Update () {
//
//	}
}
