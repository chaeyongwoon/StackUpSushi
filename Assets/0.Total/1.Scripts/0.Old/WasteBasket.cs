using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WasteBasket : MonoBehaviour
{

    public GameObject PopUp_Canvas;
    public Text PopUp_Text;
    public string[] Text_Type;


    [SerializeField] Vector3 Start_Pos;
    [SerializeField] bool isEnd = true;
    private void Awake()
    {
        Start_Pos = PopUp_Text.rectTransform.position;
    }

    private void OnEnable()
    {
        PopUp_Canvas.SetActive(false);
        PopUp_Text.rectTransform.position = Start_Pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Luggage"))
        {
            if (other.GetComponent<Luggage>().state == Luggage.State.Trash)
            {
                if (other.GetComponent<Luggage>().isTrashCheck == false)
                {
                    other.GetComponent<Luggage>().isTrashCheck = true;
                    GameManager.instance.Separate_Trash_Bonus += 5f;
                    if (isEnd == true)
                    {
                        isEnd = false;
                        StartCoroutine(Cor_InTrash());
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Luggage"))
        {
            if (other.GetComponent<Luggage>().state == Luggage.State.Trash)
            {
                if (other.GetComponent<Luggage>().isTrashCheck == true)
                {
                    other.GetComponent<Luggage>().isTrashCheck = false;
                    GameManager.instance.Separate_Trash_Bonus -= 5f;
                }
            }
        }
    }



    IEnumerator Cor_InTrash()
    {


        yield return null;
        PopUp_Text.rectTransform.position = Start_Pos;
        PopUp_Canvas.SetActive(true);
        PopUp_Text.text
            = string.Format("{0}", Text_Type[Random.Range(0, Text_Type.Length)]);
        PopUp_Text.rectTransform.DOMoveY(Start_Pos.y + 5f, 1f);
        yield return new WaitForSeconds(1f);
        PopUp_Canvas.SetActive(false);

        isEnd = true;


    }


}
