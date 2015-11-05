using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
internal class GameHud { }
public partial class Hud : bs
{
    public EasyFontTextMesh score;
    public EasyFontTextMesh time;
    public EasyFontTextMesh life;
    public EasyFontTextMesh wrongWay;
    public EasyFontTextMesh players;
    public Button menu;
    public Text money;
    public float money2;
    public GUIText chatGui;
    public GUIText killGui;

    public Chat killText = new Chat() { seconds = 5 };
    Chat cText = new Chat() { seconds = 1 };
    public EffectManager m_centerText;
    public EasyFontTextMesh m_centerText2;
    public void centerText2(string s)
    {
        m_centerText2.Text = s;
        Animator component = m_centerText2.GetComponent<Animator>();
        component.CrossFade(0, 0);
    }
    public void Awake()
    {
        _Hud = this;
        _Hud.centerText2("");
        menu.onClick.AddListener(Menu_click);

    }

    private void Menu_click()
    {
        win.ToggleWindow(_GameGui.Menu);
    }

    public void OnValidate()
    {
        _Hud = this;
    }
    private int counter;
    public void Update()
    {
        if (_Player == null || !_Player.inited) return;


        if (_Game.gameState == GameState.none)
        {
            if (counter != (int)(_Game.timeCountMatch - bs.room.matchTime))
            {
                counter = (int)(_Game.timeCountMatch - bs.room.matchTime);
                centerText2((counter + 1).ToString());
                if (!GameType.weapons)
                    PlayOneShotGui(counter == 0 ? res.one : counter == 1 ? res.two : res.three);
                PlayOneShotGui(res.beep);
            }
        }

        //if (!survival)
        if (!lowestQuality || FramesElapsed(30))
        {
            if (android)
            {
                players.renderer.enabled = players.enabled = false;
                time.Text = Tr("Time:") + TimeToStr(Mathf.Max(0, _Game.timeCountMatch), false);
                life.Text = ((int)_Player.life).ToString();
            }
            else
            {

                time.Text = Tr("Time:") + TimeToStr(Mathf.Max(0, _Game.timeCountMatch), false);
                //else
                //    time.Text = Tr("Time:") + TimeToStr( _Game.spTime * 60);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Tr("Life: ") + ((int)_Player.life));
                var Car = _Player.m_Car;

                float SpeedMS = new Vector2(Vector3.Dot(Car.rigidbody.velocity, Car.tr.forward), Vector3.Dot(Car.rigidbody.velocity, Car.tr.right)).magnitude;
                sb.AppendLine(Tr("Speed: ") + string.Format("{0:0.}", SpeedMS * 3.6f) + " km/h");
                if (GameType.pursuitOrRace)
                {
                    var gear = _Player.m_CarSound.currentGear;
                    sb.AppendLine(Tr("Gear: ") + (gear > 0 ? gear.ToString() : gear < 0 ? "R" : "N"));
                }
                sb.AppendLine(Tr("Nitro: ") + (int)_Player.nitro);
                if (GameType.weapons)
                {
                    sb.AppendLine(Tr("Bullets: ") + (int)_Player.machineGun.bullets);
                    sb.AppendLine(Tr("Rockets: ") + (int)_Player.rocketLauncher.bullets);
                }

                sb.AppendLine(Tr("Score: ") + _PlayerView.scoreInt);
                life.Text = sb.ToString();


                if (!lowestQuality || FramesElapsed(30))
                {
                    sb = new StringBuilder();
                    int i = 1;
                    foreach (Player a in _Game.GetPlayers(true, false))
                    {
                        if (a.voiceChatting)
                            sb.Append("ö ");
                        if (a != _Player)
                            sb.Append(" ");
                        sb.Append((a.cop ? "X" : i.ToString()) + ". " + a.pv.playerName);
                        if (bs.room.gameType == GameTypeEnum.pursuit)
                            sb.Append("  " + a.pv.score);

                        sb.AppendLine();
                        i++;
                    }
                    players.Text = sb.ToString();
                }
            }
        }

        chatGui.text = _ChatGui.chatOutput.text;
        killGui.text = killText.text;
        if (MoneyDif > 0)
            money.animation.Play();
        money1 = photonPlayer.curGame.GetMoney();
        money2 = Mathf.Lerp(money2, money1, Time.deltaTime * 3);
        money.text = _Loader.GetMoneyText(money2);
    }

    private int money1;
    public int MoneyDif { get { return photonPlayer.curGame.GetMoney() - money1; } }

    public TextMesh zombieScore;
    public Animation zombieScoreAnim;
    public Transform damage;
    public Animation damageAnim;
    public int acScore;
    public void PlayScore(int s, Color c)
    {
        if (zombieScore.animation.isPlaying)
            acScore += s;
        else
            acScore = s;
        zombieScore.text = acScore.ToString();
        zombieScore.renderer.material.color = c;
        zombieScore.animation.Rewind();
        zombieScore.animation.Play();
    }

    public void centerText(string s)
    {
        cText.chat(s);
        m_centerText.SetText(cText.text);
        if (m_centerText.Playing)
        {
            //    m_centerText.SetAnimationState(0,1);
            m_centerText.PlayAnimation();
            m_centerText.m_animation_timer = 2;
        }
        else
            m_centerText.PlayAnimation();
    }
}

public class Chat
{
    public string text = "";
    public int seconds = 20;
    public bs b { get { return bs._Game; } }
    public void chat(string Obj)
    {
        string output = Obj + "\r\n";
        if (!text.EndsWith(output))
        {
            text += output;
            ClearChat();
        }

    }

    private void ClearChat()
    {
        if (text.SplitString().Length > 5)
            text = RemoveFirstLine(text);
        else
            b.StartCoroutine(bs.AddMethod(seconds, delegate () { text = RemoveFirstLine(text); }));
    }

    public string RemoveFirstLine(string s)
    {
        //Debug.Log("remove LIne");
        int i = s.IndexOf("\r\n", System.StringComparison.Ordinal) + 2;
        //if (i == s.Length) return "";
        return s.Substring(i, s.Length - i);
    }
}