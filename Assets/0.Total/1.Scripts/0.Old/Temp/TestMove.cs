using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public GameObject Guide_Cube;


    public Collider[] _cols;
    public bool isQuit = false;
    private void Start()
    {
        StartCoroutine(Guide());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Input.GetAxis("Horizontal") * Vector3.right);
            StopAllCoroutines();
            StartCoroutine(Guide());
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Input.GetAxis("Horizontal") * Vector3.right);
            StopAllCoroutines();
            StartCoroutine(Guide());
        }
    }

    IEnumerator Guide()
    {
        float offset = 0f;
        yield return null;
        while (true)
        {
            if (isQuit == false)
            {               
                _cols = Physics.OverlapBox(transform.position + Vector3.down * offset, Vector3.one * 0.5f);
                offset += 0.05f;
                Guide_Cube.transform.position = transform.position + Vector3.down * offset;

                if (_cols.Length != 0)
                {
                    offset -= 0.05f;
                    Guide_Cube.transform.position = transform.position + Vector3.down * offset;
                    //isQuit = true;
                    break;
                }

                //foreach (Collider _col in _cols)
                //{
                //    if (_col.Equals(Guide_Cube.GetComponent<Collider>()))
                //    {

                //    }
                //    else
                //    {
                //        //isQuit = true;

                //        Guide_Cube.transform.position = transform.position + Vector3.down * offset + new Vector3(0f, 0.2f, 0f);
                //        if (Guide_Cube.transform.position.y < 0.5f)
                //        {
                //            isQuit = true;
                //        }
                //        break;
                //    }
                //}

                //if (isQuit == true)
                //{
                //    break;
                //}

            }
        }


    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Guide_Cube.transform.position, Vector3.one);
    }

}
