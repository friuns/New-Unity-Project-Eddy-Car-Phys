using System.Collections.Generic;
using System.Linq;
#pragma warning disable 162
#pragma warning disable 429
using UnityEngine;

public class AndroidHud : bs
{
    public Player pl { get { return _Player; } }
    public Archor pad;
    public Archor minigun;
    public Archor rocketLauncher;
    public Archor brake;
    public Archor forward;
    public Archor padTouch;
    public Archor left;
    public Archor right;
    public Archor nitro;
    public Archor drift;
    public Archor chat;
    public Archor q;
    private Dictionary<int, KeyHudBool> m_dict;
    public Dictionary<int, KeyHudBool> dict { get { if (m_dict == null)InitDict(); return m_dict; } }

    internal Vector2 mouse;
    private KeyHudBool last;
    public void Start()
    {
        if(!android)
            gameObject.SetActive(false);
        else
        {
            InitDict();
            bsInput.Input2.dict = dict;
        }
    }
    private void keyhudAdd(KeyHudBool keyHudBool)
    {
        if(!keyHudBool.archor.gameObject.activeSelf)return;
        keyHudBool.LoadPos();
        dict.Add((int)keyHudBool.key, keyHudBool);
    }

    public void InitDict()
    {
        m_dict = new Dictionary<int, KeyHudBool>();
        keyhudAdd(new KeyHudBool() { key = KeyCode.W, archor = forward });
        keyhudAdd(new KeyHudBool() { key = KeyCode.S, archor = brake });
        keyhudAdd(new KeyHudBool() { key = KeyCode.A, archor = left });
        keyhudAdd(new KeyHudBool() { key = KeyCode.D, archor = right });
        keyhudAdd(new KeyHudBool() { key = KeyCode.Tab, archor = minigun });
        keyhudAdd(new KeyHudBool() { key = KeyCode.None, archor = pad });
        keyhudAdd(new KeyHudBool() { key = KeyCode.LeftShift, archor = nitro });
        keyhudAdd(new KeyHudBool() { key = KeyCode.Mouse1, archor = rocketLauncher });
        keyhudAdd(new KeyHudBool() { key = KeyCode.Space, archor = drift });
        keyhudAdd(new KeyHudBool() { key = KeyCode.Q, archor = q });
        keyhudAdd(new KeyHudBool() { key = KeyCode.Return, archor = chat });

    }
    public new Camera camera;
    public void Update()
    {
        if (!pl) return;

        padTouch.enabled = pad.enabled = Loader.controls == Contr.mouse;
        rocketLauncher.enabled = _Player.curWeapon.bullets > 0;
        bool keys = Loader.controls == Contr.keys;

        left.enabled = right.enabled = keys;
        //nitro.enabled = pl.nitro > 0;

        //Log("Android Mouse " + mouse + " Taps" + Input.touchCount);
        mouse = Vector3.Lerp(mouse, Vector3.zero, Time.deltaTime * 10);
        bool padDown = false;
        if (Loader.controls == Contr.acel)
            mouse = new Vector2(Mathf.Clamp(Input.acceleration.x * 2, -1, 1), 0);
        else if (Loader.controls == Contr.mouse)
        {

            foreach (var touch in Input.touches)
            {
                Vector3 mpos = touch.position;
                Vector3 pp = pad.inversePos;
                pp.x *= Screen.width;
                pp.y *= Screen.height;
                if (mpos.x < Screen.width / 2f && pp.x < Screen.width / 2f || mpos.x > Screen.width / 2f && pp.x > Screen.width / 2f)
                {
                    Vector3 mouseTmp = (pp - mpos) / (pad.size.x / 2f) * (InputManager.secondPlayer ? 1 : -1);
                    mouseTmp.x = Mathf.Clamp(mouseTmp.x, -1, 1);
                    mouseTmp.y = Mathf.Clamp(mouseTmp.y, -1, 1);
                    if (!InputManager.splitScreen || mpos.y > Screen.height / 2f && InputManager.secondPlayer || mpos.y < Screen.height / 2f && !InputManager.secondPlayer)
                    {
                        mouse = mouseTmp;
                        padTouch.screenPos = mpos;
                        padDown = true;
                    }
                }

            }
            #if UNITY_ANDROID 
            if (AndroidInput.secondaryTouchEnabled && AndroidInput.touchCountSecondary > 0)
            {
                Vector2 p = AndroidInput.GetSecondaryTouch(0).position;
                p.x /= AndroidInput.secondaryTouchWidth;
                p.x -= .5f;
                p.x = Mathf.Clamp(p.x * 2, -1, 1);
                mouse = p;
            }
#endif
            if (!padDown)
                padTouch.tr.position = pad.tr.position;
        }
        if (_Game.editControls)
        {
            if (Input.touchCount == 1)
            {
                foreach (KeyHudBool a in dict.Values)
                {
                    Touch? touch = HitTest(a);
                    if (touch != null)
                    {
                        last = a;
                        Vector2 mpos = touch.Value.position;
                        mpos = camera.ScreenToViewportPoint(mpos);
                        a.archor.inversePos = mpos;
                        a.posx = mpos.x;
                        a.posy = mpos.y;
                    }
                }
            }
            else if (last != null)
            {
                last.scale += GetDoubleTouch() * 0.01f;
                last.scale = Mathf.Max(.7f, last.scale);
                last.UpdateScale();
            }
        }
        if (!_Game.editControls)
        {
            if (keys && !InputManager.splitScreen)
            {
                foreach (KeyHudBool a in dict.Values)
                    a.hitTest = null;
                foreach (Touch b in touches)
                {
                    Vector2 pos = b.position;
                    pos.x /= Screen.width;
                    pos.y /= Screen.height;                    
                    KeyHudBool d = FirstOrDefault(pos);
                    //print(d.guitext.name);
                    if (d != null)
                        d.hitTest = b;
                }
            }
            else
            {
                foreach (KeyHudBool a in dict.Values)
                    a.hitTest = HitTest(a);
            }

            foreach (KeyHudBool a in dict.Values)
            {
                TouchPhase ph = a.hitTest.HasValue ? a.hitTest.Value.phase : ((TouchPhase)221);
                a.hold = ph == TouchPhase.Stationary || ph == TouchPhase.Moved;
                a.up = ph == TouchPhase.Canceled || ph == TouchPhase.Ended;
                a.down = ph == TouchPhase.Began;
                if (!InputManager.splitScreen)
                    a.hudColor = padDown && a.archor == pad || a.hold ? Color.white : Color.white / 2f;
            }
        }
    }
    private KeyHudBool FirstOrDefault(Vector2 pos)
    {
        KeyHudBool min=null;
        foreach (KeyHudBool a in dict.Values)
            if (a.archor.enabled)
            {
                a.dist = Vector2.Distance(pos, a.archor.pos);
                if (a.dist < .1f * a.scale)
                {
                    if (min == null || a.dist < min.dist)
                        min = a;
                }
            }
        return min;
        //return dict.Values.Slinq().Where(a => a.archor.enabled && (a.dist = Vector2.Distance(pos, a.archor.pos)) < .1f * a.scale).OrderBy(a => a.dist).FirstOrDefault();
    }

    private Touch? HitTest(KeyHudBool hud)
    {
        foreach (Touch b in Input.touches)
        {
            var pos = b.position;
            if (hud.archor.HitTest(pos)) return b;
        }
        return null;
    }

    public static float GetDoubleTouch()
    {
        if (touches.Length > 1)
        {
            Vector2 curDist = touches[0].position - touches[1].position;
            Vector2 prevDist = (touches[0].position - touches[0].deltaPosition) - (touches[1].position - touches[1].deltaPosition);
            return curDist.magnitude - prevDist.magnitude;
        }
        return 0;
    }

    public static Touch[] touches { get { return Input.touches; } }
}

