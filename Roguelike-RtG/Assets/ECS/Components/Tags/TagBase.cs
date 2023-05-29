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
    /// Tag to mark an object as being immune to damage.
    /// </summary>
    public class NoDamage : TagBase
    {
    }

    /// <summary>
    /// Tag to mark an object as being immune to effects.
    /// </summary>
    public class NoEffect : TagBase
    {
    }

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

    public class Gender : TagBase
    {
        public Gender(string gender)
        {
            Value = gender;
        }
    }

    public class Item : TagBase
    {
    }
}