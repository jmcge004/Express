using System;
using System.Collections.Generic;
using System.Text;

namespace TestSerializer.TestObjects
{
    public class Character
    {
        public Character() { }
        public Character(string name, int level)
        {
            Name = name;
            Level = level;
        }
        public string Name { get; set; }
        public int Level { get; set; }

        public Money Balance { get; set; }
        public List<CharacterAttribute> Attributes { get; set; }
    }
}
