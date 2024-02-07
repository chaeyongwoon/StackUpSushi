using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootObj : MonoBehaviour
{
    public Vector3 Rot = new Vector3(0f, 180f, 0f);
    public float Size_X = 0f, Size_Y = 0f, Size_Z = 0f;
    public bool isCol = false;
    public Vector3 Scale = Vector3.one;


    private void Awake()
    {
        Size_X = transform.localScale.x * GetComponent<BoxCollider>().size.x;
        Size_Y = transform.localScale.y * GetComponent<BoxCollider>().size.y;
        Size_Z = transform.localScale.z * GetComponent<BoxCollider>().size.z;
        Scale = transform.localScale;
    }

    private void Start()
    {
        SetRot();
    }

    public void SetRot()
    {
        transform.rotation = Quaternion.Euler(Rot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Rail") || collision.transform.CompareTag("Ground"))
        {
            StartCoroutine(DestroyObj());

        }
    }


    IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(2f);
        if (isCol == false)
        {
            Destroy(this.gameObject);
        }
    }
}
