using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIppoArena : MonoBehaviour
{
    [SerializeField] private HungryHippo HungryHippoScript;

    private float temp = 0;
    private bool available = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        while (temp % 60 < HungryHippoScript.ShrinkActivationTimeMinutes)
        {
            temp += Time.deltaTime;
        }

    }
}
