using UnityEngine;
using UnityEngine.AI;

public class CatController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _defaultSpeed = 5f;
    [SerializeField] private float _chaseSpeed = 7f;

    private NavMeshAgent _catAgent;

    private bool _isWaiting;

    private void Awake()
    {
        _catAgent = GetComponent<NavMeshAgent>();
    }

    public void SetPatrolMovement()
    {
        
    }
}
