using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Spawn")]
    public GameObject playerPrefab;

    [Header("Hapis - Prisoner Spawn")]
    public Transform[] spawnPoints;

    [Header("Prisoner - NPC Karakter")]
    public GameObject npcPrefab;
    float tempTimePrisoner = 0f;

    [Header("Police Kule Spawn")]
    public GameObject policeKuleNpcPrefab;

    [Header("Police Spawn")]
    public GameObject policeNpcPrefab;
    float tempTimePolice = 0f;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        //Spawn point sayýsý kadar rastgele bir sayý döndürüyor.
        int randomIndex = Random.Range(0, spawnPoints.Length);

        //Karakteri spawn eden kod
        playerPrefab.transform.position = spawnPoints[randomIndex].transform.position;



        //Prisoner Jail Spawn Fonksiyonu
        PrisonerJailSpawn();

        //Polis Kule Spawn Fonksiyonu
        PoliceKuleSpawn();

        //Polis Spawn Fonksiyonu
        PoliceSpawn();

        //Police Spawmn Süresi
        tempTimePolice = Time.time + 30f;

        //Prisoner Spawn Fonksiyonu
        PrisonerSpawn();

        //Prisoner Spawmn Süresi
        tempTimePrisoner = Time.time + 120f;


        //Konsole dan kontrol etme
        Debug.Log(randomIndex + " - PlayerSpawner.cs");
    }

    void Update()
    {
        //Belirli bir süre arayla tekrardan police spawn etme
        if (Time.time >= tempTimePolice)
        {
            PoliceSpawn();
            tempTimePolice = Time.time + 30f;
        }


        //Belirli bir süre arayla tekrardan prisoner spawn etme
        if (Time.time >= tempTimePrisoner)
        {
            PrisonerSpawn();
            tempTimePrisoner = Time.time + 120f;
        }
    }

    //Polis Spawn Fonksiyonu
    private void PoliceSpawn()
    {
        //Police Clone Spawn Kodu
        GameObject[] policeSpawnPoint = GameObject.FindGameObjectsWithTag("PoliceSpawn");
        foreach (GameObject point in policeSpawnPoint)
        {
            Instantiate(policeNpcPrefab, point.transform.position, point.transform.rotation);

        }
    }


    //Polis Kule Spawn Fonksiyonu
    private void PoliceKuleSpawn()
    {
        //Police Kule Clone Spawn Kodu
        GameObject[] policeKuleSpawn = GameObject.FindGameObjectsWithTag("PoliceKuleSpawn");
        foreach (GameObject point in policeKuleSpawn)
        {
            Instantiate(policeKuleNpcPrefab, point.transform.position, point.transform.rotation);

        }
    }


    //Prisoner Jail Spawn Fonksiyonu
    private void PrisonerJailSpawn()
    {
        //Prisoner Jail Clone Spawn Kodu
        GameObject[] npsSpawnPoints = GameObject.FindGameObjectsWithTag("NPCSpawn");
        foreach (GameObject point in npsSpawnPoints)
        {
            Instantiate(npcPrefab, point.transform.position, point.transform.rotation);
        }
    }

    //Prisoner Spawn Fonksiyonu
    private void PrisonerSpawn()
    {
        //Prisoner Clone Spawn Kodu
        GameObject[] prisonerSpawn = GameObject.FindGameObjectsWithTag("PrisonerSpawn");
        foreach (GameObject point in prisonerSpawn)
        {
            Instantiate(npcPrefab, point.transform.position, point.transform.rotation);
        }
    }





}
