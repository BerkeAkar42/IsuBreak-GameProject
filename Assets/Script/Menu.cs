using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{


    public Image ayarlarBG;
    public Button carpi;
    bool isAyarlar = true;

    // Start is called before the first frame update
    void Start()
    {
        ayarlarBG.gameObject.SetActive(false);
        carpi.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None; //Mouse aç
        Cursor.visible = true; //Mouse görünür yap
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void basla()
    {
        SceneManager.LoadScene(1);
    }


    public void bitir()
    {
        Application.Quit();
    }





    public void ayarlar()
    {
        if (isAyarlar)
        {
            ayarlarBG.gameObject.SetActive(true);
            carpi.gameObject.SetActive(true);
            isAyarlar = false;
        }
    }


    public void ayarlarClose()
    {
        if (isAyarlar != true)
        {
            ayarlarBG.gameObject.SetActive(false);
            carpi.gameObject.SetActive(false);
            isAyarlar = true;
        }
    }




}
