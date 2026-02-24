using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystems : MonoBehaviour
{
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

    float range = 100f; // Menzil
    float hasar = 15f;

    void Start()
    {
        sesKaynak = GetComponent<AudioSource>();
    }

    public void pistolAtes()
    {
        // Ekran merkezinden (crosshair noktasý) bir ray oluþtur
        Ray centerRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Bu ray’in hedef noktasýný bul
        Vector3 hedefNokta;
        if (Physics.Raycast(centerRay, out hit, range))
        {
            hedefNokta = hit.point;
        }
        else
        {
            // Hiçbir þeye çarpmadýysa ileriye doðru farazi bir hedef noktasý belirle
            hedefNokta = centerRay.GetPoint(range);
        }

        // Þimdi silahýn ucundan o hedef noktaya doðru ray at
        Vector3 atisYon = (hedefNokta - rayPoint.transform.position).normalized;

        if (Physics.Raycast(rayPoint.transform.position, atisYon, out hit, range))
        {
            // Efektler
            if (muzzleFlash != null)
                muzzleFlash.Play();

            if (sesKaynak != null && fireSound != null)
                sesKaynak.PlayOneShot(fireSound);

            Debug.Log("Vurulan nesne: " + hit.transform.name);
            Debug.DrawRay(rayPoint.transform.position, atisYon * range, Color.red, 1f);

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
                NPCHealth npc = hit.transform.GetComponent<NPCHealth>();
                if (npc != null)
                    npc.TakeDamage(hasar);
            }


        }
    }
}
