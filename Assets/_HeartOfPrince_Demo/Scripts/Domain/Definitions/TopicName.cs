using System;
using System.Collections.Generic;
using System.Text;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public readonly struct TopicName : IEquatable<TopicName>
    {
        public string Value { get; }

        public TopicName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Topic name cannot be null or empty.", nameof(value));

            Value = value.Trim();
        }

        public bool Equals(TopicName other) => Value == other.Value;
        public override bool Equals(object obj) => obj is TopicName other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static implicit operator string(TopicName topicName) => topicName.Value;
        public static explicit operator TopicName(string value) => new(value);
    }
}
