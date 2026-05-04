using System;
using System.Collections.Generic;
using System.Text;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public readonly struct CharacterID : IEquatable<CharacterID>
    {
        public string Value { get; }

        public CharacterID(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Character ID cannot be null or empty.", nameof(value));

            Value = value.Trim();
        }

        public bool Equals(CharacterID other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CharacterID other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static implicit operator string(CharacterID characterId) => characterId.Value;
        public static explicit operator CharacterID(string value) => new(value);
    }
}
