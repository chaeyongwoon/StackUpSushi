using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MondayOFF;

public class NewUIManager : MonoBehaviour
{

    NewGameManager _gm;

    public GameObject Ready_Panel;
    public GameObject Guide_Panel;
    public GameObject Clear_Panel;
    public GameObject Fail_Panel;

    public Image Guide_Img;
    public Image TapToStart_Text;
    public Image DragToDown_Text;

    public Text Stage_Level_Text;
    Vector3 start_pos;

    public Button Sound_Button, Vibe_Button;

    public Sprite Sound_On, Sound_Off;
    public Sprite Vibe_On, Vibe_Off;



    //[SerializeField] Vector3 Start_pos;
    private void Start()
    {
        _gm = NewGameManager.instance;
        start_pos = Guide_Img.rectTransform.localPosition;

        //Start_pos = transform.position;

    }



    public void Retry_Button()
    {
        _gm.Retry();
    }


    public void Stage_Level_Button(int _num)
    {
        _gm.Stage_Level_Func(_num);
        //TimeInterstitialShower.instance.CheckTimeAndShowInterstitial();
        AdsManager.ShowInterstitial();
    }



    public void Viewdistance(Slider _slider)
    {
        //transform.position = Start_pos - transform.forward * 30 * _slider.value;
        //GetComponent<Camera>().orthographicSize = 30f - 10 * _slider.value; 
    }

    public void Init()
    {
        StopAllCoroutines();
        Ready_Panel.SetActive(true);
        Guide_Panel.SetActive(false);
        Clear_Panel.SetActive(false);
        Fail_Panel.SetActive(false);

        StartCoroutine(BlinkText_Func(TapToStart_Text));
        Stage_Level_Text.text = string.Format("Stage {0}", _gm.Stage_Level);

    }

    public void OnReady()
    {
        Ready_Panel.SetActive(false);
        if (_gm.Stage_Level < 3)
        {
            Guide_Panel.SetActive(true);
            StartCoroutine(Cor_Guide());
            StartCoroutine(BlinkText_Func(DragToDown_Text));
        }
        else
        {
            _gm.isReady = false;
        }
    }



    IEnumerator Cor_Guide()
    {
        yield return null;


        //Debug.Log(start_pos);
        while (_gm.isReady == true)
        {
            Guide_Img.rectTransform.localPosition = start_pos;
            Guide_Img.rectTransform.DOLocalMoveY(-590f, 2f);
            yield return new WaitForSeconds(2f);

            // move rect transform position,
        }
    }

    public void OffGuide()
    {
        _gm.isReady = false;
        Guide_Panel.SetActive(false);

    }


    public void EndPanel_Func(bool isClear)
    {
        switch (isClear)
        {
            case true:

                //EventsManager.instance.ClearStage(_gm.Stage_Level);
                EventTracker.ClearStage(_gm.Stage_Level);
                Clear_Panel.SetActive(true);
                _gm._audio.clip = _gm._Clip[2];
                _gm.Sound_Play();

                break;

            case false:
                Fail_Panel.SetActive(true);
                //_gm._audio.clip = _gm._Clip[3];
                break;


        }



    }

    IEnumerator BlinkText_Func(Image _img)
    {
        while (_gm.isReady == true)
        {
            _img.DOColor(new Vector4(1f, 1f, 1f, 0.4f), 1f);
            yield return new WaitForSeconds(1f);
            _img.DOColor(new Vector4(1f, 1f, 1f, 1f), 1f);
            yield return new WaitForSeconds(1f);

        }
    }



    public void Sound_OnOff()
    {
        _gm.isSound = !_gm.isSound;
        if (_gm.isSound)
        {
            Sound_Button.image.sprite = Sound_On;
        }
        else
        {
            Sound_Button.image.sprite = Sound_Off;
        }

    }

    public void Vibe_OnOff()
    {
        _gm.isVibe = !_gm.isVibe;
        if (_gm.isVibe)
        {
            Vibe_Button.image.sprite = Vibe_On;
        }
        else
        {
            Vibe_Button.image.sprite = Vibe_Off;
        }
    }

}
