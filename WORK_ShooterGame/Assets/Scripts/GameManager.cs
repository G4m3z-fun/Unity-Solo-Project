
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{

    PlayerController player;

    GameObject pauseMenu;

    Image healthBar;

    public bool isPaused = false; 

    void Start()
    {

        if (SceneManager.GetActiveScene().buildIndex >= 1)

        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            healthBar = GameObject.FindGameObjectWithTag("ui_health").GetComponent<Image>();

            pauseMenu = GameObject.FindGameObjectWithTag("pause");
            pauseMenu.SetActive(false);
        }
    }

    
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 1)
            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;
    }

    public void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;

            pauseMenu.SetActive(true);

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }

        else
            Resume();

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadLevel(int Level)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(Level);
    }

    public void MainMenu()
    {
        LoadLevel(0); 
    }

    public void Resume()
    {
        if (!isPaused)
        {
            isPaused = false;

            pauseMenu.SetActive(false);

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
    }


}
