using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

[Title("Set Game Object To State")]
[Description("Changes the state of a GameObject with multiple visual or logical states.")]

[Category("Game Object/Set Object State")]
[Icon(RuntimePaths.GIZMOS + "IconGameObject.png")]
[Serializable]
public class SetGameObjectToState : Instruction
{
    [SerializeField] private GameObjectWithState gameObject;
    [SerializeField] private int stateIndex = 0;
    protected override Task Run(Args args)
    {
        if (gameObject != null)
        {
            gameObject.SetState(stateIndex);
        }
        return DefaultResult;
    }
}
