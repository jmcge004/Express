using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Express.Structures
{
    public class InnerExpression : Expressable
    {
        private const string RGXID = @"(?<before>.*?)\((?<inner>.*[+\-/%*=><|&!].*)\)(?<after>.*)";
        private const string rgxMATH = @"[+\-%*/]\s*\(|\s*\)[+\-%*/]";
        private const string rgxCOMP = @"[&|<>!=]+\s*\(|\)\s*[&><=!|]+";
        public override int ZIndex => 0;

        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, RGXID, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture) &&
                (Regex.IsMatch(strInput, rgxMATH) || Regex.IsMatch(strInput, rgxCOMP));
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            var match = Regex.Match(strInput, RGXID);

            const string rgxMATH_BEFORE = @"(?<arg>.+)\s*(?<op>[+\-%/*])\s*\(";
            const string rgxMATH_AFTER = @"\s*\)\s*(?<op>[+\-%/*])\s*(?<arg>.+)\s*";
            const string rgxCOMP_BEFORE = @"(?<arg>.+)\s*(?<op>[&|><!=]{1,2})\s*\(";
            const string rgxCOMP_AFTER = @"\s*\)\s*(?<op>[&|><!=]{1,2})\s*(?<arg>.+)\s*";
            var inner = ParseExpression(match.Groups["inner"].Value, localContext);
            strInput = strInput.Replace($"({match.Groups["inner"].Value})", "()");
            Expression left;
            Expression After;
            Match beforeMatch = null;
            Match afterMatch = null;
            var blnMath = Regex.IsMatch(strInput, rgxMATH);
            beforeMatch = (blnMath) ? Regex.Match(strInput, rgxMATH_BEFORE) : Regex.Match(strInput, rgxCOMP_BEFORE);
            afterMatch = (blnMath) ? Regex.Match(strInput, rgxMATH_AFTER) : Regex.Match(strInput, rgxCOMP_AFTER);

            if (beforeMatch.Success)
                left = GetOperation(ParseExpression(beforeMatch.Groups["arg"].Value, localContext), inner, beforeMatch.Groups["op"].Value);
            else
                left = inner;
            if (afterMatch.Success)
                left = GetOperation(left, ParseExpression(afterMatch.Groups["arg"].Value, localContext), afterMatch.Groups["op"].Value);
            return left;
        }
        private Expression GetOperation(Expression left, Expression right, string strOperation)
        {
            List<(string, ExpressionType)> lstComparisons = new List<(string, ExpressionType)>
            {
            (">=", ExpressionType.GreaterThanOrEqual),
            ("<=", ExpressionType.LessThanOrEqual),
            (">", ExpressionType.GreaterThan),
            ("<", ExpressionType.LessThan),
            ("!=", ExpressionType.NotEqual),
            ("==", ExpressionType.Equal),
            ("+", ExpressionType.Add),
            ("-", ExpressionType.Subtract),
            ("/", ExpressionType.Divide),
            ("%", ExpressionType.Modulo),
            ("*", ExpressionType.Multiply),
            ("&", ExpressionType.AndAlso),
            ("&&", ExpressionType.AndAlso),
            ("|", ExpressionType.OrElse),
            ("||", ExpressionType.OrElse),
            };
            if (!lstComparisons.Any(s => s.Item1 == strOperation))
                throw new Exception($"unknown operation {strOperation}");
            return Expression.MakeBinary(lstComparisons.First(s => s.Item1 == strOperation).Item2, left, right);
        }
    }
}
