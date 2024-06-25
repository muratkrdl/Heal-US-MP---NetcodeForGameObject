using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicknameCanvas : MonoBehaviour
{
    Camera cam;

    bool finding = true;

    float time;
    const float MAX_TIME = 2;

    void Update() 
    {
        if(finding)
        {
            time += Time.deltaTime;
            if(time >= MAX_TIME)
            {
                cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                time = 0;
                if(cam != null)
                {
                    transform.LookAt(cam.transform);
                    transform.Rotate(Vector3.up * 180);
                    finding = false;
                }
            }
        }
        else
        {
            if(cam == null) return;
            transform.LookAt(cam.transform);
            transform.Rotate(Vector3.up * 180);
        }
    }
    
}
