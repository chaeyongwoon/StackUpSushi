using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using MondayOFF;

public class NewGameManager : MonoBehaviour
{
    public int Stage_Level = 1;
    public GameObject[] Stage_List;
    public GameObject[] Bullet_List;

    

    public Transform Cam_Ending_Pos;
    public Transform[] Car_Ending_Pos;
    public Transform[] Portal;
    public Material[] Rail_Mat;

    public Transform Ending_Block_Group;
    public Transform Ending_Effect_Group;

    public SkinnedMeshRenderer _Eye; // 0 : sad , 1 : happy
    public SkinnedMeshRenderer _EyeBrow; // 0 : happy , 1: sad
    public SkinnedMeshRenderer _Mouth; // 0: size , 1: happy , 2: sad

    // sad : 0,1,2
    // happy : 1,0,0
    // normal : 0,0,0~1




    [Header("SerializeField")]
    [SerializeField] bool isEnd = false;
    [SerializeField] public int Max_Vehicle = 0;
    [SerializeField] int Current_Vehicle = 0;
    [SerializeField] float j = 0, q = 0, k = 0;
    [SerializeField] float _X = 0f, _Z = 0f;
    [SerializeField] Queue<Vehicle> Vehicle_List;
    [SerializeField] Vector3 Start_pos;
    [SerializeField] bool RunningCor = true;



    public Shoot ShootPlayer;
    public Ending_Dish Ending_Dish;
    public static NewGameManager instance;


    public bool isCenter = false;
    public Transform Mouth;
    public bool FirstEnding = true;
    public bool isRandom = false;
    public bool isTop = false;
    public float MoveSpeed = 5f;
    public bool isReady = true;
    [SerializeField] GameObject[] _list1 = null;
    // ------------------------------------------------
    NewUIManager _newUiManager;
    public AudioSource _audio;
    public AudioClip[] _Clip;

    public bool isVibe = true;
    public bool isSound = true;
    public bool isCpi = false;
    


    private void Awake()
    {
        Application.targetFrameRate = 60;
        instance = this;
        if (Vehicle_List != null)
            Vehicle_List.Clear();
        Start_pos = transform.position;

        _newUiManager = GetComponent<NewUIManager>();
        _audio = GetComponent<AudioSource>();

        // key name
        // Stage_Level , isVibe ,isSound

        if (PlayerPrefs.HasKey("Stage_Level"))
        {
            Stage_Level = PlayerPrefs.GetInt("Stage_Level");
        }
        else
        {
            Stage_Level = 1;
            PlayerPrefs.SetInt("Stage_Level", Stage_Level);
        }

        if (PlayerPrefs.HasKey("isVibe"))
        {
            isVibe = PlayerPrefs.GetInt("isVibe") == 1 ? true : false;
        }
        else
        {
            isVibe = true;
            PlayerPrefs.SetInt("isVibe", 1);
        }

        
            

    }
    private void Start()
    {


        Vehicle_List = new Queue<Vehicle>();
        Init();
        Update_Stage();
        StartCoroutine(Cor_Update());
    }

