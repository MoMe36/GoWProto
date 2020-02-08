using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class CharacterCombat : MonoBehaviour
{
    
    public static Hitbox[] GetAllHitboxes(GameObject highest_parent){

        Hitbox[] result = highest_parent.GetComponentsInChildren<Hitbox>(); 
        return result; 
    }

    public static Dictionary<string, Hitbox> GetHBDict(GameObject highest_parent){
        
        Hitbox[] hbs = GetAllHitboxes(highest_parent); 
        Dictionary<string, Hitbox> dico = new Dictionary<string, Hitbox>(); 
        foreach(Hitbox hb in hbs){
            dico.Add(hb.BoxName, hb); 
        }
        return dico; 
    }


}

}
 