using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net;

public class ServerConnection {
	
	private string ServerUrl = globalData.getServerAddress();
	private string ServerDataUrl = globalData.getDataServerAddress();
	public bool ready = true;
	int videoIndex = 0;
	CurDataVideo[] videos = null;
	long allFilesSize =0;
	int downloadingProgressComplete=0;


	public void putDataString(bool trData,string data){		
		try {
			string docs = "docs=[{\"uuid\":\""+globalData.getDeviceUUID()+"\",\"time\":\""+System.DateTime.UtcNow.ToString("yyyy/MM/dd/HH:mm")+"\","+ data + "}]";
			string strDataEnd = "";
			if (!trData){strDataEnd+="_test";}
			string uriStr = ServerDataUrl +strDataEnd+"/_insert";
			Debug.Log ("start Upload url"+uriStr);
			WebClient client = new WebClient ();
			client.Headers.Add ("Content-Type", "application/x-www-form-urlencoded");
			byte[] byteDocs = Encoding.UTF8.GetBytes (docs);
			client.UploadDataCompleted += new UploadDataCompletedEventHandler (UploadDataResult);
			client.UploadDataAsync (new System.Uri (uriStr), "POST", byteDocs);
			Debug.Log ("start Upload");				
			} catch (System.Exception e) {	Debug.Log ("Connection error: " + e); }
	}

	private void UploadDataResult(object sender, UploadDataCompletedEventArgs e){
		try{
			Debug.Log ("Upload result catch!!");
			byte[] data = (byte[])e.Result;
			string reply = System.Text.Encoding.UTF8.GetString (data);
				Debug.Log ("!!!!!Upload result: " + reply);	
		}catch (System.Exception ee){
			Debug.Log ("!!!!!Upload error: "+ee);
		}
	}


	public bool getAllVideo(string name){
		if(ready){ready = false;
			Debug.Log ("start 0 index");
			videoIndex = 0;
			videos = null;
			allFilesSize =0;
			downloadingProgressComplete = 0;
	//		System.DateTime dt = System.DateTime.Now + System.TimeSpan.FromSeconds(2);
	//		do {} while (System.DateTime.Now < dt);
	//		Debug.Log ("start 2 index=");
			if (!globalData.getTrReadyVideo ()) {
				videos = globalData.getVideosList (name); 
				foreach (CurDataVideo video in videos) {allFilesSize += video.sizeVideo;}
				getNextVideo ();
				//Debug.Log ("All files are downloaded "+ Directory.GetCurrentDirectory());
				//addOevrlayInfo ("Videos are downloaded "+ Directory.GetCurrentDirectory());
			} else {
				Debug.Log ("Videos already are downloaded");
				//addOevrlayInfo ("Videos already are downloaded");
			}
			ready = true;		
		}else{Debug.Log ("connection is busy.");}
		return ready;
	}


	private void getNextVideo(){
		Debug.Log ("start 1 download video videoIndex="+videoIndex);
		if (videos != null) {
			if (videos.Length > videoIndex) {				
				CurDataVideo video = videos [videoIndex];
				Debug.Log ("start 2 download video name=" + video.name + " videoIndex" + videoIndex);
				string localPath = globalData.getLocalPath ();
				string curVideo = localPath + "/" + video.fileName;
				string curAudio = localPath + "/" + video.audio;
				string videoUrl = ServerUrl + "" + video.fileName;
				string audioUrl = ServerUrl + "" + video.audio;
				Debug.Log ("start 2 download video " + videoIndex + " AllSize="+ allFilesSize+ " path=" + curVideo+"url="+videoUrl);
				//downFilesSize += video.sizeVideo;
				Debug.Log ("start 3 download file " + video.fileName+" md5="+video.md5_audio);
				bool existFile2 = checkFile (curAudio, video.md5_audio);
				if (!existFile2) {
					WebClient client = new WebClient ();
					client.QueryString.Add ("file", curAudio);
					client.QueryString.Add ("index", videoIndex.ToString ());
					//client.DownloadProgressChanged += DownloadVideoProgress;
					client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler (DownloadAudioCompleted);
					client.DownloadFileAsync (new System.Uri (audioUrl), curAudio);
				} else {
					Debug.Log ("audio file already exist " + video.name);
					globalData.addAudio (videoIndex.ToString (), curAudio);
				}
				bool existFile = checkFile (curVideo, video.md5_video);
				if (!existFile) {
					WebClient client = new WebClient ();
					client.DownloadProgressChanged += DownloadVideoProgress;
					client.QueryString.Add ("file", curVideo);
					client.QueryString.Add ("index", videoIndex.ToString ());
					float dff = (float) video.sizeVideo;
					float percent = dff/allFilesSize;
					//Debug.Log ("step=" + percent.ToString());
					client.QueryString.Add ("step", percent.ToString ());
					client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler (DownloadVideoCompleted);
					client.DownloadFileAsync (new System.Uri (videoUrl), curVideo);
				} else {
					Debug.Log ("video file already exist " + video.name);
					int percent = System.Convert.ToInt32( video.sizeVideo * 100 / allFilesSize);
					if ((downloadingProgressComplete+percent) < 101) {
						downloadingProgressComplete += percent;
					}
					globalData.setDownloadingProgress (downloadingProgressComplete);
					globalData.addVideo (videoIndex.ToString (), curVideo);
					videoIndex++;
					getNextVideo ();
				}
			} else {globalData.setTrReadyVideo (true);}
		} else {Debug.Log ("videos == null");}
	}

