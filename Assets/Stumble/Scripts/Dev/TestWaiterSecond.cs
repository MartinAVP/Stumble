using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TestWaiterSecond : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Task Setup = setup();
    }

    private async Task setup()
    {
        /*        await Task.Run(async () => {
                    while (TestHandler.Instance == null || TestHandler.Instance.enabled == false) await Task.Delay(10); 
                });
                Debug.Log("The Test Handler has been found");*/
        while (TestWaiter.Instance == null || TestWaiter.Instance.enabled == false || TestWaiter.Instance.isReady == false)
        {
            Debug.Log("Doing stuff Secondary");
            await Task.Delay(1);
        }

        Debug.Log("I'm Outside on the Secondary");
    }
}
