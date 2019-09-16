using Express;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TestSerializer.TestObjects;

namespace TestSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Character testr = new Character
            {
                Name = "Strange\",sp\"spspsp,,,LastName",
                Level = 5,
                Attributes = new List<CharacterAttribute> { new Strength(1) },
                Balance = new Money { Gold = 10, Silver = 25, Copper = 67 }
            };


            List<string> st = new List<string>();
            string testPEMDAS = "5 + 3 - ((8/9)+1)";
            string testmethodCreation = "Boolean LessThanForty(string xz){return xz.Length < 40;}";

            string exampleBlock = "Character boi = new Character(\"maboi\",6);\r\nreturn boi.Level > 5;";
            string ComplexBlocks = "int main(){\r\nvoid SetLevel(Character subject,int iLvl){ subject.Level = iLvl; }\r\n}";


            Expressable.ExposeTypes = new List<Type> { typeof(Character),typeof(Money),typeof(CharacterAttribute),typeof(Strength),
                typeof(string), typeof(int), typeof(long), typeof(decimal), typeof(DateTime), typeof(bool) };
            Expressable.ExposeMembers = new List<System.Reflection.MemberInfo> { typeof(string).GetProperty("Length") };

            BlockExpression blockExpression =(BlockExpression) Expressable.ParseExpression(exampleBlock);
            

            Func<bool> compExpress = Expression.Lambda<Func<bool>>(blockExpression).Compile();
            var result = compExpress();

        }
    }
}
