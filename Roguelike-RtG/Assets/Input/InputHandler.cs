using UnityEngine;
using UnityEngine.InputSystem;

namespace JS.ECS
{
    public class InputHandler : ComponentBase
    {
        public InputHandler(InputActionAsset asset,TimedActor actor, Locomotion locomotion)
        {
            entity = actor.entity;
            actionAsset = asset;

            this.actor = actor;
            this.locomotion = locomotion;
            actor.onTurnStart += CheckForButtonDown;

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

            _center.performed += i => Center();
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

            _center.performed -= i => Center();
        }

        private void CheckForButtonDown()
        {
            if (_north.IsPressed()) North();
            else if (_south.IsPressed()) South();
            else if (_east.IsPressed()) East();
            else if (_west.IsPressed()) West();
            else if (_northEast.IsPressed()) NorthEast();
            else if (_northWest.IsPressed()) NorthWest();
            else if (_southEast.IsPressed()) SouthEast();
            else if (_southWest.IsPressed()) SouthWest();
        }


        private bool CanAct()
        {
            if (actor == null || !actor.IsTurn) return false; 
            return true;
        }
        private void North()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {

            }
            else
            {
                PerformAction.TryMoveAction(actor, locomotion, Compass.North);
            }
        }

        private void South()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {

            }
            else
            {
                PerformAction.TryMoveAction(actor, locomotion, Compass.South);
            }
        }

        private void East()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {

            }
            else
            {
                PerformAction.TryMoveAction(actor, locomotion, Compass.East);
            }
        }

        private void West()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {

            }
            else
            {
                PerformAction.TryMoveAction(actor, locomotion, Compass.West);
            }
        }

        private void NorthEast()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {

            }
            else
            {
                PerformAction.TryMoveAction(actor, locomotion, Compass.NorthEast);
            }
        }

        private void NorthWest()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {

            }
            else
            {
                PerformAction.TryMoveAction(actor, locomotion, Compass.NorthWest);
            }
        }

        private void SouthEast()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {

            }
            else
            {
                PerformAction.TryMoveAction(actor, locomotion, Compass.SouthEast);
            }
        }

        private void SouthWest()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {

            }
            else
            {
                PerformAction.TryMoveAction(actor, locomotion, Compass.SouthWest);
            }
        }

        private void Center()
        {
            if (!CanAct()) return;
            PerformAction.SkipAction(actor);
        }
    }
}