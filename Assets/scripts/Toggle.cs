//using UnityEngine;

//namespace mygui
//{
//    public class Toggle : GuiBase
//    {
//        public bool value { get { return save ? s_toggle2 : m_toggle2; } set { if (save) s_toggle2 = value; else m_toggle2 = value; } }
//        private bool s_toggle2 { get { return (n_toggle ?? (n_toggle = PlayerPrefsGetBool(name + " toggle", true)).Value); } set { PlayerPrefsSetBool(name + "toggle", (n_toggle = value).Value); } }
//        private bool? n_toggle;
//        private bool m_toggle2;


//        public bool save;
      
//        public string name;
//        public void Draw(string s)
//        {
//            name = s;
//            value = GUILayout.Toggle(value, Tr(s));
//        }
//    }

//    public class GuiBase
//    {
//        public static string Tr(string s, bool sk = false, bool save = true)
//        {
//            return bs.Tr(s, sk, save);
//        }
//        public string Trs(string s)
//        {
//            return Tr(s, true);
//        }
//        public string Trn(string s)
//        {
//            return Tr(s, false, false);
//        }

//        public bool PlayerPrefsGetBool(string a, bool b)
//        {
//            return PlayerPrefs.GetInt(a, b ? 1 : 0) == 1;
//        }
//        public void PlayerPrefsSetBool(string a, bool b)
//        {
//            PlayerPrefs.SetInt(a, b ? 1 : 0);
//        }
//    }
//}