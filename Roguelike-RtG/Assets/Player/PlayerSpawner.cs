using UnityEngine;
using UnityEngine.U2D;
using JS.ECS;
using UnityEngine.InputSystem;
using JS.ECS.Tags;

namespace JS.WorldMap
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private Sprite[] playerSprites;

        private void Awake() => SpawnPlayer();

        private void SpawnPlayer()
        {
            var player = Blueprints.GetCreature("Player");
            player.AddTag(new PlayerTag());

            var transform = player.GetComponent<ECS.Transform>();
            player.GetComponent<Brain>().IsSleeping = false;

            transform.WorldPosition = new Vector2Int(playerData.worldX, playerData.worldY);
            transform.RegionPosition = new Vector2Int(playerData.regionX, playerData.regionY);
            transform.LocalPosition = new Vector2Int(playerData.localX, playerData.localY);
            transform.Depth = playerData.depth;

            player.AddComponent(new WorldLocomotion());
            player.AddComponent(new InputHandler(inputActionAsset));
            player.AddComponent(new RenderCompound(transform, playerSprites));
            player.AddComponent(new CameraFocus());
        }
    }
}