using UnityEngine;

namespace JS.ECS
{
    /// <summary>
    /// Smooths the camera to focus on the entity whenever it moves.
    /// </summary>
    public class CameraFocus : ComponentBase
    {
        private Transform transform;
        public Transform Transform
        {
            get
            {
                if (transform == null)
                {
                    transform = entity.GetComponent<Transform>();
                }
                return transform;
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
            if (Transform.Depth == 1)
            {
                Controller.SmoothToPosition(Transform.WorldPosition);
            }
            else
            {
                Controller.SmoothToPosition(Transform.LocalPosition);
            }
        }
    }
}