    public void Init()
    {
        Camera.main.orthographicSize = 7.5f;
        Max_Vehicle = 0;
        Current_Vehicle = 0;
        Vehicle_List.Clear();
        isReady = true;
        //ShootPlayer.Init();
        transform.position = Start_pos;
        isEnd = false;
        j = 0;
        q = 0;
        k = 0;
        Ending_Dish.Speed = MoveSpeed;
        Ending_Dish.Init();

        _Eye.SetBlendShapeWeight(0, 100);
        _Eye.SetBlendShapeWeight(1, 0);
        _EyeBrow.SetBlendShapeWeight(0, 0);
        _EyeBrow.SetBlendShapeWeight(1, 0);
        _Mouth.SetBlendShapeWeight(0, 100);
        _Mouth.SetBlendShapeWeight(1, 0);
        _Mouth.SetBlendShapeWeight(2, 0);



        for (int i = 0; i < Ending_Block_Group.childCount; i++)
        {
            Ending_Block_Group.GetChild(i).GetComponent<Ending_Block>().Init();
            //Ending_Block_Group.GetChild(i).GetComponent<Ending_Block>().Effect
            //    = Ending_Effect_Group.GetChild(i).gameObject;
        }



        GameObject[] Destroy_List = GameObject.FindGameObjectsWithTag("Luggage");
        foreach (GameObject _obj in Destroy_List)
        {
            Destroy(_obj);
        }

        _newUiManager.Init();




    }
    public void Update_Stage()
    {
        foreach (GameObject _map in Stage_List)
        {
            _map.SetActive(false);
        }
        Stage_List[(Stage_Level - 1) % Stage_List.Length].SetActive(true);

        //EventsManager.instance.TryStage(Stage_Level);
        EventTracker.TryStage(Stage_Level);
    }


