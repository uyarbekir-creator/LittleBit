using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private void Awake()
    {
        // Sahnede baska bir EventSystem var mı kontrol et
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (eventSystems.Length > 1)
        {
            // Eger bu obje 'ana' olan degilse kendini yok et
            foreach (EventSystem es in eventSystems)
            {
                if (es.gameObject != this.gameObject)
                {
                    Destroy(this.gameObject);
                    return;
                }
            }
        }

        // Sahne degisimlerinde hayatta kalmasını sagla
        DontDestroyOnLoad(gameObject);
    }
}