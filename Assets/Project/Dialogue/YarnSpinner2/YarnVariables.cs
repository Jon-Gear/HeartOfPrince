using Yarn.Unity;

[System.CodeDom.Compiler.GeneratedCode("YarnSpinner", "3.0.3.0")]
public partial class YarnVariables : Yarn.Unity.InMemoryVariableStorage, Yarn.Unity.IGeneratedVariableStorage {
    // Accessor for String $topic1
    public string Topic1 {
        get => this.GetValueOrDefault<string>("$topic1");
        set => this.SetValue<string>("$topic1", value);
    }

    // Accessor for String $topic2
    public string Topic2 {
        get => this.GetValueOrDefault<string>("$topic2");
        set => this.SetValue<string>("$topic2", value);
    }

    // Accessor for String $topic3
    public string Topic3 {
        get => this.GetValueOrDefault<string>("$topic3");
        set => this.SetValue<string>("$topic3", value);
    }

    // Accessor for String $topicNPC
    public string TopicNPC {
        get => this.GetValueOrDefault<string>("$topicNPC");
        set => this.SetValue<string>("$topicNPC", value);
    }

}
