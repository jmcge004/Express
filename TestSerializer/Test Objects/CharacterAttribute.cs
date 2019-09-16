using System;
using System.Collections.Generic;
using System.Text;

namespace TestSerializer.TestObjects
{
    public abstract class CharacterAttribute
    {
        public abstract string DisplayName { get; }
        public int Level { get; set; }
    }
    public class Strength : CharacterAttribute
    {
        public override string DisplayName => "Strength";
        public Strength(int iLevel = 0)
        {
            Level = iLevel;
        }
    }
}
