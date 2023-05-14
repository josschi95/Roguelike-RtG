using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    public int WorldMapTile { get; set; } //The player's location on the World Map
    public int RegionMapTile { get; set; } //The player's location on the Region Map, within the WorldMapTile
}