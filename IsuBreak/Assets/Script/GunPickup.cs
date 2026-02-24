using UnityEngine;

public class GunPickup : MonoBehaviour
{
    // Silahýn sahneden silinmeden önce ne kadar süre kalacaðýný belirler (örneðin 20 saniye)
    public float destroyTime = 5f;

    void Start()
    {
        // Belirtilen süre sonra Die metodu çaðrýlacak
        Invoke("StartDestroyTimer", 0.5f);
    }

    void StartDestroyTimer()
    {
        // Belirtilen süre sonra Die metodu çaðrýlacak
        Invoke("Die", destroyTime);
    }

    // Silahý sahneden silen metod
    void Die()
    {
        // Henüz alýnmadýysa, objeyi sil
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Yalnýzca Player ile çarpýþma ve Rigidbody'si olan (yani düþmüþ) objeler için devam et.
        if (other.CompareTag("Player") && GetComponent<Rigidbody>() != null)
        {
            PlayerMove playerMove = other.GetComponent<PlayerMove>();

            if (playerMove != null)
            {
                // Alýndýktan sonra otomatik silme iþlemini iptal et
                CancelInvoke("StartDestroyTimer");
                CancelInvoke("Die");

                playerMove.PickupGun(gameObject);

                Destroy(this); // Sadece scripti sil
            }
        }
    }
}