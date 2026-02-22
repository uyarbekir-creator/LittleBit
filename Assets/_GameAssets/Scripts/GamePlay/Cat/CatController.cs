using UnityEngine;

public class CatController : MonoBehaviour
{
    private NavMeshAgent _catAgent;

    private void Awake()
    {
        _catAgent = GetComponent<NavMeshAgent>();
    }

    public void SetPatrolMovement()
    {
        
    }
}
