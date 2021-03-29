using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongListInstance : MonoBehaviour
{
    private static SongListInstance _instance;
    public static List<SongBase> SongList = new List<SongBase>();

    public static SongListInstance SongInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SongListInstance)) as SongListInstance;
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent(typeof(SongListInstance)) as SongListInstance;
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        ParseSongJSON();
    }

    private void ParseSongJSON()
    {
        TextAsset ta = Resources.Load<TextAsset>("json/SongList");
        JSONObject j = new JSONObject(ta.text);
        Debug.Log("json:" + j);

        foreach (var item in j.list)
        {
            Debug.Log("jsonitem:" + item["songName"].str);
            int id = (int) item["id"].n;
            string songName = item["songName"].str;
            string songDescription = item["songDescription"].str;
            string songDescriptionSprite = item["songDescriptionSprite"].str;
            string songKoreographyEasy = item["songKoreographyEasy"].str;
            string songKoreographyDifficult = item["songKoreographyDifficult"].str;
            string songSprite = item["songSprite"].str;
            string songRank = item["songRank"].str;
            string songCombo = item["songCombo"].str;


            SongBase addItem = new SongBase(id, songName, songDescription, songDescriptionSprite, songKoreographyEasy,
                songKoreographyDifficult, songSprite,songRank,songCombo);
            SongList.Add(addItem);

        }
    }
}