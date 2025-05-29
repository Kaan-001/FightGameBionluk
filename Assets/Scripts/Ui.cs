using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ui : MonoBehaviour
{
    [Header("UI Images")]
    public Image playerHealthBar;
    public Image enemyHealthBar;
    [Header("Health Values")]
    public float maxHealth = 10f;
    public float maxHealthe = 20f;
    public GameObject Player, Enemy;
    void Update()
    {
        UpdateHealthBars();
    }
    public void Goback() 
    {
        SceneManager.LoadScene(0);
    }
    void UpdateHealthBars()
    {
        // Clamp ile aþýrý deðerleri engelle



        // Fill amount hesaplama (0-1 arasý deðer)
        if (Player != null) playerHealthBar.fillAmount = Player.GetComponent<Player>().currentHealth / maxHealth;
        if(Enemy!=null)  enemyHealthBar.fillAmount = Enemy.GetComponent<Enemy>().currentHealth / maxHealthe;
        else { enemyHealthBar.fillAmount = 0; }
    }

    // Saðlýk setleme fonksiyonlarý
   
}
