using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public bool fromMap = false;

    public void Switch(string name) 
    {
        if (fromMap) 
        {
            GameObject.FindGameObjectWithTag("MapPanel").SetActive(false);
        }
            SceneManager.LoadScene(name); 
    }
    public void Quit()
    {
        Application.Quit();
    }
}
