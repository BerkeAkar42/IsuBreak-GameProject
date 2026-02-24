using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
    - Player Hareket Scripti -
 
 
 */


public class PlayerMove : MonoBehaviour
{
    Animator animator;


    CharacterController Controller;
    Vector3 velocity;
    bool isGrounded;

    public Transform ground;
    public float distance = 0.3f;

    public float speed;
    public float jumpHeight;
    public float gravify;

    public LayerMask mask;


    //Boxing/Gun/Normal anim kontrol
    bool isBoxing = false;
    bool isGun = false;
    string oyunModu = "normal";

    //Envanter
    [Header("UI Ayarlarý - Envanter")]
    public Image boxingIcon; //Yumruk
    public Image gunIcon; //Silah
    public Image crossHair; //Crosshair
    public Image hizIcon; //Hýzlanýnca çýkan icon


    [Header("Silah")]
    public GameObject pistol;
    GunSystems GunSys;

    [Header("Silah Sabitleme Ayarlarý")]
    public Transform rightHandTransform; // mixamorig:RightHand Transform'u
    private bool hasPickedUpGun = false; // Silahý alýp almadýðýmýzý tutar


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();

        GunSys = pistol.GetComponent<GunSystems>(); //Silahýn içerisinden GunSystems scriptini al.

        hizIcon.gameObject.SetActive(false); //hýz iconu baþta kapalý

        // mixamorig:RightHand transform'unu bulma (Player objesi altýndan)
        // Eðer elin adý tam olarak "mixamorig:RightHand" ise
        rightHandTransform = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand");

