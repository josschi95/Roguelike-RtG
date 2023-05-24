using System;
using UnityEngine;
using UnityEngine.U2D;
using JS.ECS;
using UnityEngine.InputSystem;

namespace JS.WorldMap
{
    public class PlayerWorldMapLocator : MonoBehaviour
    {
        [SerializeField] private GameObject playerObject;
        [SerializeField] private PlayerData playerData;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private InputActionAsset inputActionAsset;

        [Space]

        [SerializeField] private PixelPerfectCamera pixelCam;

        public void PlacePlayer()
        {
            CreatePlayerEntity();

            var convertedPos = worldMap.TerrainData.Origin + new Vector3Int(playerData.worldX, playerData.worldY);
            playerObject.transform.position = convertedPos;

            CenterCameraOnPlayer();
        }

        private void CreatePlayerEntity()
        {
            var playerEntity = new Entity();
            var transform = new ECS.Transform(playerEntity);
            var locomotion = new Locomotion(transform);
            var actor = new TimedActor(playerEntity);
            actor.Speed = 100;
            var input = new InputHandler(inputActionAsset);
            input.entity = playerEntity;
            input.actor = actor;
            input.locomotion = locomotion;
        }

        private void CenterCameraOnPlayer()
        {
            var newPos = playerObject.transform.position;
            newPos.z = pixelCam.transform.position.z;
            pixelCam.transform.position = newPos;

            pixelCam.assetsPPU = 64;
        }
    }
}