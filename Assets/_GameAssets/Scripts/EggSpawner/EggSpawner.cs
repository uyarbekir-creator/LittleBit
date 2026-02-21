using System.Collections.Generic;
using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    // 5 farklı yumurtanı buraya sürükleyeceğiz
    public GameObject[] yumurtaPrefablar; 

    void Start()
    {
        // 1. Sahnedeki tüm yuvaları (çocuk objeleri) bul ve listeye ekle
        List<Transform> yuvalar = new List<Transform>();
        foreach (Transform cocuk in transform)
        {
            yuvalar.Add(cocuk);
        }

        // 2. 5 farklı yumurtanın her biri için bir yuva seç ve oraya koy
        for (int i = 0; i < yumurtaPrefablar.Length; i++)
        {
            // Eğer yuva kalmadıysa dur (Hata almamak için)
            if (yuvalar.Count == 0) break;

            // Rastgele bir yuva seç
            int rastgeleIndex = Random.Range(0, yuvalar.Count);
            Transform secilenYuva = yuvalar[rastgeleIndex];

            // Yumurtayı o yuvaya ışınla (Oluştur)
            Instantiate(yumurtaPrefablar[i], secilenYuva.position, Quaternion.identity);

            // O yuvayı listeden sil ki ikinci bir yumurta aynı yere gelmesin!
            yuvalar.RemoveAt(rastgeleIndex);
        }
    }
}