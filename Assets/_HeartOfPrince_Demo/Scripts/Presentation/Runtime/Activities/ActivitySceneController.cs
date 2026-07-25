using HeartOfPrince.Application;
using HeartOfPrince.Domain;
using UnityEngine;

namespace HeartOfPrince.Presentation
{
    [DisallowMultipleComponent]
    public sealed class ActivitySceneController : MonoBehaviour
    {
        public ActivityRunState CurrentRun =>
            GameSession.Instance?.State?.Day?.CurrentActivity;

        private void Start()
        {
            if (CurrentRun == null)
            {
                Debug.LogWarning(
                    $"Activity scene '{gameObject.scene.name}' was opened " +
                    "without an active ActivityRunState.",
                    this);
            }
        }

        public TData GetRunData<TData>()
            where TData : class, IActivityRunData
        {
            return CurrentRun?.GetData<TData>();
        }

        public void Complete()
        {
            GameLoopService.Instance?.NotifyActivityCompleted(
                ActivityResult.Success());
        }
    }
}
