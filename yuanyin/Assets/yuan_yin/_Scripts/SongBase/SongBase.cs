using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class SongBase
{
    public int ID { get; set; }
    public string SongName { get; set; }
    public string SongDescription { get; set; }
    public string SongDescriptionSprite { get; set; }
    public string SongKoreographyEasy { get; set; }
    public string SongKoreographyDifficult { get; set; }
    public string SongSprite { get; set; }

    public string SongRank { get; set; }

    public string SongCombo { get; set; }

    public SongBase()
    {
    }

    public SongBase(int id, string songName, string songDescription, string songDescriptionSprite,
        string songKoreographyEasy, string songKoreographyDifficult, string songSprite, string songRank,
        string songCombo)
    {
        this.ID = id;
        this.SongName = songName;
        this.SongDescription = songDescription;
        this.SongDescriptionSprite = songDescriptionSprite;
        this.SongKoreographyEasy = songKoreographyEasy;
        this.SongKoreographyDifficult = songKoreographyDifficult;
        this.SongSprite = songSprite;
        this.SongRank = songRank;
        this.SongCombo = songCombo;
    }
}