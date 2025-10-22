// Example: BootstrapLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (SceneManager.GetActiveScene().name != "Bootstrap")
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Additive);
        }
    }
}