        if (rightHandTransform == null)
            Debug.LogError("mixamorig:RightHand bulunamadý! Lütfen yolu kontrol edin.");
    }


    // Update is called once per frame
    void Update()
    {
        //Animasyonlarý Çaðýrdýðýmýz Fonksiyon
        animasyonlar();

        #region Movement / Hareket Scripti
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        Controller.Move(move * speed * Time.deltaTime);



        //Hýz Ayarlamasý
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftControl))
        {
            speed = 20f; //Koþma
            hizIcon.gameObject.SetActive(true); // hýz iconunu açma
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            speed = 7; //Shift - Sessiz yürüyüþ
        }
        else
        {
            speed = 10; //Yürüme
            hizIcon.gameObject.SetActive(false); // hýz iconunu kapatma
        }


        #endregion


        #region Jump / Zýplama Scripti
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //jump
            //Yukarýya 10x gibi bi kuvvetle çýkýp, bu x i yavaþ yavaþ düþüren bir fonksiyon
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravify);
        }
        #endregion



        #region Gravity / Yer çekimi Scripti

        isGrounded = Physics.CheckSphere(ground.position, distance, mask);

        //Karakterin yere deðip deðmemesine göre yer çekimi aktif veya deðil
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Yer çekimini ayarlayan kod.
        velocity.y += gravify * Time.deltaTime;

        //Karaktere yer çekimini etkiledik.
        Controller.Move(velocity * Time.deltaTime);

        #endregion




        //Debug.Log(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftControl));
    }


    // Silahý yerden alma metodu
    public void PickupGun(GameObject pickedUpGun)
    {
        if (hasPickedUpGun) return; // Zaten silah varsa tekrar almayý engelle

        // Silahýn fizik bileþenlerini devre dýþý býrak
        Rigidbody rb = pickedUpGun.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

        // Silahý, oyuncunun eline çocuk (child) yap
        pickedUpGun.transform.SetParent(rightHandTransform);

        // Pozisyon, Rotasyon ve Scale ayarlarýný yap
        pickedUpGun.transform.localPosition = new Vector3(-0.0549000017f, 0.148800001f, 0.0162000004f);
        pickedUpGun.transform.localRotation = Quaternion.Euler(-87.569f, -95.228f, 178.38f);
        pickedUpGun.transform.localScale = new Vector3(0.6923075f, 0.6923075f, 0.6923075f);

        // Elimizde silah olduðunu iþaretle
        hasPickedUpGun = true;

    }


    //Yumruk atma fonksiyonu
    void PunchHitCheck()
    {
        float damage = 5f;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 5f, 5f);

        foreach (Collider col in hitColliders)
        {
            NPCHealth npc = col.GetComponent<NPCHealth>();
            if (npc != null)
            {
                npc.TakeDamage(damage);
                Debug.Log("Yumruk Hasarý " + damage);
            }
        }
    }



    //Animasyonlar
    void animasyonlar()
    {

        #region Oyun Modlarý
        //Boks modu tuþ kontrolü
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isBoxing = !isBoxing; // Aç/Kapa

            if (isBoxing)
            {
                oyunModu = "boxing";
            }
            else
            {
                oyunModu = "normal";
                animator.SetBool("IsBoxingIdle", false); //Oyun modu false olduðundan boks anim de bunu kapatamýyorduk.
            }
        }

        //Silah modu tuþ kontrolü
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            if (hasPickedUpGun)
            {
                isGun = !isGun; // Aç/Kapa

                if (isGun)
                {
                    oyunModu = "gun";
                }
                else
                {
                    oyunModu = "normal";
                    animator.SetBool("IsGunIdle", false); //Oyun modu false olduðundan gun anim de bunu kapatamýyorduk.
                }
            }
            else
            {
                // Silah yoksa, bu tuþa basýldýðýnda normal modda kal.
                isGun = false;
                oyunModu = "normal";
            }
        }
        #endregion

        #region Mod Geçiþleri
        //Boks modundaysak;
        if (oyunModu == "boxing")
        {
            //--------------------------------//
            //Yumruk emojisini açma
            boxingIcon.gameObject.SetActive(true);

            //Crosshair aktif etme
            crossHair.gameObject.SetActive(true);

            //Box modundaysak,
            Cursor.lockState = CursorLockMode.Locked; //Mouse kitle
            Cursor.visible = false; //Mouse görünmez yap
            //--------------------------------//


            //Idle Animasyonu
            if (isBoxing)
            {
                animator.SetBool("IsBoxingIdle", true);
            }

            //Boks modunda yumruk atma
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("IsBoxing", true);
                Invoke("PunchHitCheck", 0.5f);
            }
            else
            {
                animator.SetBool("IsBoxing", false);
            }

            //Boks modunda ileri gitme
            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("IsBoxWalkForward", true);
            }
            else
            {
                animator.SetBool("IsBoxWalkForward", false);
            }

            //Boks modunda geri gitme
            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("IsBoxWalkBack", true);
            }
            else
            {
                animator.SetBool("IsBoxWalkBack", false);
            }


        }
        else if (oyunModu == "gun")
        {
            //--------------------------------//
            //Silah emojisini açma
            gunIcon.gameObject.SetActive(true);

            //Elindeki silahý aktif etme
            pistol.SetActive(true);

            //Crosshair aktif etme
            crossHair.gameObject.SetActive(true);

            //Silah modundaysak,
            Cursor.lockState = CursorLockMode.Locked; //Mouse kitle
            Cursor.visible = false; //Mouse görünmez yap
            //--------------------------------//


            //Idle Animasyonu
            if (isGun)
            {
                animator.SetBool("IsGunIdle", true);
            }

            //Gun modunda ateþ atma
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GunSys.pistolAtes();
                GunSys.gunTimer = Time.time + GunSys.gunCoolDown;
            }
            else
            {
                //animator.SetBool("IsBoxing", false);
            }

            //Gun modunda ileri gitme
            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("IsGunRun", true);
            }
            else
            {
                animator.SetBool("IsGunRun", false);
            }

            //Gun modunda geri gitme
            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("IsGunWalkBack", true);
            }
            else
            {
                animator.SetBool("IsGunWalkBack", false);
            }
        }
        else if (oyunModu == "normal")//Normal mod
        {
            //--------------------------------//
            //Crosshair deaktif etme
            crossHair.gameObject.SetActive(true);

            //Yumruk emojisini kapatma
            boxingIcon.gameObject.SetActive(false);

            //Silah emojisini kapatma
            gunIcon.gameObject.SetActive(false);

            //Elindeki silahý deaktif etme
            pistol.SetActive(false);



            //Box ve Silah modunda deðilsek,
            Cursor.lockState = CursorLockMode.Locked; //Mouse aç
            Cursor.visible = false; //Mouse görünür yap
            //--------------------------------//


            //Yürüme Animasyonu 
            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("IsWalk", true);
            }
            else if (!Input.GetKey(KeyCode.W))
            {
                animator.SetBool("IsWalk", false);
            }

            //Koþma Animasyonu 
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftControl))
            {
                animator.SetBool("IsRun", true);
            }
            else if (!Input.GetKey(KeyCode.W) || !Input.GetKey(KeyCode.LeftControl))
            {
                animator.SetBool("IsRun", false);
            }

            //Eðilerek Hareket Etme  
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("IsCrouched", true);
            }
            else if (!Input.GetKey(KeyCode.W) || !Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("IsCrouched", false);
            }

            //Zýplama Animasyonu 
            if (Input.GetKey(KeyCode.Space))
            {
                animator.SetBool("IsJump", true); //Eðilirken zýplayabiliyor!!!
            }
            else if (!Input.GetKey(KeyCode.Space))
            {
                animator.SetBool("IsJump", false);
            }

            //Geriye Yürüme Animasyonu 
            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("IsWalkBackward", true);
            }
            else if (!Input.GetKey(KeyCode.S))
            {
                animator.SetBool("IsWalkBackward", false);
            }

            //Sola Dönme/Yürüme Animasyonu 
            if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool("IsLeftTurn", true);
            }
            else if (!Input.GetKey(KeyCode.A))
            {
                animator.SetBool("IsLeftTurn", false);
            }

            //Saða Dönme/Yürüme Animasyonu 
            if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool("IsRightTurn", true);
            }
            else if (!Input.GetKey(KeyCode.D))
            {
                animator.SetBool("IsRightTurn", false);
            }

        }
        #endregion 


    }


}
