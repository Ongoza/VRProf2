using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndIntro : MonoBehaviour {

// This C# function can be called by an Animation Event
    public void PrintEvent()
    {
        Debug.Log("Ent Intro");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

}
