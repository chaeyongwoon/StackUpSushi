using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    public enum State
    {
        Plus,
        Minus,
        Multiple
    } public State state;

    public float Calc_Value = 1f;

    public int Index=1;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Luggage"))
        {
            if(other.GetComponent<Luggage>().Index != Index)
            {
                other.GetComponent<Luggage>().Index = Index;
                Instantiate(other.gameObject, other.transform.position, Quaternion.identity);
            }
        }
    }
}
