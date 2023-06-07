using UnityEngine;
using JS.ECS;
using UnityEngine.InputSystem;
using JS.ECS.Tags;

namespace JS.WorldMap
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private Sprite[] playerSprites;

        private void Awake() => SpawnPlayer();

        private void SpawnPlayer()
        {
            //var player = Blueprints.GetCreature("Player");
            var player = EntityFactory.GetEntity("BaseCreature");
            player.AddTag(new PlayerTag());

            var physics = player.GetComponent<ECS.Physics>();
            var brain = player.GetComponent<Brain>();
            brain.IsSleeping = false;
            brain.HasOverride = true;

            physics.Position = playerData.Position;
            physics.LocalPosition = playerData.LocalPosition;
            player.AddComponent(new RenderCompound(physics, playerSprites, true, true));

            player.AddComponent(new WorldLocomotion());
            player.AddComponent(new InputHandler());

            player.GetComponent<RenderCompound>().RenderOnWorldMap = true;
            player.AddComponent(new CameraFocus());
        }
    }
}