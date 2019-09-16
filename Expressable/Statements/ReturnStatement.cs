using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Express.Statements
{
    public class ReturnStatement : Expressable
    {
        public override int ZIndex => 0;
        private const string RETURN = @"return\s+(?<inner>.*?);";
        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, RETURN, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            Match m = Regex.Match(strInput, RETURN);
            return ParseExpression(m.Groups["inner"].Value, localContext);
        }
    }
}
