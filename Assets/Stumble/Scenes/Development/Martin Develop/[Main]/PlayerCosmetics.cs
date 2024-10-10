using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCosmetics : MonoBehaviour
{
    [Header ("Body Parts")]
    [SerializeField] public GameObject body; //{ private set; get; }
    [SerializeField] public GameObject fins; //{ private set; get; }
    [SerializeField] public GameObject eyes; //{ private set; get; }

    [Header ("Other Transforms")]
    [SerializeField] public Transform hatPos; //{ private set; get; }
    [SerializeField] public Transform leftFoot; //{ private set; get; }
    [SerializeField] public Transform rightFoot; //{ private set; get; }

}
