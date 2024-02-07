using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum State
    {
        Ready,
        Wait,
        Play,
        Rope,
        SafeBar,
        Drive,
        //Driving,
        End,
        Clear,
        Fail,
        ClearEnding1,
        ClearEnding2,
        FailEnding

    }
    public State state;

    public int Stage_Level = 1;
    public GameObject[] Stage;

    public GameObject[] Map_Background;

    [HideInInspector] public Transform Map_House;
    [HideInInspector] public Transform Map_House_Shutter;
    [HideInInspector] public Transform Map_StartPos_Trans;
    [HideInInspector] public Transform Map_EndPos_Trans;
    [HideInInspector] public Transform Map_Goal_Trans;

    public float ConstantForce_y = 20f;

    public Transform[] WaitLugg = new Transform[3];

    public GameObject Count_Panel;
    public Vector3 Count_Panel_Pos;
    public GameObject Price_Panel;
    public Vector3 Price_Panel_Pos;
    public GameObject Smoke_Effect;

    public Transform Ending_Jump_Pos;
    public AnimationCurve[] _Curve;

    public Material Broke_Mat;

    //public Dictionary<string, Vector2> Item = new Dictionary<string, Vector2>();

    public GameObject Lugg_PopUp_Canvas;
    public Dictionary<string, Info> Item = new Dictionary<string, Info>();






    [Header("Serialize")]
    /////////////////////////
    [SerializeField] public float Total_Score = 0f;
    [SerializeField] public float Separate_Trash_Bonus = 0f;
    public UIManager _uiManager;
    public static GameManager instance;
    public Player _player;
    [SerializeField] public StageManager _stageManager;
    public float Clear_Score = 0f;

    public bool isEnd = false;
    [SerializeField] bool isSafe = false;


    // //////////////////




    //////////////////////////////////////////////

    void Awake()
    {
        //Item = new object[5][];
        Application.targetFrameRate = 60;
        instance = this;
        StartCoroutine(Cor_Update());


        Reset();
    }


    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Reset");
                Reset();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Stage_Level--;
                if (Stage_Level < 1)
                {
                    Stage_Level = 1;
                }
                Reset();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Stage_Level++;
                if (Stage_Level > Stage.Length)
                {
                    Stage_Level = Stage.Length;
                }
                Reset();
            }


            switch (state)
            {

                case State.Ready:

                    break;

                case State.Wait:
                    yield return new WaitForSeconds(0.5f);
                    state = State.Play;
                    _player.Reload(false);

                    break;

                case State.Play:

                    break;

                case State.Rope:
                    yield return new WaitForSeconds(3f);


                    state = State.Drive;


                    break;

                case State.SafeBar:
                    //yield return new WaitForSeconds(2f);
                    if (isSafe == false)
                    {
                        isSafe = true;
                        _stageManager.Drive();
                    }


                    //state = State.Drive;
                    break;

                case State.Drive:



                    break;
                //case State.Driving:

                //    break;

                case State.End:
                    // add ending direction
                    //yield return new WaitForSeconds(3f);
                    //state = State.Clear;
                    break;

                case State.Clear:
                    //if (isEnd == false)
                    //{
                    //    isEnd = true;
                    //    yield return new WaitForSeconds(2f);
                    //    _uiManager.Clear();
                    //}

                    break;

                case State.Fail:
                    //if (isEnd == false)
                    //{
                    //    isEnd = true;
                    //    yield return new WaitForSeconds(2f);
                    //    _uiManager.Fail();
                    //}
                    break;

                case State.ClearEnding1:

                    break;

                case State.ClearEnding2:
                    break;

                case State.FailEnding:

                    break;


                default:
                    break;
            }
        }
    }





    public void Reset()
    {
        StopAllCoroutines();
        Init();
        InitUI();
        StartCoroutine(Cor_Update());
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Update_Stage();

        _player.Init();
    }

    public void Init()
    {
        state = State.Ready;
        Total_Score = 0f;
        isEnd = false;
        isSafe = false;
        Separate_Trash_Bonus = 0f;

        GameObject[] destroy_list = GameObject.FindGameObjectsWithTag("Luggage");
        foreach (GameObject _obj in destroy_list)
        {
            if (_obj.GetComponent<Luggage>().isFixObj == true)
            {
                _obj.SetActive(false);
                _obj.SetActive(true);
            }
            else
            {
                Destroy(_obj);
            }
        }

        GameObject[] destroy_list2 = GameObject.FindGameObjectsWithTag("Luggage_Group");
        foreach (GameObject _obj2 in destroy_list2)
        {
            Destroy(_obj2);
        }

        GameObject[] destroy_list3 = GameObject.FindGameObjectsWithTag("Lugg_Child");
        foreach (GameObject _obj3 in destroy_list3)
        {
            Destroy(_obj3);
        }


        Item.Clear();




    }

    public void Update_Stage()
    {
        foreach (GameObject _obj in Stage)
        {
            _obj.SetActive(false);
        }
        Stage[Stage_Level - 1].SetActive(true);
        _stageManager = Stage[Stage_Level - 1].GetComponent<StageManager>();


        foreach (GameObject _map in Map_Background)
        {
            _map.SetActive(false);
        }
        Map_Background[_stageManager.Map_Num].SetActive(true);


        ////////////////////////////////////////

        Map_House = Map_Background[_stageManager.Map_Num].transform.GetChild(0);
        Map_House_Shutter = Map_House.GetChild(0);
        Map_StartPos_Trans = Map_Background[_stageManager.Map_Num].transform.GetChild(1);
        Map_EndPos_Trans = Map_Background[_stageManager.Map_Num].transform.GetChild(2);
        Map_Goal_Trans = Map_Background[_stageManager.Map_Num].transform.GetChild(3);



        Map_StartPos_Trans.GetComponent<Renderer>().enabled = false;
        Map_EndPos_Trans.GetComponent<Renderer>().enabled = false;

    }

    public void InitUI()
    {
        _uiManager.Init();
    }

    public void NextStage()
    {
        Stage_Level++;
        if (Stage_Level > Stage.Length)
        {
            //Stage_Level = Stage.Length-1;
            Stage_Level = 1;
        }

        Reset();
    }


    public void ClearCheck()
    {
        bool isbool = Total_Score >= Clear_Score ? true : false;
        _uiManager.Ending(isbool);

    }

    public void PopUpFunc(Vector3 _pos, bool isBool)
    {
        _uiManager.PopUpFunc(_pos, isBool);

    }


    //// Temp Func
   public  void Stage_Change(int _num)
    {
        Stage_Level +=_num;
        if (Stage_Level < 1)
        {
            Stage_Level = 1;
        }
        else if (Stage_Level > Stage.Length)
        {
            Stage_Level = Stage.Length;
        }
        Reset();
    }

}
