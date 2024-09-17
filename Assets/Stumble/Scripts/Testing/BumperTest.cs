using UnityEngine;

public class BumperTest : MonoBehaviour
{
    [SerializeField] private Transform testTarget;
    private void OnTriggerEnter(Collider hit)
    {
        Debug.Log("Collision");
        // Check if it has the player Tag
        IBumper bumper = hit.gameObject.GetComponent<IBumper>();
        if (bumper != null)
        {
            //Vector3 bumpDirection = hit.transform.position - transform.position;
            Vector3 bumpDirection = hit.transform.position - transform.position;
            float bumpMagnitude = 15f;

            bumper.Bump(bumpDirection, bumpMagnitude);
        }
    }

}
