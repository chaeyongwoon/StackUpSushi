using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shoot : MonoBehaviour
{
    public GameObject Selected_Obj;
    public GameObject[] Shoot_List;
    public GameObject Guide_Obj;

    public int Max_Count = 5, Current_Count = 0;
    public float Constantforce_Y = -70f;
    public float Obj_Distance_Interval = 5f;
    public float Obj_Move_Interval = 0.5f;
    public float Obj_BackDistance_Interval = 5f;

    public float Reload_Interval = 0f;

    public float Sens_magnif = 0.01f;
    public float power = 50f;
    public int Line_Pos_Count = 100;

    public float Angle = 45f;
    public float Parabola_time = 2f;
    public float Parabola_Division_time = 0.02f;
    public Vector3 temp_pos;


    public Vector3 Dir, Dir2;
    public float distance = 0f;
    public int Rand_Num = 0;
    public float Shoot_Interval = 0.2f;

    public int Max_Bullet = 5;
    public float Shoot_Interval2 = 0.2f;

    // ------------------------------

    public Queue<GameObject> ShootQueue;

    [SerializeField] Collider[] _cols;
    bool isClick = false;
    //bool isShoot = false;
    bool islast = false;
    public Vector3 Start_Pos, End_Pos;
    LineRenderer _line;
    AudioSource _audio;
    bool isReload = true;

    // ---------------------------------

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        ShootQueue = new Queue<GameObject>();
        _audio = GetComponent<AudioSource>();
    }

    public void Init()
    {
        StopAllCoroutines();
        StartCoroutine(Cor_Update());

        Current_Count = 0;
        isClick = false;
        Guide_Obj.SetActive(false);
        if (ShootQueue.Count != 0)
        {
            ShootQueue.Clear();
        }

        int _num = 0;
        foreach (GameObject _shootObj in Shoot_List)
        {
            GameObject _obj = Instantiate(_shootObj, new Vector3(0f, 0f, 0f), Quaternion.identity);
            ShootQueue.Enqueue(_obj);
            if (_num == 0)
            {
                _obj.transform.DOMove(transform.position, Obj_Move_Interval);
            }
            else
            {
                _obj.transform.DOMove(new Vector3(-Obj_Distance_Interval + Obj_Distance_Interval * (_num - 1), 0f, transform.position.z - Obj_BackDistance_Interval), Obj_Move_Interval);
            }
            _num++;

        }



    }

    private void Start()
    {
        StartCoroutine(Cor_Update());
    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            if (NewGameManager.instance.isReady == false)
            {
                StartCoroutine(MouseFunc());
            }

            if (Input.GetKey(KeyCode.A))
            {
                if (isReload)
                {
                    StartCoroutine(Cor_Shoot());
                    //Cor_Shoot();
                }
            }

            yield return null;
        }
    }


    IEnumerator MouseFunc()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Guide_Obj.SetActive(true);
            Start_Pos = Input.mousePosition;
            End_Pos = Input.mousePosition;

            if (isClick == false /*&& isShoot == false*/ && Current_Count < Max_Count)
            {
                isClick = true;

                _line.positionCount = 0;
                Line_Pos_Count = (int)(Parabola_time / Parabola_Division_time);
                _line.positionCount = Line_Pos_Count + 1;

                Guide_Obj.transform.position = Vector3.down * 100f;
                //Guide_Obj.SetActive(true);
                Selected_Obj = ShootQueue.Dequeue();
                Selected_Obj.transform.DOMove(transform.position, 0.3f);
                //Instantiate(Shoot_List[Current_Count], transform.position, Quaternion.identity);
                Current_Count++;





            }
        }

        else if (Input.GetMouseButton(0))
        {

            if (isClick == true /*&& isShoot == false*/)
            {


                End_Pos = Input.mousePosition;

                Dir = new Vector3(Start_Pos.x - End_Pos.x, 0f, Start_Pos.y - End_Pos.y).normalized;
                distance = Vector3.Distance(End_Pos, Start_Pos);



                float _time = 0f;

                islast = false;

                for (int i = 0; i <= Parabola_time / Parabola_Division_time; i++)
                {
                    Dir2 = new Vector3(Dir.x
                        , Mathf.Sin(Mathf.PI * Angle / 180f) * Mathf.Sqrt(Dir.x * Dir.x + Dir.z * Dir.z)
                        , Mathf.Cos(Mathf.PI * Angle / 180f) * Dir.z);


                    _line.SetPosition(i,
                        transform.position
                        + Dir2
                        * distance * Sens_magnif * _time
                        + Vector3.up * 0.5f * (Physics.gravity.y + Constantforce_Y) * _time * _time
                        );
                    _time += Parabola_Division_time;


                    if (i > 2)
                    {
                        if (islast == false)
                        {
                            _cols = Physics.OverlapBox(_line.GetPosition(i) + Guide_Obj.GetComponent<BoxCollider>().center
                                , Guide_Obj.GetComponent<BoxCollider>().size * 0.5f
                                , Quaternion.identity);

                            Guide_Obj.transform.position = _line.GetPosition(i);
                            if (_cols.Length != 0)
                            {
                                islast = true;
                                temp_pos = _line.GetPosition(i);
                                //temp_pos = new Vector3(_line.GetPosition(i).x, 0.25f, _line.GetPosition(i).z);
                                Guide_Obj.transform.position = temp_pos;
                            }

                        }
                        else
                        {
                            _line.SetPosition(i, temp_pos);

                        }
                    }


                }




                //Guide_Obj.transform.localScale = Selected_Lugg.transform.GetChild(0).transform.localScale;

                //Guide_Obj.GetComponent<MeshFilter>().mesh
                //= Selected_Lugg.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
                //= Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Guide_Mesh;
                //Guide_Obj.transform.rotation = Quaternion.Euler(Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Rot);





            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            Guide_Obj.SetActive(false);
            End_Pos = Vector3.zero;

            if (isClick == true && distance >= 100f)
            {
                if (NewGameManager.instance.isSound)
                {
                    _audio.Play();
                }
                isClick = false;
                //isShoot = true;
                //Start_Pos = Vector3.zero;

                Rigidbody Selected_rb = Selected_Obj.GetComponent<Rigidbody>();
                Selected_rb.GetComponent<Collider>().enabled = true;
                Selected_rb.GetComponent<Collider>().isTrigger = false;
                Selected_rb.isKinematic = false;
                Selected_rb.GetComponent<ConstantForce>().force = Vector3.up * Constantforce_Y;
                Selected_rb.AddForce(Dir2 * power * distance * Sens_magnif);


                //StartCoroutine(ShootLugg2());


                _line.positionCount = 0;
                Guide_Obj.SetActive(false);

                yield return new WaitForSeconds(Reload_Interval);


                Selected_Obj = null;

                if (Current_Count >= Max_Count)
                {
                    yield return new WaitForSeconds(2f);
                    NewGameManager.instance.EndingFunc();
                }


                int _num = 0;
                foreach (GameObject _obj in ShootQueue)
                {
                    if (_num == 0)
                    {
                        _obj.transform.DOMove(transform.position, Obj_Move_Interval);
                    }
                    else
                    {
                        _obj.transform.DOMove(new Vector3(-Obj_Distance_Interval + Obj_Distance_Interval * (_num - 1), 0.2f, transform.position.z - Obj_BackDistance_Interval), Obj_Move_Interval);
                    }
                    _num++;

                }

                //_audio.Stop();

            }
        }
    }

    IEnumerator Cor_Shoot()
    {
        //for (int i = 0; i < Max_Bullet; i++)
        //{
        isReload = false;
        GameObject _obj = Instantiate(
            NewGameManager.instance.Bullet_List[Random.Range(0, NewGameManager.instance.Bullet_List.Length)]
            //Shoot_List[Current_Count-1]
            , transform.position, Quaternion.identity);

        Rigidbody Selected_rb = _obj.GetComponent<Rigidbody>();
        Selected_rb.GetComponent<Collider>().enabled = true;
        Selected_rb.GetComponent<Collider>().isTrigger = false;
        Selected_rb.isKinematic = false;
        Selected_rb.GetComponent<ConstantForce>().force = Vector3.up * Constantforce_Y;
        Selected_rb.AddForce(Dir2 * power * distance * Sens_magnif);

        yield return new WaitForSeconds(Shoot_Interval2);
        isReload = true;
        //}
    }


}
