using System;
using Newtonsoft.Json;

namespace SharedKernel
{
    public class Entity : IEntity
    {
        [JsonProperty]
        public Guid Guid { get; protected set; } = Guid.NewGuid();

        public bool IsActive { get; init; }
        public override bool Equals(object obj)
        {
            return (obj as IEntity)?.Guid == Guid;
        }

        protected bool Equals(Entity other)
        {
            return Guid.Equals(other.Guid);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}