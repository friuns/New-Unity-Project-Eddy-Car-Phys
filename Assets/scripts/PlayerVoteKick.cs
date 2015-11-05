using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
public partial class Player
{

    private float voteTime;
    public static Player kickPlayer;

    public static bool votedKick;
    public static void VoteKick(Player pl)
    {
        if (votedKick) return;
        votedKick = true;
        pl.owner.stats.reports++;
        _Game.players[pl.viewId].CallRPC(_Player.VoteKick, _Loader.playerName);
    }
    private int KickVotes;

    [RPC]
    public void VoteKick(string who)
    {
        _ChatGui.Chat(who + Tr(" Voted kick ") + playerName);

        voteTime = Time.time;

        if (Time.time - voteTime > 5)
        {
            _Hud.centerText("Press F3 to vote kick " + playerName);
            kickPlayer = this;
        }
        KickVotes++;

        if (KickVotes > _Game.players.Count / 3)
            Kick();

        print("voteKick " + playerName);
    }
    [RPC]
    public void Kick()
    {
        if (!_Game) return;
        _ChatGui.Chat(ToString() + Tr(" kicked"));
        if (IsMine)
        {
            win.ShowPopup(" kicked");
            _Loader.ignoredRooms.Add(room.name);
            _Game.LeaveRoom();
        }
    }
   



}