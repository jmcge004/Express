using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Express.Structures
{
    public class BinaryOperation : Expressable
    {
        const string RULE = @"(?<left>.+?)\s*(?<comp>[&|<>=!/%*+\-]{1,2})\s*(?<right>.+?)\s*";
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
        public override int ZIndex => 1;
        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, RULE, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            var match = Regex.Match(strInput, RULE, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            var strLeft = match.Groups["left"].Value;
            var strRight = match.Groups["right"].Value;
            var strComp = match.Groups["comp"].Value;
            var left = ParseExpression(strLeft, localContext);
            var right = ParseExpression(strRight, localContext);
            return Expression.MakeBinary(lstComparisons.First(s => s.Item1 == strComp).Item2, left, right);
        }
    }
}
