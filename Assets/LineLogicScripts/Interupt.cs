using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Interupt : MonoBehaviour
{
    private float timer = 0;
    private float timerEnd = 5;
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        gameObject.GetComponentInChildren<TextMeshPro>().text =
            "!! " + gameObject.GetComponentInChildren<TextMeshPro>().text + " !!";
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timerEnd) Destroy(this);
    }
}
