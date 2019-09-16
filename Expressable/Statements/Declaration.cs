using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Express.Statements
{
    public class Declaration : Expressable
    {
        private const string INSTANTIATION = @"^\s*(?<type>[a-z][a-z_0-9]*(\[\])?)\s+(?<name>[a-z][a-z_0-9]*?)\s?=(?<right>.*);\s*$";
        private const string DECLARATION = @"^\s*(?<type>[a-z][a-z_0-9]*(\[\])?)\s+(?<name>[a-z][a-z_0-9]*?);";
        public override int ZIndex => 0;

        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, INSTANTIATION, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture) ||
                Regex.IsMatch(strInput, DECLARATION, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            var res = ParseInstanciation(strInput, localContext);
            return (res.Item2 == null) ? res.Item1 : res.Item2;
        }

        public static (ParameterExpression, Expression) ParseInstanciation(string strInput, List<(string, Expression)> localContext = null)
        {
            if (localContext == null)
                localContext = new List<(string, Expression)>();
            var rightMatch = Regex.Match(strInput, INSTANTIATION, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            var match = Regex.Match(strInput, DECLARATION, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            var eVar = Expression.Variable(ExposeTypes.First(s => string.Equals(s.Name, rightMatch.Groups["type"].Value, StringComparison.OrdinalIgnoreCase))
                    , rightMatch.Groups["name"].Value);
            if (!rightMatch.Success)
            {
                localContext.Add((rightMatch.Groups["name"].Value, eVar));
                return (eVar, null);
            }
            else
            {
                var strRight = rightMatch.Groups["right"].Value;
                if (!ExposeTypes.Any(s => string.Equals(s.Name, rightMatch.Groups["type"].Value, StringComparison.OrdinalIgnoreCase)))
                    throw new Exception("No type found");

                var right = ParseExpression(strRight, localContext);
                var result = Expression.Assign(eVar, right);
                localContext.Add((rightMatch.Groups["name"].Value, result));
                return (eVar, result);
            }
        }
    }
}
