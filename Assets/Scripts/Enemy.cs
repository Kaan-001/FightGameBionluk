using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth=10;

    public Transform CheckerPoint;
    public LayerMask PlayerMask;
    public GameObject PlayerObj;
    public float DistanceChecker;
    public Animator animator;
    public Rigidbody2D rb;
    public AudioSource audioSource;
    public AudioClip hurt, Slash;
    private bool isExecutingStrategy = false;
    private bool hasHitInThisAttack = false; // ✅ hasar sadece 1 kere vurulsun diye

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(AI_Loop());
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
       if(PlayerObj!=null&&!isExecutingStrategy) FaceEnemy(PlayerObj.transform);
    }

    void FaceEnemy(Transform enemy)
    {
        if (enemy == null) return;

        if (enemy.position.x > transform.position.x)
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
    }

    IEnumerator AI_Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (!isExecutingStrategy)
                EnemyChooseStrategy();
        }
    }

    public void EnemyChooseStrategy()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(CheckerPoint.position, direction, DistanceChecker, PlayerMask);

        Debug.DrawRay(CheckerPoint.position, direction * DistanceChecker, hit.collider != null ? Color.green : Color.red);

        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.x - transform.position.x);
            Debug.Log("🎯 Hit var! Mesafe: " + distance);

            if (distance < 0.5f)
                StartCoroutine(AttackRoutine());
            else if (distance < 6f)
                StartCoroutine(ApproachRoutine());
            else
                StartCoroutine(ObserveRoutine());

            isExecutingStrategy = true;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.Play("Idle");
            Debug.Log("❌ Hit yok. Beklemede...");
        }
    }

    IEnumerator EvadeFromPlayer()
    {
        Debug.Log(gameObject.name + " kaçış moduna geçti!");

        // Geçici inaktif hale getir
        isExecutingStrategy = true;
       
        // Player'ın yönünü tespit et (örnek: player tag’li objeye göre)
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        float playerPosX = player.position.x;
        float enemyPosX = transform.position.x;

        // Ters yöne bak
        if (playerPosX > enemyPosX)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);

        // Kaçış hareketi
        float fleeDir = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = new Vector2(fleeDir * 4f, rb.velocity.y); // daha hızlı geri çekilsin

        animator.Play("Run");

        // 🕒 1–3 saniye arası kaçış
        yield return new WaitForSeconds(1);

        Debug.Log(gameObject.name + " kaçış bitti, normale döndü.");
        isExecutingStrategy = false;
    }

    void TryAttackHit()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.6f, 0), direction, 0.6f, PlayerMask);
      
        if (hit.collider != null)
        {
            Debug.Log("🎯 Sadece 1 kez vuruldu: " + hit.collider.name);
            hit.collider.GetComponent<Player>()?.TakeDamage(1);
            hasHitInThisAttack = true; // ✅ tekrar vurmasın
        }
        
        Debug.DrawRay(transform.position - new Vector3(0, 0.3f, 0), direction * 0.6f, Color.red, 0.2f);
    }
    IEnumerator AttackRoutine()
    {
        transform.GetComponent<SpriteRenderer>().flipX = false;
        Debug.Log("💥 Yakın mesafe: Saldırı başlatıldı");
        int random = Random.Range(0,2);
        if (random == 0)
        {
            audioSource.PlayOneShot(Slash);
            animator.Play("Attack");
            rb.velocity = new Vector2(0, rb.velocity.y);
            hasHitInThisAttack = false;
            // ✅ bu saldırı için sadece 1 kez vursun
        }

        float duration = 1f;
            float timer = 0f;

            while (timer < duration)
            {



                timer += Time.deltaTime;
                yield return null;
            }
            animator.Play("Idle");
            isExecutingStrategy = false;
      
    }
    IEnumerator ApproachRoutine()
    {
        Debug.Log("🏃 Orta mesafe: Yaklaşılıyor");
        transform.GetComponent<SpriteRenderer>().flipX = false;
        // 🦘 Zıplama: sadece coroutine başında bir defa
        int random = Random.Range(0, 6);
        if (random == 0)
        {
            rb.AddForce(new Vector2(0,150f)); // zıplama kuvveti
            animator.Play("Jump");
            Debug.Log("🦘 Başlangıçta zıpladı!");
        }
       
        

        yield return new WaitForSeconds(0.1f); // kısa gecikme ile zıplamaya zaman tanı

        // Yürümeye geç
        animator.Play("Run");

        float moveDir = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = new Vector2(moveDir * 2.5f, rb.velocity.y);

        yield return new WaitForSeconds(0.1f); // toplamda 0.4s olacak şekilde
        isExecutingStrategy = false;
    }

    IEnumerator ObserveRoutine()
    {
        Debug.Log("👁️ Uzak mesafe: Gözlemleme");

        int random = Random.Range(0, 8);
        if (random == 0)
        {
            animator.Play("Idle");
            transform.GetComponent<SpriteRenderer>().flipX = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if(random <5&&random>0)
        {
           


            float moveDir = transform.localScale.x > 0 ? 1 : -1;
            rb.velocity = new Vector2(moveDir * 2f, rb.velocity.y);
            animator.Play("Run");
            transform.GetComponent<SpriteRenderer>().flipX = false;
        }
        else 
        {

            transform.GetComponent<SpriteRenderer>().flipX = true;
            float moveDir = transform.localScale.x > 0 ? 1 : -1;
            rb.velocity = new Vector2(-moveDir *2f, rb.velocity.y);
            animator.Play("Run");
        }

            yield return new WaitForSeconds(0.2f);
        isExecutingStrategy = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " hasar aldı! Kalan can: " + currentHealth);
        audioSource.PlayOneShot(hurt);
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // 🎲 %40 ihtimalle kaçış davranışı başlat
        if (Random.value < 0.2f)
        {
            StartCoroutine(EvadeFromPlayer());
        }
    }


    void Die()
    {
        Debug.Log(gameObject.name + " öldü.");
        Destroy(gameObject);
    }
}
