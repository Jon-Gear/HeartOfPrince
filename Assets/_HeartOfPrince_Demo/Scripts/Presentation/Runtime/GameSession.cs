using HeartOfPrince.Domain;
using UnityEngine;

namespace HeartOfPrince.Presentation
{
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; } 

        public GameState State { get; private set; }

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildRuntime();

        }

        private void OnDestroy()
        {
            if(Instance == this)
            {
                Instance = null;
            }
        }

        private void BuildRuntime()
        {

        }
    }
}
