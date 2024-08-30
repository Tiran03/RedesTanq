using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuPausa : MonoBehaviour
{

    [SerializeField] private GameObject botonPausa;

    [SerializeField] private GameObject menuPausa;

    public void Pausa()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }


    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Cerrar()
    {
        Application.Quit();
    }

    //public void Restart()
    //{
    //    Time.timeScale = 1f;
    //    LoadingManager.Instance.LoadScene(3, 1);

    //}

    //public void Restart1()
    //{
    //    Time.timeScale = 1f;
    //    LoadingManager.Instance.LoadScene(2, 1);
    //}

    //public void Restart3()
    //{
    //    Time.timeScale = 1f;
    //    LoadingManager.Instance.LoadScene(4, 1);
    //}
}
