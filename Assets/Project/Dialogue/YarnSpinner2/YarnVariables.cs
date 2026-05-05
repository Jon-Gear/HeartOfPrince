using Yarn.Unity;

[System.CodeDom.Compiler.GeneratedCode("YarnSpinner", "3.0.3.0")]
public partial class YarnVariables : Yarn.Unity.InMemoryVariableStorage, Yarn.Unity.IGeneratedVariableStorage {
    // Accessor for String $topicNPC
    public string TopicNPC {
        get => this.GetValueOrDefault<string>("$topicNPC");
        set => this.SetValue<string>("$topicNPC", value);
    }

}
