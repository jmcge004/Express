using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Express
{
    public class ArrayArg:Expressable
    {
        public override int ZIndex => 10;

        public override bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, "^\\s*.*\\[(?<str>\".*\")?(?<num>\\d+)\\]\\s*$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }

        public override Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            throw new NotImplementedException();
        }
    }
}
