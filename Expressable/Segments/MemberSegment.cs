using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

namespace Express
{
    public class MemberSegment : Expressable
    {
        public override int ZIndex => 2;
        private const string RGXID = @"^\s*(?<root>[$a-z_0-9]+)(?<add>\.(?<val>[a-z_0-9]+))+\s*$";
        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, RGXID, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            var match = Regex.Match(strInput, RGXID, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            var strParent = match.Groups["root"].Value;
            
            if (!localContext.Any(s => string.Equals(s.Item1, strParent))&&!GlobalContext.Any(s => string.Equals(s.Item1, strParent)))
                throw new Exception($"root Type not found for member access {strParent}");
            Expression rootObj = localContext.Any(z=>z.Item1 == strParent)?localContext.First(y=>y.Item1 == strParent).Item2
                : GlobalContext.First(s => string.Equals(s.Item1, strParent)).Item2;
            foreach(Capture additional in match.Groups["add"].Captures)
            {
                var member = GetMember(rootObj.Type, additional.Value.TrimStart('.'));
                rootObj = Expression.MakeMemberAccess(rootObj, member);
            }
            return rootObj;
        }

        private MemberInfo GetMember(Type tOwner, string strName,MemberTypes mType = MemberTypes.Property)
        {
            var allMembers = tOwner.GetMembers();
            var lstMatches = new List<MemberInfo>();
            for (int i=0;i<allMembers.Length;i++)
            {
                if (string.Equals(allMembers[i].Name, strName, StringComparison.OrdinalIgnoreCase))
                    lstMatches.Add(allMembers[i]);
            }
            if (lstMatches.Any(s => ExposeMembers.Any(t => t.DeclaringType == s.DeclaringType && string.Equals(t.Name, s.Name, StringComparison.OrdinalIgnoreCase)))==false
            && ExposeTypes.Any(z => z == tOwner) == false)
                throw new Exception($"type {tOwner} or member {strName} not exposed");
            if (lstMatches.Count == 0)
                throw new Exception($"Member not found with name {strName} on type {tOwner.Name}");
            return (lstMatches.Count > 1&&lstMatches.Any(s=>s.MemberType == mType))? lstMatches.FirstOrDefault(t => t.MemberType == mType):lstMatches.FirstOrDefault();
        }
    }
}
