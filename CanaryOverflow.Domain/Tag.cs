using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain
{
    public class Tag : Entity
    {
        public string Name { get; private set; }
        public ICollection<Question> Questions { get; private set; }
    }
}