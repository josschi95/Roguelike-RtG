using UnityEngine;

namespace JS.ECS
{
    /// <summary>
    /// Smooths the camera to focus on the entity whenever it moves.
    /// </summary>
    public class CameraFocus : ComponentBase
    {
        public CameraFocus() { }

        private Transform _transform;
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = EntityManager.GetComponent<Transform>(entity);
                }
                return _transform;
            }
        }

        private CameraController controller;
        public CameraController Controller
        {
            get
            {
                if (controller == null)
                {
                    controller = Camera.main.GetComponent<CameraController>();
                }
                return controller;
            }

        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TransformChanged) FollowTarget();
        }

        private void FollowTarget()
        {
            if (GridManager.WorldMapActive)
            {
                Controller.SmoothToPosition((Vector2Int)Transform.WorldPosition);
                return;
            }

            //Player crossed into a new map, change scenes
            if (GridManager.ActiveGrid.World != Transform.WorldPosition || GridManager.ActiveGrid.Region != Transform.RegionPosition)
            {
                //Debug.Log("Player Switching Map");
                GridManager.OnEnterLocalMap(Transform.WorldPosition, Transform.RegionPosition);
            }

            Controller.SmoothToPosition(Transform.LocalPosition);

            /*if (Transform.Depth == 1)
            {
                Controller.SmoothToPosition(Transform.WorldPosition);
            }
            else
            {
                Controller.SmoothToPosition(Transform.LocalPosition);
            }*/
        }
    }
}