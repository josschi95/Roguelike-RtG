using UnityEngine;
using UnityEngine.U2D;

public class PlayerWorldMapLocator : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private WorldMapData worldMap;

    [Space]

    [SerializeField] private PixelPerfectCamera pixelCam;

    public void PlacePlayer()
    {
        ArrayHelper.Convert1DCoordinateTo2DCoordinate(worldMap.Width, worldMap.Height, playerData.WorldMapTile, out int x, out int y);

        var convertedPos = worldMap.TerrainData.mapOrigin + new Vector3Int(x, y);
        playerObject.transform.position = convertedPos;

        CenterCameraOnPlayer();
    }

    private void CenterCameraOnPlayer()
    {
        var newPos = playerObject.transform.position;
        newPos.z = pixelCam.transform.position.z;
        pixelCam.transform.position = newPos;

        pixelCam.assetsPPU = 64;
    }
}