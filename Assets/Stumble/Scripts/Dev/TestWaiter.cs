using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TestWaiter : MonoBehaviour
{
    public bool isReady = false;

    public static TestWaiter Instance { get; private set; }

    // Executed on Awake
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        setup();
    }

    private async Task setup()
    {
        /*        await Task.Run(async () => {
                    while (TestHandler.Instance == null || TestHandler.Instance.enabled == false) await Task.Delay(10); 
                });
                Debug.Log("The Test Handler has been found");*/
        while (TestHandler.Instance == null || TestHandler.Instance.enabled == false)
        {
            Debug.Log("Doing stuff Primary");
            await Task.Delay(1);
        }

        Debug.Log("I'm Outside Primary");
        isReady = true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
