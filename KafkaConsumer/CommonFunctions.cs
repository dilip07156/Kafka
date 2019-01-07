using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KafkaConsumer
{
    public static class CommonFunctions
    {
        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static string[] unitNumerMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN" };

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            char[] arr = str.Where(c => (char.IsLetterOrDigit(c))).ToArray();
            return new string(arr);
        }

        public static string RemoveSpecialCharactersAndAlphabates(string str)
        {
            StringBuilder sb = new StringBuilder();
            char[] arr = str.Where(c => (char.IsDigit(c))).ToArray();
            return new string(arr);
        }

        public static string GetDigits(string str, int length)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                str = RemoveSpecialCharactersAndAlphabates(str);
                int len = str.Length;
                if (len > length)
                    return str.Substring(len - length);
                else
                    return str;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetCharacter(string str, int lenghth)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            str = RemoveSpecialCharacters(str);
            int len = str.Length;
            if (len > lenghth)
                return str.Substring(len - lenghth);
            else
                return str;
        }

        public static string SubString(string str, int length)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            else
            {
                if (str.Length <= length || length <= 0)
                {
                    return str;
                }
                else
                {
                    return str.Substring(0, length);
                }
            }
        }

        public static string LatLongTX(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                return string.Empty;
            }

            string ret = "";
            string[] brkparam = param.Split('.');
            if (brkparam.Length > 1)
            {
                if (!string.IsNullOrEmpty(brkparam[1]))
                {
                    ret = brkparam[0] + ".";
                    if (brkparam[1].Length > 4)
                        return ret + brkparam[1].Substring(0, 4);
                    else
                        return ret + brkparam[1];
                }
                else
                    return param;
            }
            else
                return param;

        }

        public static string RemoveSpecialChars(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("'", "").Replace("!", "").Replace("#", "").Replace("\"", "");
            }
            else
            {
                return string.Empty;
            }
        }

        public static string RemoveVowels(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "");
            }
            else
            {
                return string.Empty;
            }
        }

        public static string RemoveNumbers(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");
            }
            else
            {
                return string.Empty;
            }
        }

        //public static string HotelNameTX(string HotelName, string cityname, string countryname, ref List<DataContracts.Masters.DC_Keyword> Keywords)
        //{

        //    string returnString = string.Empty;
        //    List<DC_SupplierRoomName_AttributeList> AttributeList = new List<DC_SupplierRoomName_AttributeList>();
        //    string TX_Value = string.Empty;
        //    string SX_Value = string.Empty;

        //    if (!string.IsNullOrWhiteSpace(HotelName))
        //    {
        //        returnString = CommonFunctions.TTFU(ref Keywords, ref AttributeList, ref TX_Value, ref SX_Value, HotelName, new string[] { cityname, countryname });
        //        return returnString;
        //    }
        //    else
        //        return string.Empty;
        //}

        public static string ReturnNumbersFromString(string str)
        {
            return Regex.Replace(str, @"[^\d]", "");
        }

        public static string NumberTo3CharString(int num)
        {
            int n = 0;
            string ret = "000";
            if (num < 0)
            {
                string negative = num.ToString().Replace("-", "");
                n = Convert.ToInt32(negative);
            }
            else
                n = num;

            if (n > 0)
            {
                if (n < 99)
                {
                    if (n < 9)
                    {
                        ret = "00" + n.ToString();
                    }
                    else
                        ret = "0" + n.ToString();
                }
                else
                    ret = n.ToString();
            }

            return ret;
        }

        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Remove all Html tag in a string
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveAllHtmlTag(string content)
        {
            string result = content;
            result = Regex.Replace(result, "<.*?>", " ");
            //result = Regex.Replace(result, "<(.|\n)*?>", " ");
            result = Regex.Replace(result, @"\s+", " ");

            return result;
        }

        public static string RemoveLineEndings(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", " ")
                        .Replace("\n", " ")
                        .Replace("\r", " ")
                        .Replace("\f", " ")
                        .Replace("\t", " ")
                        .Replace(lineSeparator, " ")
                        .Replace(paragraphSeparator, " ");
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

    }
}
