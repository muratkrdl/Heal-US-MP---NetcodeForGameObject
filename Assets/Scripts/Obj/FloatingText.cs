using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshPro myText;

    [SerializeField] float speed;
    [SerializeField] float lerpTimeColor;
    [SerializeField] float lerpTimeAlpha;

    [SerializeField] float lifeTime;

    Vector3 randomVector3;
    Camera target;

    void Awake() 
    {
        randomVector3 = Random.insideUnitSphere;
        randomVector3.y -= .5f;
    }

    void Start() 
    {
        target = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        transform.position += randomVector3;
        StartCoroutine(LerpAlpha());
        Invoke(nameof(KYS), lifeTime);
    }

    void KYS()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if(target == null) { return; }
        transform.LookAt(target.transform);
        transform.Translate(speed * Time.deltaTime * Vector3.up);
        myText.color = Color.Lerp(myText.color, Color.white, lerpTimeColor * Time.deltaTime);
    }

    public void SetTextValues(string text, Color color)
    {
        myText.text = text;
        myText.color = color;
    }

    IEnumerator LerpAlpha()
    {
        float elapsed = 0;
        float duration = 1;
        float t = 0;
        while(elapsed <= 3)
        {
            yield return null;
            elapsed += Time.deltaTime;
            t = elapsed / duration;
            myText.alpha = Mathf.Lerp(myText.alpha, 0, t * Time.deltaTime);
        }
    }

}
