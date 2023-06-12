using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// A component that represents anatomical layout of a crature and holds reference to all of its body part entities 
    /// </summary>
    public class Body : ComponentBase
    {
        public Body() { }

        public string Anatomy = "Humanoid";

        public List<BodyPart> BodyParts { get; set; }

        public override void OnRegistered() => CreateAnatomy();

        private void CreateAnatomy()
        {
            //UnityEngine.Debug.Log(Anatomy);
            BodyParts = new List<BodyPart>();

            var parts = BodyFactory.GetNewBody(Anatomy);
            BodyParts.AddRange(parts);

            for (int i = 0; i < BodyParts.Count; i++)
            {
                //UnityEngine.Debug.Log(BodyParts[i].Name);

                /*if (BodyParts[i].DefaultBehavior != null)
                {
                    UnityEngine.Debug.Log(BodyParts[i].Name + " wields " + BodyParts[i].DefaultBehavior.Name);
                }*/
            }
        }

        public override void OnEvent(Event newEvent)
        {
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyParts[i].OnEvent(newEvent);
            }
        }
    }

    /*
     * Ok so Equipment slots are going to be a Component
     * The question is.... is the body a separate entity
     * or do I just add that component
     * 
     * so do I have a single generic equipment slot which cna hold 
     */
}

/* So what I need to do is figure out exactly what it is that I want to accomplish, and then from there I can decide how I want to accomplish it
 * I want body parts to be tracked separately, so that they can be added and removed at will 
 * Each body part should hold reference to the items which are worn on it, held by it, can be worn/held by it
 * However, this is not a simple 1 : 1 system where each body part grants one slot for items. Some need to be able to have none while others have multiple. 
 * 
 * 
 */

public enum Laterality
{
    None, 
    Left,
    Right,
}

public enum BodySlot
{
    Head,   //Helms, Hoods, Hats, Caps, Crowns, Masks
    Eyes,   //Glasses, Blindfolds, Goggles, 
    Neck,   //Amulets, Necklaces, Medallions
    Body,   //Armor, Robes, 
    Back,   //Capes, Cloaks, Mantles, ?Wings
    Arm,   //Bracers, Bucklers, Manacles, Shackles, Bracelets (if wrists)
    Hand,  //Gauntlets, Gloves
    Ring,   //Rings
    Belt,   //Belts, Girdles, Bandoliers
    Feet,   //Boots, Sandles, Shoes
}