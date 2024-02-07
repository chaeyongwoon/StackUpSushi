using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDictionary : MonoBehaviour
{
    public Dictionary<string, Vector2> Dic = new Dictionary<string, Vector2>();

    public Dictionary<string, Item> Dic2 = new Dictionary<string, Item>();



    public class Item
    {
        public Sprite _image;
        public float _price;
        public int _count;
    }



    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Dic.ContainsKey("Chair"))
            {
                Dic["Chair"] += new Vector2(0, 1);
            }
            else
            {
                Dic.Add("Chair", new Vector2(10, 1));
            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Dic2.ContainsKey("Bed"))
            {
                Dic2["Bed"]._count++;
            }
            else
            {
                Item _item = new Item();
                _item._price = 100f;
                _item._count = 1;

                Dic2.Add("Bed", _item);
            }
        }




        if (Input.GetKeyDown(KeyCode.Space))
        {

            foreach (KeyValuePair<string, Vector2> _dic in Dic)
            {
                Debug.Log(string.Format("{0} : {1}", _dic.Key, _dic.Value));
            }

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (KeyValuePair<string, Item> _dic2 in Dic2)
            {
                Debug.Log(string.Format("name : {0} , image : {1} , Price : {2} , Count : {3} ",
                    _dic2.Key, _dic2.Value._price, _dic2.Value._price, _dic2.Value._count));
            }
        }
    }
}
