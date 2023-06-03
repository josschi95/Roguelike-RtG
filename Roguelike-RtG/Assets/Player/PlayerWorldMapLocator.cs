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

            transform.Depth = 1;
            transform.WorldPosition = new Vector2Int(playerData.worldX, playerData.worldY);
            transform.RegionPosition = new Vector2Int(playerData.regionX, playerData.regionY);
            transform.LocalPosition = new Vector2Int(playerData.localX, playerData.localY);

            player.AddComponent(new WorldLocomotion());
            player.AddComponent(new InputHandler(inputActionAsset));
            player.AddComponent(new RenderCompound(transform, playerSprites));
            player.AddComponent(new CameraFocus());

            CenterCameraOnPlayer(transform.WorldPosition);
            pixelCam.assetsPPU = 64;
        }

        private void CenterCameraOnPlayer(Vector2 pos)
        {
            var newPos = new Vector3(pos.x, pos.y, pixelCam.transform.position.z);
            pixelCam.transform.position = newPos;
        }
    }
}