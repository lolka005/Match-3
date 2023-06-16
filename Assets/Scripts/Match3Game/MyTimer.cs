using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MyTimer : MonoBehaviour
{
    public static MyTimer Instance { get; private set; }
    private float _time = 5;
    public float time
    {
        get { return _time; }
        set { _time = value; } 
    }
    private bool start = false;
    [SerializeField] private TextMeshProUGUI value;
    public void StartTimer()
    {
        start = true;
    }
    void Update()
    {
        if (start == true && time > 0)
        {
            time -= Time.deltaTime;
            value.SetText(time.ToString("0.00"));
        }
        else if(time < 0)
        {
            start = false;
            SceneManager.LoadScene(4);
        }
        else
        {
            return;
        }
    }
    public void Stop()
    {
        start = false;
    }
    private void Awake()
    {
        Instance = this;
        start = true;
    }
}
