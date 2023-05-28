using UnityEngine;
using UnityEngine.InputSystem;

namespace JS.ECS
{
    public class InputHandler : ComponentBase
    {
        public InputHandler(InputActionAsset asset,TimedActor actor, Locomotion locomotion)
        {
            actor.entity.AddComponent(this);
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

        //Check if a button is pressed at the start of the player's turn
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
            if (_control.IsPressed()) TryAttack(Compass.North);
            else TryMove(Compass.North);
        }

        private void South()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryAttack(Compass.South);
            else TryMove(Compass.South);
        }

        private void East()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryAttack(Compass.East);
            else TryMove(Compass.East);
        }

        private void West()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryAttack(Compass.West);
            else TryMove(Compass.West);
        }

        private void NorthEast()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryAttack(Compass.NorthEast);
            else TryMove(Compass.NorthEast);
        }

        private void NorthWest()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryAttack(Compass.NorthWest);
            else TryMove(Compass.NorthWest);

        }

        private void SouthEast()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryAttack(Compass.SouthEast);
            else TryMove(Compass.SouthEast);
        }

        private void SouthWest()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryAttack(Compass.SouthWest);
            else TryMove(Compass.SouthWest);
        }

        private void Center()
        {
            if (!CanAct()) return;
            Actions.SkipAction(actor);
        }

        private void TryMove(Compass direction)
        {
            Actions.TryMoveAction(actor, locomotion, direction);
        }

        private void TryAttack(Compass direction)
        {
            Actions.TryAttack(actor, direction);
        }
    }
}