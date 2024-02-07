using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide_Obj : MonoBehaviour
{
    public bool isCol = false;


    private void OnTriggerEnter(Collider other)
    {
        isCol = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isCol = false;
    }
}
