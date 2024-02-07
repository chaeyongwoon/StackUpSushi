using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public int Map_Num = 0; // 0 : Street , 1 : Sea , 2 : Train
    //public Transform House_obj;
    //public Transform House_Door;

    public GameObject[] Luggage_Prefabs;
    public int[] Max_Count;
    public Car[] _Car;
    //public Transform Goal_Trans;

    public float _Clear_Score = 100f;



    public Transform[] WaitLugg_Pos = new Transform[3];
    public bool isfix = false;



    //[SerializeField] int Car_Goal_Count = 0;

    [SerializeField] int Current_Count = 0;

    [SerializeField] GameObject[] WaitLugg = new GameObject[3];



    public bool isRandom = false;

    public Queue<GameObject> Lugg_List;
    Player _player;
    GameManager _gm;




    // ------------------------------------------------------------
    private void Awake()
    {
        _gm = GameManager.instance;

        Lugg_List = new Queue<GameObject>(Max_Count.Length);

        for (int i = 0; i < 3; i++)
        {
            WaitLugg_Pos[i] = _gm.WaitLugg[i];
        }
    }


    // Start is called before the first frame update
    private void OnEnable()
    {
        _player = _gm._player;
        //_gm._stageManager = this;
        _player._stageManager = this;

        Camera.main.GetComponent<Cam>().Map_Num = Map_Num;

        Init();
        Update_Stage();
        Camera.main.GetComponent<Cam>().Car_Trans = null;
        _Car[0].SetCam();


    }

    public void Init()
    {
        Current_Count = 0;
        for (int i = 0; i < 3; i++)
        {
            WaitLugg[i] = null;
        }

        Lugg_List.Clear();
        Lugg_List = new Queue<GameObject>(Max_Count.Length);
        _gm.Clear_Score = _Clear_Score;
    }

    public void Update_Stage()
    {
        for (int i = 0; i < Max_Count.Length; i++)
        {
            if (isRandom == true)
            {
                Lugg_List.Enqueue(Luggage_Prefabs[Random.Range(0, Luggage_Prefabs.Length)]);
            }
            else
            {
                Lugg_List.Enqueue(Luggage_Prefabs[i]);
            }
        }


        for (int i = 0; i < 3; i++)
        {
            GameObject Lugg_Group = new GameObject();
            Lugg_Group.transform.tag = string.Format("Luggage_Group");
            Lugg_Group.transform.name = string.Format("Lugg_Group_{0}", Current_Count);
            Lugg_Group.transform.position = WaitLugg_Pos[i].transform.position;
            WaitLugg[i] = Lugg_Group;

            for (int j = 0; j < Max_Count[i]; j++)
            {
                GameObject _lugg = Instantiate(Lugg_List.Peek()
                    , Lugg_Group.transform.position
                    + Vector3.up * Lugg_List.Peek().GetComponent<BoxCollider>().size.y * j
                    , Quaternion.Euler(Lugg_List.Peek().GetComponent<Luggage>().Rot));
                _lugg.transform.SetParent(Lugg_Group.transform);
                _lugg.GetComponent<Collider>().isTrigger = true;
                _lugg.GetComponent<Collider>().enabled = false;

            }
            Lugg_List.Dequeue();

            GameObject _Count_Panel = Instantiate(_gm.Count_Panel
                , Lugg_Group.transform.position + _gm.Count_Panel_Pos
                , _gm.Count_Panel.transform.rotation);
            _Count_Panel.transform.SetParent(Lugg_Group.transform);
            _Count_Panel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                = string.Format("X{0}", Max_Count[i]);

            Current_Count++;


            //GameObject _lugg = Instantiate(Lugg_List.Peek()
            //    , WaitLugg_Pos[i].transform.position
            //    , Quaternion.Euler(Lugg_List.Dequeue().GetComponent<Luggage>().Rot));
            //_lugg.GetComponent<Collider>().isTrigger = true;
            //WaitLugg[i] = _lugg;
            //_lugg.GetComponent<Collider>().enabled = false;
            //_lugg.GetComponent<Luggage>().Continuous_Count = Max_Count[Current_Count];

            //GameObject _Count_Panel = Instantiate(_gm.Count_Panel
            //    , _lugg.transform.position + _gm.Count_Panel_Pos
            //    , _gm.Count_Panel.transform.rotation);
            //_Count_Panel.transform.SetParent(_lugg.transform);
            //_Count_Panel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
            //    = string.Format("X{0}", _lugg.GetComponent<Luggage>().Continuous_Count);
            //Current_Count++;
        }
    }


    public bool Reload(float _time, bool _isbomb)
    {


        if (WaitLugg[0] != null)
        {
            _player.Selected_Lugg = WaitLugg[0];
            WaitLugg[0].transform.DOMove(_player.Start_Luggage_Pos.position, _time);
            for (int i = 0; i < WaitLugg[0].transform.childCount - 1; i++)
            {
                WaitLugg[0].transform.GetChild(i).GetComponent<ConstantForce>().force = Vector3.up * (-_gm.ConstantForce_y);
                //_player.Selected_Lugg.GetComponent<ConstantForce>().force = Vector3.up * (-_gm.ConstantForce_y);

            }

            //for(int i=1; i<_player.Selected_Lugg.GetComponent<Luggage>().Continuous_Count; i++)
            //{
            //    GameObject _obj = Instantiate(_player.Selected_Lugg, _player.Start_Luggage_Pos.position
            //        + new Vector3(2f, -2f + 2f * i, 0f),Quaternion.Euler( _player.Selected_Lugg.GetComponent<Luggage>().Rot));
            //}
        }


        //if (WaitLugg[0] != null)
        //{
        //    _player.Selected_Lugg = WaitLugg[0];
        //    WaitLugg[0].transform.DOMove(_player.Start_Luggage_Pos.position, _time);
        //    _player.Selected_Lugg.GetComponent<ConstantForce>().force =Vector3.up*(-_gm.ConstantForce_y);

        //    //for(int i=1; i<_player.Selected_Lugg.GetComponent<Luggage>().Continuous_Count; i++)
        //    //{
        //    //    GameObject _obj = Instantiate(_player.Selected_Lugg, _player.Start_Luggage_Pos.position
        //    //        + new Vector3(2f, -2f + 2f * i, 0f),Quaternion.Euler( _player.Selected_Lugg.GetComponent<Luggage>().Rot));
        //    //}
        //}
        try
        {
            WaitLugg[1].transform.DOMove(WaitLugg_Pos[0].transform.position, _time);
            WaitLugg[2].transform.DOMove(WaitLugg_Pos[1].transform.position, _time);
        }
        catch { }
        WaitLugg[0] = WaitLugg[1];
        WaitLugg[1] = WaitLugg[2];
        WaitLugg[2] = null;


        if (Current_Count < Max_Count.Length)
        {
            GameObject Lugg_Group = new GameObject();
            Lugg_Group.transform.tag = string.Format("Luggage_Group");
            Lugg_Group.transform.name = string.Format("Lugg_Group_{0}", Current_Count);
            Lugg_Group.transform.position = WaitLugg_Pos[2].transform.position;
            WaitLugg[2] = Lugg_Group;

            for (int j = 0; j < Max_Count[Current_Count]; j++)
            {
                GameObject _lugg = Instantiate(Lugg_List.Peek()
                    , Lugg_Group.transform.position
                    + Vector3.up * Lugg_List.Peek().GetComponent<BoxCollider>().size.y * j
                    , Quaternion.Euler(Lugg_List.Peek().GetComponent<Luggage>().Rot));
                _lugg.transform.SetParent(Lugg_Group.transform);
                _lugg.GetComponent<Collider>().isTrigger = true;
                _lugg.GetComponent<Collider>().enabled = false;

            }
            Lugg_List.Dequeue();

            GameObject _Count_Panel = Instantiate(_gm.Count_Panel
                , Lugg_Group.transform.position + _gm.Count_Panel_Pos
                , _gm.Count_Panel.transform.rotation);
            _Count_Panel.transform.SetParent(Lugg_Group.transform);
            _Count_Panel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                = string.Format("X{0}", Max_Count[Current_Count]);
            Current_Count++;

        }




        //if (Current_Count < Max_Count.Length)
        //{
        //    WaitLugg[2] = Instantiate(Lugg_List.Peek()
        //        , WaitLugg_Pos[2].transform.position
        //        , Quaternion.Euler(Lugg_List.Dequeue().GetComponent<Luggage>().Rot));
        //    WaitLugg[2].GetComponent<Collider>().isTrigger = true;
        //    WaitLugg[2].GetComponent<Collider>().enabled = false;
        //    WaitLugg[2].GetComponent<Luggage>().Continuous_Count = Max_Count[Current_Count];


        //    GameObject _Count_Panel = Instantiate(_gm.Count_Panel
        //        , WaitLugg[2].transform.position + _gm.Count_Panel_Pos
        //        , _gm.Count_Panel.transform.rotation);
        //    _Count_Panel.transform.SetParent(WaitLugg[2].transform);
        //    _Count_Panel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
        //        = string.Format("X{0}", WaitLugg[2].GetComponent<Luggage>().Continuous_Count);


        //    Current_Count++;
        //}

        if (_player.Selected_Lugg == null)
        {
            //_gm.state = GameManager.State.SafeBar;


            StartCoroutine(SetGmState(GameManager.State.SafeBar, _isbomb));
            return true;  // 'true' is player can't reload
            //break;
        }

        //else
        //{
        //    _player.Selected_Lugg = WaitLugg[0];
        //}
        return false; // 'false' is player can reload

        IEnumerator SetGmState(GameManager.State _state, bool _isbomb2)
        {
            if (_isbomb2 == true)
            {
                yield return new WaitForSeconds(2f);
            }


            foreach (Car _car in _Car)
            {
                _car.car_state = Car.Car_State.Wait;
            }
            yield return new WaitForSeconds(2f);
            _gm.state = _state;
        }
    }


    public void Drive()
    {
        foreach (Car _car in _Car)
        {
            _car.MoveCar();
        }
    }

    public void ArriveCar(int _num)
    {
        //Debug.Log("Arriver");
        //Car_Goal_Count++;

        switch (_num)
        {
            case 0:
                //if (Car_Goal_Count >= _Car.Length)
                //{
                foreach (Car _car in _Car)
                {
                    //Debug.Log("Showlugg");
                    _car.ShowLuggFunc();
                }
                //}
                break;

            case 1:
                //foreach (Car _car in _Car)
                //{
                //    _car.ShowLuggFunc();
                //}
                break;

            case 2:
                break;

            default:

                break;
        }

        //if (isTrain)
        //{
        //    foreach (Car _car in _Car)
        //    {
        //        _car.ShowLuggFunc();
        //    }
        //}
        //else if (Car_Goal_Count >= _Car.Length)
        //{
        //    //_gm.state = GameManager.State.End;

        //    foreach (Car _car in _Car)
        //    {
        //        _car.ShowLuggFunc();
        //    }
        //}





    }



}
