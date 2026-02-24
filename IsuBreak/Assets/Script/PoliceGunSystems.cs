using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceGunSystems : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Gun Systems")]
    RaycastHit hit; // Görünmez ýþýn
    public GameObject rayPoint; // Silahýn ucu
    public Camera playerCamera; // Oyuncunun kamerasý (Inspector’dan ata)
    public bool canFire = true;
    public float gunTimer;
    public float gunCoolDown = 0.08f;

    public ParticleSystem muzzleFlash;
    AudioSource sesKaynak;
    public AudioClip fireSound;

    float range = 500f; // Menzil
    float hasar = 3f;

    public Transform playerTarget; // Player referansý
    CanSistemi playerHealth;

    void Start()
    {
        sesKaynak = GetComponent<AudioSource>();
    }

    public void pistolAtes()
    {
        // Hedefin dünya üzerindeki pozisyonunu al
        Vector3 hedefPozisyonu = playerTarget.position + Vector3.up * 1.5f; // biraz yukarýdan (gövde hizasý)

        // Silahýn ucundan hedefe doðru yön hesapla
        Vector3 atisYon = (hedefPozisyonu - rayPoint.transform.position).normalized;

        // Raycast’i gönder
        if (Physics.Raycast(rayPoint.transform.position, atisYon, out hit, range))
        {
            // Görsel efekt ve ses
            if (muzzleFlash != null)
                muzzleFlash.Play();

            if (sesKaynak != null)
                sesKaynak.PlayOneShot(fireSound);

            Debug.DrawRay(rayPoint.transform.position, atisYon * range, Color.red, 0.5f);
            Debug.Log("AI vurdu: " + hit.transform.name);

            // NPC Prisoner'a hasar ver
            if (hit.transform.CompareTag("Prisoner"))
            {
                NPCHealth npc = hit.transform.GetComponent<NPCHealth>();
                if (npc != null)
                    npc.TakeDamage(hasar);
            }

            // NPC Police'ye hasar ver
            if (hit.transform.CompareTag("Police"))
            {
                NPCHealth npc = hit.transform.GetComponent<NPCHealth>();
                if (npc != null)
                    npc.TakeDamage(hasar);
            }

            // Player'a hasar ver
            if (hit.transform.CompareTag("Player"))
            {
                playerHealth = GameObject.FindGameObjectWithTag("CanSistemi").GetComponent<CanSistemi>();
                if (playerHealth != null)
                    playerHealth.TakeDamage(hasar);
            }
        }
        else
        {
            // Boþa giden atýþ
            Debug.DrawRay(rayPoint.transform.position, atisYon * range, Color.yellow, 0.5f);
        }


    }
}
