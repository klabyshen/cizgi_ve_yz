using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppControlScript : MonoBehaviour
{
    [SerializeField] private MnistScript mnistScript;
    [SerializeField] private DrawLineScript drawLineScript;
    // Start is called before the first frame update
    void Start()
    {
        drawLineScript.LineDrawn += mnistScript.OnLineDrawn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
