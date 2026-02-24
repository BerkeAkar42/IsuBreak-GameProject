using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PoliceKuleMove : MonoBehaviour
{
    [Header("Referanslar")]
    private Animator animator;
    private CharacterController controller;
    private Transform player;
    private PoliceGunSystems gunSystem;
    CanSistemi playerHealth;

    [Header("Polis Ayarlarý")]
    float moveSpeed = 20f;
    float backSpeed = 7f;
    float stopDistance = 200f; // Bu mesafede ateþ etmeye baþlar
    float safeDistance = 5f;  // Bu mesafenin altýna inerse geri yürür
    float rotationSpeed = 5f;
    public LayerMask engelKatmani;   // Görüþ engelleri için layer mask

    [Header("Ateþ Etme Süresi")]
    float fireRate = 2f;
    private float nextFireTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerHealth = GameObject.FindGameObjectWithTag("CanSistemi").GetComponent<CanSistemi>();


        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player bulunamadý! Lütfen Player objesine 'Player' tag'i ekle.");
        }

        gunSystem = GetComponentInChildren<PoliceGunSystems>();
        if (gunSystem == null)
        {
            Debug.LogError("PoliceGunSystems scripti bulunamadý! Lütfen Pistol_B objesinde olduðundan emin olun.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        // Player’a dön
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Player ile arada duvar/engel var mý kontrol et
        bool engelVar = Physics.Raycast(transform.position + Vector3.up * 1.5f, player.position - transform.position, distance, engelKatmani);

        if (engelVar)
        {
            // Görüþ engellendiyse dur
            animator.SetBool("IsGunRun", false);
            animator.SetBool("IsGunIdle", true);
            animator.SetBool("IsGunWalkBack", false);
            return;
        }

        // --- Görüþ açýkken normal davranýþlar ---
        if (distance > stopDistance)
        {
            // Player’a doðru koþ
            animator.SetBool("IsGunRun", true);
            animator.SetBool("IsGunIdle", false);
            animator.SetBool("IsGunWalkBack", false);

            controller.SimpleMove(direction * moveSpeed);
        }
        else if (distance <= stopDistance && distance > safeDistance)
        {
            // Dur ve ateþ et
            animator.SetBool("IsGunRun", false);
            animator.SetBool("IsGunIdle", true);
            animator.SetBool("IsGunWalkBack", false);

            if (Time.time >= nextFireTime && playerHealth.can > 0)
            {
                nextFireTime = Time.time + fireRate;
                if (gunSystem != null)
                    gunSystem.pistolAtes();
            }
        }
        else if (distance <= safeDistance)
        {
            // Çok yaklaþtýysa geri geri yürü
            animator.SetBool("IsGunRun", false);
            animator.SetBool("IsGunIdle", false);
            animator.SetBool("IsGunWalkBack", true);

            Vector3 backDirection = -direction;
            controller.SimpleMove(backDirection * backSpeed);
        }
    }

    // Görüþ hattýný sahnede görmek için debug çizgisi
    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, player.position);
    }
}
