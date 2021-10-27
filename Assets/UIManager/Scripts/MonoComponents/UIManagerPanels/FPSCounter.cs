using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private Text txt;

    void Start() 
    {
        txt = GetComponent<Text>();
    }


    void Update() 
    {
        txt.text = "" + (int)(1f/Time.unscaledDeltaTime);
    }
}
