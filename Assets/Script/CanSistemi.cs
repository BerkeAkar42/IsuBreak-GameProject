using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/*
    - Player Can Scripti -
 
 
 */

public class CanSistemi : MonoBehaviour
{
    //--- Can Bar Deðiþkenleri ---
    public float can = 100f, animasyonYavasligi = 50f; //Karakterin caný
    float maxCan, gercekScale;


    //--- Otomatik Ýyileþme ---
    public float healDelay = 10f; // hasar almadan geçmesi gereken süre
    public float healRate = 1f; // saniyede kaç can dolacak
    private float lastDamageTime; // son hasar zamaný


    public Image oldunEkrani;
    private bool oldunEkraniAcildi = false;

    // Start is called before the first frame update
    void Start()
    {
        maxCan = can; //Max can kontrolü

        lastDamageTime = Time.time; //Son hazar alma zamanýný tutma


        // Baþta görünmez
        if (oldunEkrani != null)
        {
            Color c = oldunEkrani.color;
            c.a = 0f;
            oldunEkrani.color = c;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Can barý kontrol eden fonksiyon
        canBar();


        if (can > 0)
        {
            //Hasar almadýysa ve süre dolduysa can yenile
            if (Time.time - lastDamageTime >= healDelay && can < maxCan)
            {
                can += healRate * Time.deltaTime;
                can = Mathf.Min(can, maxCan);
            }


        }

        //Debug.Log(can);




        //Test Azalt
        if (Input.GetKeyDown("f") && can > 0)
        {
            can -= 10;
            lastDamageTime = Time.time; //Karakter her hasar aldýðýnda, hasar aldýðý süreyi elinde tutar.
        }

        //Test Arttýr
        if (Input.GetKeyDown("r") && can < maxCan)
        {
            can += 10;
        }


        //Con kontrol fonksiyonu
        canKontrol();
    }


    //Hasar Aldýrma Fonksiyonu:
    public void TakeDamage(float amount)
    {
        can -= amount;

        if (can <= 0)
        {
            die();
            //Debug.Log("Hasar Verildi!");
            can = 0;
        }
        lastDamageTime = Time.time; //Karakter her hasar aldýðýnda, hasar aldýðý süreyi elinde tutar.
    }


    void canBar()
    {

        gercekScale = can / maxCan;

        //Bar can azalma
        if (transform.localScale.x > gercekScale)
        {
            transform.localScale = new Vector3(transform.localScale.x - (transform.localScale.x - gercekScale) / animasyonYavasligi, transform.localScale.y, transform.localScale.z);
        }


        //Bar can arttýrma
        if (transform.localScale.x < gercekScale)
        {
            transform.localScale = new Vector3(transform.localScale.x + (gercekScale - transform.localScale.x) / animasyonYavasligi, transform.localScale.y, transform.localScale.z);
        }

    }



    void canKontrol()
    {
        //Canýn eksi (-) deðerlere düþmesini engelliyoruz.
        if (can <= 0)
        {
            can = 0;

            //Player öldü!!!
            die();
        }

        //Canýn yüz (100) den fazla olmasýný engelliyoruz.transform.localScale.x
        if (can > maxCan)
        {
            can = maxCan;
        }
    }

    //Ölüm sonucu devreye girecek olan fonksiyon
    void die()
    {
        //Debug.Log("Die Fonksiyonu Çalýþtý!");
        //Karakter öldü, hareket edemez. Hareket scriptini kaldýrdýk

        //"Player" taglý game objesini alýr
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //Böyle bir obje varsa
        if (player != null)
        {
            //Player üzerinde bu script var mý bilgisini alýr
            PlayerMove move = player.GetComponent<PlayerMove>();


            // Player üzerindeki animatoru al
            Animator anim = player.GetComponent<Animator>();

            if (anim != null)
            {
                anim.SetBool("IsDying", true);
            }


            if (move != null)
                move.enabled = false;


            // Ölünce ekraný göster (sadece bir kere)
            if (!oldunEkraniAcildi && oldunEkrani != null)
            {
                StartCoroutine(OldunEkraniFadeIn());
                oldunEkraniAcildi = true;
            }


            Invoke("anaMenuyeDon", 10f);
        }

    }

    //Öldün ekraný
    IEnumerator OldunEkraniFadeIn()
    {
        float duration = 4f; // saniyede tamamen görünsün
        float elapsed = 0f;
        Color c = oldunEkrani.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            oldunEkrani.color = c;
            yield return null;
        }

        c.a = 1f;
        oldunEkrani.color = c;
    }


    private void anaMenuyeDon()
    {
        SceneManager.LoadScene(0);
    }

}
