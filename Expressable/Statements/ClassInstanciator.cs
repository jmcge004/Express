using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Express.Statements
{
    public class ClassInstanciator : Expressable
    {
        private const string RGXID = @"^\s*new\s+(?<type>[a-z][a-z_0-9]*)\((?<args>.*)\)\s*$";
        public override int ZIndex => 0;

        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, RGXID, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            var match = Regex.Match(strInput, RGXID, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            var tClass = ExposeTypes.FirstOrDefault(s=>string.Equals(s.Name, match.Groups["type"].Value,StringComparison.OrdinalIgnoreCase));
            var lstArgs = StringUtils.SplitAt(',', match.Groups["args"].Value).Select(t=> ParseExpression(t, localContext));
            if (tClass == null||tClass.IsAbstract)
                throw new Exception($"invalid type specified{strInput}");
            var constructor = tClass.GetConstructor(lstArgs.Select(t => t.Type).ToArray());
            if ( constructor == null)
                throw new Exception($"no constructor found for args {string.Join(',', lstArgs.Select(t => t.Type.Name))}");
            return Expression.New(constructor, lstArgs);
        }
    }
}
