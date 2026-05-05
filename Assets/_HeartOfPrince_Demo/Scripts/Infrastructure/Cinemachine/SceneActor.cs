using System;
using UnityEngine;




public class SceneActor : MonoBehaviour
{
    public enum Act
    {
        Idle,
        LookAway,
        Fidget,
        StepBack,
        CrossArms,
        Shrug,
        Nod,
        ShakeHead,
        Recoil,
        Sit,
        Stand
    }

    [Serializable]
    public struct ActBinding
    {
        public Act act;
        public AnimationClip animationClip;
    }


    [SerializeField] public string actorName = "Actor";

    [SerializeField] public ActBinding[] actBindings;


    private void Start()
    {
        SceneDirector.Instance?.RegisterActor(this);
    }

    public void Emote()
    {

    }


}
