//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using EP.U3D.EDITOR.BASE;

namespace EP.U3D.EDITOR.PLAT
{
    public class WinPlat : Window
    {
        [MenuItem(Constants.MENU_WIN_PLAT, false, 3)]
        public static void Invoke()
        {
            GetWindowWithRect(WindowType, WindowRect, WindowUtility, WindowTitle);
        }

        public static Type WindowType = typeof(WinPlat);
        public static Rect WindowRect = new Rect(30, 30, 280, 160);
        public static bool WindowUtility = false;
        public static string WindowTitle = "Platform";
        public static Type TargetType = typeof(Platform);

        public class Platform : LIBRARY.BASE.Platform
        {
            [Header("General")]
            [Field] [SerializeField] public new string Project = string.Empty;
            [Field] [SerializeField] public new string AppName = string.Empty;
            [Field] [SerializeField] public new string Channel = string.Empty;
            [Field] [SerializeField] public new string Version = string.Empty;
            [Field] [SerializeField] public new string JsonUrl = string.Empty;

            [Space(5)]
            [Horizontal]
            [Button("Save")] [NonSerialized] public Action<WinPlat> OnSave = (window) => window.OnSave();
            [Button("Apply")] [NonSerialized] public Action<WinPlat> OnApply = (window) => window.OnApply();
        }

        public override void OnEnable() { OpenFile(); }

        public override void OnDestroy() { SaveFile(); }

        public virtual void OpenFile()
        {
            try
            {
                Target = Helper.JsonToObject(File.ReadAllText(Constants.PLAT_FILE_PATH), TargetType);
            }
            catch { Target = Activator.CreateInstance(TargetType); }
        }

        public virtual void SaveFile()
        {
            if (Target == null) Target = Activator.CreateInstance(TargetType);
            Helper.SaveText(Constants.PLAT_FILE_PATH, Helper.ObjectToJson(Helper.ObjectToDict(Target)));
        }

        public virtual bool Validate() { return true; }

        public virtual void OnSave()
        {
            if (Validate())
            {
                SaveFile();
                AssetDatabase.Refresh();
                Helper.Log("[FILE@{0}] Save platform settings success.", Constants.PLAT_FILE_PATH);
                Helper.ShowToast("Save platform settings success.");
            }
        }

        public virtual void OnApply()
        {
            if (Validate())
            {
                SaveFile();
                Helper.CopyFile(Constants.PLAT_FILE_PATH, Constants.PLAT_STEAMING_FILE);
                AssetDatabase.SaveAssets();
                Helper.Log("[FILE@{0}] Compile platform success.", Constants.PLAT_FILE_PATH);
                Helper.ShowToast("Compile platform success.");
            }
        }
    }
}