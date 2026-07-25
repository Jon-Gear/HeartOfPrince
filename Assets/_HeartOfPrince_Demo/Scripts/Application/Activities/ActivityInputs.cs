using System;

namespace HeartOfPrince.Application
{
    public interface IActivityInput
    {
        string SelectionKey { get; }
        bool Matches(IActivityInput other);
    }

    [Serializable]
    public sealed class NoActivityInput : IActivityInput
    {
        public static NoActivityInput Instance { get; } = new();

        public string SelectionKey => string.Empty;

        private NoActivityInput()
        {
        }

        public bool Matches(IActivityInput other)
        {
            return other is NoActivityInput;
        }
    }

    [Serializable]
    public sealed class TalkActivityInput : IActivityInput
    {
        public string CharacterId { get; }
        public string SelectionKey => CharacterId;

        public TalkActivityInput(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException(
                    "A character ID is required.",
                    nameof(characterId));
            }

            CharacterId = characterId.Trim();
        }

        public bool Matches(IActivityInput other)
        {
            return other is TalkActivityInput talk &&
                   string.Equals(
                       CharacterId,
                       talk.CharacterId,
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
