public static class Size
{
    public static string GetName(int size)
    {
        switch (size)
        {
            case 0: return "Tiny";
            case 1: return "Small";
            case 2: return "Medium";
            case 3: return "Large";
            case 4: return "Huge";
            case 5: return "Gargantuan";
            default: return "Medium";
        }
    }

    public static int GetHitDie(int size)
    {
        switch (size)
        {
            case 0: return 4; //tiny
            case 1: return 6; //small
            case 2: return 8; //medium
            case 3: return 10; //large
            case 4: return 12; //huge
            case 5: return 20; //gargantuan
            default: return 8;
        }
    }

    public static int GetStealthModifier(int size)
    {
        switch (size)
        {
            case 0: return 8; //tiny
            case 1: return 4; //small
            case 2: return 0; //medium
            case 3: return -4; //large
            case 4: return -8; //huge
            case 5: return -12; //gargantuan
            default: return 0;
        }
    }
}
