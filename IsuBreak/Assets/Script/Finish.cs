using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
[Header("Kazandýnýz Görseli")]
    public Image kazandinizImage; // Inspector’a sürükle

    private bool oyunBitti = false;


    void Start()
    {
        kazandinizImage.gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (oyunBitti) return;

        if (other.CompareTag("Player"))
        {
            oyunBitti = true;
            Debug.Log("Oyun Bitti!");

            //Hareket scriptlerini devre dýþý býrak
            DisableAllMovementScripts();

            //Kazandýnýz efektini baþlat
            StartCoroutine(KazandinizEfekti());
        }
    }

    void DisableAllMovementScripts()
    {
        // Sahnedeki tüm scriptleri bulup kontrol et
        foreach (var script in FindObjectsOfType<MonoBehaviour>())
        {
            if (script is PlayerMove ||
                script is PoliceMove ||
                script is NPCMovement ||
                script is PoliceKuleMove)
            {
                script.enabled = false;
            }
        }

    }

    IEnumerator KazandinizEfekti()
    {
        if (kazandinizImage == null)
        {
            Debug.LogWarning("Kazandiniz Image atanmamýþ!");
            yield break;
        }

        // Görseli aktif et
        kazandinizImage.gameObject.SetActive(true);

        // Baþlangýç ölçeði
        kazandinizImage.rectTransform.localScale = Vector3.one;

        Vector3 hedef = new Vector3(30f, 30f, 30f);
        float hýz = 0.5f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * hýz;
            kazandinizImage.rectTransform.localScale = Vector3.Lerp(Vector3.one, hedef, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        //Belirli bir saniye sonunda ana menüye oyunu döndürecek.
        Invoke("anaMenuDon", 5f);
    }


    //Ana menüye dönüþ
    void anaMenuDon()
    {
        SceneManager.LoadScene(0);
    }
}
