using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JS.ECS;

public class InventoryDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetAllItems(Entity entity)
    {
        foreach(var item in entity.GetComponent<Inventory>().Contents)
        {
            
        }
    }
}
