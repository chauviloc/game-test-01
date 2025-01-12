﻿using UnityEngine;
using System.Collections;


public class FPSDisplay : MonoBehaviour
{

    public float FPS => fps;

    private float deltaTime = 0.0f;
    private float fps = 0;

    GUIStyle mStyle;
    void Awake()
    {
        mStyle = new GUIStyle();
        mStyle.alignment = TextAnchor.UpperLeft;
        mStyle.normal.background = null;
        mStyle.fontSize = 40;
        mStyle.normal.textColor = new Color(1f, 0f, 0f, 1.0f);
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width;
        int h = Screen.height;
        Rect rect = new Rect(100, 0, w, h * 2 / 100);
        fps = 1.0f / deltaTime;
        string text = string.Format("   {0:0.} FPS", fps);
        GUI.Label(rect, text, mStyle);
    }
}

