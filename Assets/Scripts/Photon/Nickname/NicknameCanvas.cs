using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicknameCanvas : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        StartCoroutine(FindMainCamera());        
    }

    IEnumerator FindMainCamera()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            if(cam != null)
            {
                transform.LookAt(cam.transform);
                transform.Rotate(Vector3.up * 180);

                break;
            }
        }
        StopCoroutine(FindMainCamera());
    }

    void Update() 
    {
        if(cam == null) return;
        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
    
}
