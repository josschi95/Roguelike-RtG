public static class IntToBool
{
    public static int GetInt(bool value)
    {
        if (value == true) return 1;
        return 0;
    }

    public static bool GetBool(int value)
    {
        if (value == 0) return false; 
        return true;
    }
}