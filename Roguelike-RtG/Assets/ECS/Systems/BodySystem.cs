using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Is it worth considering merging the CorpseManager into this system?

namespace JS.ECS
{
    public class BodySystem : MonoBehaviour
    {
        private static BodySystem instance;

        private List<Body> bodies;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            bodies = new List<Body>();
        }

        public static void Register(Body newBody)
        {
            if (!instance.bodies.Contains(newBody))
                instance.bodies.Add(newBody);
        }

        public static void Unregister(Body oldBody)
        {
            instance.bodies.Remove(oldBody);
        }

        private void OnDismemberment(Body body)
        {
            int count = 0;
            for (int i = 0; i < body.BodyParts.Count; i++)
            {
                if (!body.BodyParts[i].IsAppendage) continue;
            }
        }

        private bool FindDismemberableBodyPart(Body body)
        {
            bool[] validParts = new bool[body.BodyParts.Count];
            for (int i = 0; i < body.BodyParts.Count; i++)
            {
                if (body.BodyParts[i].IsAppendage) continue;
                if (body.BodyParts[i].IsVital) continue;
                validParts[i] = true;
            }


            return false;
        }
        private int FindDismemberablePart(Body body)
        {
            int index = Random.Range(0, body.BodyParts.Count);
            //find an appendage which is not the head
            if (body.BodyParts[index].IsAppendage && !body.BodyParts[index].IsVital) return index;
            else return FindDismemberablePart(body); //so this could result in an infinite loop
            //if the creature has been dismembered so much that they're only a body and a head

            //so this ain't it
            //just loop through all body parts a maximum of once, excluding previously checked ones, 
            //and if I still can't find one, then return false
        }
    }
}