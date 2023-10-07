using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant_AI : AI
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        AllAI.Add(this);
    }

    void OnDisable()
    {
        AllAI.Remove(this);
        StopAllCoroutines();
    }
}
