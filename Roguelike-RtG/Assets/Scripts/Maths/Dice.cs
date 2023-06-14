using UnityEngine;

public static class Dice
{
    public static int Roll(DiceCombo combo)
    {
        return Roll(combo.diceCount, combo.diceSides, combo.modifier);
    }

    public static int Roll(int count, int sides, int modifier = 0)
    {
        int rolledValue = modifier;
        for (int i = 0; i < count; i++)
        {
            rolledValue += Random.Range(1, sides);
        }
        return rolledValue;
    }

    public static int RollMax(int count, int sides, int modifier = 0)
    {
        int rolledValue = modifier;
        for (int i = 0; i < count; i++)
        {
            rolledValue += sides;
        }
        return rolledValue;
    }

    public static int Roll(int value)
    {
        return Random.Range(1, value + 1);
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
                return Roll(count, sides);
        }

        else if (int.TryParse(value, out int dmg)) return dmg;

        return 0;
    }

    public static int RollMax(string value)
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
                    return RollMax(count, sides, mod);
                }
            }

            else if (int.TryParse(dice[1], out int sides))
                return RollMax(count, sides);
        }

        else if (int.TryParse(value, out int dmg)) return dmg;

        return 0;
    }
}