namespace JS.ECS.Tags
{
    /// <summary>
    /// Base Type for all tags.
    /// </summary>
    public abstract class TagBase
    {
        public string Value { get; protected set; }
    }

    /// <summary>
    /// Tag to mark an object as being immune to effects.
    /// </summary>
    public class NoEffect : TagBase { }

    public class Gender : TagBase
    {
        public Gender(string gender)
        {
            Value = gender;
        }
    }

    public class Item : TagBase { }

    /// <summary>
    /// Tag to mark that an item is part of a creature's anatomy.
    /// </summary>
    public class NaturalGear : TagBase { }

    /// <summary>
    /// Tag to indicate that an item should never be stacked.
    /// </summary>
    public class NeverStack : TagBase { }

    /// <summary>
    /// Tag to indicate that the entity blocks movement into its space.
    /// </summary>
    public class BlocksNode : TagBase { }

    /// <summary>
    /// Tag to indicate that an item can be broken.
    /// </summary>
    public class Breakable : TagBase { }

    #region - Creature Tags -
    /// <summary>
    /// Tag to mark an object as a creature
    /// </summary>
    public class Creature : TagBase { }

    /// <summary>
    /// Tag to mark the player entity
    /// </summary>
    public class PlayerTag : TagBase { }

    /// <summary>
    /// Tag to mark the Family of a creature: Humanoid, Demihuman, Monstrous.
    /// </summary>
    public class Family : TagBase
    {
        public Family(string family)
        {
            Value = family;
        }
    }

    /// <summary>
    /// Tag to mark the Base Race of a creature e.g. Human, Demon, Naga.
    /// </summary>
    public class Genus : TagBase
    {
        public Genus(string genus)
        {
            Value = genus;
        }
    }

    /// <summary>
    /// Tag to indicate a creature's primary atttack method when unarmed
    /// </summary>
    public class PrimaryLimb : TagBase 
    {
        public PrimaryLimb(string limb)
        {
            Value = limb; //e.g. Hand, Feet, Body, Head
        }
    }
    #endregion
}