using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeartOfPrince.Presentation
{
    public sealed class SceneTransitionService
    {
        public IEnumerator LoadSingle(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError(
                    "[SceneTransition] Cannot load an empty scene name.");
                yield break;
            }

            AsyncOperation operation =
                SceneManager.LoadSceneAsync(
                    sceneName,
                    LoadSceneMode.Single);

            if (operation == null)
            {
                Debug.LogError(
                    $"[SceneTransition] Unity could not begin loading " +
                    $"scene '{sceneName}'.");
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}
