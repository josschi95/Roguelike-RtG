using JS.CharacterSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JS.ECS
{
    public class InputHandler : ComponentBase
    {
        public InputHandler()
        {
            var inputActions = Resources.Load("Input/PlayerInputActions") as InputActionAsset;
            MapActions(inputActions);
            SetActions();
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TurnStart) CheckForButtonDown();
        }

        public override void Disassemble()
        {
            if (EntityManager.TryGetComponent<Brain>(entity, out var brain)) brain.HasOverride = false;

            base.Disassemble();
            ClearActions();
        }

        #region - Components -
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

        private TimedActor _actor;
        public TimedActor Actor
        {
            get
            {
                if (_actor == null)
                {
                    _actor = EntityManager.GetComponent<TimedActor>(entity);
                }
                return _actor;
            }
        }

        private Combat _combat;
        public Combat Combat
        {
            get
            {
                if (_combat == null)
                {
                    _combat = EntityManager.GetComponent<Combat>(entity);
                }
                return _combat;
            }
        }
        #endregion

        public int MoveSpeed
        {
            get
            {
                if (EntityManager.TryGetStat(entity, "WALK", out StatBase stat))
                {
                    return stat.Value;
                }
                return 1;
            }
        }

        private InputAction _north;
        private InputAction _east;
        private InputAction _south;
        private InputAction _west;
        private InputAction _northEast;
        private InputAction _northWest;
        private InputAction _southEast;
        private InputAction _southWest;


        private InputAction _up;
        private InputAction _down;
        private InputAction _cell;

        private InputAction _control;
        private InputAction _wait;

        private void MapActions(InputActionAsset actionAsset)
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

            _up = input.FindAction("Up");
            _down = input.FindAction("Down");
            _cell = input.FindAction("Cell");

            _control = input.FindAction("Control");
            _wait = input.FindAction("Wait");
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

            _up.performed += i => Up();
            _down.performed += i => Down();
            _cell.performed += i => Cell();
            _wait.performed += i => Wait();
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

            _up.performed -= i => Up();
            _down.performed -= i => Down();
            _cell.performed -= i => Cell();
            _wait.performed -= i => Wait();
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

            else if (_cell.IsPressed()) Cell();
            else if (_wait.IsPressed()) Wait();
        }

        private bool CanAct()
        {
            if (Actor == null || !Actor.IsTurn) return false; 
            return true;
        }

        #region - Directional Input -
        private void North()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryMeleeAttack(Vector2Int.up);
            else TryMove(Compass.North);
        }

        private void South()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryMeleeAttack(Vector2Int.down);
            else TryMove(Compass.South);
        }

        private void East()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryMeleeAttack(Vector2Int.right);
            else TryMove(Compass.East);
        }

        private void West()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryMeleeAttack(Vector2Int.left);
            else TryMove(Compass.West);
        }

        private void NorthEast()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryMeleeAttack(Vector2Int.one);
            else TryMove(Compass.NorthEast);
        }

        private void NorthWest()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryMeleeAttack(Vector2Int.up + Vector2Int.left);
            else TryMove(Compass.NorthWest);

        }

        private void SouthEast()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryMeleeAttack(Vector2Int.down + Vector2Int.right);
            else TryMove(Compass.SouthEast);
        }

        private void SouthWest()
        {
            if (!CanAct()) return;
            if (_control.IsPressed()) TryMeleeAttack(Vector2Int.down + Vector2Int.left);
            else TryMove(Compass.SouthWest);
        }

        private void Up()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {
                //Try to attack up, only possible when standing on stairs
            }
            else TryMoveUp();
        }

        private void Down()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {
                //Try to attack down, only possible when standing on stairs
            }
            else TryMoveDown();
        }

        private void Cell()
        {
            if (!CanAct()) return;
            if (_control.IsPressed())
            {
                //Try to attack a creature occupying the same space, don't attack self
                //Examples might be swarms, oozes, etc. or attacking at the player's feet/the ground
                TryMeleeAttack(Vector2Int.zero);
            }
        }

        private void Wait()
        {
            if (!CanAct()) return;
            Actions.SkipAction(Actor);
        }
        #endregion

        #region - Movement -
        private void TryMoveUp()
        {
            if (GridManager.WorldMapActive) return;
            else if (Transform.Position.z == 0) //if I do add upward verticality, will instead need to check if outside
            {
                //Load World Map
                WorldLocomotionSystem.SwitchToWorldMap(Transform);
            }
            else LocomotionSystem.TryMoveUp(Transform);
        }

        private void TryMoveDown()
        {
            if (GridManager.WorldMapActive) //World Map Movement
            {
                //Load Local Map
                WorldLocomotionSystem.SwitchToLocalMap(Transform);
            }
            else LocomotionSystem.TryMoveDown(Transform);
        }

        private void TryMove(Compass direction)
        {
            //Debug.Log("TryMove " + _actor.entity.Name);

            if (GridManager.WorldMapActive) Actions.TryMoveWorld(Transform, direction);//World Map Movement
            else Actions.TryMoveLocal(Transform, direction);
        }
        #endregion

        private void TryMeleeAttack(Vector2Int direction)
        {
            Actions.TryMeleeAttack(Combat, Transform.LocalPosition + direction);
        }
    }
}