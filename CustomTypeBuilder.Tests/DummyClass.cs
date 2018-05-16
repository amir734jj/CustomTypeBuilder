using System;
using System.Collections.Generic;
using System.Text;

namespace CustomTypeBuilder.Tests
{
    public class DummyClass
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime DateOfBith { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        private bool Equals(DummyClass other)
        {
            return string.Equals(Name, other.Name) && Age == other.Age && DateOfBith.Equals(other.DateOfBith);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DummyClass)obj);
        }
    }
}
