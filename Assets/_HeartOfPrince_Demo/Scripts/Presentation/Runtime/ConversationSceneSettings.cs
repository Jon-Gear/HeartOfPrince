using HeartOfPrince.Domain;
using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    public class ConversationSceneSettings : MonoBehaviour
    {
        [SerializeField] DialogueRunner dialogueRunner;
        [SerializeField] string characterID;


        private CharacterID CurrentCharacter;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            CurrentCharacter = (CharacterID) characterID;
            GameSession.Instance.Conversation.SetCurrentCharacter(CurrentCharacter);

            Debug.Log("Current Character from start"+ GameSession.Instance.Conversation.GetCurrentCharacter());
            
            dialogueRunner.StartDialogue("Start");
        }

        // Update is called once per frame
        void Update()
        {
        
        }


    }
}
