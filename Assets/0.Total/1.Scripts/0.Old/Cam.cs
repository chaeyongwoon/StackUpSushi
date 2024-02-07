using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cam : MonoBehaviour
{
    //public Transform StartPos_Trans;
    //public Transform[] EndPos_Trans;
    public Transform MovingPos_Trans;

    public Transform Car_Trans;
    //public Vector3 Offset;
    public Vector3 Start_pos;
    //public Quaternion Start_Rot;

    public Vector3 Moving_Pos;
    public Vector3 Moving_Rot;
    public float Moving_Speed = 5f;
    public float Rot_Speed = 1f;

    public bool isEndingView = false;

    public int Map_Num = 0;

    public GameManager _gm;
    private void Awake()
    {
        Start_pos = transform.position;
        //Start_Rot = transform.rotation;

        //PosRenderOff();
        _gm = GameManager.instance;
    }


    // Update is called once per frame
    void Update()
    {
        if (_gm == null)
        {
            _gm = GameManager.instance;
        }


        if (Car_Trans)
        {
            switch (_gm.state)
            {
                case GameManager.State.Ready:
                case GameManager.State.Wait:
                    //case GameManager.State.Play:
                    //transform.position =
                    //    new Vector3(0f, Car_Trans.position.y, Car_Trans.position.z)
                    //    + new Vector3(0f, Offset.y, Offset.z);
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Start_Rot, Time.deltaTime * Rot_Speed);

                    transform.position =
                      _gm.Map_StartPos_Trans.position;
                    //StartPos_Trans.position;
                    transform.rotation = Quaternion.Lerp(transform.rotation
                        , _gm.Map_StartPos_Trans.rotation
                        , Time.deltaTime * Rot_Speed);


                    break;
                case GameManager.State.SafeBar:
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(MovingPos_Trans.position.x, MovingPos_Trans.position.y, MovingPos_Trans.position.z), Time.deltaTime * Moving_Speed);
                    transform.rotation = Quaternion.Lerp(transform.rotation, MovingPos_Trans.rotation, Time.deltaTime * Rot_Speed);

                    //
                    //transform.DOMove(MovingPos_Trans.position, 1f);
                    //transform.DORotate(Quaternion.ToEulerAngles(MovingPos_Trans.rotation), 1f);
                    //transform.DORotate(Vector3.one, 1f);

                    break;
                case GameManager.State.Drive:


                    if (isEndingView == false)
                    {
                        //transform.position =
                        //    new Vector3(0f, 0f, Car_Trans.position.z) + Moving_Pos;
                        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Moving_Rot), Time.deltaTime * Rot_Speed);

                        transform.position =
                            new Vector3(MovingPos_Trans.position.x, MovingPos_Trans.position.y, MovingPos_Trans.position.z);
                        //MovingPos_Trans.position;
                        transform.rotation = MovingPos_Trans.rotation;

                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position
                            , _gm.Map_EndPos_Trans.position
                            //, EndPos_Trans[0].position
                            , Time.deltaTime * Moving_Speed);
                        transform.rotation = Quaternion.Lerp(transform.rotation
                            , _gm.Map_EndPos_Trans.rotation
                            , Time.deltaTime * Rot_Speed);
                    }


                    break;

                //case GameManager.State.Driving:
                //    break;

                case GameManager.State.End:
                    transform.position = Vector3.Lerp(transform.position, _gm.Map_EndPos_Trans.position, Time.deltaTime * Moving_Speed);

                    transform.rotation = Quaternion.Lerp(transform.rotation, _gm.Map_EndPos_Trans.rotation, Time.deltaTime * Rot_Speed);

                    break;

                case GameManager.State.Clear:
                    //transform.position = Vector3.Lerp(transform.position, EndPos_Trans[1].position, Time.deltaTime * Moving_Speed);

                    //transform.rotation = Quaternion.Lerp(transform.rotation, EndPos_Trans[1].rotation, Time.deltaTime * Rot_Speed);

                    break;


            }


        }
        else
        {
            transform.position = Start_pos;
        }




    }

    public void SetOffset(Transform _trans)
    {
        //Car_Trans = _trans;
        //Offset = Start_pos - Car_Trans.position;
        //isEndingView = false;

        ///
        Car_Trans = _trans;
        MovingPos_Trans = Car_Trans.GetComponent<Car>().MovingPos_Trans;
        isEndingView = false;

    }

    //public void PosRenderOff()
    //{
    //    _gm.Map_StartPos_Trans[Map_Num].GetComponent<Renderer>().enabled = false;
    //    _gm.Map_EndPos_Trans[Map_Num].GetComponent<Renderer>().enabled = false;
        

    //}



}
