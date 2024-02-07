using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Luggage : MonoBehaviour
{
    public string Obj_Name = "No_Name";
    public Sprite Img;
    public int Price = 100;
    public Mesh Guide_Mesh;


    public int Continuous_Count = 1;
    public float Shoot_Interval = 0.2f;

    public Vector3 Rot = new Vector3(0f, 180f, 0f);


    public int Index = 0;
    public enum State
    {
        Normal,
        Glass,
        Bomb,
        Trash,
        Animal
    }
    public State state;

    public bool isBreak = false;
    public bool isCheck = false;

    //public bool onCol = false;
    public bool isPopUp = false;
    public bool isTrashCheck = false;
    [Header("Bomb")]
    public float Bomb_Interval = 3f;
    public float Bomb_Radius = 2f;
    public float Explosion_Power = 100f;
    public Vector3 Bomb_Pos;

    //////////////////////////////
    public bool isFixObj = false;
    [SerializeField] Vector3 Start_Pos;
    [SerializeField] Quaternion Start_Rot;
    //////////////////////////////

    private void Awake()
    {
        Start_Pos = transform.position;
        Start_Rot = transform.rotation;
    }


    private void OnEnable()
    {
        transform.position = Start_Pos;
        transform.rotation = Start_Rot;

        isCheck = false;
        isBreak = false;
        isPopUp = false;

        switch (state)
        {
            case State.Normal:

                break;


            case State.Glass:

                break;

            case State.Trash:

                break;

            default:

                break;
        }
    }

    public void State_Func()
    {
        switch (state)
        {
            case State.Bomb:
                StartCoroutine(Cor_Bomb());
                break;

            case State.Animal:

                break;

            default:
                break;
        }
    }




    IEnumerator Cor_Bomb()
    {
        yield return new WaitForSeconds(Bomb_Interval);

        Collider[] _cols = Physics.OverlapSphere(transform.position, Bomb_Radius);
        foreach (Collider _col in _cols)
        {
            if (_col.CompareTag("Luggage"))
            {
                _col.GetComponent<Rigidbody>()
                    .AddExplosionForce(Explosion_Power, transform.position + Bomb_Pos, Bomb_Radius);
            }
        }

        this.gameObject.SetActive(false);
    }

    IEnumerator Cor_ShapeKey()
    {
        yield return new WaitForSeconds(Bomb_Interval);

        float _val = 0f;
        while (true)
        {
            GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, _val);
            _val += Time.deltaTime;
            if (_val >= 1f)
            {
                break;
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (state == State.Bomb)
        {
            Gizmos.DrawSphere(transform.position + Bomb_Pos, Bomb_Radius);
            Gizmos.color = new Vector4(1f, 1f, 1f, 0.3f);
        }
    }


    public void Init_Constant(float _val)
    {
        StartCoroutine(Cor_InitConstant(_val));

        IEnumerator Cor_InitConstant(float _time)
        {
            yield return new WaitForSeconds(_time);
            //GetComponent<ConstantForce>().force = new Vector3(0f,-20f,0f);
        }
    }

    public void BreakFunc()
    {
        if (isBreak == false)
        {
            isBreak = true;
            int _count = transform.childCount;

            GetComponent<Renderer>().material = GameManager.instance.Broke_Mat; // 부서졌을때 머테리얼
            GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;

            for (int i = 0; i < _count; i++)
            {
                Transform _obj = transform.GetChild(0);

                _obj.GetComponent<Renderer>().material = GameManager.instance.Broke_Mat; // 부서졌을때 머테리얼
                MeshCollider _meshcol = _obj.gameObject.AddComponent<MeshCollider>();
                _meshcol.sharedMesh = _obj.GetComponent<MeshFilter>().mesh;
                _meshcol.convex = true;
                Rigidbody _rb = _obj.gameObject.AddComponent<Rigidbody>();
                _rb.interpolation = RigidbodyInterpolation.Interpolate;
                _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                _rb.gameObject.AddComponent<ConstantForce>().force
                    = Vector3.up * (-GameManager.instance.ConstantForce_y);
                _obj.tag = "Lugg_Child";
                _obj.SetParent(null);
            }
            // is break  =true;
            // int count = transform.childcount;
            // for(int i=0; i<count i++){
            // transform.getchild(0).gameobject.addcomponent<Rigidbody>();
            //transform.getchild(0).gameobject.addcomponent<meshcollider>().shardmesh
            //= getcomponent<meshfiler>().mesh;
            // transform.getchild(0).setparent(null);
            //
            // }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string _str = collision.transform.tag;
        if (isFixObj == false)
        {
            switch (_str)
            {
                case "Ground":
                    if (isPopUp == false)
                    {
                        isPopUp = true;

                        GameManager.instance.PopUpFunc(transform.position, false);
                        if (state == State.Glass)
                        {
                            BreakFunc();
                        }
                    }

                    break;

                case "Player_Ground":
                    if (isPopUp == false)
                    {
                        isPopUp = true;


                        GameManager.instance.PopUpFunc(transform.position, false);
                        if (state == State.Glass)
                        {
                            BreakFunc();
                        }
                    }
                    StartCoroutine(Cor_Off());
                    break;

                default:

                    break;
            }
        }

    }

    IEnumerator Cor_Off()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
