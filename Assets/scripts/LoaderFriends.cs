using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Permissions;
using System.Text;
using ExitGames.Client.Photon;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;
using gui = UnityEngine.GUILayout;


public partial class Loader
{


    public void FriendsWindow()
    {

        gui.Label("Your name is: " + PhotonNetwork.playerName);
        if (gui.Button("Add Friend", gui.ExpandWidth(false)))
        {
            string s = "";
            win.ShowWindow(delegate
            {
                gui.BeginHorizontal();
                Label("Name:");
                s = gui.TextField(s);
                gui.EndHorizontal();
                if (Button("Add"))
                {
                    bs._Loader.AddFriend(s);
                    win.Back();
                }
            });

        }

        foreach (var a in Friends.OrderByDescending(a => a.IsOnline))
        {
            if (Button(a.Name + (!a.IsOnline ? "(Offline)" : "")))
                bs._Loader.FriendWindow(a);
        }
    }
    private List<FriendInfo> m_emptyFriends = new List<FriendInfo>();
    public List<FriendInfo> Friends { get { return PhotonNetwork.Friends ?? m_emptyFriends; } }
    private void LoadFriends()
    {
        friends = new HashSet<string>(PlayerPrefs.GetString("friends", "").SplitString());
    }
    private void SaveFriends()
    {
        PlayerPrefs.SetString("friends", string.Join("\n", friends.ToArray()));
    }
    public void RemoveFriend(string s)
    {
        friends.Remove(s);
        SaveFriends();
        RefreshFriends();
    }
    public void AddFriend(string s)
    {
        if (!friends.Contains(s))
            friends.Add(s);
        SaveFriends();
        RefreshFriends();
    }
    public void RefreshFriends()
    {
        if (PhotonNetwork.connectionStateDetailed == PeerState.JoinedLobby && _Loader.friends.Count > 0)
            PhotonNetwork.FindFriends(bs._Loader.friends.ToArray());
    }
    public void FriendWindow(FriendInfo f)
    {
        win.ShowWindow(delegate
        {
            bool inFr = bs._Loader.friends.Contains(f.Name);

            Label("Name: " + f.Name);
            Label("Status: " + (f.IsOnline ? "Online" : "Offline"));
            if (f.IsInRoom)
            {
                gui.BeginHorizontal();
                Label("Room:" + f.Room);
                if (inFr && Button("Join"))
                {
                    hostRoom = PhotonNetwork.GetRoomList().FirstOrDefault(a => a.name == f.Room);
                    _Loader.StartCoroutine(_Loader.LoadLevel(false));
                }
                gui.EndHorizontal();
            }
            if (!inFr)
            {
                if (Button("Add to friends"))
                    AddFriend(f.Name);
            }
            else
                if (Button("Remove from friends"))
                RemoveFriend(f.Name);
        });
    }
    public HashSet<string> friends = new HashSet<string>();

}