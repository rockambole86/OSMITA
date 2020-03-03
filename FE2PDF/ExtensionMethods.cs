using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE2PDF
{
    public static class ExtensionMethods
    {
        public static object ToDbNull(this object value)
        {
            return value ?? DBNull.Value;
        }

        public static object ToDbNull(this int value)
        {
            return value <= 0
                ? (object) DBNull.Value
                : value;
        }

        public static object ToDbNull(this int? value)
        {
            return !value.HasValue || value.Value <= 0
                ? (object) DBNull.Value
                : value;
        }

        public static object ToDbNull(this string value)
        {
            return string.IsNullOrEmpty(value)
                ? (object) DBNull.Value
                : value;
        }

        public static object ToDbNull(this DateTime value)
        {
            return value == DateTime.MinValue ? (object) DBNull.Value : value;
        }

        public static object ToDbNull(this DateTime? value)
        {
            return value ?? (object) DBNull.Value;
        }

        public static int ToInt(this object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (value is decimal)
                return Convert.ToInt32(value);

            return ToInt(value.ToString());
        }

        public static int ToInt(this string value)
        {
            int.TryParse(value, out var ret);

            return ret;
        }

        public static int ToInt(this string value, int result)
        {
            return int.TryParse(value, out var ret)
                ? ret
                : result;
        }

		public static long ToLong(this object value)
		{
			if (value == null || value == DBNull.Value)
				return 0;

			if (value is decimal)
				return Convert.ToInt64(value);

			return ToInt(value.ToString());
		}

		public static long ToLong(this string value)
		{
            long.TryParse(value, out var ret);

			return ret;
		}

		public static long ToLong(this string value, long result)
		{
            return long.TryParse(value, out var ret)
				? ret
				: result;
		}

		public static decimal ToDecimal(this object value)
        {
            return value == null || value == DBNull.Value ? 0 : ToDecimal(value.ToString());
        }

        public static decimal ToDecimal(this string value)
        {
            decimal.TryParse(value, out var ret);

            return ret;
        }

        public static decimal ToDecimal(this string value, Decimal result)
        {
            return decimal.TryParse(value, out var ret) ? ret : result;
        }

        public static decimal ToCurrency(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            value = value.Replace("$", "");
            value = value.Replace(",", "");
            decimal.TryParse(value, out var ret);

            return ret;
        }

        public static bool ToBool(this object value)
        {
            if (value == null || value == DBNull.Value)
                return false;

            if (value is int)
                return Convert.ToBoolean(value);

            return ToBool(value.ToString());
        }

        public static bool IsInt(string s)
        {
            return !string.IsNullOrEmpty(s) && s.All(Char.IsNumber);
        }

        public static bool ToBool(this string value)
        {
            if (IsInt(value))
                return Convert.ToBoolean(ToInt(value));

            bool.TryParse(value, out var ret);

            return ret;
        }

        public static string ToCsv(this List<int> list)
        {
            return string.Join(",", list.Select(x => x.ToString()).ToArray());
        }

        public static string ToCsv(this List<string> list)
        {
            return string.Join(",", list.Select(x => x).ToArray());
        }

        public static DateTime? ToDateTime(this object value)
        {
            if (value == null || value == DBNull.Value || value.ToString() == "" || value.ToString() == "From" || value.ToString() == "To")
                return null;

            return Convert.ToDateTime(value);
        }

        public static DateTime? ToDate(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return DateTime.Parse(value);
        }

        public static string ToShortDatestring(this object date)
        {
            if (date == DBNull.Value || date == null)
                return "";

            return ((DateTime)date).ToShortDatestring();
        }

        public static string ToShortDate(this DateTime? date)
        {
	        return date.HasValue ? date.Value.ToShortDatestring() : string.Empty;
        }

	    public static string ToYesNoLetter(this bool value)
        {
            return value ? "Y" : "N";
        }

        public static string ToNA(this int? value)
        {
	        return value.HasValue ? value.ToString() : "N/A";
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool HasChanged(this string valueA, string valueB)
        {

            if (string.IsNullOrEmpty(valueA) && string.IsNullOrEmpty(valueB))
                return false;

            return valueA != valueB;
        }

        public static string NullEmptystring(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static string ToSentence(this string input)
        {
            return new string(input.ToCharArray().SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new[] { ' ', c } : new[] { c }).ToArray());
        }

        public static string RemovePunctuation(this string s)
        {
            var sb = new StringBuilder();

            foreach (var c in s.Where(c => !char.IsPunctuation(c)))
                sb.Append(c);

            return sb.ToString();
        }

        public static string SpacesToUnderscores(this string s)
        {
            return s.Replace(' ', '_');
        }

        public static int Age(this DateTime dt)
        {
            var edad = DateTime.Now.Year - dt.Year;

            if (DateTime.Now.Month < dt.Month || (DateTime.Now.Month == dt.Month && DateTime.Now.Day < dt.Day))
                edad--;

            return edad == 0 ? 1 : edad;
        }

        #region Form Controls

        public static void Fill(this System.Windows.Forms.ComboBox cbo, object dataSource, string valueField, string textField)
        {
            cbo.DataSource = dataSource;
            cbo.ValueMember = valueField;
            cbo.DisplayMember = textField;
            cbo.Update();
        }

        #endregion
    }
}
