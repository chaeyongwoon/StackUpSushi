using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ending_Dish : MonoBehaviour
{
    [SerializeField] Vector3 Start_Pos;
    public int Ending_Block_Count = 0;
    //public bool isStop = true;
    public Transform End_Trans;
    [SerializeField] int _n = 0;

    public Stack<GameObject> Stack_list;
    public Queue<GameObject> Queue_list;
    //[SerializeField] Transform[] _list;
    public float Speed = 5f;
    bool IsStack = false;
    [SerializeField] float _height=0f;


    private void Awake()
    {
        Start_Pos = transform.position;
        Stack_list = new Stack<GameObject>();
        Queue_list = new Queue<GameObject>();
    }

    public void Init()
    {
        _height = 0.1f;
        transform.position = Start_Pos;
        GetComponent<Collider>().enabled = true;
        //isStop = true;
        _n = 0;
        if (Stack_list.Count != 0)
        {
            Stack_list.Clear();
        }
        if (Queue_list.Count != 0)
        {
            Queue_list.Clear();
        }
    }

    private void Update()
    {
        //if (isStop == false)
        //{
        //    transform.Translate(transform.forward * Speed * Time.deltaTime);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isClear = false;
        if (other.CompareTag("Ending_Block"))
        {

            if (other.GetComponent<Ending_Block>().isFinal == false)
            {
                NewGameManager.instance.Vibe(3);
                for (int i = 0; i < other.GetComponent<Ending_Block>().Food_Count; i++)
                {
                    if (IsStack == true)
                    {
                        if (Stack_list.Count != 0)
                        {
                            other.GetComponent<Ending_Block>().AddStack(Stack_list.Peek());
                            Stack_list.Pop().transform.SetParent(null);

                        }
                        else
                        {
                            //isStop = true;
                        }
                    }
                    else
                    {
                        if (Queue_list.Count != 0)
                        {
                            if (NewGameManager.instance.isVibe)
                            {
                                NewGameManager.instance.Vibe(3);
                            }

                            other.GetComponent<Ending_Block>().AddStack(Queue_list.Peek());
                            Queue_list.Dequeue().transform.SetParent(null);

                            int _queueCount = 0;
                            _queueCount = Queue_list.Count;
                            for (int j = 0; j < _queueCount; j++)
                            {
                                Queue_list.Peek().transform.DOMoveY(Queue_list.Peek().transform.position.y - 0.6f * 5, 0.5f);
                                Queue_list.Enqueue(Queue_list.Dequeue());
                            }
                        }
                        else
                        {
                            //isClear = false;
                            //isStop = true;
                        }
                    }
                }
            }
            else // isFianl == true
            {
                
                GetComponent<Collider>().enabled = false;
                //isStop = true;
                bool istrue = false;
                while (true)
                {
                    if (Stack_list.Count != 0)
                    {
                        other.GetComponent<Ending_Block>().AddStack(Stack_list.Peek());
                        Stack_list.Pop().transform.SetParent(null);
                        istrue = true;
                    }
                    else
                    {
                        isClear = istrue == true ? true : false;


                        break;
                    }
                }
                //NewGameManager.instance.Ending_Func(isClear);
            }

            other.GetComponent<Ending_Block>().Cor_Move_Func(isClear);


        }
    }

    public void AddObj(GameObject _obj, bool isStack)
    {
        IsStack = isStack;
        if (isStack == true)
        {
            Stack_list.Push(_obj);
            _obj.transform.position = new Vector3(transform.position.x
                , _height
                //, _n * 0.6f
                , transform.position.z);
            _height += _obj.GetComponent<ShootObj>().Size_Y;
            _obj.transform.SetParent(transform);
            _n++;
        }
        else
        {
            Queue_list.Enqueue(_obj);
            _obj.transform.position = new Vector3(transform.position.x
                //, _obj.GetComponent<ShootObj>().Size_Y * _n
                , _n * 0.6f
                , transform.position.z);
            _obj.transform.SetParent(transform);
            _n++;
        }
    }


    public void EndingDish_Start()
    {
        StartCoroutine(Cor_Move());

        IEnumerator Cor_Move()
        {
            transform.position = Start_Pos;
            transform.DOMoveX(0f, 1f);
            yield return new WaitForSeconds(1f);
            transform.DOMoveZ(End_Trans.position.z, 4f).SetEase(Ease.Linear);

            //yield return new WaitForSeconds(4f);
            //NewGameManager.instance.Ending_Func(true);
            //isStop = false;
        }
    }


}
