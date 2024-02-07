using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewStageManager : MonoBehaviour
{
    public int Bullet_Count = 3;
    public int End_Food_Count = 3;
    public GameObject[] Bullet_List;
    public Transform[] Rail_Nodes;
    public Vehicle[] Vehicle_List;

    public float Rail_Speed = 3f;

    float _x = 0f;
    // ----------------------------------

    private void Awake()
    {
        int _count = transform.GetChild(0).childCount;
        Rail_Nodes = new Transform[_count];
        for (int i = 0; i < _count; i++)
        {
            Rail_Nodes[i] = transform.GetChild(0).GetChild(i);
            transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }

        int _count2 = transform.GetChild(1).childCount;
        Vehicle_List = new Vehicle[_count2];
        for (int i = 0; i < _count2; i++)
        {
            Vehicle_List[i] = transform.GetChild(1).GetChild(i).GetComponent<Vehicle>();
            Vehicle_List[i].Speed = Rail_Speed * 2f;
        }
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        Bullet_List = NewGameManager.instance.Bullet_List;

        GameObject _temp;
        int _rand;
        for (int i = 0; i < Bullet_List.Length; i++)
        {
            _rand = Random.Range(0, Bullet_List.Length);
            _temp = Bullet_List[i];
            Bullet_List[i] = Bullet_List[_rand];
            Bullet_List[_rand] = _temp;
        }
        //Bullet_List = new GameObject[Bullet_Count];


        NewGameManager.instance.ShootPlayer.Shoot_List = null;
        //NewGameManager.instance.ShootPlayer.Shoot_List = Bullet_List;
        NewGameManager.instance.ShootPlayer.Shoot_List = new GameObject[Bullet_Count];
        for (int j = 0; j < Bullet_Count; j++)
        {
            NewGameManager.instance.ShootPlayer.Shoot_List[j] = Bullet_List[j];
        }

        NewGameManager.instance.ShootPlayer.Max_Count = Bullet_Count;
        NewGameManager.instance.ShootPlayer.Init();
        foreach (Vehicle _vehicle in Vehicle_List)
        {
            _vehicle.Tmp_Rail_List = Rail_Nodes;

        }
        NewGameManager.instance.SetFoodCount(End_Food_Count);

        if (NewGameManager.instance.Stage_Level > NewGameManager.instance.Stage_List.Length)
        {
            SetDishVal();
        }

    }

    private void Update()
    {
        _x += Rail_Speed * Time.deltaTime;
        NewGameManager.instance.Rail_Mat[0].SetTextureOffset("_MainTex", new Vector2(-_x, 0f));
        //NewGameManager.instance.Rail_Mat[1].SetTextureOffset("_MainTex", new Vector2(_x, 0f));
    }

    //private void Start()
    //{
    //    foreach (Vehicle _vehicle in Vehicle_List)
    //    {
    //        _vehicle.Tmp_Rail_List = Rail_Nodes;
    //        //_vehicle.SetNode();

    //        //foreach (Transform _trans in Rail_Nodes)
    //        //{
    //        //    _vehicle.Rail_Nodes.Enqueue(_trans);
    //        //}

    //    }
    //}


    public void SetDishVal()
    {
        
        int Deco_count = 0;

        foreach (Vehicle _vehicle in Vehicle_List)
        {
            if (_vehicle.isDeco == true)
            {
                Deco_count++;
            }
        }

        int Total_Count = (Vehicle_List.Length - Deco_count) * 7;

        int _tmpCount = 0;
        int _tmpRand = 0;
        bool isfirst = true;
        foreach (Vehicle _vehicle in Vehicle_List)
        {
            if (_vehicle.isDeco == false)
            {

                int _num = Random.Range(1, 11);

                if (_num == 1 )
                {
                    if (isfirst)
                    {
                        isfirst = false;
                    }
                    else
                    {
                        _num = 3;
                    }
                }

                switch (_num)
                {
                    case int n when (n == 1 ):
                        _tmpRand = Random.Range(20, 25);
                        break;
                    case int n when (n >= 2 && n < 3):
                        _tmpRand = Random.Range(10, 20);
                        break;
                    case int n when (n >= 3 && n <= 10):
                        _tmpRand = Random.Range(2, 9);
                        break;
                }

                
                _tmpCount += _tmpRand;
                if (_tmpCount > Total_Count)
                {
                    _tmpCount -= _tmpRand;
                    int _val = Random.Range(2, 10);
                    _tmpCount += _val;
                    _vehicle.Value = _val;

                }
                else
                {
                    _vehicle.Value = _tmpRand;
                    if (_tmpRand >= 10)
                    {
                        _vehicle.OnParticle();
                    }
                }
                _vehicle.ValueText.text = string.Format("X{0}", _vehicle.Value);
            }
        }

    }

}
