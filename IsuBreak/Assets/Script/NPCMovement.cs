using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    - NPC Hareket Scripti -
 
 
 */

[RequireComponent(typeof(CharacterController))]
public class NPCMovement : MonoBehaviour
{
    Animator animator;
    CharacterController controller;

    float walkSpeed = 8f;
    public float turnSpeed = 120f;
    public float gravity = -9.81f;

    Vector3 velocity;
    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    float actionTimer = 0f;
    float actionDuration = 0f;
    int currentAction = 0; // 0 = Idle, 1 = WalkForward, 2 = TurnLeft, 3 = TurnRight


    [Header("Player Takip Ayarları")]
    public Transform playerTarget;
    CanSistemi playerHealth;

    float followSpeed = 8f;       // Takip hızı
    float attackRange = 5f;       // Saldırıya geçme mesafesi
    float attackCooldown = 1.5f;  // Saldırılar arası bekleme süresi
    float attackDamage = 5f;     // Saldırı hasarı

    private bool isAggro = false;        // NPC'nin sinirlenip oyuncuyu takip edip etmediği
    private float nextAttackTime = 0f;


    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        playerHealth = GameObject.FindGameObjectWithTag("CanSistemi").GetComponent<CanSistemi>();

        // İlk davranışı başlat
        ChooseNewAction();
    }

    void Update()
    {
        // Yere temas kontrolü
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (isAggro && playerTarget != null)
        {
            // Oyuncuyu takip ve saldırı
            HandleAggroMovement();
        }
        else
        {
            // Eski rastgele hareket mantığı
            actionTimer += Time.deltaTime;

            if (actionTimer >= actionDuration)
                ChooseNewAction();

            // Mevcut aksiyona göre hareket et
            switch (currentAction)
            {
                case 0:
                    Idle();
                    break;
                case 1:
                    WalkForward();
                    break;
                case 2:
                    TurnLeft();
                    break;
                case 3:
                    TurnRight();
                    break;
            }
        }

        // Yer çekimi uygula
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }




    public void StartAggro(Transform target)
    {
        //Debug.Log("1");
        isAggro = true;
        playerTarget = target;
        // NPC hasar aldığında hareket scripti kapanmasın diye tekrar açma kontrolü
        if (!enabled)
        {
            enabled = true;
        }
        // Rastgele hareket animasyonlarını kapat
        animator.SetBool("IsWalk", false);
        animator.SetBool("IsRun", false);
        animator.SetBool("IsCrouched", false);

        // NPC'yi boks duruşuna geçir
        animator.SetBool("IsBoxingIdle", true);
    }

    void HandleAggroMovement()
    {
        //Debug.Log("2");

        if (playerTarget == null) return;

        // 1. Oyuncuya Yönelme
        Vector3 directionToTarget = playerTarget.position - transform.position;
        directionToTarget.y = 0; // Y ekseninde dönmeyi engelle
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        float distance = directionToTarget.magnitude;

        //Debug.Log("distance" + distance);
        //Debug.Log("attackRange" + attackRange);

        // NPC'yi boks duruşuna geçir
        animator.SetBool("IsBoxingIdle", true);
        if (distance <= attackRange)
        {
            // 2. Saldırı Menzilinde: Saldır
            AttackPlayer();
            animator.SetBool("IsBoxWalkForward", false); // Yürümeyi durdur
        }
        else
        {
            // 3. Saldırı Menzili Dışında: Takip Et (Box Yürüme)
            Vector3 moveDirection = transform.forward;
            controller.Move(moveDirection * followSpeed * Time.deltaTime);

            animator.SetBool("IsBoxWalkForward", true); // Box yürüme animasyonu
            animator.SetBool("IsBoxingIdle", true); // Boks duruşu devam etmeli
        }
    }

    void AttackPlayer()
    {
        Debug.Log("3");
        // Yürümeyi durdur.
        animator.SetBool("IsBoxWalkForward", false);

        if (Time.time >= nextAttackTime)
        {
            // Saldırı animasyonunu başlat
            animator.SetBool("IsBoxing", true);

            // Hasar verme: Oyuncunun CanSistemi scriptine erişim ve hasar verme.
            if (playerTarget != null)
            {

                if (playerHealth != null)
                {
                    // Oyuncuya hasar ver (CanSistemi.cs'teki TakeDamage metodu çağrılıyor)
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log(gameObject.name + " Player'a yumruk attı. Hasar: " + attackDamage);
                }
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }






    void ChooseNewAction()
    {
        // Aggro modunda değilse, eski animasyonları kapat
        animator.SetBool("IsBoxing", false);
        animator.SetBool("IsBoxWalkForward", false);

        actionTimer = 0f;
        actionDuration = Random.Range(1f, 3f); // Her davranış 2-5 sn arası sürsün
        currentAction = Random.Range(0, 4); // 0-3 arası rastgele bir davranış seç
    }

    void Idle()
    {
        animator.SetBool("IsWalk", false);
        animator.SetBool("IsRun", false);
        animator.SetBool("IsCrouched", false);
    }

    void WalkForward()
    {
        animator.SetBool("IsWalk", true);
        Vector3 move = transform.forward * walkSpeed * Time.deltaTime;
        controller.Move(move);
    }

    void TurnLeft()
    {
        animator.SetBool("IsWalk", false);
        transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
    }

    void TurnRight()
    {
        animator.SetBool("IsWalk", false);
        transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
    }
}
