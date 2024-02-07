using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    public int Ending_Num;
    public float Angle = 45f;
    public float Move_interval = 5f;

    [SerializeField] float _distance;
    [SerializeField] bool isGizmoOn = true;
    //public bool isTrain = false;
    public Car isConnected = null;
    public int _stateNum = 0;
    public AnimationCurve[] MoveCurve;
    public float Power;
    public GameObject[] Bar;
    public GameObject Wall;

    //public Material InColor, OutColor, Broke_Color;

    public float Broke_Limit_Power = 10f;
    public Transform MovingPos_Trans;
    public List<GameObject> Save_Lugg_List;

    [SerializeField]
    StageManager _stageManager;
    //[SerializeField]
    //Transform Goal_Trans;

    public enum Mattress_Type
    {
        Normal,
        Mattress,
        Person
    }
    public Mattress_Type state;
    public enum Car_State
    {
        Wait,
        Run,
        Horizontal,
        Vertical

    }
    public Car_State car_state;

    Car_State _temp_car_state;
    //public GameObject[] Portal = new GameObject[2];
    public float Bar_Speed = 50f;

    public Vector3 Gizmo_Pos;
    public Vector3 Gizmo_Size = Vector3.one;
    ////////////////

    [SerializeField] Vector3 start_pos;
    [SerializeField] Quaternion start_rot;
    [SerializeField] bool isLeft = false;
    Rigidbody rb;

    Coroutine _cor, _cor2;


    bool isEnd = false;
    public bool isBarUp = false;
    [SerializeField] bool isCenter = false;
    public BoxCollider Range_Col;

    bool isInit = true;

    public float Through_Power = 100f;
    bool first = true;
    Sequence _sequence;

    GameManager _gm;

    
    private void Awake()
    {

        start_pos = transform.position;
        start_rot = transform.rotation;
        if (car_state == Car_State.Run)
        {
            //Portal[0] = GameObject.FindGameObjectWithTag("Portal1");
            //Portal[1] = GameObject.FindGameObjectWithTag("Portal2");
        }

        //_cor = StartCoroutine(BarOnOff(false));

        _stageManager = transform.parent.GetComponent<StageManager>()
            == null ? transform.parent.parent.GetComponent<StageManager>()
            : transform.parent.GetComponent<StageManager>();

        //Goal_Trans = _stageManager.Goal_Trans;
        _temp_car_state = car_state;
    }

    private void OnEnable()
    {
        _gm = GameManager.instance;
        transform.position = start_pos;
        transform.rotation = start_rot;
        Wall.SetActive(false);

        car_state = _temp_car_state;
        //_gm.Item = new object[_stageManager.Luggage_Prefabs.Length][];
        //_gm.Item = new object[6][];

        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }

        isEnd = false;
        isCenter = false;
        if (rb != null)
            rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        Save_Lugg_List.Clear();
        first = true;

        if (car_state == Car_State.Run)
        {
            rb.isKinematic = false;
            //GetComponent<Collider>().enabled = true;
        }

        //_gm.Map_House_Shutter.transform.DOScaleY(0.1f, 1f);
        if (isConnected == null)
        {
            _gm.Map_Background[_stageManager.Map_Num].transform.GetChild(0).GetChild(0).DOScaleY(0.1f, 1f);
        }
        //_stageManager.House_Door.DOScaleY(0.1f, 1f);

        isInit = true;
        switch (car_state)
        {
            case Car_State.Horizontal:
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                break;

            case Car_State.Vertical:
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                break;

            default:
                if (rb != null)
                    rb.constraints =
                    RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
                break;
        }


        //StartCoroutine(BarOnOff(false));
        _cor2 = StartCoroutine(CarStateMove());

    }

    void Update()
    {
        InputKey();

        if (Input.GetKey(KeyCode.Space))
        {
            //rb.velocity += transform.forward * Power * Time.deltaTime;

            //rb.AddForce(transform.forward * Power);
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Power);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
            }
            transform.position = start_pos;
            transform.rotation = start_rot;
            isEnd = false;

        }

        if (car_state == Car_State.Run)
        {
            rb.velocity = transform.forward * Power;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            //transform.DOShakeScale(5f, 1f,2,20f,false);
            transform.DOShakeRotation(5f, new Vector3(5, 5, 10), 5, 90f, true);


        }

        //if (GameManager.instance.state == GameManager.State.End)
        //{
        //    if (isEnd == false)
        //    {
        //        isEnd = true;
        //        StartCoroutine(ShowLuggListFunc());


        //    }
        //}

        if (_gm.state == GameManager.State.Wait)
        {
            if (isInit)
            {
                isInit = false;
                _cor = StartCoroutine(BarOnOff(false));
            }
        }


        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    foreach (GameObject _obj in Save_Lugg_List)
        //    {
        //        _obj.GetComponent<Rigidbody>()
        //                 .AddForce(
        //                 new Vector3(0f
        //                 , Mathf.Sin(Mathf.PI * Angle / 180f)
        //                 , Mathf.Cos(Mathf.PI * Angle / 180f)) * Through_Power);
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    GetComponent<Rigidbody>().AddTorque(Through_Power, 0f, 0f);
        //}

        CarStateMove();

    }

    IEnumerator CarStateMove()
    {
        while (true)
        {
            yield return null;
            if (_gm.state == GameManager.State.Play)
            {
                if (isConnected == false)
                {
                    switch (car_state)
                    {
                        case Car_State.Horizontal:
                            if (isLeft == true)
                            {
                                GetComponent<Rigidbody>().DOMoveX(start_pos.x + Move_interval, 2f);
                                yield return new WaitForSeconds(2f);
                                isLeft = false;
                            }
                            else
                            {
                                GetComponent<Rigidbody>().DOMoveX(start_pos.x - Move_interval, 2f);
                                yield return new WaitForSeconds(2f);
                                isLeft = true;
                            }

                            break;

                        case Car_State.Vertical:
                            if (isLeft == true)
                            {
                                GetComponent<Rigidbody>().DOMoveZ(start_pos.z + Move_interval, 2f);
                                yield return new WaitForSeconds(2f);
                                isLeft = false;
                            }
                            else
                            {
                                GetComponent<Rigidbody>().DOMoveZ(start_pos.z - Move_interval, 2f);
                                yield return new WaitForSeconds(2f);
                                isLeft = true;
                            }

                            break;

                        default:

                            break;
                    }
                }
            }
        }
    }

    public void ShowLuggFunc()
    {
        StartCoroutine(ShowLuggListFunc());
    }

    IEnumerator ShowLuggListFunc()
    {
        //yield return new WaitForSeconds(1f);
        _gm.state = GameManager.State.End;
        yield return new WaitForSeconds(1f);

        Collider[] _cols = Physics.OverlapBox(transform.position + Range_Col.center, Range_Col.size);

        foreach (Collider _col in _cols)
        {
            if (_col.CompareTag("Luggage"))
            {
                if (_col.GetComponent<Luggage>().isBreak == false && _col.GetComponent<Luggage>().isCheck == false)
                {
                    _col.GetComponent<Luggage>().isCheck = true;
                    Save_Lugg_List.Add(_col.gameObject);
                }
            }
        }


        // ///////////////////////////
        switch (Ending_Num)
        {
            case 1:
                int k = 0;
                for (int i = 0; i < Mathf.Round(Save_Lugg_List.Count / 4) + 1; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (k < Save_Lugg_List.Count)
                        {
                            Save_Lugg_List[k].GetComponent<Rigidbody>().isKinematic = true;
                            Save_Lugg_List[k].transform.DOJump(new Vector3(-10.5f + j * 7f, 0.2f, 205f - i * 7f), 20f, 1, 0.5f);
                            Save_Lugg_List[k].transform.DORotate(
                                Save_Lugg_List[i].GetComponent<Luggage>().Rot // 
                                                                              //Vector3.zero
                                , 0.5f);
                            _gm.Total_Score += Save_Lugg_List[i].GetComponent<Luggage>().Price;

                            //     GameObject _panel = Instantiate(_gm.Price_Panel
                            //, Save_Lugg_List[k].transform.position + _gm.Price_Panel_Pos
                            //, _gm.Price_Panel.transform.rotation);
                            //     _panel.transform.SetParent(Save_Lugg_List[k].transform);
                            //     _panel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                            //= string.Format("$ {0}", Save_Lugg_List[k].GetComponent<Luggage>().LuggScore);
                            //.DOMove(new Vector3( 0f + i * 2f, 3f, 210f),0.2f);
                            yield return new WaitForSeconds(0.1f);
                            k++;
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);

                foreach (GameObject _obj in Save_Lugg_List)
                {
                    GameObject _panel = Instantiate(_gm.Price_Panel
                        , _obj.transform.position + _gm.Price_Panel_Pos
                        , _gm.Price_Panel.transform.rotation);
                    _panel.transform.SetParent(_obj.transform);
                    _panel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                         = string.Format("${0}", _obj.GetComponent<Luggage>().Price);

                }


                yield return new WaitForSeconds(2f);
                _gm.Smoke_Effect.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                foreach (GameObject _obj in Save_Lugg_List)
                {
                    _obj.SetActive(false);
                }


                break;

            case 2:


                GetComponent<Rigidbody>().DORotate(new Vector3(Angle, 0f, 0f), 0.2f);
                yield return new WaitForSeconds(0.1f);

                //Collider[] _carcols = transform.GetChild(0).GetComponents<Collider>();

                //foreach(Collider _col in _carcols)
                //{
                //    _col.isTrigger = true;
                //}
                //Physics.IgnoreCollision(0, 7, false);

                foreach (GameObject _obj in Save_Lugg_List)
                {
                    _gm.Total_Score += _obj.GetComponent<Luggage>().Price;

                    _obj.GetComponent<Rigidbody>()
                        .AddForce(
                        new Vector3(0f
                        , Mathf.Sin(Mathf.PI * Angle / 180f)
                        , Mathf.Cos(Mathf.PI * Angle / 180f)) * Through_Power);




                    //yield return null;
                }
                yield return new WaitForSeconds(0.3f);
                GetComponent<Rigidbody>().DORotate(new Vector3(0f, 0f, 0f), 0.2f);
                yield return new WaitForSeconds(0.2f);

                //foreach (Collider _col in _carcols)
                //{
                //    _col.isTrigger = false;
                //}

                break;

            case 3:


                //GetComponent<Rigidbody>().DORotate(new Vector3(Angle, 0f, 0f), 0.2f);
                //yield return new WaitForSeconds(0.1f);

                foreach (GameObject _obj in Save_Lugg_List)
                {
                    _gm.Total_Score += _obj.GetComponent<Luggage>().Price;
                    _obj.GetComponent<Collider>().isTrigger = true;
                    _obj.GetComponent<ConstantForce>().force = Vector3.zero;
                    _obj.GetComponent<Rigidbody>()
                        .DOJump(new Vector3(
                            _obj.transform.position.x
                        , _gm.Ending_Jump_Pos.position.y
                        , _gm.Ending_Jump_Pos.position.z), 30f, 0, 1f)
                        .SetEase(_gm._Curve[0]);

                    //yield return new WaitForSeconds(0.2f);
                    //_sequence.AppendInterval(0.9f)
                    //    .Append(_stageManager.House_obj.DOScale(Vector3.one * 0.8f, 0.05f))
                    //    .Append(_stageManager.House_obj.DOScale(Vector3.one * 0.667f, 0.05f));


                    yield return new WaitForSeconds(0.1f);
                    _gm.Map_House.transform.DOScale(Vector3.one * 0.8f, 0.05f);
                    //_stageManager.House_obj.DOScale(Vector3.one * 0.8f, 0.05f);
                    yield return new WaitForSeconds(0.05f);
                    _gm.Map_House.transform.DOScale(Vector3.one * 0.667f, 0.05f);
                    //_stageManager.House_obj.DOScale(Vector3.one * 0.667f, 0.05f);
                    yield return new WaitForSeconds(0.05f);

                }
                yield return new WaitForSeconds(0.3f);
                //GetComponent<Rigidbody>().DORotate(new Vector3(0f, 0f, 0f), 0.2f);
                //yield return new WaitForSeconds(0.2f);


                break;


            case 4:

                foreach (GameObject _obj in Save_Lugg_List)
                {
                    _gm.Total_Score += _obj.GetComponent<Luggage>().Price;
                    _sequence.Append(_gm.Map_House.transform.DOScale(Vector3.one * 1.2f, 0.05f)).SetEase(MoveCurve[1])
                        .Append(_gm.Map_House.transform.DOScale(Vector3.one, 0.05f)).SetEase(MoveCurve[1]);

                    yield return new WaitForSeconds(0.1f);

                    InsertItem(_obj);

                }

                break;
            default:

                break;
        }
        yield return new WaitForSeconds(1f);

        if (_stateNum == 0)
        {
            //Debug.Log(string.Format("{0} : {1}", transform.name, _stateNum));
            _gm.ClearCheck();
            //Debug.Log("ClearCheck");
        }


    }

    public void InsertItem(GameObject _obj)
    {
        Luggage _lugg = _obj.GetComponent<Luggage>();

        if (_gm.Item.ContainsKey(_lugg.Obj_Name))
        {
            _gm.Item[_lugg.Obj_Name]._count++;
        }
        else
        {
            Info _info = new Info();
            _info._img = _lugg.Img;
            _info._price = _lugg.Price;
            _info._count = 1;

            _gm.Item.Add(_lugg.Obj_Name, _info);
        }

    }



    public void InputKey()
    {
        switch (Input.inputString)
        {
            case "3":
                try
                {
                    StopCoroutine(_cor);
                }
                catch { }
                _cor = StartCoroutine(BarOnOff(true));

                break;
            case "4":
                try
                {
                    StopCoroutine(_cor);
                }
                catch { }
                _cor = StartCoroutine(BarOnOff(false));

                break;


            case "Escape":
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.isKinematic = false;
                }
                transform.position = start_pos;
                transform.rotation = start_rot;
                break;

            default:

                break;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Luggage"))
        {
            //other.GetComponent<MeshRenderer>().material = InColor;
            if (other.GetComponent<Luggage>().isBreak == false)
            {
                //Save_Lugg_List.Add(other.gameObject);
                //GameManager.instance.Total_Score += other.GetComponent<Luggage>().LuggScore;
                //Debug.Log(Save_Lugg_List.Count);
            }

        }

        else if (other.CompareTag("Goal"))
        {
            //_stageManager.House_Door.DOScaleY(1f, 1f);
            _gm.Map_House_Shutter.transform.DOScaleY(1f, 1f);


            CheckScore();
        }

        else if (other.CompareTag("Ending_View"))
        {
            Camera.main.GetComponent<Cam>().isEndingView = true;
        }

        //else if (other.CompareTag("Portal1"))
        //{
        //    transform.position = Portal[1].transform.position;
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Luggage"))
        {
            if (other.GetComponent<Luggage>().isBreak == false)
            {
                //other.GetComponent<MeshRenderer>().material = OutColor;
                //GameManager.instance.Total_Score -= other.GetComponent<Luggage>().LuggScore;
            }
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Luggage"))
        {
            Luggage _lug = collision.transform.GetComponent<Luggage>();

            if (_lug.isFixObj == false)
            {
                switch (state)
                {
                    case Mattress_Type.Normal:
                        if (_lug.state == Luggage.State.Glass)
                        {
                            if (_lug.isPopUp == false)
                            {
                                _lug.isPopUp = true;
                                _lug.BreakFunc();
                                GameManager.instance.PopUpFunc(collision.transform.position, false);
                            }


                            //collision.transform.GetComponent<MeshRenderer>().material = Broke_Color;
                            //collision.transform.GetComponent<Luggage>().isBreak = true;
                        }
                        else
                        {
                            if (_lug.isPopUp == false)
                            {
                                _lug.isPopUp = true;
                                GameManager.instance.PopUpFunc(collision.transform.position, true);
                            }
                        }


                        break;

                    case Mattress_Type.Mattress:
                        if (_lug.isPopUp == false)
                        {
                            _lug.isPopUp = true;
                            GameManager.instance.PopUpFunc(collision.transform.position, true);
                        }
                        break;

                    case Mattress_Type.Person:
                        collision.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

                        break;

                    default:

                        break;
                }
            }
        }
    }

    public void MoveCar()
    {
        if (_stateNum == 2)
        {
            _cor = StartCoroutine(BarOnOff(true));
        }

        else if (_stateNum != 2)
        {
            rb.isKinematic = false;
            //GetComponent<Rigidbody>().DOMove(new Vector3(0f,5f,25f), 1f);
            //transform.DOMove(start_pos, 1f);

            StopAllCoroutines();
            StopCoroutine(_cor2);


            StartCoroutine(Cor_StartMove());

            try
            {
                StopCoroutine(_cor);
            }
            catch { }
            _cor = StartCoroutine(BarOnOff(true));


            StartCoroutine(Cor_Move());

            /////////////////////////////
            IEnumerator Cor_StartMove()
            {
                while (true)
                {
                    yield return null;
                    transform.position = Vector3.MoveTowards(transform.position, start_pos, Time.deltaTime * 5f);
                    _distance = Vector3.Distance(transform.position, start_pos);
                    if (_distance < 0.05f)
                    {
                        //transform.position = start_pos;
                        GetComponent<Rigidbody>().constraints =
                            //RigidbodyConstraints.FreezePositionX |
                            RigidbodyConstraints.FreezeRotation;
                        isCenter = true;
                        break;
                    }
                }
            }

            IEnumerator Cor_Move()
            {
                float _time = 0f;
                //rb.isKinematic = false;
                float max_distance = Vector3.Distance(transform.position, _gm.Map_Goal_Trans.position);
                float _distance = 0f;

                while (isEnd == false)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                    if (isBarUp == true && isCenter == true)
                    {
                        if (first == false)
                        {
                            first = true;
                            //rb.isKinematic = false;
                            _gm.state = GameManager.State.Drive;
                        }
                        _time += Time.deltaTime;
                        //rb.AddForce(transform.forward * Power);
                        //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Power*MoveCurve[0].Evaluate(_time/5f));
                        _distance = Vector3.Distance(transform.position,
                            _gm.Map_Goal_Trans.position);

                        //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Power * MoveCurve[0]
                        rb.velocity = rb.transform.forward * Power * MoveCurve[0]
                            .Evaluate((max_distance - _distance + 10f) / max_distance);
                        //.Evaluate((Goal_Trans.position.z - (Goal_Trans.position.z - transform.position.z + 10f)) / Goal_Trans.position.z));
                        //transform.Translate(transform.forward * Power * Time.deltaTime);
                    }
                }
            }
        }
    }

    public void CheckScore()
    {
        //isEnd = true;
        GetComponent<Collider>().enabled = true;
        //GameManager.instance.state = GameManager.State.End;
        if (isEnd == false)
        {
            isEnd = true;
            _stageManager.ArriveCar(_stateNum);
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().isKinematic = true;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            //StartCoroutine(ShowLuggListFunc());
        }
    }

    private void OnDrawGizmos()
    {
        if (isGizmoOn == true)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position + Range_Col.center, Range_Col.size);
        }

    }


    public void SetCam()
    {
        if (car_state != Car_State.Run)
        {
            Camera.main.GetComponent<Cam>().SetOffset(transform);
        }
    }


    IEnumerator BarOnOff(bool isOn)
    {
        if (isOn == false)
        {
            //Vector3 _scale = Bar[0].transform.GetChild(0).transform.localScale;
            Bar[0].transform.transform.DOScaleY(0f, 0.1f);
            isBarUp = false;
        }

        else if (isOn == true)
        {
            //Vector3 _scale = Bar[0].transform.GetChild(0).transform.localScale;
            Bar[0].transform.transform.DOScaleY(1f, 1f);
            yield return new WaitForSeconds(1f);
            isBarUp = true;
            first = false;
            Wall.SetActive(true);
        }


        ////////////////////////////////////////////////////////////////////////


        //float _angle = 0f;
        //float _total_angle = 0f;
        //if (isOn)
        //{

        //    _angle = Bar_Speed;
        //    while (true)
        //    {
        //        Bar[0].transform.Rotate(_angle, 0f, 0f);
        //        _total_angle += _angle;
        //        if (_total_angle >= 135f)
        //        {
        //            isBarUp = true;
        //            Bar[0].transform.rotation = Quaternion.Euler(Vector3.zero);
        //            break;
        //        }
        //        yield return new WaitForSeconds(Time.deltaTime);
        //    }
        //}
        //else
        //{
        //    isBarUp = false;
        //    _angle = -Bar_Speed;
        //    while (true)
        //    {
        //        Bar[0].transform.Rotate(_angle, 0f, 0f);
        //        _total_angle += _angle;
        //        if (_total_angle <= -135f)
        //        {

        //            Bar[0].transform.rotation = Quaternion.Euler(new Vector3(-135f, 0f, 0f));
        //            break;
        //        }

        //        yield return new WaitForSeconds(Time.deltaTime);
        //    }
        //}



    }


}