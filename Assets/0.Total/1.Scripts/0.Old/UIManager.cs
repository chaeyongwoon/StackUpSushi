using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{


    public GameObject Ready_Panel;
    public GameObject Ending_Panel;
    public GameObject Fail_Panel;
    public GameObject Shop_Panel;

    //public Text Temp_Text;
    //public Text Total_Score_Text;
    public Text Stage_Text;
    public Text Bill_Text;
    public GameObject Bill_ContentGroup;
    public GameObject Bill_Unit;
    public Text Separate_Trash_Bonus_Text;
    public Text Total_Text;
    //public Text Test_Text;

    public GameObject[] Stamp;
    public GameObject Retry_Button;
    public GameObject Next_Button;

    public GameObject PopUp_UI;
    public Image PopUp_Img;
    public Sprite[] PopUp_Img_List;

    ///////////////////////
    GameManager _gm;

    [SerializeField] bool isPopUp = false;
    string _str;


    private void Awake()
    {
        _gm = GameManager.instance;
        Init();

    }

    private void Update()
    {
        //Temp_Text.text = string.Format("{0}", _gm.state);
        //Total_Score_Text.text = string.Format("Score : {0}", _gm.Total_Score);
        Stage_Text.text = string.Format("Stage : {0}", _gm.Stage_Level);


    }


    public void Init()
    {
        StopAllCoroutines();
        Ready_Panel.SetActive(true);
        Ending_Panel.SetActive(false);
        Fail_Panel.SetActive(false);
        Stamp[0].SetActive(false);
        Stamp[1].SetActive(false);
        Next_Button.SetActive(false);
        Retry_Button.SetActive(false);
        PopUp_Img.gameObject.SetActive(false);
        isPopUp = false;
        Shop_Panel.SetActive(false);


    }


    public void Ending(bool _isbool)
    {
        Ending_Panel.SetActive(true);
        //float _totalPrice = 0f;

        StartCoroutine(Bill_Func(_isbool));


        ///////////////////////
        //_str = null;
        //foreach (KeyValuePair<string, Info> _item in _gm.Item)
        //{
        //    _str += string.Format("{0} \t {1} \t {2} \n", _item.Key, _item.Value._count, _item.Value._price);
        //    _totalPrice += _item.Value._count * _item.Value._price;
        //}
        //_str += string.Format("\n\n\n Total : {0}", _totalPrice);
        //Bill_Text.text = _str;
        ////////////////////
    }


    public void Fail()
    {
        Fail_Panel.SetActive(true);
    }

    public IEnumerator Bill_Func(bool _isbool)
    {
        Bill_Clear();


        int i = 0;
        float _totalPrice = 0f;

        Bill_ContentGroup.GetComponent<VerticalLayoutGroup>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        foreach (KeyValuePair<string, Info> _item in _gm.Item)
        {
            RectTransform _billUnit = Instantiate(Bill_Unit, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();
            _billUnit.SetParent(Bill_ContentGroup.transform);
            //_billUnit.position = new Vector3(495f + 125f, -400f - 300f * i, 0f);

            //_billUnit.localPosition = new Vector3(0f, -400f - 300f * i, 0f);
            //_billUnit.GetComponent<RectTransform>().domov
            //_billUnit.DOLocalMove(new Vector3(495f, -150f - _billUnit.GetComponent<RectTransform>().rect.height * i, 0f), 0.5f);

            //_billUnit.transform.localScale = Vector3.one * 10f;
            //_billUnit.transform.DOScale(Vector3.one, 0.3f);

            _billUnit.GetChild(0).GetComponent<Image>().sprite = _item.Value._img;
            _billUnit.GetChild(1).GetComponent<Text>().text = _item.Key;
            _billUnit.GetChild(2).GetComponent<Text>().text = _item.Value._count.ToString();
            _billUnit.GetChild(3).GetComponent<Text>().text = string.Format("$ {0}", _item.Value._count * _item.Value._price);
            _totalPrice += _item.Value._count * _item.Value._price;



            i++;
            Bill_ContentGroup.GetComponent<RectTransform>().sizeDelta
                = new Vector2(Bill_ContentGroup.GetComponent<RectTransform>().rect.width, 300f * i);
            yield return new WaitForSeconds(0.2f);

        }
        Separate_Trash_Bonus_Text.text = string.Format("Separate Trash Bonus : +{0}%", _gm.Separate_Trash_Bonus);
        yield return new WaitForSeconds(0.1f);
        _totalPrice *= (100 + _gm.Separate_Trash_Bonus) * 0.01f;
        Total_Text.text = "$" + _totalPrice.ToString();
        //Total_Text.rectTransform.localScale = Vector3.one * 100f;
        //Total_Text.rectTransform.DOScale(Vector3.one, 0.5f);


        yield return new WaitForSeconds(1f);
        //Bill_ContentGroup.GetComponent<VerticalLayoutGroup>().enabled = true;
        //Total_Text.text = "$" + _totalPrice.ToString();
        //Total_Text.rectTransform.localScale = Vector3.one * 100f;
        //Total_Text.rectTransform.DOScale(Vector3.one, 0.5f);

        if (_isbool)
        {
            Stamp[0].SetActive(true);
            Stamp[0].transform.localScale = Vector3.one * 10f;
            Stamp[0].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(1f);
            Next_Button.SetActive(true);

        }
        else
        {
            Stamp[1].SetActive(true);
            Stamp[1].transform.localScale = Vector3.one * 10f;
            Stamp[1].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(1f);
            Retry_Button.SetActive(true);

        }


        //yield return new WaitForSeconds(0.5f);


        yield return null;
    }

    public void Bill_Clear()
    {
        int _childcount = Bill_ContentGroup.transform.childCount;
        for (int j = 0; j < _childcount; j++)
        {
            Destroy(Bill_ContentGroup.transform.GetChild(j).gameObject);
        }

        Bill_ContentGroup.GetComponent<RectTransform>().sizeDelta
            = new Vector2(Bill_ContentGroup.GetComponent<RectTransform>().rect.width, 1200f);
        Bill_ContentGroup.GetComponent<VerticalLayoutGroup>().enabled = false;

        Separate_Trash_Bonus_Text.text = string.Format("Separate Trash Bonus :");
        Total_Text.text = null;
    }

    ///////////// UI Buttons Func /////////////////////


    public void Start_Button()
    {
        if (_gm.state == GameManager.State.Ready)
        {
            Ready_Panel.SetActive(false);
            _gm.state = GameManager.State.Wait;
        }
    }


    public void Reset_Func()
    {
        _gm.Reset();
    }


    public void Next_Func()
    {
        _gm.NextStage();
    }



    public void PopUpFunc(Vector3 _pos, bool isBool)
    {

        if (isPopUp == false)
        {
            isPopUp = true;
            _pos = _pos.y < 0 ? new Vector3(_pos.x, 0f, _pos.z) : _pos;
            PopUp_UI.transform.position = _pos;
            PopUp_Img.rectTransform.localPosition = new Vector3(0f, 3f, 0f);

            StartCoroutine(Cor_PopUp(isBool));


            IEnumerator Cor_PopUp(bool _Good)
            {
                int _RandNum = _Good == true ? Random.Range(0, 3) : Random.Range(4, 6);
                PopUp_Img.sprite = PopUp_Img_List[_RandNum];

                PopUp_Img.gameObject.SetActive(true);
                PopUp_Img.rectTransform.DOMoveY(PopUp_Img.rectTransform.position.y + 5f, 1f);
                yield return new WaitForSeconds(1f);

                PopUp_Img.gameObject.SetActive(false);
                isPopUp = false;
            }
        }
    }

    public void Shop_Button()
    {
        if (_gm.state == GameManager.State.Ready)
        {
            if (Shop_Panel.activeSelf == false)
            {
                Shop_Panel.SetActive(true);
            }
            else
            {
                Shop_Panel.SetActive(false);
            }
        }
    }

    public void Skin_Button(int _num)
    {
         // 
    }
}
