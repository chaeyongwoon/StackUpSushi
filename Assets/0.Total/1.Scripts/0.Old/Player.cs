using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    //public GameObject[] Luggage_Prefabs;
    public GameObject Guide_Obj;
    //public int Continuous_Count = 1;

    public Transform Start_Luggage_Pos;


    public float Sens_magnif = 0.01f;
    public float power = 50f;
    public int Line_Pos_Count = 100;

    public float Reload_Interval = 0.5f;
    public GameObject Selected_Lugg;

    public float limit_height = 4f;
    public PhysicMaterial Default_PhysicsMat;
    public PhysicMaterial Fix_Mat;



    //////////////////
    [Header("Serialize")]

    [SerializeField] bool isbreak;
    [SerializeField] bool islast = false;
    [SerializeField] Collider[] _cols;
    [SerializeField] float Angle = 45f;
    [SerializeField] float Parabola_time = 2f;
    [SerializeField] float Parabola_Division_time = 0.02f;
    [SerializeField] Vector3 temp_pos;


    [SerializeField] Vector3 Dir, Dir2;
    [SerializeField] float distance = 0f;
    [SerializeField] int Rand_Num = 0;
    [SerializeField] float Shoot_Interval = 0.2f;
    public StageManager _stageManager;


    [Space]
    Vector3 Start_Pos, End_Pos;
    LineRenderer _line;
    //[SerializeField] GameObject _Luggage;
    [SerializeField] bool isClick = false;
    [SerializeField] bool isShoot = false;



    GameManager _gm;


    private void OnEnable()
    {
        _gm = GameManager.instance;

        if (!_line)
        {
            _line = GetComponent<LineRenderer>();
        }

        Guide_Obj.SetActive(false);


        StopAllCoroutines();
        StartCoroutine(Cor_Update());

    }

    public void Init()
    {
        isClick = false;
        isShoot = false;
        Guide_Obj.SetActive(false);
    }


    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return null;
            if (_gm.state == GameManager.State.Play)
            {
                StartCoroutine(MouseFunc());


                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                //    Test_Mesh_Spawn();
                //}
            }
        }


    }


    IEnumerator MouseFunc()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isClick == false && isShoot == false)
            {
                isClick = true;
                Start_Pos = Input.mousePosition;

                _line.positionCount = 0;
                Line_Pos_Count = (int)(Parabola_time / Parabola_Division_time);
                _line.positionCount = Line_Pos_Count + 1;

                Guide_Obj.transform.position = Vector3.down * 100f;
                Guide_Obj.SetActive(true);

                //Rand_Num = Random.Range(0, Luggage_Prefabs.Length);

                //_Luggage = Instantiate(Luggage_Prefabs[Rand_Num],
                //    Start_Luggage_Pos.position,
                //    Quaternion.Euler(Luggage_Prefabs[Rand_Num].GetComponent<Luggage>().Rot));
                //_Luggage.GetComponent<Rigidbody>().isKinematic = true;

            }
        }

        else if (Input.GetMouseButton(0))
        {

            if (isClick == true && isShoot == false)
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
                        Start_Luggage_Pos.position
                        + Dir2
                        * distance * Sens_magnif * _time
                        + Vector3.up * 0.5f * (Physics.gravity.y - _gm.ConstantForce_y) * _time * _time
                        );
                    _time += Parabola_Division_time;

                    // ////////
                    //if (_line.GetPosition(i).y < limit_height)
                    //{
                    //    if (islast == false)
                    //    {

                    //        islast = true;
                    //        temp_pos = _line.GetPosition(i);
                    //    }
                    //    _line.SetPosition(i, new Vector3(temp_pos.x, limit_height, temp_pos.z));
                    //}
                    //Guide_Obj.transform.position = _line.GetPosition(i);
                    // ///////

                    if (i > 2)
                    {
                        if (islast == false)
                        {
                            _cols = Physics.OverlapBox(_line.GetPosition(i) + Selected_Lugg.transform.GetChild(0).GetComponent<BoxCollider>().center
                                , Selected_Lugg.transform.GetChild(0).GetComponent<BoxCollider>().size * 0.5f
                                , Quaternion.Euler(Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Rot));
                            //Debug.Log(Selected_Lugg.GetComponent<BoxCollider>().size);
                            Guide_Obj.transform.position = _line.GetPosition(i);
                            if (_cols.Length != 0)
                            {
                                islast = true;
                                temp_pos = _line.GetPosition(i);
                                Guide_Obj.transform.position = temp_pos;
                            }

                        }
                        else
                        {
                            _line.SetPosition(i, temp_pos);

                        }
                    }




                    //if (i > 2)
                    //{
                    //    if (islast == false)
                    //    {
                    //        _cols = Physics.OverlapBox(_line.GetPosition(i) + Selected_Lugg.GetComponent<BoxCollider>().center
                    //            , Selected_Lugg.GetComponent<BoxCollider>().size * 0.5f
                    //            , Quaternion.Euler(Selected_Lugg.GetComponent<Luggage>().Rot));
                    //        //Debug.Log(Selected_Lugg.GetComponent<BoxCollider>().size);
                    //        Guide_Obj.transform.position = _line.GetPosition(i);
                    //        if (_cols.Length != 0)
                    //        {
                    //            islast = true;
                    //            temp_pos = _line.GetPosition(i);
                    //            Guide_Obj.transform.position = temp_pos;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        _line.SetPosition(i, temp_pos);

                    //    }
                    //}


                }



                //Guide_Obj.transform.position = _line.GetPosition(Line_Pos_Count - 1);
                Guide_Obj.transform.localScale = Selected_Lugg.transform.GetChild(0).transform.localScale;

                Guide_Obj.GetComponent<MeshFilter>().mesh
                    //= Selected_Lugg.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
                    = Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Guide_Mesh;
                Guide_Obj.transform.rotation = Quaternion.Euler(Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Rot);


                //Guide_Obj.GetComponent<MeshFilter>().mesh = Selected_Lugg.GetComponent<MeshFilter>().mesh;
                //Guide_Obj.transform.rotation = Quaternion.Euler(Selected_Lugg.GetComponent<Luggage>().Rot);



            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (isClick == true)
            {

                isClick = false;
                isShoot = true;
                Start_Pos = Vector3.zero;
                End_Pos = Vector3.zero;




                //Destroy(Selected_Lugg.transform.GetChild(0).gameObject);

                StartCoroutine(ShootLugg2());

                //////////////////////////////////////////////////////////////////////

                //isClick = false;
                //isShoot = true;
                //Start_Pos = Vector3.zero;
                //End_Pos = Vector3.zero;

                //Destroy(Selected_Lugg.GetComponent<BoxCollider>());
                //Selected_Lugg.AddComponent<MeshCollider>().convex = true;
                //Selected_Lugg.GetComponent<MeshCollider>().sharedMesh = Selected_Lugg.GetComponent<MeshFilter>().mesh;
                //if (_stageManager.isfix == true)
                //{
                //    Selected_Lugg.GetComponent<MeshCollider>().material = Fix_Mat;                    
                //}
                //else
                //{
                //    Selected_Lugg.GetComponent<MeshCollider>().material = Default_PhysicsMat;
                //}

                //Destroy(Selected_Lugg.transform.GetChild(0).gameObject);
                //Rigidbody Lug_rb = Selected_Lugg.GetComponent<Rigidbody>();



                //Lug_rb.isKinematic = false;
                //Lug_rb.GetComponent<Collider>().enabled = true;
                //Lug_rb.GetComponent<Collider>().isTrigger = false;
                //Lug_rb.AddForce(Dir2 * power * distance * Sens_magnif);

                //if (Lug_rb.GetComponent<Luggage>().state == Luggage.State.Bomb)
                //{
                //    Lug_rb.GetComponent<Luggage>().Bomb();
                //}

                //Lug_rb.GetComponent<Luggage>().Init_Constant(3f);
                //StartCoroutine(ShootLugg(Lug_rb.transform.gameObject));
                //Selected_Lugg = null;
                _line.positionCount = 0;
                Guide_Obj.SetActive(false);
                //Reload();
                yield return new WaitForSeconds(Reload_Interval);
                //isShoot = false;


            }
        }


        IEnumerator ShootLugg(GameObject _obj)
        {
            for (int i = 1; i < _obj.GetComponent<Luggage>().Continuous_Count; i++)
            {
                //yield return new WaitForSeconds( Shoot_Interval);
                yield return new WaitForSeconds(_obj.GetComponent<Luggage>().Shoot_Interval);
                GameObject _lugg = Instantiate(_obj, Start_Luggage_Pos.position,
                    Quaternion.Euler(_obj.GetComponent<Luggage>().Rot));
                Rigidbody _lugg_rb = _lugg.GetComponent<Rigidbody>();
                _lugg_rb.AddForce(Dir2 * power * distance * Sens_magnif);


                _lugg_rb.GetComponent<Luggage>().State_Func();

                //_lugg_rb.GetComponent<Luggage>().Init_Constant(3f);
                //Selected_Lugg = null;

            }

        }


        IEnumerator ShootLugg2()
        {
            

            Destroy(
               Selected_Lugg.transform.GetChild(
                   Selected_Lugg.transform.childCount - 1).gameObject);
            //yield return new WaitForEndOfFrame();
            yield return null;

            Destroy(Selected_Lugg.transform.GetChild(0).GetComponent<BoxCollider>());
            Selected_Lugg.transform.GetChild(0).gameObject.AddComponent<MeshCollider>().convex = true;
            Selected_Lugg.transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh
                //= Selected_Lugg.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
                = Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Guide_Mesh;
            Selected_Lugg.transform.GetChild(0).GetComponent<Collider>().enabled = false;
            if (_stageManager.isfix == true)
            {
                Selected_Lugg.transform.GetChild(0).GetComponent<MeshCollider>().material = Fix_Mat;
            }
            else
            {
                Selected_Lugg.transform.GetChild(0).GetComponent<MeshCollider>().material = Default_PhysicsMat;
            }



            Rigidbody Lug_rb = Selected_Lugg.transform.GetChild(0).GetComponent<Rigidbody>();

            Lug_rb.isKinematic = false;
            Lug_rb.GetComponent<Collider>().enabled = true;
            Lug_rb.GetComponent<Collider>().isTrigger = false;
            Lug_rb.AddForce(Dir2 * power * distance * Sens_magnif);


            Lug_rb.GetComponent<Luggage>().State_Func();


            Lug_rb.GetComponent<Luggage>().Init_Constant(3f);

            Lug_rb.transform.SetParent(null);
            

            int _count = Selected_Lugg.transform.childCount;

            for (int i = 0; i < _count; i++)
            {
                for (int j = 0; j < Selected_Lugg.transform.childCount; j++)
                {

                    Selected_Lugg.transform.GetChild(j).transform.DOMoveY(
                        Selected_Lugg.transform.GetChild(j).transform.position.y
                        - 1f * Selected_Lugg.transform.GetChild(j).GetComponent<BoxCollider>().size.y
                        , Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Shoot_Interval);
                }
                yield return new WaitForSeconds(Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Shoot_Interval);

                Destroy(Selected_Lugg.transform.GetChild(0).GetComponent<BoxCollider>());
                Selected_Lugg.transform.GetChild(0).gameObject.AddComponent<MeshCollider>().convex = true;
                Selected_Lugg.transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh
                    //= Selected_Lugg.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
                    = Selected_Lugg.transform.GetChild(0).GetComponent<Luggage>().Guide_Mesh;
                Selected_Lugg.transform.GetChild(0).GetComponent<Collider>().enabled = false;
                if (_stageManager.isfix == true)
                {
                    Selected_Lugg.transform.GetChild(0).GetComponent<MeshCollider>().material = Fix_Mat;
                }
                else
                {
                    Selected_Lugg.transform.GetChild(0).GetComponent<MeshCollider>().material = Default_PhysicsMat;
                }


                Rigidbody Lug_rb2 = Selected_Lugg.transform.GetChild(0).GetComponent<Rigidbody>();

                Lug_rb2.isKinematic = false;
                Lug_rb2.GetComponent<Collider>().enabled = true;
                Lug_rb2.GetComponent<Collider>().isTrigger = false;
                Lug_rb2.AddForce(Dir2 * power * distance * Sens_magnif);

                Lug_rb2.GetComponent<Luggage>().State_Func();


                Lug_rb2.GetComponent<Luggage>().Init_Constant(3f);
                Lug_rb2.transform.SetParent(null);

            }

            Selected_Lugg = null;
            //_line.positionCount = 0;
            //Guide_Obj.SetActive(false);
            isShoot = Reload(Lug_rb.GetComponent<Luggage>().state == Luggage.State.Bomb ? true : false);
            yield return new WaitForSeconds(Reload_Interval);

        }
    }


    public bool Reload(bool _isbomb)
    {
        return _stageManager.Reload(Reload_Interval, _isbomb);
    }

    //private void OnDrawGizmos()
    //{
    //    if (Selected_Lugg != null)
    //    {
    //        Gizmos.DrawCube(Guide_Obj.transform.position + Selected_Lugg.transform.GetChild(0).GetComponent<BoxCollider>().center
    //            , Selected_Lugg.transform.GetChild(0).GetComponent<BoxCollider>().size);
    //        Gizmos.color = Color.red;
    //
    //    }
    //}


}