	private bool checkFile (string name, string new_md5){
		//Debug.Log ("shekFile 0 download file " + name+" md5="+new_md5);
		bool result = false;
		if (!name.Equals ("")) {
			var fileInfo = new System.IO.FileInfo (name);
			if (fileInfo.Exists) {				
				try {
					FileStream file = new FileStream (name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					MD5 md5 = new MD5CryptoServiceProvider (); byte[] retVal = md5.ComputeHash (file); file.Close ();
					StringBuilder sb = new StringBuilder (); for (int i = 0; i < retVal.Length; i++) {sb.Append (retVal [i].ToString ("x2"));}
					string md5_old = sb.ToString ();
					Debug.Log ("shekFile 5 name=" + name + " oldmd5= " + md5_old + " newmd5= " + new_md5);
					globalData.server.putDataString (false, "\"md5\":\"md5 " + name + "=" + md5_old + "=" + new_md5 + "\"");
					if (md5_old.Equals (new_md5)) {	result = true;
						globalData.server.putDataString (false, "\"md5\":\"md5 ok " + name + "=" + md5_old + "=" + new_md5 + "\"");
						Debug.Log ("shekFile !!!!5 equal name=" + name + " = " + md5_old + " = " + new_md5);	
					} else {File.Delete (name);}
				} catch (IOException e) {
					globalData.server.putDataString (false, "\"md5\":\"error open file!\"");
					Debug.Log ("shekFile error open file check= name" + name);
				}				
			}
		}
		//Debug.Log ("shekFile 7 result=" + result);
		return result;
	}		


	private void DownloadVideoProgress(object sender, DownloadProgressChangedEventArgs e){
		try{
			WebClient client = (WebClient) sender;
			float step = float.Parse(client.QueryString["step"]);
			float delta = e.ProgressPercentage*step;
			int newValue = System.Convert.ToInt32(downloadingProgressComplete+delta);
			if(newValue!=globalData.getDownloadingProgress()&&newValue<101){
				//Debug.Log ("Progerss delta="+newValue+"="+delta+"="+e.ProgressPercentage);
				globalData.setDownloadingProgress (newValue);}
		}catch(System.Exception ee){Debug.Log ("Progress downloading video error "+ee);}
	}
//
	private void DownloadVideoCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e){
		try{
			WebClient client = (WebClient) sender;
			string curVideo = client.QueryString["file"];
			string videoIndexFile = client.QueryString["index"];
			float step = float.Parse(client.QueryString["step"]);
			int delta = System.Convert.ToInt32(Mathf.Round(100*step));
			if ((downloadingProgressComplete+delta) < 101) {
				downloadingProgressComplete += delta;
			}
			Debug.Log ("Progerss delta new="+downloadingProgressComplete);
			bool tr = true;
			Debug.Log ("video file downloading complete"+curVideo+" "+videoIndex);
			if (e.Cancelled){Debug.Log ("The download has been cancelled"); tr = false;}
			if (e.Error != null) {Debug.Log ("Server ansver error: " + e.ToString ());tr = false;}
			//Debug.Log ("start next file download");
			if (tr) {globalData.addVideo (videoIndexFile, curVideo);}
			videoIndex++; getNextVideo();
		}catch(System.Exception ee){Debug.Log ("Complete downloading video error "+ee);}
	}

	private void DownloadAudioCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e){
		try{
			WebClient client = (WebClient) sender;
			string curAudio = client.QueryString["file"];
			string videoIndexFile = client.QueryString["index"];
			Debug.Log ("audio file downloading complete"+curAudio+" "+videoIndexFile);
			if (e.Cancelled){Debug.Log ("The download has been cancelled");	return;	}
			if (e.Error != null) {Debug.Log ("Server ansver error: " + e.ToString ()); return;}
			globalData.addAudio (videoIndexFile, curAudio);
		}catch(System.Exception ee){Debug.Log ("Complete downloading video error "+ee);}
	}		

}
