using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ending_Block : MonoBehaviour
{

    public Transform Dish_Trans;
    public Queue<GameObject> End_List;
    public int Food_Count = 5;
    public bool isFinal = false;
    public GameObject Effect;
    public bool isFull = false;

    //Material _mat;

    AudioSource _audio;

    static public Ending_Block _instance;

    private void Awake()
    {
        End_List = new Queue<GameObject>();
        //_mat = GetComponent<MeshRenderer>().material;
        _audio = GetComponent<AudioSource>();


        if (isFinal == true)
        {
            _instance = this;
        }

        Effect.SetActive(false);
    }

    public void Init()
    {
        if (End_List.Count != 0)
        {
            End_List.Clear();
        }

        if (Effect != null)
        {
            Effect.SetActive(false);
        }
        isFull = false;
        //_mat.SetFloat("_Metallic", 0.5f);
        //_mat.SetFloat("_Glossiness", 1f);
        Effect.SetActive(false);
    }


    public void AddStack(GameObject _obj)
    {

        End_List.Enqueue(_obj);
        isFull = true;
        //_mat.SetFloat("_Metallic", 0f);
        //_mat.SetFloat("_Glossiness", 0.5f);
        //_audio.Play();
    }

    public void Cor_Move_Func(bool isClear)
    {
        StartCoroutine(Cor_Move(isClear));

        
    }

    

    IEnumerator Cor_Move(bool isClear)
    {

        int _count = End_List.Count;
        float _y = 0.25f;

        if (isFinal == true)
        {
            if (isClear)
            {
                yield return new WaitForSeconds(1f);

               
            }
        }

        for (int i = 0; i < _count; i++)
        {
            
            if (NewGameManager.instance.isSound)
            {
                _audio.Play();
            }
            if (isFinal == false)
            {
                End_List.Peek().transform
                    .DOJump(new Vector3(Dish_Trans.position.x
                    , _y
                    , Dish_Trans.position.z)
                    , 7, 0
                    , 0.7f);
                _y += End_List.Peek().GetComponent<ShootObj>().Size_Y;
            }
            else
            {

                End_List.Peek().transform
               .DOMove(new Vector3(Dish_Trans.position.x
               //, End_List.Peek().GetComponent<ShootObj>().Size_Y * i
               , Dish_Trans.position.y
               , Dish_Trans.position.z)
               , 0.5f);
                //_y += End_List.Peek().GetComponent<ShootObj>().Size_Y;
            }

            End_List.Enqueue(End_List.Dequeue());
            yield return new WaitForSeconds(0.05f);
        }
        if (isFinal)
        {
            if (isClear)
            {
                yield return new WaitForSeconds(0.6f);
            }

            NewGameManager.instance.Ending_Func(isClear);
        }
        else
        {
            //yield return new WaitForSeconds(0.5f);
        }
        if (isFull)
        {
            //yield return new WaitForSeconds(1f);
            if (isFinal)
            {
                yield return new WaitForSeconds(2f);
            }
            Effect.SetActive(true);
        }
    }

    public void OffObj()
    {
        foreach (GameObject _obj in End_List)
        {
            _obj.SetActive(false);
        }
    }
}
