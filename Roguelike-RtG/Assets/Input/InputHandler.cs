using UnityEngine;
using UnityEngine.InputSystem;

namespace JS.ECS
{
    public class InputHandler : ComponentBase
    {
        public InputHandler(InputActionAsset asset)
        {
            actionAsset = asset;
            MapActions();
            SetActions();
        }

        public TimedActor actor { get; set; }
        public Locomotion locomotion { get; set; }

        public InputActionAsset actionAsset { get; set; }

        private InputAction _north;
        private InputAction _east;
        private InputAction _south;
        private InputAction _west;
        private InputAction _northEast;
        private InputAction _northWest;
        private InputAction _southEast;
        private InputAction _southWest;

        private InputAction _center;

        private InputAction _up;
        private InputAction _down;
        private InputAction _control;

        private void SetActions()
        {
            _north.performed += i => North();
            _south.performed += i => South();
            _east.performed += i => East();
            _west.performed += i => West();

            _northEast.performed += i => NorthEast();
            _northWest.performed += i => NorthWest();
            _southEast.performed += i => SouthEast();
            _southWest.performed += i => SouthWest();
        }

        private void MapActions()
        {
            var input = actionAsset.FindActionMap("Player");

            _north = input.FindAction("North");
            _south = input.FindAction("South");
            _east = input.FindAction("East");
            _west = input.FindAction("West");

            _northEast = input.FindAction("NorthEast");
            _northWest = input.FindAction("NorthWest");
            _southEast = input.FindAction("SouthEast");
            _southWest = input.FindAction("SouthWest");

            _center = input.FindAction("Center");
            _up = input.FindAction("Up");
            _down = input.FindAction("Down");
            _control = input.FindAction("Control");
        }

        private void ClearActions()
        {
            _north.performed -= i => North();
            _south.performed -= i => South();
            _east.performed -= i => East();
            _west.performed -= i => West();

            _northEast.performed -= i => NorthEast();
            _northWest.performed -= i => NorthWest();
            _southEast.performed -= i => SouthEast();
            _southWest.performed -= i => SouthWest();
        }

        private void North()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {

            }
            else
            {
                Action.MoveAction(actor, locomotion, new Vector2Int(0, 1));
            }
        }

        private void South()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {

            }
            else
            {
                Action.MoveAction(actor, locomotion, new Vector2Int(0, -1));
            }
        }

        private void East()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {

            }
            else
            {
                Action.MoveAction(actor, locomotion, new Vector2Int(1, 0));
            }
        }

        private void West()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {

            }
            else
            {
                Action.MoveAction(actor, locomotion, new Vector2Int(-1, 0));
            }
        }

        private void NorthEast()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {

            }
            else
            {
                Action.MoveAction(actor, locomotion, new Vector2Int(1, 1));
            }
        }

        private void NorthWest()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {

            }
            else
            {
                Action.MoveAction(actor, locomotion, new Vector2Int(1, -1));
            }
        }

        private void SouthEast()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {

            }
            else
            {
                Action.MoveAction(actor, locomotion, new Vector2Int(-1, 1));
            }
        }

        private void SouthWest()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {

            }
            else
            {
                Action.MoveAction(actor, locomotion, new Vector2Int(-1, -1));
            }
        }

        private void Center()
        {
            if (actor == null || !actor.HasActed) return;
            if (_control.WasPressedThisFrame())
            {
                //Attack down/same tile
            }
            else
            {
                Action.SkipAction(actor);
            }
        }
    }
}