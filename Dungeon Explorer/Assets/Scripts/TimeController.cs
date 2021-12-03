using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{

    public static TimeController instance;

    public Text counter;

    private TimeSpan _timePlaying;
    private bool _timerGoing;

    private float _elapsedTime;

    private string _formattedTime;


    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        counter.text = "Time: 00:00.00";
        _timerGoing = false;
    }

    public void BeginTimer()
    {

        _timerGoing = true;
        _elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public string EndTimer()
    {
        _timerGoing = false;
        return _formattedTime;
    }

    private IEnumerator UpdateTimer()
    {
        while (_timerGoing)
        {
            _elapsedTime += Time.deltaTime;
            _timePlaying = TimeSpan.FromSeconds(_elapsedTime);
            _formattedTime = "Time: " + _timePlaying.ToString("mm':'ss'.'ff");
            counter.text = _formattedTime;

            yield return null;
        }
    }
}