using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource audioSource;
    public int maxHealth = 10;
    public int currentHealth = 10;
    private Rigidbody2D rb;
    private Animator animator;
    public float speed,JumpForce, movHorizontal,attackRange = 1.5f;
    bool isJump, getHit, isAttack;
    public LayerMask Groundlayer, enemyLayer;
    public bool CanJump;
    private string currentState;
    public bool count;
    public AudioClip hurt, Slash;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb =GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
    }
    
    void Movement()
    {
   
        int MovementHorizontal = Input.GetAxis("Horizontal") > 0.1f ? 1 :
                                  Input.GetAxis("Horizontal") < -0.1f ? -1 : 0;
        movHorizontal = MovementHorizontal;

        switch (MovementHorizontal)
        {
            case 0: State( "Idle"); break;
            case 1: if (!isAttack) transform.localScale = new Vector2(1, 1); State( "Run"); break;
            case -1: if(!isAttack)transform.localScale = new Vector2(-1, 1); State("Run"); break;
        }
       if(!isAttack) rb.velocity = new Vector2(movHorizontal * speed, rb.velocity.y);
    }
    // Update is called once per frame
    void Update()
    {
        Jump();
        Movement();
        Attack();
    }
    void State(string state)
    {
        if (isAttack)
        {
            if(count)animator.Play("Attack1");
            else  animator.Play("Attack2");// Saldırıyorsa her şeyin önüne geç
            return;
        }

        if (CanJump)
        {
            animator.Play(state); // Idle veya Run
        }
        else
        {
            animator.Play("Jump");
        }
    }
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isAttack)
        {
            isAttack = true;
            count = !count;
            audioSource.PlayOneShot(Slash);
            // Süresi bitince geri normale dön
            Invoke(nameof(EndAttack), 0.5f); // animasyon süresine göre ayarla
        }
    }




    public void AttackCheck()
    {
        // Karakterin bakış yönüne göre ışın yönü
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(-0.1f, 0.4f, 0), direction, 0.6f, enemyLayer);

        if (hit.collider != null)
        {
            Debug.Log("Düşmana ışınla vuruldu: " + hit.collider.name);
            hit.collider.GetComponent<Enemy>()?.TakeDamage(1);
        }

        // Debug çizgisi
        Debug.DrawRay(transform.position - new Vector3(0, 0.4f, 0), direction * attackRange, Color.red, 0.2f);
    }
    void Die()
    {
        Debug.Log(gameObject.name + " öldü.");
        Destroy(gameObject); // Düşmanı sahneden sil
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " hasar aldı! Kalan can: " + currentHealth);
        audioSource.PlayOneShot(hurt);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void EndAttack()
    {
        isAttack = false;
    }
    void Jump()
    {
        GroundChecker();
        if (Input.GetKeyDown(KeyCode.Space) && CanJump)
        {
            isJump = true;
            CanJump = false;
            
            rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
        }

        // Havadayken yere inip inmediğimizi kontrol et
       
    }
   
    // animation eventle attack1 ve attack2 yao bakalım
   
    public Transform groundCheckPoint; // karakterin ayak hizasına küçük boş GameObject

    public float groundCheckDistance = 0.1f;

    public void GroundChecker()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, Groundlayer);
        CanJump = hit.collider != null;
        isJump = !CanJump;

        Debug.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckDistance, Color.red);
    }

}
