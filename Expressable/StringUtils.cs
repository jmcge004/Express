using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Express
{
    public static class StringUtils
    {
        public static int CountUnquoted(char match, string input)
        {
            int count = 0;
            bool blnQuoted = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '"' && (i == 0 ||
                    (i > 0 && input[i - 1] != '\\')))
                    blnQuoted = !blnQuoted;
                if (input[i] == match && !blnQuoted)
                    count++;
            }
            return count;
        }

        public static string[] SplitAt(char[] separators, string input, bool includeSplitChar = false)
        {
            List<string> result = SplitAt(separators[0], input, includeSplitChar).ToList();
            for(int i =1;i<separators.Length;i++)
            {
                for (int j = 0; j < result.Count; j++)
                {
                    var nResult = SplitAt(separators[i], result[j], includeSplitChar).ToList();
                    result[j] = nResult[0];
                    if (nResult.Count > 1)
                        result.InsertRange(j + 1, nResult.Skip(1));
                }
            }
            return result.ToArray();
        }

        public static string[] SplitAt(char separator, string input,bool includeSplitChar = false)
        {
            var lstReslts = new List<string>();
            bool blnQuoted = false;
            int iStart = 0;
            for(int i =0;i<input.Length;i++)
            {
                if (input[i] == '"' && (i == 0 ||
                    (i > 0 && input[i - 1] != '\\')))
                    blnQuoted = !blnQuoted;
                else if (!blnQuoted && input[i] == separator)
                {
                    if (i>0 && input[i-1] == separator)
                    {
                        //dont allow for 0 width splits, dont allow stray characters to stay in the string not in quotes
                        iStart = i + 1;
                        continue;
                    }
                    lstReslts.Add(input.Substring(iStart, i - ((includeSplitChar)?iStart-1: iStart)));
                    iStart = i + 1;
                }
                else if (i+1 == input.Length)
                {
                    lstReslts.Add(input.Substring(iStart));
                }
            }
            return lstReslts.ToArray();
        }

        public static string[] SplitAt(string separator, string input)
        {
            var lstReslts = new List<string>();
            bool blnQuoted = false;
            int pos=0;
            while (input.Contains(separator))
            {
                int posSplit = input.IndexOf(separator,pos);
                for (int i = pos; i < posSplit; i++)
                {
                    if (input[i] == '"' && (i == 0 ||
                        (i > 0 && input[i - 1] != '\\')))
                        blnQuoted = !blnQuoted;
                }
                if (blnQuoted)
                    pos = posSplit + separator.Length;
                else if (!blnQuoted && posSplit - pos > 0 )
                {
                    lstReslts.Add(input.Substring(pos, posSplit - pos));
                    input = input.Substring(posSplit + separator.Length);
                }
            }
            return lstReslts.ToArray();
        }
    }
}
