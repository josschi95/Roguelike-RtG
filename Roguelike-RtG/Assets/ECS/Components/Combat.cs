namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to declare attacks against others
    /// </summary>
    public class Combat : ComponentBase
    {
        public bool hasMultiStrike = false; //removes major penalty to making multiple attacks
        public int CritRangeBonus = 0;

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is DeclareMeleeAttack declaration) OnDeclareMeleeAttack(declaration);
        }

        private void OnDeclareMeleeAttack(DeclareMeleeAttack declaration)
        {
            //Get Melee Weapons
            var E1 = new GetMeleeAttacks(); 
            entity.FireEvent(E1); //getting back a list of all physics object which will make an attack, could be 0, could be 100


            //Skipping the step where Attack Rolls need to be made


            //Tell each successful attack that they hit
            for (int i = 0; i < E1.attacks.Count; i++)
            {
                var E2 = new MeleeAttackHit(declaration.target);
                E1.attacks[i].entity.FireEvent(E2);
            }
        }
    }
}


//Ok so DeclareMeleeAttack