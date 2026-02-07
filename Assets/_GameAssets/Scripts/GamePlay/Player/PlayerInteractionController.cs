using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
      if(other.CompareTag(Consts.WheatTypes.GOLD_WHEAT))
      {Debug.Log("GOLD WHEAT");}

      if(other.CompareTag(Consts.WheatTypes.HOLY_WHEAT))
      {Debug.Log("HOLY WHEAT");}

      if(other.CompareTag(Consts.WheatTypes.ROTTEN_WHEAT))
      {Debug.Log("ROTTEN WHEAT");}
    }
    
}
