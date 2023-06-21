using JS.EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

//So another possible option would be to separate Menu Input, World Map Input, and Local Map Input into 3 separate
//InputActionAssets and also into separate scripts

namespace JS.ECS
{
    public class InputSystem : MonoBehaviour
    {
        private static InputSystem instance;

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private InputActionAsset inputActions;

        [Header("Game Events")]
        [SerializeField] private GameEvent openPlayerMenu;
        [SerializeField] private GameEvent closePlayerMenu;
        private bool menuOpen = false;

        private bool allowInput = false; //set to true on World or Local map scene loaded
        private bool controlPressed = false;

        private InputHandler inputTarget;
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        private void OnEnable()
        {
            //Mouse
            playerInput.actions["Mouse Position"].performed += i => MousePosition();
            playerInput.actions["LMB"].performed += i => LMB();
            playerInput.actions["RMB"].performed += i => RMB();

            //Directional Input
            playerInput.actions["North"].performed += i => North();
            playerInput.actions["South"].performed += i => South();
            playerInput.actions["East"].performed += i => East();
            playerInput.actions["West"].performed += i => West();

            playerInput.actions["NorthEast"].performed += i => NorthEast();
            playerInput.actions["NorthWest"].performed += i => NorthWest();
            playerInput.actions["SouthEast"].performed += i => SouthEast();
            playerInput.actions["SouthWest"].performed += i => SouthWest();

            playerInput.actions["Up"].performed += i => Up();
            playerInput.actions["Down"].performed += i => Down();
            playerInput.actions["Cell"].performed += i => Cell();

            //Menus
            playerInput.actions["Menu"].performed += i => Menu();

            //Interactions
            playerInput.actions["Get"].performed += i => Get();

            //Secondary Input
            playerInput.actions["Control"].performed += i => controlPressed = true;
            playerInput.actions["Control"].canceled += i => controlPressed = false;
        }

        private void OnDisable()
        {
            //Mouse
            playerInput.actions["Mouse Position"].performed -= i => MousePosition();
            playerInput.actions["LMB"].performed -= i => LMB();
            playerInput.actions["RMB"].performed -= i => RMB();

            //Directional Input
            playerInput.actions["North"].performed -= i => North();
            playerInput.actions["South"].performed -= i => South();
            playerInput.actions["East"].performed -= i => East();
            playerInput.actions["West"].performed -= i => West();

            playerInput.actions["NorthEast"].performed -= i => NorthEast();
            playerInput.actions["NorthWest"].performed -= i => NorthWest();
            playerInput.actions["SouthEast"].performed -= i => SouthEast();
            playerInput.actions["SouthWest"].performed -= i => SouthWest();

            playerInput.actions["Up"].performed -= i => Up();
            playerInput.actions["Down"].performed -= i => Down();
            playerInput.actions["Cell"].performed -= i => Cell();

            //Menus
            playerInput.actions["Menu"].performed -= i => Menu();

            //Interactions
            playerInput.actions["Get"].performed -= i => Get();

            //Secondary Input
            playerInput.actions["Control"].performed -= i => controlPressed = true;
            playerInput.actions["Control"].canceled -= i => controlPressed = false;
        }

        //Called from GameEventListener on World/LocalMapSceneLoaded
        public void OnGameStart() => allowInput = true;
        public void OnGameStop() => allowInput = false;

        public static void OnNewInputTarget(InputHandler input)
        {
            if (instance.inputTarget != null) EntityManager.RemoveComponent(input.entity, instance.inputTarget);

            instance.inputTarget = input;
        }

        private bool CanAct()
        {
            if (!allowInput || inputTarget == null || inputTarget.Actor == null || !inputTarget.Actor.IsTurn) return false;
            return true;
        }

        #region - Mouse Input -
        private void MousePosition()
        {

        }

        private void LMB()
        {

        }

        private void RMB()
        {

        }
        #endregion

