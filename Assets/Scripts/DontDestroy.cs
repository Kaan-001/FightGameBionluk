using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    private static DontDestroy instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // Zaten bir tane varsa, yenisini yok et
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject); // Ýlk oluþturulan objeyi koru
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
