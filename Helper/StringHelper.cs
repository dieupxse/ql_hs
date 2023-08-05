using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace QL_HS.Helper
{
    public class StringHelper
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public static int CountTotalPage(int total, int rowPerPage)
        {
            if (total % rowPerPage > 0)
            {
                return (total / rowPerPage) + 1;
            }
            else
            {
                return (total / rowPerPage);
            }
        }

        public static string RandomCode(int len, bool spechar, bool number)
        {
            var alpha = "QWERTYUIOPLKJHGFDSAZXCVBNM";
            var num = "1234567890";
            var spec = "~!@#$%^&*()`<>,.?'\"\\}{][+_|";
            var str = alpha;
            var rs = "";
            if (spechar) str += spec;
            if (number) str += num;
            Random rd = new Random();
            for (int i = 1; i <= len; i++)
            {
                rs += str[rd.Next(0, str.Length - 1)];
            }
            return rs;
        }

        public static string RandomOTP(int len)
        {
            var str = "1234567890";
            var rs = "";
            Random rd = new Random();
            for (int i = 1; i <= len; i++)
            {
                rs += str[rd.Next(0, str.Length - 1)];
            }
            return rs;
        }

        public static string GenerateNiceUrl(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            char[] ch = { '-' };
            temp = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            temp = Regex.Replace(temp, "[^0-9a-zA-Z-\\s]+", "");
            temp = string.Join("-", temp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return Regex.Replace(temp, @"\-+", @"-").TrimStart(ch).TrimEnd(ch).ToLower();
        }

        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return random.Next(min, max);
            }

        }

        public static Tuple<DateTime, DateTime> GetDateRangeQuery(
          string start,
          string end)
        {
            start = start ?? "";
            end = end ?? "";
            DateTime dateTime = DateTime.Now.AddDays(-30.0);
            DateTime now = DateTime.Now;
            if (!string.IsNullOrEmpty(start))
                dateTime = DateTime.Parse(start);
            if (!string.IsNullOrEmpty(end))
                now = DateTime.Parse(end);
            return Tuple.Create<DateTime, DateTime>(dateTime, now);
        }

        public static Tuple<DateTime, DateTime> StaticGetDateRangeQuery(
  string start,
  string end)
        {
            start = start ?? "";
            end = end ?? "";
            DateTime dateTime = DateTime.Now.AddDays(-30.0);
            DateTime now = DateTime.Now;
            if (!string.IsNullOrEmpty(start))
                dateTime = DateTime.Parse(start);
            if (!string.IsNullOrEmpty(end))
                now = DateTime.Parse(end);
            return Tuple.Create<DateTime, DateTime>(dateTime, now);
        }

        public static string ConvertToUnSign(string input)
        {
            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty);
            string str3 = str2.Replace('đ', 'd').Replace('Đ', 'D');

            return str3;
        }

        public static string GenerateSlug(string phrase)
        {
            string str = ConvertToUnSign(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static int StaticCountTotalPage(int total, int rowPerPage)
        {
            if (total % rowPerPage > 0)
            {
                return total / rowPerPage + 1;
            }
            return total / rowPerPage;
        }

        public static List<T> StaticGetList<T>(string data, char mark)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            List<T> list = new List<T>();
            Type type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                type = Nullable.GetUnderlyingType(type);
            }
            string[] array = data.Split(mark);
            foreach (string value in array)
            {
                T item = (T)Convert.ChangeType(value, type);
                list.Add(item);
            }
            return list;
        }

        public static string StaticStripVietnamese(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}
