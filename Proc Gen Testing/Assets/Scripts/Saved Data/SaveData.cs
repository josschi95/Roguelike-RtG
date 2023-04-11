using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public string saveFileName; //The default name for the file
    public string saveFileNameOverride;  //If the player chooses to name a save file directly. We'll see if I implement this

    public MapData mapData; //Saves the state of the world map, height, heat, moisture, mountains, rivers, etc.


}
