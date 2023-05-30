using UnityEngine;
using UnityEngine.U2D;
using JS.ECS;
using UnityEngine.InputSystem;
using JS.ECS.Tags;

namespace JS.WorldMap
{
    public class PlayerWorldMapLocator : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private Sprite[] playerSprites;

        [Space]

        [SerializeField] private PixelPerfectCamera pixelCam;

        public void PlacePlayer()
        {
            var player = Blueprints.GetCreature("Player");
            player.AddTag(new PlayerTag());

            var transform = player.GetComponent<ECS.Transform>();
            transform.LocalPosition = new Vector2Int(playerData.worldX + worldMap.TerrainData.OriginX, playerData.worldY + worldMap.TerrainData.OriginY);

            var actor = player.GetComponent<TimedActor>();
            player.AddComponent(new InputHandler(inputActionAsset, actor));

            new RenderCompound(transform, playerSprites);

            CenterCameraOnPlayer(transform.LocalPosition);
            pixelCam.assetsPPU = 64;
            var cam = pixelCam.GetComponent<CameraController>();

            transform.onTransformChanged += delegate
            {
                cam.SmoothToPosition(transform.LocalPosition);
            };
        }

        private void CenterCameraOnPlayer(Vector2 pos)
        {
            var newPos = new Vector3(pos.x, pos.y, pixelCam.transform.position.z);
            pixelCam.transform.position = newPos;
        }
    }
}