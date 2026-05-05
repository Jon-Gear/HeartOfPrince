using HeartOfPrince.Application;
using HeartOfPrince.Domain;
using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        [SerializeField] public YarnProject yarnProject;
        public GameState State { get; private set; }

        public ConversationService Conversation { get; private set; }


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
            BuildStateRuntime();
            Conversation = new ConversationService(State);
            Debug.Log("Built");
        }

        private void BuildStateRuntime()
        {
            State = new GameState();

            CharacterID munir = new CharacterID("Munir");

            CharacterTopicState munirTopicState = new CharacterTopicState(munir);

            munirTopicState.AddCharacterToPlayerTopic(new TopicName("PlaceholderTopic1"));
            munirTopicState.AddPlayerToCharacterTopic(new TopicName("PlaceholderTopic2"));

            State.ConversationState = new ConversationState();
            State.CharactersTopics.Add(munir, new CharacterTopicState(munir));

        }
    }
}
