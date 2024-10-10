using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungeryHippo : MonoBehaviour
{
    public float hippoMouthSpeed = 10f;
    public float hippoMoveSpeed = 2;

    public GameObject playerKillzone;

    public bool triggered = false;

    public bool reset = false;






    void FixedUpdate()
    {
        if (triggered)
        {
            if (transform.localEulerAngles.x > 320 || transform.localEulerAngles.x < 310)
            {


                Debug.Log("localEuler: " + transform.localEulerAngles.x);
                Quaternion mouthRotation = Quaternion.Euler(-hippoMouthSpeed * Time.deltaTime, 0f, 0f);
                Debug.Log("mouthRot: " + mouthRotation);

                this.transform.rotation = this.transform.rotation * mouthRotation;
                Debug.Log("localEuler: " + this.transform.rotation);
            }

        }

        if (reset)
        {
            reset = false;
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        

    }








    // trigger

    //open mouth

    //shoot forward (optional)

    //close

    //kill box activates

    //retracts (optional)

    //kill box deactivates

    //repeat

}
