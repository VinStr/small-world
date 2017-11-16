using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    public GameObject panel;

    private Vector2[] startPos;

    private bool opening = true;

    public void OnClick()
    {
        opening = !opening;
    }

    // Use this for initialization
    IEnumerator Start()
    {
        startPos = new Vector2[panel.transform.childCount];
        System.Array.Clear(startPos, 0, panel.transform.childCount);

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < panel.transform.childCount; i++)
        {
            startPos[i] = panel.transform.GetChild(i).position;
            panel.transform.GetChild(i).position = panel.transform.position;
        }
        panel.SetActive(false);
    }

    private float t = 0;
    // Update is called once per frame
    void Update()
    {
        t = Mathf.Clamp(t, 0, 1);

        if (!opening && t >=0) panel.SetActive(true);
        else if(opening &&t >=1) panel.SetActive(false);

        if(opening) t += Time.deltaTime / 0.5f;
        else t -= Time.deltaTime / 0.5f;
        for (int i = 0; i < panel.transform.childCount; i++)
        {
                panel.transform.GetChild(i).position = Vector2.Lerp(startPos[i], panel.transform.position, t);
        }
        

    }
}

