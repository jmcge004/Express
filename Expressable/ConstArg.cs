using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Express
{
    public class ConstArg : Expressable
    {
        const string CONST_ARGUMENT = "^\\s*(\".*\"|\\d+\\.?\\d*|null|true|false)\\s*$";
        const string CAPTURED = "\"(?<str>.*)\"|(?<num>\\d+\\.?\\d*)|(?<null>null)|(?<bool>true|false)\\s*";
        List<(string, Type)> lstTypes = new List<(string, Type)> { ("bool", typeof(bool)), ("str", typeof(string)) };
        public override int ZIndex => 10;

        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, CONST_ARGUMENT, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            var match = Regex.Match(strInput, CAPTURED, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            var inputType = match.Groups.First(s => s.Success&&s.Name != "0");
            if (inputType.Name == "null")
                return Expression.Constant(null);
            else if (inputType.Name == "num")
            {
                if (inputType.Value.Contains('.'))
                    return Expression.Constant(decimal.Parse(inputType.Value));
                else if (int.TryParse(strInput, out var lResult))
                    return Expression.Constant(lResult);
                else if (long.TryParse(strInput, out var intResult))
                    return Expression.Constant(intResult);
            }
            else
                return Expression.Constant(Convert.ChangeType(inputType.Value, lstTypes.First(s => s.Item1 == inputType.Name).Item2));
            return null;
        }
    }
}
