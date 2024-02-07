using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float Max_Buoyancy_Power = 85f;
    public float Min_Buoyancy_Power = 75f;

    public float Buoyancy_Power;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Luggage"))
        {
            //other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX; //| RigidbodyConstraints.FreezePositionZ;
            //other.GetComponent<Rigidbody>().velocity = new Vector3(0f, other.GetComponent<Rigidbody>().velocity.y, 0f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Luggage") || other.CompareTag("Lugg_Child") /*|| other.CompareTag("Ship")*/ || other.CompareTag("Deco_obj"))
        {
            Buoyancy_Power
                = Max_Buoyancy_Power
                - (Mathf.Abs(transform.position.y - other.transform.position.y)
                * (Max_Buoyancy_Power - Min_Buoyancy_Power) / Mathf.Abs(transform.position.y));
            //if (other.CompareTag("Ship"))
            //{
            //    other.transform.parent.GetComponent<Rigidbody>().AddForce(Vector3.up * Buoyancy_Power);
            //}
            //else
            //{
            other.GetComponent<Rigidbody>().AddForce(Vector3.up * Buoyancy_Power);
            //}
        }
    }

}
