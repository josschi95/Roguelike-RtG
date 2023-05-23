using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    //Position
    public int worldX, worldY; //player's location on the world map
    public int regionX, regionY; //player's location on the region map
    public int localX, localY; //player's location on the local map


}