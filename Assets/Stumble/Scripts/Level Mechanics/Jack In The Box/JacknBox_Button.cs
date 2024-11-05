using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JacknBox_Button : MonoBehaviour
{
    [SerializeField] Animator animator;

    public GameObject Squish;
    public float buttonResetTimer = 2f;

    public bool resetIfPlayerIsStillOn = false;
    private bool StillSquished = false;

    private Vector3 InitalPos;
    private Vector3 EndingPos;
    private float SquishAnimationTime = .1f;

    void Start()
    {
        InitalPos = Squish.transform.position;
        EndingPos = new Vector3(InitalPos.x, InitalPos.y - 0.15f, InitalPos.z);
    }


    private void OnTriggerEnter(Collider other)
    {
        //print("Jack box triggered");
        if (other.gameObject.tag == "Player")
        {
            StillSquished = true;
            StartCoroutine(GetSquished());
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("JackNDaBox")) return;

            animator.Play("JackNDaBox");

            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StillSquished = false;
        }
    }

    private IEnumerator GetSquished()
    {
        
        float time = 0;
        while (time < SquishAnimationTime)
        {
            Squish.transform.position = Vector3.Lerp(InitalPos, EndingPos, time / SquishAnimationTime);
            time += Time.deltaTime;
            yield return null;
        }
        Squish.transform.position = EndingPos;

        yield return new WaitForSeconds(buttonResetTimer);

        if (!resetIfPlayerIsStillOn)
        {
            yield return new WaitUntil(() => !StillSquished);
        }

        time = 0;
        while (time < SquishAnimationTime)
        {
            Squish.transform.position = Vector3.Lerp(EndingPos, InitalPos, time / SquishAnimationTime);
            time += Time.deltaTime;
            yield return null;
        }
        Squish.transform.position = InitalPos;

        if (resetIfPlayerIsStillOn && StillSquished)
        {
            StartCoroutine(GetSquished());
        }
    }
}
