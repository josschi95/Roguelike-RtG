using UnityEngine;

public static class SpriteHelper
{
    public static Sprite GetSprite(string path, string name)
    {
        var sprites = Resources.LoadAll<Sprite>(path);
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name == name) return sprites[i];
        }
        return null;
    }
}
