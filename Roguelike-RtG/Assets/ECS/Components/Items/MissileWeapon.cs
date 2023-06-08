namespace JS.ECS
{
    public class MissileWeapon : ComponentBase
    {
        public MissileWeapon() { }

        public int RangeIncrement = 3; //-1 penalty to Accuracy for Distance to target divided by this value


        public override void OnEvent(Event newEvent)
        {
            if (newEvent is DeclareMissileAttack declaration)
            {
                
            }
        }
    }
}

