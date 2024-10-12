using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungryHippo : MonoBehaviour
{
    public float hippoMouthSpeed = 10f;
    public float hippoMoveSpeed = 2;
    public float hippoLungeDuration = 4f;

    public GameObject playerKillzone;

    public bool triggered = false;
    private bool available = true;

    public bool reset = false;






    void FixedUpdate()
    {
        if (triggered && available)
        {

            StartCoroutine(HippoMotion());
        }

        if (reset)
        {
            reset = false;
            triggered = false;
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        

    }


    private IEnumerator HippoMotion()
    {
        float hippoDistance = 0;
        while (transform.localEulerAngles.x > 320 || transform.localEulerAngles.x < 310)
        {

            //Debug.Log("localEuler: " + transform.localEulerAngles.x);
            Quaternion mouthRotation = Quaternion.Euler(-hippoMouthSpeed * Time.deltaTime, 0f, 0f);
            //Debug.Log("mouthRot: " + mouthRotation);

            this.transform.rotation = this.transform.rotation * mouthRotation;
            //Debug.Log("localEuler: " + this.transform.rotation);

        }
        Debug.Log("mouth opened");
        yield return new WaitForSecondsRealtime(.1f);

        while (hippoDistance < hippoLungeDuration)
        {
            transform.position -= transform.forward * Time.deltaTime * hippoMoveSpeed;
            hippoDistance += Time.deltaTime;
        }

        

    }





    //trigger - fixed up

    //open mouth - co ru

    //shoot forward (optional) - co ru

    //close - co ru

    //kill box activates - co ru?

    //retracts (optional) - co ru

    //kill box deactivates - co ru?

    //repeat - fixed up

}
