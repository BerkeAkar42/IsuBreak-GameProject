using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    - NPC Can Scripti -
 
 
 */


public class NPCHealth : MonoBehaviour
{
    NPCMovement move1;
    PoliceMove move2;
    PoliceKuleMove move3;

    public float health = 100f;
    private Transform playerTransform; // Oyuncu Transform'u tutmak için

    private bool isDead = false;

    [Header("Silah Düþürme")]
    public GameObject silahObjesi; // Polis elindeki silah objesi (Artýk sadece yok etmek için)
    public GameObject droppedGunPrefab; // Yere düþecek olan Prefab

    // ... Start metodu ...

    void Start()
    {
        // NPC öldü, hareket edemez. Eðer NPC hareket scripti varsa kapat
        move1 = GetComponent<NPCMovement>(); // NPC’nin hareket scripti
        move2 = GetComponent<PoliceMove>(); // Polis NPC'nin hareket scripti
        move3 = GetComponent<PoliceKuleMove>(); // Polis Kule NPC'nin hareket scripti

        // Sahnedeki "Player" tag'li nesneyi bul (Player objeniz "Player" tag'ine sahip olmalý)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("HATA: Sahne'de 'Player' tag'li nesne bulunamadý. NPC takip sistemi devre dýþý.");
        }
    }


    public void TakeDamage(float amount)
    {
        // 1. Zaten ölü ise hiçbir þey yapma
        if (isDead) return;

        health -= amount;
        Debug.Log(gameObject.name + " NPC caný: " + health);

        if (health <= 0)
        {
            isDead = true;
            Die();
            //Debug.Log("Hasar Verildi!");
            health = 0;
        }
        else // Eðer NPC ölmediyse ve hasar aldýysa
        {
            NPCMovement move = GetComponent<NPCMovement>();
            if (move != null && playerTransform != null)
            {
                // NPC'yi takip moduna sok
                move.StartAggro(playerTransform);
            }
        }
    }

    //NPC'lerin ölüm kodu.
    void Die()
    {

        if (move1 != null) //NPC Prisoner Scripti
        {
            move1.enabled = false;

            // Animator'u al ve ölü animasyonunu oynat
            Animator anim1 = move1.GetComponent<Animator>();

            if (anim1 != null)
            {
                anim1.SetBool("IsDying", true);
            }

        }
        else if (move2 != null) //NPC Police Scripti
        {
            move2.enabled = false;

            // Animator'u al ve ölü animasyonunu oynat
            Animator anim2 = move2.GetComponent<Animator>();

            if (anim2 != null)
            {
                anim2.SetBool("IsDying", true);
            }
        }
        else if (move3 != null) //NPC Police Kule Scripti
        {
            move3.enabled = false;

            // Animator'u al ve ölü animasyonunu oynat
            Animator anim3 = move3.GetComponent<Animator>();

            if (anim3 != null)
            {
                anim3.SetBool("IsDying", true);
            }
        }

        // Silahý düþür
        if (droppedGunPrefab != null)
        {
            // 1. Düþen silahý, polisin elinin pozisyonunda oluþtur
            GameObject dropped = Instantiate(droppedGunPrefab, silahObjesi.transform.position, silahObjesi.transform.rotation);

            // 2. Silahý yere düþürmek için rastgele bir kuvvet uygula (Rigidbody zaten Prefab'da var)
            Rigidbody rb = dropped.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * 2f + Vector3.up * 1f, ForceMode.Impulse);
            }
            else
            {
                // Bu hata, Prefab'a Rigidbody eklenmediyse tetiklenir
                Debug.LogError("HATA: Dropped Gun Prefab'ýna Rigidbody bileþeni eklenmemiþ!");
            }
        }

        // Polis elindeki objeyi (animasyon hiyerarþisinin parçasý) yok et
        if (silahObjesi != null)
        {
            silahObjesi.SetActive(false);
        }

        // NPC'nin kendisini 5 saniye sonra yok et (burasý zaten doðru)
        Destroy(gameObject, 10f);

    }
}
