using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeChild : MonoBehaviour
{
    public bool triggered;
    public bool chosen;
    public bool endOfRound;
    public GameObject self;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (chosen && triggered)
        {

        }
        else if (triggered)
        {
            self.SetActive(false);
        }
        if(endOfRound && triggered && !chosen)
        {
            //self.transform;
        }
        
    }
}
