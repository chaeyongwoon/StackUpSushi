using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Vehicle : MonoBehaviour
{
    public bool isDeco = false;
    public float Sense = 100f;
    public int QueuePos_Num = 0;
    public float Speed = 10f;
    public int Value = 5;
    public Transform Stack_Pos;
    public float Interval = 0.05f;
    public AnimationCurve Curve, Curve2;
    public Direct Dirstate;

    public Text ValueText;




    public Stack<GameObject> Obj_List;
    public Queue<Transform> Rail_Nodes;
    public Transform[] Tmp_Rail_List;
    public Transform[] Shake_List;
    public bool isBounce = true;


    [Header("Serialize")]
    [SerializeField] Transform[] Portal;
    [SerializeField] Vector3 Dir;
    [SerializeField] bool isFull = false;
    [SerializeField] public bool isStop = false;
    [SerializeField] Vector3 Start_Pos;
    [SerializeField] Quaternion Start_Rot;
    /// /////
    Sequence _sequence;
    AudioSource _audio;

    public GameObject[] Particle;

    public enum Direct
    {
        Dish,
        Left,
        Right,
        Forward,
        Back

    }



    /// ///////////////////////////////////

    private void Awake()
    {
        Obj_List = new Stack<GameObject>();
        Rail_Nodes = new Queue<Transform>();

        Start_Pos = transform.position;
        Start_Rot = transform.rotation;
        Portal = new Transform[2];
        if (Stack_Pos == null)
        {
            Stack_Pos = transform;
        }

        _sequence = DOTween.Sequence();
        _audio = GetComponent<AudioSource>();



    }

    private void OnEnable()
    {
        Init();
        NewGameManager.instance.Max_Vehicle++;
        Shake_List = new Transform[Value];
    }

    private void Start()
    {
        Portal[0] = NewGameManager.instance.Portal[0];
        Portal[1] = NewGameManager.instance.Portal[1];

        SetNode();



        switch (Dirstate)
        {
            case Direct.Dish:


                break;

            case Direct.Left:
                Dir = -transform.right;
                break;

            case Direct.Right:
                Dir = transform.right;
                break;

            case Direct.Forward:
                Dir = transform.forward;
                break;

            case Direct.Back:
                Dir = -transform.forward;
                break;

            default:

                break;
        }

        ValueText.text = string.Format("X{0}", Value);
    }


    // Update is called once per frame
    void Update()
    {
        if (Dirstate != Direct.Dish)
        {
            if (isStop == false)
            {
                transform.Translate(Dir * Speed * Time.deltaTime);
            }
        }
        else
        {
            if (isStop == false)
            {
                //Debug.Log(Rail_Nodes.Count);
                transform.position = Vector3.MoveTowards(transform.position,
                    //Rail_Nodes.Peek().position
                    new Vector3(Rail_Nodes.Peek().position.x, transform.position.y, Rail_Nodes.Peek().position.z)
                    , Speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, Rail_Nodes.Peek().position) <= 0.1f)
                {

                    Rail_Nodes.Enqueue(Rail_Nodes.Dequeue());

                }
                // = Vector3.MoveTowards(transform.position, )
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {

        if (other.Equals(Portal[0].GetComponent<Collider>()))
        {
            if (isFull == false)
            {
                transform.position = new Vector3(Portal[1].position.x - 2f, transform.position.y, transform.position.z);
                Rail_Nodes.Enqueue(Rail_Nodes.Dequeue());
            }
            else if (isFull == true)
            {
                isStop = true;
                transform.position = Vector3.one * 100f;
                //NewGameManager.instance.AddVehicle(this);
            }
        }
        else if (other.Equals(Portal[1].GetComponent<Collider>()))
        {
            if (isFull == false)
            {
                transform.position = new Vector3(Portal[0].position.x + 2f, transform.position.y, transform.position.z);
                Rail_Nodes.Enqueue(Rail_Nodes.Dequeue());
            }
            else if (isFull == true)
            {
                isStop = true;
                transform.position = Vector3.one * 100f;
                //NewGameManager.instance.AddVehicle(this);
            }
        }

        else if (other.CompareTag("FullCheck"))
        {
            if (isFull == true)
            {
                isStop = true;
                transform.position = Vector3.one * 100f;
            }
        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDeco == false)
        {
            if (collision.transform.CompareTag("Luggage") && isFull == false)
            {
                if (collision.transform.GetComponent<ShootObj>().isCol == false)
                {
                    collision.transform.GetComponent<ShootObj>().isCol = true;
                    collision.transform.position = -Vector3.one * 100f;
                    isFull = true;
                    StartCoroutine(SpawnObj(collision.gameObject));
                    //Destroy(collision.collider);
                    collision.collider.enabled = false;
                    //collision.gameObject.SetActive(false);
                    collision.transform.GetComponent<Rigidbody>().isKinematic = true;
                }

            }
        }
    }


    IEnumerator SpawnObj(GameObject _CorObj)
    {
        //float _y = 0f;
        //_y = _CorObj.transform.localScale.y * _CorObj.GetComponent<BoxCollider>().size.y;

        ValueText.enabled = false;

        float _pitch = 1f;
        for (int i = 0; i < Value; i++)
        {
            GameObject _obj =
                Instantiate(_CorObj, Stack_Pos.position + Vector3.up * 0.1f, Quaternion.Euler(_CorObj.GetComponent<ShootObj>().Rot));
            Obj_List.Push(_obj);
            Shake_List[i] = _obj.transform;
            _audio.pitch = _pitch;
            if (NewGameManager.instance.isSound)
            {
                _audio.Play();
            }
            if (NewGameManager.instance.isVibe)
            {
                NewGameManager.instance.Vibe(2);
            }
            _pitch += 0.1f;
            //_obj.GetComponent<Rigidbody>().isKinematic = true;


            _obj.transform.localScale = Vector3.zero;
            _obj.transform.DOScale(_obj.GetComponent<ShootObj>().Scale * 1 / transform.localScale.y, Interval).SetEase(Curve);

            _obj.transform.SetParent(transform);

            _obj.transform.DOMoveY(Stack_Pos.position.y + _obj.GetComponent<ShootObj>().Size_Y * i + 0.1f, Interval).SetEase(Curve);
            //_obj.GetComponent<Collider>().enabled = false;
            //Destroy(_obj.GetComponent<Collider>());

            Destroy(_obj.GetComponent<ConstantForce>());
            Destroy(_obj.GetComponent<Rigidbody>());
            yield return new WaitForSeconds(Interval * 0.1f);
        }


        NewGameManager.instance.AddVehicle(this);
        //yield return new WaitForSeconds(2f);
        //isStop = true;
        //transform.position = new Vector3(0f, 20f, 150f);
        //StartCoroutine(Cor_Shake());
        //StartCoroutine(Cor_Shake2());
        StartCoroutine(Cor_Shake3());
    }

    public void Init()
    {
        OnParticle();

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        isFull = false;
        isStop = false;
        transform.position = Start_Pos;
        transform.rotation = Start_Rot;
        ValueText.enabled = true;
        Obj_List.Clear();
        try
        {
            SetNode();
        }
        catch { }
    }

    public void SetNode()
    {
        if (Rail_Nodes.Count != 0)
        {
            Rail_Nodes.Clear();
        }
        foreach (Transform _trans in Tmp_Rail_List)
        {
            Rail_Nodes.Enqueue(_trans);
            //Debug.Log("enqueue");
        }


        for (int i = 0; i < QueuePos_Num; i++)
        {
            Rail_Nodes.Enqueue(Rail_Nodes.Dequeue());
        }
    }

    IEnumerator Cor_Shake()
    {
        isBounce = true;
        yield return new WaitForSeconds(0.5f);

        Vector3 _scale = Shake_List[0].transform.GetComponent<ShootObj>().Scale;

        while (isBounce)
        {
            for (int i = 0; i < Shake_List.Length; i++)
            {
                //_sequence.Append(Shake_List[i].transform.DOScale(_scale * 0.7f, 0.3f))
                //    .AppendInterval(1f)
                //    .Append(Shake_List[i].transform.DOScale(_scale, 0.3f));

                Shake_List[i].transform.DOScale(_scale * 0.85f, 0.2f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(0.03f);
                //Shake_List[i].transform.DOScale(_scale, 0.3f);
            }
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < Shake_List.Length; i++)
            {
                Shake_List[i].transform.DOScale(_scale, 0.2f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(0.03f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Cor_Shake2()
    {
        isBounce = true;
        yield return new WaitForSeconds(0.5f);

        Vector3 _scale = Shake_List[0].transform.GetComponent<ShootObj>().Scale;

        while (isBounce)
        {
            for (int i = 0; i < Shake_List.Length; i++)
            {
                Shake_List[i].transform.DOScale(_scale * 0.85f, 0.2f).SetEase(Ease.Linear);
                //yield return new WaitForSeconds(0.03f);

            }
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < Shake_List.Length; i++)
            {
                Shake_List[i].transform.DOScale(_scale, 0.2f).SetEase(Ease.Linear);
                //yield return new WaitForSeconds(0.03f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Cor_Shake3()
    {
        isBounce = true;
        yield return new WaitForSeconds(0.5f);

        Vector3 _scale = Shake_List[0].transform.GetComponent<ShootObj>().Scale;

        while (isBounce)
        {
            for (int i = 0; i < Shake_List.Length; i++)
            {
                Shake_List[i].transform.DOScale(_scale * 0.85f, 0.4f).SetEase(Curve2);
                yield return new WaitForSeconds(0.06f);

            }
            yield return new WaitForSeconds(1.5f);
            //for (int i = 0; i < Shake_List.Length; i++)
            //{
            //    Shake_List[i].transform.DOScale(_scale, 0.2f).SetEase(Ease.Linear);
            //    //yield return new WaitForSeconds(0.03f);
            //}
            //yield return new WaitForSeconds(1f);
        }
    }

    public void OnParticle()
    {
        if (isDeco == false)
        {
            foreach (GameObject _obj in Particle)
            {
                _obj.SetActive(false);
            }

            if (Value >= 10)
            {
                Particle[(Value / 10) - 1].SetActive(true);
            }
        }
    }
}