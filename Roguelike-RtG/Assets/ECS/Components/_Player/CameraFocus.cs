using UnityEngine;

namespace JS.ECS
{
    /// <summary>
    /// Smooths the camera to focus on the entity whenever it moves.
    /// </summary>
    public class CameraFocus : ComponentBase
    {
        public CameraFocus() { }

        private Physics _physics;
        public Physics Physics
        {
            get
            {
                if (_physics == null)
                {
                    _physics = EntityManager.GetComponent<Physics>(entity);
                }
                return _physics;
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
                Controller.SmoothToPosition((Vector2Int)Physics.WorldMapPosition);
            }
            else
            {
                Controller.SmoothToPosition(Physics.LocalPosition);
            }

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