using Express.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Express.Structures
{
    public class BlockExpression : Expressable
    {
        public override int ZIndex => 0;
        private const string BLOCK = @"(?<ret>\S+)\s+(?<fname>\w+)\((?<arg>.*?)\)\s*\{(?<context>.*?)\}\s*$";

        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, BLOCK, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Multiline)
                || StringUtils.CountUnquoted(';', strInput) > 1;
        }
        string vv = "(?<=^([^\"]|\"[^\"]*\")*)[}]";
        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            if (StringUtils.CountUnquoted('}', strInput) > 1)
                strInput = ParseNested(strInput, localContext);
            const string ARGUMENT = @"(?<arg>(?<type>.+)\s+(?<name>\w+),?)*";
            List<ParameterExpression> lstArguments = new List<ParameterExpression>();
            var match = Regex.Match(strInput, BLOCK, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            foreach (Match arg in Regex.Matches(match.Groups["arg"].Value, ARGUMENT, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(arg.Value))
                    continue;
                var strType = arg.Groups["type"].Value;
                var name = arg.Groups["name"].Value;
                var tParam = ExposeTypes.FirstOrDefault(s => string.Equals(s.Name, strType, StringComparison.OrdinalIgnoreCase));
                if (tParam == null)
                    throw new Exception($"parameter fail {strType} {name}");
                var newArg = Expression.Parameter(tParam, name);
                GlobalContext.Add((name, newArg));
                lstArguments.Add(newArg);
            }
            Type tReturn = null;
            string[] lstLines;
            if (match.Success)
            {
                //TODO: If nested blocks exist, handle those first
                lstLines = StringUtils.SplitAt(new[] { ';', '\n' }, match.Groups["context"].Value, true);
                if (match.Groups["ret"].Value != "void")
                    tReturn = ExposeTypes.FirstOrDefault(s => string.Equals(s.Name, match.Groups["ret"].Value, StringComparison.OrdinalIgnoreCase));
            }
            else
                lstLines = StringUtils.SplitAt(';', strInput, true).Select(line => line.Trim(new[] { '\r', '\n', ' ', '\t' })).ToArray();
            List<Expression> lstInScope = new List<Expression>(lstLines.Length);
            List<(string, Expression)> bContext = new List<(string, Expression)>();
            string finalReturnStatement = null;
            foreach (var line in lstLines)
            {
                if (ParseType(line).GetType() == typeof(Declaration))
                {
                    var result = Declaration.ParseInstanciation(line, bContext);
                    lstArguments.Add(result.Item1);
                    if (result.Item2 != null)
                        lstInScope.Add(result.Item2);
                }
                else
                    lstInScope.Add(ParseExpression(line, bContext));
                if (ParseType(line).GetType() == typeof(ReturnStatement))
                    finalReturnStatement = line;
            }
            if (!match.Success)
                tReturn = ParseExpression(finalReturnStatement, bContext).Type;
            if (tReturn != null)
                return (lstArguments.Count > 0) ? Expression.Block(tReturn, lstArguments, lstInScope) : Expression.Block(tReturn, lstInScope);
            else
                return (lstArguments.Count > 0) ? Expression.Block(lstArguments, lstInScope) : Expression.Block(lstInScope);
        }

        private string ParseNested(string strInput, List<(string, Expression)> localContext = null)
        {
            const string innerBlock = "(?<ret>\\S+)\\s+(?<fname>\\w+)\\((?<arg>.*?)\\)\\s*\\{(?<context>(?<=^([^\"]|\"[^\"]*\")*)[^{}]+)\\}\\s*";
            while (Regex.IsMatch(strInput,innerBlock,RegexOptions.ExplicitCapture|RegexOptions.Multiline|RegexOptions.IgnoreCase))
            {

            }
        }
    }
}
