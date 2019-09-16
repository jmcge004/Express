using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Express.Segments
{
    public class InverseSegment : Expressable
    {
        public override int ZIndex => 4;
        private const string RGXID = "!(?<inner>[^=]+)";
        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput,RGXID );
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            var match = Regex.Match(strInput, RGXID,RegexOptions.ExplicitCapture|RegexOptions.IgnoreCase);
            var inner = ParseExpression(match.Groups["inner"].Value, localContext);
            return Expression.Not(inner);
        }
    }
}
