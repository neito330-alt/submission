using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frame : MonoBehaviour
{
    public int _framerate = 60;
    public float _timeScale = 1.0f;
    // Start is called before the first frame update
    private int framerate = 60;
    private float timeScale = 1.0f;

    void Start()
    {
        Application.targetFrameRate = _framerate;
        Time.timeScale = _timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (framerate != _framerate)
        {
            Application.targetFrameRate = _framerate;
            framerate = _framerate;
        }
        else if (timeScale != _timeScale)
        {
            Time.timeScale = _timeScale;
            timeScale = _timeScale;
        }
    }
}
