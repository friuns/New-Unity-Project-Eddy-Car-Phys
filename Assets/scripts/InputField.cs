using System;
using UnityEngine;

namespace doru
{


    public class InputField : bs
    {
        private EasyFontTextMesh chatInput;
        public string Text { get { return chatInput.Text; } set { chatInput.Text = value; } }

        public void Start()
        {
            chatInput = GetComponent<EasyFontTextMesh>();
        }
        public Action a;
        private void Update()
        {
            if (win.enabled) return;

            if ((Button.Intersects(renderer) || Text.Length < 3) && Input.touchCount > 0)
            {
                print("Show Input");
                ShowTouchKeyboard();
            }


            foreach (char c in Input.inputString)
            {
                if (c == '\b')
                {

                    if (Text.Length > 0)
                        Text = Text.Substring(0, Text.Length - 1);
                }
                else if (c == '\n' || c == '\r')
                {
                    a();
                }
                else
                    Text += c;
                Check();
            }
            //Text = Loader.Filter(Text);
        }
        public void ShowTouchKeyboard()
        {
            print("ShowKeyboard");
            var t = TouchScreenKeyboard.Open(Text);
            StartCoroutine(AddMethod(() => t.done || t.wasCanceled, delegate
                                                                    {
                                                                        Text = t.text;
                                                                        if (t.done && Check())
                                                                            a();
                                                                    }));
        }
        public bool Check()
        {
            if (Text.Length > 14)
                Text = Text.Substring(0, 14);
            return Text.Length > 3;
        }
    }



#if !UNITY_ANDROID || UNITY_EDITOR
    public class TouchScreenKeyboard
    {
        public string text = "";
        public bool done;
        public bool wasCanceled = true;
        public static TouchScreenKeyboard Open(string Text)
        {
            return new TouchScreenKeyboard();
        }
    }
#endif
}