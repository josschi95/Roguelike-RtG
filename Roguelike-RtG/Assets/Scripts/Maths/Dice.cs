using UnityEngine;

public static class Dice
{
    public static int Roll(DiceCombo combo)
    {
        return Roll(combo.diceCount, combo.diceSides, combo.modifier);
    }

    public static int Roll(int count, int sides, int modifier)
    {
        int rolledValue = 0;
        for (int i = 0; i < count; i++)
        {
            rolledValue += Random.Range(1, sides);
        }
        rolledValue += modifier;
        return rolledValue;
    }

    public static int Roll(int value)
    {
        return Random.Range(1, value);
    }

    public static int Roll(string value)
    {
        if (value.Contains('d'))
        {
            var dice = value.Split('d');
            int.TryParse(dice[0], out int count);

            if (dice[1].Contains("+"))
            {
                var num = dice[1].Split("+");
                if (int.TryParse(num[0], out int sides) && int.TryParse(num[1], out int mod))
                {
                    return Roll(count, sides, mod);
                }
            }
            else if(int.TryParse(dice[1], out int sides))
            {
                return Roll(count, sides, 0);
            }
        }
        else if (int.TryParse(value, out int dmg))
        {
            return dmg;
        }

        return 0;
    }
}