        #region - Directional Input -
        private void North()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.up);
            else TryMove(Compass.North);
        }
        private void South()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.down);
            else TryMove(Compass.South);
        }
        private void East()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.right);
            else TryMove(Compass.East);
        }
        private void West()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.left);
            else TryMove(Compass.West);
        }
        private void NorthEast()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.one);
            else TryMove(Compass.NorthEast);
        }
        private void NorthWest()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.up + Vector2Int.left);
            else TryMove(Compass.NorthWest);
        }
        private void SouthEast()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.down + Vector2Int.right);
            else TryMove(Compass.SouthEast);
        }
        private void SouthWest()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.down + Vector2Int.left);
            else TryMove(Compass.SouthWest);
        }
        private void Up()
        {
            if (!CanAct()) return;
            if (controlPressed)
            {
                //Try to attack up, only possible when standing on stairs
            }
            else TryMoveUp();
        }
        private void Down()
        {
            if (!CanAct()) return;
            if (controlPressed)
            {
                //Try to attack down, only possible when standing on stairs
            }
            else TryMoveDown();
        }
        private void Cell()
        {
            if (!CanAct()) return;
            if (controlPressed) TryMelee(Vector2Int.zero);
            else Actions.SkipAction(inputTarget.Actor);
        }
        #endregion

        #region - Menu Input -
        private void Menu()
        {
            if (!allowInput) return;

            menuOpen = !menuOpen;
            if (menuOpen) openPlayerMenu?.Invoke();
            else closePlayerMenu?.Invoke();
        }

        private void Inventory()
        {

        }

        private void Journal()
        {

        }
        #endregion

        private void Get()
        {
            if (!CanAct()) return;
            if (GridManager.WorldMapActive) return;

            var items = TransformSystem.GetTakeablesAt(inputTarget.Transform.Position, inputTarget.Transform.LocalPosition);
            items.Remove(inputTarget.Physics);

            if (items.Count == 0) MessageSystem.NewMessage("There is nothing to take.");
            else if (items.Count == 1) Actions.TryTakeItem(inputTarget.entity, items[0]);
            else
            {
                Debug.Log("Multiple items here. Not yet implemented");
                for (int i = 0; i < items.Count; i++)
                {
                    Debug.Log(items[i].entity.Name);
                    for (int j = 0; j < items.Count; j++)
                    {
                        if (items[i].entity == items[j].entity)
                        {
                            Debug.LogWarning("Its the same item");
                            //EntityManager.TryGetComponent<Transform>(items[i].entity, out var ti);
                            //EntityManager.TryGetComponent<Transform>(items[j].entity, out var tj);

                            //if (ti != null) Debug.Log("ti: " + ti.LocalPosition);
                            //if (tj != null) Debug.Log("tj: " + tj.LocalPosition);
                        }
                    }
                }
            }
        }

        private void TryMove(Compass direction)
        {
            if (GridManager.WorldMapActive) Actions.TryMoveWorld(inputTarget.Transform, direction);//World Map Movement
            else Actions.TryMoveLocal(inputTarget.Transform, direction);
        }

        private void TryMoveUp()
        {
            if (GridManager.WorldMapActive) return;
            else if (inputTarget.Transform.Position.z == 0) //if I do add upward verticality, will instead need to check if outside
            {
                //Load World Map
                WorldLocomotionSystem.SwitchToWorldMap(inputTarget.Transform);
            }
            else LocomotionSystem.TryMoveUp(inputTarget.Transform);
        }

        private void TryMoveDown()
        {
            if (GridManager.WorldMapActive) //World Map Movement
            {
                //Load Local Map
                WorldLocomotionSystem.SwitchToLocalMap(inputTarget.Transform);
            }
            else LocomotionSystem.TryMoveDown(inputTarget.Transform);
        }

        private void TryMelee(Vector2Int direction)
        {
            Actions.TryMeleeAttack(inputTarget.Combat, inputTarget.Transform.LocalPosition + direction);
        }
    }
}