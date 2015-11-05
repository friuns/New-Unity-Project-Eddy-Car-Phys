using System.Collections;
using UnityEngine;


public partial class Player
{
    internal float voiceChatTime;
    public bool voiceChatting { get { return Time.time - voiceChatTime < .3f; } }

#if !UNITY_WP8 && VOICECHAT
    private VoiceChatPlayer voiceChatPlayer;
#endif
    protected void VoiceChatSend() { }
    private bool voiceChatFirst;
    [RPC]
    public void SendAudio(byte[] data, int length, int id)
    {
        if (owner.mute)
            return;
#if !UNITY_WP8 && VOICECHAT
        voiceChatTime = Time.realtimeSinceStartup;
        if (_Loader.voiceChatVolume < 0.01) return;
        voiceChatPlayer.audio.volume = _Loader.voiceChatVolume;
        //if (isDebug)
        //print("receive Audio " + data.Length);
        if (!voiceChatFirst)
        {
            voiceChatFirst = true;
            _Hud.centerText(Tr("To use voice chat, press Y"));
        }
        voiceChatPlayer.OnNewSample(new VoiceChatPacket() { Compression = VoiceChatCompression.Speex, Data = data, Length = length, NetworkId = id });
#endif
    }
}

public partial class Game
{

#if !UNITY_WP8 && VOICECHAT
    private void VoiceChat(VoiceChatPacket obj)
    {
        _Player.voiceChatTime = Time.realtimeSinceStartup;
        _Player.CallRPCTo(_Player.SendAudio, PhotonTargets.All, obj.Data, obj.Length, PhotonNetwork.player.ID);
    }
#endif

#if !UNITY_WP8 && VOICECHAT
    private IEnumerator InitMicrophone()
    {

        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

        print("Microphones " + Microphone.devices.Length);
        if (Microphone.devices.Length > 0)
        {
            VoiceChatRecorder v = VoiceChatRecorder.Instance;
            v.enabled = true;
            v.NetworkId = PhotonNetwork.player.ID;
            v.Device = v.AvailableDevices[0];
            v.StartRecording();
            v.NewSample -= VoiceChat;
            v.NewSample += VoiceChat;
        }



        yield return null;
    }
#endif
}