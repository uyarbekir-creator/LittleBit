using UnityEngine;
using System.Collections.Generic;

public class BoosterManager : MonoBehaviour
{
    [Header("Prefab Listesi")]
    public List<GameObject> boosterPrefabs; // Yıldırım, Yay ve Radyasyon prefablarını buraya sürükle

    [Header("Spawn Ayarları")]
    public int totalBoosterCount = 12; // Sahada her zaman kaç tane obje olacak?
    public Vector3 spawnAreaCenter; // Mutfak tezgahının merkezi
    public Vector3 spawnAreaSize; // Ne kadarlık bir alana yayılsınlar? (Genişlik, Yükseklik, Derinlik)

    private List<GameObject> activeBoosters = new List<GameObject>();

    void Start()
    {
        // Oyun başında 12 tane rastgele booster oluştur
        for (int i = 0; i < totalBoosterCount; i++)
        {
            SpawnRandomBooster();
        }
    }

    public void SpawnRandomBooster()
    {
        // Listeden rastgele bir prefab seç (Yıldırım, Yay veya Radyasyon)
        int randomIndex = Random.Range(0, boosterPrefabs.Count);
        
        // Belirlediğin alan içinde rastgele bir pozisyon oluştur
        Vector3 randomPos = new Vector3(
            Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
            spawnAreaCenter.y, // Yüksekliği sabit tutabilirsin ya da rastgele yapabilirsin
            Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
        );

        // Objeyi oluştur
        GameObject newBooster = Instantiate(boosterPrefabs[randomIndex], randomPos, Quaternion.identity);
        activeBoosters.Add(newBooster);
    }

    // Bir booster toplandığında bu fonksiyonu çağıracağız
    public void OnBoosterCollected(GameObject collectedBooster)
    {
        activeBoosters.Remove(collectedBooster);
        Destroy(collectedBooster);
        SpawnRandomBooster(); // Biri gidince hemen yenisini çıkar
    }
}