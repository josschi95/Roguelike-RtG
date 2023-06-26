using UnityEngine;
using JS.ECS;
using JS.ECS.Tags;

namespace JS.WorldMap
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;

        private void Awake() => SpawnPlayer();

        private void SpawnPlayer()
        {
            var player = EntityFactory.GetEntity("BaseCreature");
            EntityManager.AddTag(player, new PlayerTag());
            player.Name = "Player";

            var physics = EntityManager.GetComponent<ECS.Transform>(player);
            TransformSystem.SetPosition(physics, playerData.World, playerData.Region, playerData.Local);

            var brain = EntityManager.GetComponent<Brain>(player);
            brain.IsHibernating = false;
            brain.HasOverride = true;

            var render = EntityManager.GetComponent<Render>(player);
            render.RenderIfDark = true;
            render.RenderOnWorldMap = true;

            EntityManager.AddComponent(player, new WorldLocomotion());
            EntityManager.AddComponent(player, new InputHandler());
            EntityManager.AddComponent(player, new CameraFocus());
        }
    }
}