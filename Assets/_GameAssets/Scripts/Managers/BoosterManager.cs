using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // NavMesh kontrolü için bu kütüphaneyi ekledik

public class BoosterManager : MonoBehaviour
{
    [Header("Gerekli Referanslar")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerStateUI _playerStateUI;

    [Header("Yumurta Ayarları")]
    [Tooltip("Elindeki 5 farklı yumurta prefabını buraya sürükle")]
    public List<GameObject> uniqueEggPrefabs; 

    [Header("Booster Ayarları")]
    public List<GameObject> boosterPrefabs; 
    public int totalBoosterCount = 12; 
    public float minDistanceBetweenBoosters = 5f;

    [Header("Alan ve Yükseklik")]
    public Vector3 spawnAreaCenter; 
    public Vector3 spawnAreaSize; 
    public float fixedSpawnY = 1.05f;

    private List<GameObject> _activeObjects = new List<GameObject>();

    private void Start()
    {
        // 1. Önce 5 farklı yumurtayı birer kez oluştur
        SpawnUniqueEggs();

        // 2. Kalan boşluklara booster'ları doldur
        for (int i = 0; i < totalBoosterCount; i++)
        {
            SpawnRandomBooster();
        }
    }

    private void SpawnUniqueEggs()
    {
        foreach (GameObject eggPrefab in uniqueEggPrefabs)
        {
            Vector3 pos = GetValidRandomPosition();
            GameObject newEgg = Instantiate(eggPrefab, pos, Quaternion.identity);
            _activeObjects.Add(newEgg);
            
            // Eğer yumurtaların da tag'i varsa buraya ekleyebilirsin
            newEgg.tag = "Egg"; 
        }
    }

    public void SpawnRandomBooster()
    {
        if (boosterPrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, boosterPrefabs.Count);
        Vector3 pos = GetValidRandomPosition();

        GameObject newBooster = Instantiate(boosterPrefabs[randomIndex], pos, Quaternion.identity);
        _activeObjects.Add(newBooster);
        
        InjectDependencies(newBooster);
    }

    private Vector3 GetValidRandomPosition()
    {
        Vector3 randomPos = Vector3.zero;
        int attempts = 0;
        bool found = false;

        // Deneme sayısını 50 yapıyoruz ki NavMesh üzerinde yer bulma şansı artsın
        while (!found && attempts < 50)
        {
            attempts++;
            float rx = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2f, spawnAreaCenter.x + spawnAreaSize.x / 2f);
            float rz = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2f, spawnAreaCenter.z + spawnAreaSize.z / 2f);
            
            Vector3 candidatePos = new Vector3(rx, fixedSpawnY, rz);
            NavMeshHit hit;

            // NavMesh.SamplePosition: Seçilen nokta duvar içindeyse en yakın boş alana (1.0f mesafe içinde) çeker
            if (NavMesh.SamplePosition(candidatePos, out hit, 1.0f, NavMesh.AllAreas))
            {
                randomPos = hit.position;
                randomPos.y = fixedSpawnY; // Yüksekliği senin orijinal ayarına sabitliyoruz

                found = true;
                foreach (var obj in _activeObjects)
                {
                    if (obj != null && Vector3.Distance(randomPos, obj.transform.position) < minDistanceBetweenBoosters)
                    {
                        found = false;
                        break;
                    }
                }
            }
        }
        return randomPos;
    }

    public void OnBoosterCollected(GameObject collectedObj)
    {
        // Listeden temizle
        if (_activeObjects.Contains(collectedObj))
        {
            _activeObjects.Remove(collectedObj);
        }

        // Sadece Booster ise yenisini çıkar (Yumurta ise sessizce yok olsun, senin sayaç devralır)
        bool isBooster = collectedObj.GetComponent<ICollectible>() != null && !collectedObj.CompareTag("Egg");
        
        if (isBooster)
        {
            SpawnRandomBooster();
        }
    }

    private void InjectDependencies(GameObject obj)
    {
        if (obj.TryGetComponent(out GoldWheatCollectible gold)) gold.Initialize(_playerController, _playerStateUI);
        else if (obj.TryGetComponent(out HolyWheatCollectible holy)) holy.Initialize(_playerController, _playerStateUI);
        else if (obj.TryGetComponent(out RottenWheatCollectible rotten)) rotten.Initialize(_playerController, _playerStateUI);
    }

    private void OnDrawGizmosSelected()
    {
        // Gizmos rengini hafif şeffaf yaparak alanı daha iyi görmeni sağlar
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);
    }
}