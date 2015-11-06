using gui = UnityEngine.GUILayout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public partial class Administration
{
    public void OnGUI()
    {
        GUI.skin = settings.unitySkin;
        GUI.depth = -1;
        gui.BeginHorizontal("", skin.GetStyle("flow background"));
        {
            if (Button("Exit"))
                CloseAdmin();
            if (isMaster)
            {
                if (Button("Load"))
                    win.ShowWindow(LoadMapWindow);
                if (Button("Save"))
                    win.ShowWindow(SaveMapWindow);
                if (Button("Clear"))
                    ClearMap();
            }
            gui.FlexibleSpace();
        }
        gui.EndHorizontal();
        //if (Event.current.type == EventType.Repaint && !string.IsNullOrEmpty(GUI.tooltip))
        //    win.tooltip = GUI.tooltip;
    }
    public void GuiWindow()
    {
        SetupWindow(300);
        win.addflexibleSpace = true;
        win.showBackButton = false;
        win.windowSkin = settings.unitySkin;
        win.showScrollAlways = true;
        win.dock = Window.Dock.Left;
        if (!isMaster)
            Label("Only host can change parameters!");
        Label(room.gameType + " " + Application.loadedLevelName);


        if (BeginVertical("Tools"))
        {

            IList<string> strings = Enum.GetNames(typeof(ToolType));
            //curTool = (ToolType)Toolbar((int)curTool, strings, false, false, 99, 2, true, "Tools");



            gui.BeginHorizontal();
            for (int i = 0, j = 0; i < res.tools.Count; i++)
            {
                var a = res.tools[i];
                if ((a.gameType.Length == 0 || a.gameType.Contains(room.gameType) || isDebug))
                {
                    if (j++ % 2 == 0 && j != 0)
                    {
                        gui.EndHorizontal();
                        gui.BeginHorizontal();
                    }
                    if (GlowButton(a.name, a == prefab, true, colors[a.index]))
                    {
                        prefab = a;
                        curTool = (ToolType)strings.Count;
                    }
                }
            }
            gui.EndHorizontal();
            for (int i = 0; i < strings.Count; i++)
            {
                if (GlowButton(strings[i], (int)curTool == i))
                    curTool = (ToolType)i;
            }
            if ((int)curTool < strings.Count)
                prefab = null;
            gui.EndVertical();
        }
        if (BeginVertical("Server settings"))
        {
            if (Button("Car Physics"))
                ShowCarPhys();

            room.matchTime = HorizontalSlider("Match Time", room.matchTime, 1, 5f * 60f, false, true);
            //BeginVertical("Level Settings");
            room.lifeDef = HorizontalSlider("Life", room.lifeDef, 30, 300);
            room.gravity = -HorizontalSlider("gravity", -bs.room.gravity, 9.81f, 20f);
            room.gameSpeed = HorizontalSlider("Game Speed", room.gameSpeed, 1, 2);
            room.disableRockets = Toggle(room.disableRockets, "Disable Rockets");
            room.disableMachineGun = Toggle(room.disableMachineGun, "Disable MachineGun");
            room.disableVoiceChat = Toggle(room.disableVoiceChat, "Disable Voice Chat");
            room.varParse.UpdateValues();

            //room.autoLifeRecovery = Toggle(room.autoLifeRecovery, "Auto Repair");
            if (GameType.race)
                room.noKillScore = Toggle(room.noKillScore, "No Kill Score");

            if (isDebug)
                bs.room.collFix = gui.Toggle(bs.room.collFix, "Coll Fix");

            gui.EndVertical();
        }
    }
    public void ShowCarPhys()
    {
        ShowWindow(delegate
        {
            win.windowSkin = settings.unitySkin;
            room.varParse.filter = GuiClasses.TextField("Search:", _Player.varParse.filter);
            room.varParse.UpdateValues(_Player.m_Car, new StringBuilder("CarPhys"));
        });
        //room.Set("dsad","sdads");
    }
}