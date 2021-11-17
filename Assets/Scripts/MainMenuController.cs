using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class MainMenuController : MonoBehaviour
{
    [UsedImplicitly]
    public void PlayGame() {
        SceneManager.LoadScene("InputSystemTest");
    }
 
    [UsedImplicitly]
    public void Options() {
        
    }
 
    [UsedImplicitly]
    public void ExitGame() {
        Application.Quit();
        print("Should be quiting the game now");
    }
}