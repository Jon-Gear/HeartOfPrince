using UnityEngine;

using Yarn;
using Yarn.Unity;

public class DialogueManager : Singleton<DialogueManager>
{

    /// <summary>
    /// Gets the underlying <see cref="Dialogue"/> object that runs the
    /// Yarn code.
    /// </summary>
    public Dialogue Dialogue => _dialogue ?? (_dialogue = CreateDialogueInstance());

    public void StartDialogue(string startNode)
    {

    }

    public void StopDialogue()
    {

    }

    public void SetYarnProject(YarnProject newYarnProject)
    {
        currentYarnProject = newYarnProject;
    }

    /// <summary>
    /// The <see cref="YarnProject"/> asset that should be loaded on
    /// scene start.
    /// </summary>
    [UnityEngine.Serialization.FormerlySerializedAs("yarnProgram")]
    public YarnProject currentYarnProject;

    /// <summary>
    /// The variable storage object.
    /// </summary>
    [UnityEngine.Serialization.FormerlySerializedAs("variableStorage")]
    [SerializeField] internal VariableStorageBehaviour _variableStorage;

    /// <inheritdoc cref="_variableStorage"/>
    public VariableStorageBehaviour VariableStorage
    {
        get => _variableStorage; 
        set
        {
            _variableStorage = value;
            if (_dialogue != null)
            {
                _dialogue.VariableStorage = value;
            }
        }
    }


    /// <summary>
    /// The underlying object that executes Yarn instructions
    /// and provides lines, options and commands.
    /// </summary>
    /// <remarks>
    /// Automatically created on first access.
    /// </remarks>
    private Dialogue _dialogue;

    
    private Dialogue CreateDialogueInstance()
    {
        if (VariableStorage == null)
        {
            // If we don't have a variable storage, create an
            // InMemoryVariableStorage and make it use that.

            VariableStorage = gameObject.AddComponent<InMemoryVariableStorage>();
        }

        // Create the main Dialogue runner, and pass our
        // variableStorage to it
        var dialogue = new Yarn.Dialogue(VariableStorage)
        {
            /*
            // Set up the logging system.
            LogDebugMessage = delegate (string message)
            {
                if (verboseLogging)
                {
                    Debug.Log(message);
                }
            },
            LogErrorMessage = delegate (string message)
            {
                Debug.LogError(message);
            },

            LineHandler = HandleLine,
            CommandHandler = HandleCommand,
            OptionsHandler = HandleOptions,
            NodeStartHandler = (node) =>
            {
                onNodeStart?.Invoke(node);
            },
            NodeCompleteHandler = (node) =>
            {
                onNodeComplete?.Invoke(node);
            },
            DialogueCompleteHandler = HandleDialogueComplete,
            PrepareForLinesHandler = PrepareForLines
            */
        };

        //selectAction = SelectedOption;
        return dialogue;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
