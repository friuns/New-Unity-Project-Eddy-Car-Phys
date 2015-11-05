using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class SecureIntTest : MonoBehaviour
{

    //public SecureInt Test = new SecureInt();

    //public int Test2 = 123123;
    //public SecureInt Test3 = 0;
    //private SecureInt Test4 = new SecureInt();
    //public float i=123123;
    public void Awake()
    {
        SecureInt.started = true;
    }
    public void Start()
    {
        
        //Test.Set(123123);
        //SecureInt.Set("Test",123123);
        //List<SecureInt> ss = new List<SecureInt>();
        //ss.Add(1);
        //ss.Add(1);
        ////Test4 = 3;
        ////Test3 -= 1;
        //IOrderedEnumerable<SecureInt> b = ss.OrderBy(a => a);
        //print(b);

        //this.GetType().GetField("Obin").SetValue(this, 3);
        
    }
    public void Update()
    {
        print(Math.Sqrt(-0.1f));
        //guiText.text = SecureInt.detected + "";

        //SecureInt.Set("Test", (int)i);
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    i++;
        //    //Test.Set(Test.Get() + 1);
        //    //Test2++;
        //}
        //print(i);
        //print(Test2);
        //print(Test3);
        //print(Test4);

    }

    //public new void print(object s)
    //{
    //    guiText.text += "\n" + s;
    //}
}