    IEnumerator Cor_Update()
    {
        yield return new WaitForSeconds(0.5f);
        RunningCor = true;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Stage_Level_Func(-1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Stage_Level_Func(1);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Retry();
            }

            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(Vehicle_List.Count);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                _Mouth.SetBlendShapeWeight(0, 100f);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                _Mouth.SetBlendShapeWeight(0, 50f);
            }
            yield return null;
        }
    }


    public void AddVehicle(Vehicle _vehicle)
    {
        Vehicle_List.Enqueue(_vehicle);
        Current_Vehicle++;

        if (Current_Vehicle >= Max_Vehicle)
        {
            if (isEnd == false)
            {
                isEnd = true;
                if (FirstEnding == true)
                {
                    StartCoroutine(Cor_Ending());
                }
                else
                {
                    StartCoroutine(Cor_Ending2());
                }
            }
        }
    }

    public void EndingFunc()
    {
        if (isEnd == false)
        {
            isEnd = true;
            if (FirstEnding == true)
            {
                StartCoroutine(Cor_Ending());
            }
            else
            {
                if (isCpi == false)
                {
                    StartCoroutine(Cor_Ending2());
                }
                else
                {
                    StartCoroutine(Cor_Ending3());
                }
            }

        }
    }


    IEnumerator Cor_Ending()
    {

        yield return new WaitForSeconds(1f);

        transform.DOMove(Cam_Ending_Pos.position, 1f);
        yield return new WaitForSeconds(1f);
        foreach (Vehicle _vehicle in Vehicle_List)
        {
            _vehicle.isStop = true;
            _vehicle.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            _vehicle.transform.position = Car_Ending_Pos[0].position;
            _vehicle.transform.DOMove(Car_Ending_Pos[1].position, 0.5f);
            yield return new WaitForSeconds(0.5f);
            bool isfirst = true;
            foreach (GameObject _obj in _vehicle.Obj_List)
            {
                if (RunningCor == false)
                {
                    yield break;
                }
                if (isfirst == true)
                {
                    isfirst = false;
                    j += _obj.GetComponent<ShootObj>().Size_X * 0.5f + _X * 0.5f;
                    //k += _obj.GetComponent<ShootObj>().Size_Z * 0.5f + _Z * 0.5f;
                }
                _obj.transform.SetParent(null);
                _obj.GetComponent<ShootObj>().SetRot();

                if (isCenter == false)
                {
                    _obj.transform.DOMove(new Vector3(-80f, 0.1f, 140f) + new Vector3(j, q, -k), 0.5f);



                    q += _obj.GetComponent<ShootObj>().Size_Y;

                    _X = _obj.GetComponent<ShootObj>().Size_X;
                    _Z = _obj.GetComponent<ShootObj>().Size_Z;
                    if (q >= 9.5f)
                    {
                        q = 0;
                        j += _obj.GetComponent<ShootObj>().Size_X * 0.5f + _X * 0.5f < 1 ? 1
                            : _obj.GetComponent<ShootObj>().Size_X * 0.5f + _X * 0.5f;

                        if (j >= 20)
                        {
                            j = 0;
                            k += _obj.GetComponent<ShootObj>().Size_Z * 0.5f + _Z * 0.5f;
                        }
                    }
                }
                else
                {
                    _obj.transform.DOMove(Mouth.position, 0.5f);
                }
                yield return new WaitForSeconds(0.1f);
            }
            q = 0;
            if (j >= 20)
            {
                j = 0;
                k += _Z;
            }
            //j +=_X *0.5f;

            _vehicle.transform.DOMove(Car_Ending_Pos[2].position, 0.3f);
        }
    }

    IEnumerator Cor_Ending2()
    {
        yield return new WaitForSeconds(1f);
        transform.DOMove(Cam_Ending_Pos.position, 1f).SetEase(Ease.Linear);
        for (float _t = 0f; _t < 1.5f; _t += Time.deltaTime * 1.5f)
        {
            Camera.main.orthographicSize = 7.5f + _t;
            yield return new WaitForSeconds(Time.deltaTime);

        }
        //yield return new WaitForSeconds(1f);

        //GameObject[] _list1 = null;

        int _count = 0;

        foreach (Vehicle _vehicle in Vehicle_List)
        {
            foreach (GameObject _obj in _vehicle.Obj_List)
            {
                _count++;
            }
        }

        _list1 = new GameObject[_count];
        _count = 0;
        foreach (Vehicle _vehicle in Vehicle_List)
        {
            _vehicle.isBounce = false;
            foreach (GameObject _obj in _vehicle.Obj_List)
            {

                if (isRandom == true)
                {
                    _list1[_count] = _obj;
                    _count++;
                }
                else
                {
                    Ending_Dish.AddObj(_obj, isTop);
                }

            }
        }

        if (isRandom == true)
        {


            int _rand = 0;
            for (int i = 0; i < _list1.Length; i++)
            {
                _rand = Random.Range(0, _list1.Length);
                GameObject temp = null;
                temp = _list1[i];
                _list1[i] = _list1[_rand];
                _list1[_rand] = temp;

            }

            foreach (GameObject _obj in _list1)
            {
                Ending_Dish.AddObj(_obj, isTop);
            }
        }




        //yield return new WaitForSeconds(1f);
        //Ending_Dish.isStop = false;
        if (isEnd == true)
        {
            Ending_Dish.EndingDish_Start();

            yield return new WaitForSeconds(1f);
            transform.DOMoveZ(50f, 4f).SetEase(Ease.Linear);
        }


        //while (true)
        //{
        //    yield return new WaitForSeconds(Time.deltaTime);
        //    if (Ending_Dish.isStop == false)
        //    {
        //        transform.Translate(Vector3.up * MoveSpeed * Time.deltaTime);
        //        //yield return new WaitForSeconds(Time.deltaTime);
        //        if (Ending_Dish.isStop == true)
        //        {
        //            break;
        //        }
        //    }
        //}

    }

    IEnumerator Cor_Ending3()
    {
        yield return null;



    }

    public void Retry()
    {
        Init();
        Update_Stage();

        StopAllCoroutines();
        RunningCor = false;
        StartCoroutine(Cor_Update());
    }


    public void Stage_Level_Func(int _num)
    {
        Stage_Level += _num;
        PlayerPrefs.SetInt("Stage_Level", Stage_Level);
        if (Stage_Level < 1)
        {
            Stage_Level = Stage_List.Length;
        }
        else// if (Stage_Level > Stage_List.Length)
        {

            //Stage_Level = 1;
        }
        Retry();

    }


    public void Ending_Func(bool isClear)
    {
        StartCoroutine(FaceFunc(isClear));






    }
    IEnumerator FaceFunc(bool isClear)
    { // sad : 0,1,2
      // happy : 1,0,0
      // normal : 0,0,0~1
      //yield return new WaitForSeconds(1f);
        float _mouth_val = 100f;
        float _mouth_val2 = 0f;
        float _eye_val = 0f;
        float _eyebrow_val = 0f;
        int _count = 0;
        bool _isUp = false;
        bool isfirst = true;


        if (isClear)
        {

            //yield return new WaitForSeconds(2f);

            while (true)
            {
                yield return null;
                //yield return new WaitForSeconds(Time.deltaTime);
                if (_count < 4)
                {
                    _Mouth.SetBlendShapeWeight(0, _mouth_val);
                    if (_isUp)
                    {
                        _mouth_val += Time.deltaTime * 100f * 5f;
                        if (_mouth_val > 99)
                        {
                            _mouth_val = 99f;
                            _isUp = false;
                        }
                    }
                    else
                    {
                        _mouth_val -= Time.deltaTime * 100f * 5f;
                        if (_mouth_val < 1)
                        {
                            _mouth_val = 1f;
                            _isUp = true;
                            _count++;
                        }
                    }
                }
                else
                {
                    Ending_Block._instance.OffObj();
                    if (isfirst)
                    {
                        yield return new WaitForSeconds(0.5f);
                        isfirst = false;
                        _audio.clip = _Clip[0];
                        Sound_Play();


                    }

                    _Eye.SetBlendShapeWeight(0, 100f);
                    _Eye.SetBlendShapeWeight(1, 100f);

                    _EyeBrow.SetBlendShapeWeight(0, _eyebrow_val);
                    _EyeBrow.SetBlendShapeWeight(1, 0f);

                    _Mouth.SetBlendShapeWeight(0, 0f);
                    _Mouth.SetBlendShapeWeight(1, _mouth_val2);
                    _Mouth.SetBlendShapeWeight(2, 0f);

                    _eyebrow_val += Time.deltaTime * 100f * 2f;
                    _mouth_val2 += Time.deltaTime * 100f * 2f;
                    if (_eyebrow_val >= 100)
                    {
                        break;
                    }
                }
            }
        }
        else // Fail Func
        {
            while (true)
            {
                yield return null;
                if (isfirst)
                {
                    yield return new WaitForSeconds(0.5f);
                    isfirst = false;
                    _audio.clip = _Clip[1];
                    Sound_Play();
                }

                //_Eye.SetBlendShapeWeight(0, _eye_val);

                _Eye.SetBlendShapeWeight(1, 0f);

                _EyeBrow.SetBlendShapeWeight(0, 0f);
                _EyeBrow.SetBlendShapeWeight(1, _eyebrow_val);

                _Mouth.SetBlendShapeWeight(0, 100 - _mouth_val2);
                _Mouth.SetBlendShapeWeight(1, 0f);
                _Mouth.SetBlendShapeWeight(2, _mouth_val2);

                _eyebrow_val += Time.deltaTime * 100f * 2f;
                _mouth_val2 += Time.deltaTime * 100f * 2f;
                _eye_val += Time.deltaTime * 100f * 2f;
                if (_eyebrow_val >= 100)
                {
                    break;
                }
            }
        }

        yield return new WaitForSeconds(1f);
        _newUiManager.EndPanel_Func(isClear);
    }


    public void SetFoodCount(int _val)
    {
        for (int i = 0; i < Ending_Block_Group.childCount; i++)
        {
            Ending_Block_Group.GetChild(i).GetComponent<Ending_Block>().Food_Count = _val;
        }
    }


    public void Sound_Play()
    {
        if (isSound)
        {
            _audio.Play();

        }
    }

    public void Vibe(int _num)
    {
        switch (_num)
        {
            case 1:
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
                break;

            case 2:
                MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                break;

            case 3:
                MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
                break;

            default:

                break;
        }
    }
}
