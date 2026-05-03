using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtension
{
//============================
	// Char
	//============================
	public static string Trim(this string current, char value, bool ignoreCase)
	{
		while (current.StartsAs(value, ignoreCase))
		{
			current = current.Substring(1);
		}

		while (current.EndsAs(value, ignoreCase))
		{
			current = current.Substring(0, current.Length - 1);
		}

		return current;
	}

	public static string TrimAll(this string current, params char[] values)
	{
		foreach (var value in values)
		{
			while (current.StartsAs(value, true))
			{
				current = current.Substring(1);
			}

			while (current.EndsAs(value, true))
			{
				current = current.Substring(0, current.Length - 1);
			}
		}

		return current;
	}

	public static string TrimRight(this string current, params char[] values)
	{
		foreach (var value in values)
		{
			while (current.EndsAs(value, true))
			{
				current = current.Substring(0, current.Length - 1);
			}
		}

		return current;
	}

	public static string TrimLeft(this string current, params char[] values)
	{
		foreach (var value in values)
		{
			while (current.StartsAs(value, true))
			{
				current = current.Substring(1);
			}
		}

		return current;
	}

	public static string TrimRight(this string current, char value, bool ignoreCase)
	{
		while (current.EndsAs(value, ignoreCase))
		{
			current = current.Substring(0, current.Length - 1);
		}

		return current;
	}

	public static string TrimLeft(this string current, char value, bool ignoreCase)
	{
		while (current.StartsAs(value, ignoreCase))
		{
			current = current.Substring(1);
		}

		return current;
	}

	public static bool Matches(this string current, char value, bool ignoreCase = false)
	{
		if (ignoreCase)
		{
			return current[0] == value;
		}

		return char.ToLowerInvariant(current[0]) == char.ToLowerInvariant(value);
	}

	public static bool MatchesAny(this string current, params char[] values)
	{
		foreach (var value in values)
		{
			if (current.Matches(value, true))
			{
				return true;
			}
		}

		return false;
	}

	public static string Replace(this string current, char search, char replace, bool ignoreCase)
	{
		search = ignoreCase ? char.ToLowerInvariant(search) : search;
		if (ignoreCase)
		{
			for (var index = 0; index < current.Length; ++index)
			{
				if (char.ToLowerInvariant(current[index]) == search)
				{
					current = replace + current.Remove(index, 1);
				}
			}

			return current;
		}

		for (var index = 0; index < current.Length; ++index)
		{
			if (current[index] == search)
			{
				current = replace + current.Remove(index, 1);
			}
		}

		return current;
	}

	public static string ReplaceFirst(this string current, char search, char replace, bool ignoreCase = false)
	{
		var position = current.IndexOf(search, ignoreCase);
		if (position == -1)
		{
			return current;
		}

		return current.Substring(0, position) + replace + current.Substring(position + 1);
	}

	public static string ReplaceLast(this string current, char search, char replace, bool ignoreCase = false)
	{
		var position = current.LastIndexOf(search, ignoreCase);
		if (position == -1)
		{
			return current;
		}

		return current.Substring(0, position) + replace + current.Substring(position + 1);
	}

	public static int IndexOf(this string current, char value, int start, bool ignoreCase)
	{
		if (ignoreCase)
		{
			for (var index = 0; index < current.Length; ++index)
			{
				if (char.ToLowerInvariant(current[index]) == value)
				{
					return index;
				}
			}

			return -1;
		}

		return current.IndexOf(value, start);
	}

	public static int IndexOf(this string current, char value, bool ignoreCase)
	{
		return current.IndexOf(value, 0, ignoreCase);
	}

	public static int IndexOf(this string current, char value, int start, int occurrence, bool ignoreCase)
	{
		while (occurrence > 0)
		{
			start = current.IndexOf(value, start + 1, ignoreCase) + 1;
			occurrence -= 1;
		}

		return Math.Max(start - 1, -1);
	}

	public static int LastIndexOf(this string current, char value, int start, bool ignoreCase)
	{
		if (ignoreCase)
		{
			for (var index = current.Length - 1; index > start - 1; --index)
			{
				if (char.ToLowerInvariant(current[index]) == value)
				{
					return index;
				}
			}

			return -1;
		}

		return current.LastIndexOf(value, start);
	}

	public static int LastIndexOf(this string current, char value, bool ignoreCase)
	{
		return current.LastIndexOf(value, current.Length - 1, ignoreCase);
	}

	public static int LastIndexOf(this string current, char value, int start, int occurrence, bool ignoreCase)
	{
		while (occurrence > 0)
		{
			start = current.LastIndexOf(value, start + 1, ignoreCase) + 1;
			occurrence -= 1;
		}

		return Math.Max(start - 1, -1);
	}

	public static bool EndsAs(this string current, params char[] values)
	{
		if (current.Length < 1)
		{
			return false;
		}

		foreach (var value in values)
		{
			if (current.EndsAs(value, true))
			{
				return true;
			}
		}

		return false;
	}

	public static bool EndsAs(this string current, char value, bool ignoreCase = false)
	{
		if (current.Length < 1)
		{
			return false;
		}

		if (ignoreCase)
		{
			return char.ToLowerInvariant(current[current.Length - 1]) == char.ToLowerInvariant(value);
		}

		return current[current.Length - 1] == value;
	}

	public static bool StartsAs(this string current, params char[] values)
	{
		if (current.Length < 1)
		{
			return false;
		}

		foreach (var value in values)
		{
			if (current.StartsAs(value, true))
			{
				return true;
			}
		}

		return false;
	}

	public static bool StartsAs(this string current, char value, bool ignoreCase = false)
	{
		if (current.Length < 1)
		{
			return false;
		}

		if (ignoreCase)
		{
			return char.ToLowerInvariant(current[0]) == char.ToLowerInvariant(value);
		}

		return current[0] == value;
	}

	public static bool Has(this string current, char value, bool ignoreCase)
	{
		return current.Contains(value, ignoreCase);
	}

	public static bool HasAny(this string current, params char[] values)
	{
		return current.ContainsAny(values);
	}

	public static bool HasAll(this string current, params char[] values)
	{
		return current.ContainsAll(values);
	}

	public static bool Contains(this string current, char value, bool ignoreCase)
	{
		var lower = ignoreCase ? char.ToLowerInvariant(value) : value;
		foreach (var letter in current)
		{
			if (ignoreCase)
			{
				return char.ToLowerInvariant(letter) == lower;
			}

			if (letter == value)
			{
				return true;
			}
		}

		return false;
	}

	public static bool ContainsAny(this string current, params char[] values)
	{
		foreach (var name in values)
		{
			if (current.Contains(name, true))
			{
				return true;
			}
		}

		return false;
	}

	public static bool ContainsAll(this string current, params char[] values)
	{
		foreach (var name in values)
		{
			if (!current.Contains(name, true))
			{
				return false;
			}
		}

		return true;
	}

	public static string TrySplit(this string current, char value, int index = 0)
	{
		return current.TrySplit(value.ToString(), index);
	}

	//============================
	// String
	//============================
	public static bool Is(this string current, params string[] values)
	{
		foreach (var value in values)
		{
			if (value == current)
			{
				return true;
			}
		}

		return false;
	}

	public static bool IsMD5(this string input)
	{
		if (String.IsNullOrEmpty(input))
		{
			return false;
		}

		return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
	}

	public static string Trim(this string current, string value, bool ignoreCase)
	{
		return current.TrimLeft(value, ignoreCase).TrimRight(value, ignoreCase);
	}

	public static string Trim(this string current, params string[] values)
	{
		foreach (var value in values)
		{
			current = current.TrimLeft(value);
			current = current.TrimRight(value);
		}

		return current;
	}

	public static string TrimRight(this string current, params string[] values)
	{
		foreach (var value in values)
		{
			current = current.TrimRight(value, true);
		}

		return current;
	}

	public static string TrimLeft(this string current, params string[] values)
	{
		foreach (var value in values)
		{
			current = current.TrimLeft(value, true);
		}

		return current;
	}

	public static string TrimRight(this string current, string value, bool ignoreCase)
	{
		if (value.IsEmpty())
		{
			return current;
		}

		while (current.EndsAs(value, ignoreCase))
		{
			current = current.Substring(0, current.Length - value.Length);
		}

		return current;
	}

	public static string TrimLeft(this string current, string value, bool ignoreCase)
	{
		if (value.IsEmpty())
		{
			return current;
		}

		while (current.StartsAs(value, ignoreCase))
		{
			current = current.Substring(value.Length);
		}

		return current;
	}

	public static bool Matches(this string current, string value, bool ignoreCase = false)
	{
		if (ignoreCase)
		{
			return String.Equals(current, value, StringComparison.OrdinalIgnoreCase);
		}

		return current == value;
	}

	public static bool MatchesAny(this string current, params string[] values)
	{
		foreach (var value in values)
		{
			if (current.Matches(value, true))
			{
				return true;
			}
		}

		return false;
	}

	public static string Replace(this string current, string search, string replace, bool ignoreCase)
	{
		if (ignoreCase)
		{
			search = Regex.Escape(search);
			replace = Regex.Escape(replace);
			var output = Regex.Replace(current, search, replace, RegexOptions.IgnoreCase | RegexOptions.Multiline);
			return Regex.Unescape(output);
		}

		return current.Replace(search, replace);
	}

	public static string ReplaceFirst(this string current, string search, string replace, bool ignoreCase = false)
	{
		var position = current.IndexOf(search, ignoreCase);
		if (position == -1)
		{
			return current;
		}

		return current.Substring(0, position) + replace + current.Substring(position + search.Length);
	}

	public static string ReplaceLast(this string current, string search, string replace, bool ignoreCase = false)
	{
		var position = current.LastIndexOf(search, ignoreCase);
		if (position == -1)
		{
			return current;
		}

		return current.Substring(0, position) + replace + current.Substring(position + search.Length);
	}

	public static int IndexOf(this string current, string value, int start, bool ignoreCase)
	{
		if (ignoreCase)
		{
			return current.IndexOf(value, start, StringComparison.OrdinalIgnoreCase);
		}

		return current.IndexOf(value, start);
	}

	public static int IndexOf(this string current, string value, bool ignoreCase)
	{
		return current.IndexOf(value, 0, ignoreCase);
	}

	public static int IndexOf(this string current, string value, int start, int occurrence, bool ignoreCase)
	{
		while (occurrence > 0)
		{
			start = current.IndexOf(value, start + 1, ignoreCase) + 1;
			occurrence -= 1;
		}

		return Math.Max(start - 1, -1);
	}

	public static int LastIndexOf(this string current, string value, int start, bool ignoreCase)
	{
		if (ignoreCase)
		{
			return current.LastIndexOf(value, start, StringComparison.OrdinalIgnoreCase);
		}

		return current.LastIndexOf(value, start);
	}

	public static int LastIndexOf(this string current, string value, bool ignoreCase)
	{
		return current.LastIndexOf(value, current.Length - 1, ignoreCase);
	}

	public static int LastIndexOf(this string current, string value, int start, int occurrence, bool ignoreCase)
	{
		while (occurrence > 0)
		{
			start = current.LastIndexOf(value, start + 1, ignoreCase) + 1;
			occurrence -= 1;
		}

		return Math.Max(start - 1, -1);
	}

	public static bool StartsWith(this string current, string value, bool ignoreCase)
	{
		return current.StartsAs(value, ignoreCase);
	}

	public static bool StartsWith(this string current, params string[] values)
	{
		foreach (var value in values)
		{
			if (current.StartsAs(value, true))
			{
				return true;
			}
		}

		return false;
	}

	public static bool EndsWith(this string current, string value, bool ignoreCase)
	{
		return current.EndsAs(value, ignoreCase);
	}

	public static bool EndsWith(this string current, params string[] values)
	{
		foreach (var value in values)
		{
			if (current.EndsAs(value, true))
			{
				return true;
			}
		}

		return false;
	}

	public static bool EndsAs(this string current, params string[] values)
	{
		if (current.Length < 1)
		{
			return false;
		}

		foreach (var value in values)
		{
			if (current.EndsAs(value, true))
			{
				return true;
			}
		}

		return false;
	}

	public static bool EndsAs(this string current, string value, bool ignoreCase = false)
	{
		if (value.Length > current.Length)
		{
			return false;
		}

		for (var index = 0; index < value.Length; ++index)
		{
			var original = current[current.Length - value.Length + index];
			var against = value[index];
			if (ignoreCase)
			{
				original = char.ToLowerInvariant(original);
				against = char.ToLowerInvariant(against);
			}

			if (original != against)
			{
				return false;
			}
		}

		return true;
	}

	public static bool StartsAs(this string current, params string[] values)
	{
		if (current.Length < 1)
		{
			return false;
		}

		foreach (var value in values)
		{
			if (current.StartsAs(value, true))
			{
				return true;
			}
		}

		return false;
	}

	public static bool StartsAs(this string current, string value, bool ignoreCase = false)
	{
		if (value.Length > current.Length)
		{
			return false;
		}

		for (var index = 0; index < value.Length; ++index)
		{
			var original = current[index];
			var against = value[index];
			if (ignoreCase)
			{
				original = char.ToLowerInvariant(original);
				against = char.ToLowerInvariant(against);
			}

			if (original != against)
			{
				return false;
			}
		}

		return true;
	}

	public static bool Has(this string current, string value, bool ignoreCase)
	{
		return current.Contains(value, ignoreCase);
	}

	public static bool HasAny(this string current, params string[] values)
	{
		return current.ContainsAny(values);
	}

	public static bool HasAll(this string current, params string[] values)
	{
		return current.ContainsAll(values);
	}

	public static bool Contains(this string current, string value, bool ignoreCase)
	{
		return current.IndexOf(value, 0, StringComparison.OrdinalIgnoreCase) != -1;
	}

	public static bool ContainsAny(this string current, params string[] values)
	{
		foreach (var name in values)
		{
			if (current.Contains(name, true))
			{
				return true;
			}
		}

		return false;
	}

	public static bool ContainsAll(this string current, params string[] values)
	{
		foreach (var name in values)
		{
			if (!current.Contains(name, true))
			{
				return false;
			}
		}

		return true;
	}

	public static string Remove(this string current, params string[] values)
	{
		var result = current;
		foreach (var value in values)
		{
			result = result.Replace(value, "", true);
		}

		return result;
	}

	public static string RemoveFirst(this string current, params string[] values)
	{
		var result = current;
		foreach (var value in values)
		{
			result = result.ReplaceFirst(value, "", true);
		}

		return result;
	}

	public static string RemoveLast(this string current, params string[] values)
	{
		var result = current;
		foreach (var value in values)
		{
			result = result.ReplaceLast(value, "", true);
		}

		return result;
	}

	public static string SkipFirst(this string current)
	{
		return current.SkipLeft(1);
	}

	public static string SkipLeft(this string current, int amount)
	{
		return current.Substring(amount, current.Length - amount);
	}

	public static string TakeLeft(this string current, int amount)
	{
		return current.Substring(0, amount);
	}

	public static string SkipLast(this string current)
	{
		return current.SkipRight(1);
	}

	public static string SkipRight(this string current, int amount)
	{
		return current.Substring(0, current.Length - amount);
	}

	public static string TakeRight(this string current, int amount)
	{
		return current.Substring(current.Length - amount, amount);
	}

	public static string[] Split(this string current, string value)
	{
		if (value.Length == 0 || !current.Contains(value))
		{
			return new string[1] { current };
		}

		return current.Split(new string[] { value }, StringSplitOptions.None);
	}

	public static string[] Split(this string current, string value, int maxPieces)
	{
		if (value.Length == 0 || !current.Contains(value))
		{
			return new string[1] { current };
		}

		return current.Split(new[] { value }, maxPieces, StringSplitOptions.None);
	}

	public static string[] SplitFirst(this string current, string value)
	{
		return current.Split(value, 2);
	}

	public static string GetNumbers(this string current)
	{
		return new string(current.Where(x => char.IsDigit(x)).ToArray());
	}

	public static string GetLetters(this string current)
	{
		return new string(current.Where(x => !char.IsDigit(x)).ToArray());
	}

	//============================
	// Checks
	//============================
	public static bool IsEmpty(this string text)
	{
		return String.IsNullOrEmpty(text);
	}

	public static bool IsBool(this string text)
	{
		return text.MatchesAny("true", "false", "0", "1", "t", "f");
	}

	public static bool IsInt(this string text)
	{
		return int.TryParse(text, out var number);
	}

	public static bool IsFloat(this string text)
	{
		return float.TryParse(text, out var number);
	}

	public static bool IsNumber(this string current)
	{
		return double.TryParse(current, out var result);
	}

	public static bool IsColorData(this string current)
	{
		return current.ContainsAny(",", "#");
	}

	public static bool IsEnum<Type>(this string current, bool ignoreCase = true)
	{
		try
		{
			var result = (Type)Enum.Parse(typeof(Type), current, ignoreCase);
			return !result.IsNull();
		}
		catch
		{
			return false;
		}
	}

	//============================
	// Path
	//============================
	public static bool IsPath(this string current)
	{
		return current.IsRelativePath() || current.IsAbsolutePath();
	}

	public static bool IsFolderPath(this string current)
	{
		return current.IsPath() && !current.FixPath().Split("/").Last().Contains(".");
	}

	public static bool IsRelativePath(this string current)
	{
		return current.StartsWith(".", "/", "\\");
	}

	public static bool IsAbsolutePath(this string current)
	{
		return current.Length > 1 && current[1] == ':';
	}

	public static string FixPath(this string current)
	{
		return current.Replace("\\", "/");
	}

	public static string GetParentPath(this string current)
	{
		var last = current.FixPath().TrimRight("/").LastIndexOf('/');
		if (last < 0)
		{
			return current.Contains(".") ? "" : current;
		}

		return current.Substring(0, last) + "/";
	}

	public static string GetRelativePath(this string current)
	{
		current = current.FixPath().Remove(System.IO.Directory.GetCurrentDirectory().FixPath()).TrimLeft("/", "./");
		if (!current.Contains("."))
		{
			current += "/";
		}

		return current;
	}

	public static string GetPathTerm(this string current)
	{
		return current.FixPath().Trim("/").Split("/").LastOrDefault() ?? "";
	}

	public static string GetFilename(this string current)
	{
		var term = current.FixPath().Split("/").LastOrDefault();
		if (term.Contains("."))
		{
			return term.ReplaceLast("." + current.GetFileExtension(), "");
		}

		return "";
	}

	public static string GetFileExtension(this string current)
	{
		var term = current.FixPath().Split("/").LastOrDefault();
		if (term.Contains("."))
		{
			return term.Split(".").Last();
		}

		return "";
	}

	//============================
	// Extension
	//============================
	public static string ToLetterSequence(this string current)
	{
		var lastDigit = current[current.Length - 1];
		if (current.Length > 1 && current[current.Length - 2] == ' ' && char.IsLetter(lastDigit))
		{
			var nextLetter = (char)(char.ToUpper(lastDigit) + 1);
			return current.TrimEnd(lastDigit) + nextLetter;
		}

		return current + " B";
	}

	public static string ToCapitalCase(this string current)
	{
		if (current.IsEmpty())
		{
			return current;
		}

		var value = current[0].ToString().ToUpper();
		if (current.Length > 1)
		{
			value += current.Substring(1);
		}

		return value;
	}

	public static string ToTitleCase(this string current)
	{
		var text = Regex.Replace(current, "(\\B[A-Z])", " $1");
		text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
		text = text.Replace("3 D", "3D").Replace("2 D", "2D");
		return text;
	}

	public static string ToPascalCase(this string current)
	{
		return current.ToTitleCase().Remove(" ");
	}

	public static string ToCamelCase(this string current)
	{
		if (current.IsEmpty())
		{
			return current;
		}

		var value = current[0].ToString().ToLower();
		if (current.Length > 1)
		{
			value += current.Substring(1).Remove(" ");
		}

		return value;
	}

	public static string ToFileSize(this long size)
	{
		string[] suffix = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" };
		if (size == 0)
		{
			return "0 " + suffix[0];
		}

		var bytes = Math.Abs(size);
		var place = System.Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
		var num = Math.Round(bytes / Math.Pow(1024, place), 1);
		return (Math.Sign(size) * num).ToString() + " " + suffix[place];
	}

	public static string AddLine(this string current, string value)
	{
		return current + value + "\r\n";
	}

	public static string AppendNew(this string current, string value)
	{
		if (!current.EndsWith(value))
		{
			return current + value;
		}

		return current;
	}

	public static string PrependNew(this string current, string value)
	{
		if (!current.StartsWith(value))
		{
			return value + current;
		}

		return current;
	}

	public static string[] GetLines(this string current)
	{
		return current.Remove("\r").Split("\n");
	}

	public static string Implode(this string current, string separator = " ")
	{
		var builder = new StringBuilder(current.Length * 2);
		foreach (var letter in current)
		{
			builder.Append(letter);
			builder.Append(separator);
		}

		return builder.ToString();
	}

	public static string Cut(this string current, int startIndex = 0, int endIndex = -1)
	{
		return current.Substring(startIndex, endIndex - startIndex + 1);
	}

	public static string Cut(this string current, string start = "", string end = "", int offset = 0,
		bool ignoreCase = true, int endCount = 1)
	{
		var startIndex = start == "" ? 0 : current.IndexOf(start, offset, ignoreCase);
		if (startIndex != -1)
		{
			if (end == "")
			{
				return current.Substring(startIndex);
			}

			var endIndex = current.IndexOf(end, startIndex + 1, ignoreCase);
			if (endIndex == -1)
			{
				return current.Substring(startIndex);
			}

			while (endCount > 1)
			{
				endIndex = current.IndexOf(end, endIndex + 1, ignoreCase);
				--endCount;
			}

			var distance = endIndex - startIndex + end.Length;
			return current.Substring(startIndex, distance);
		}

		return "";
	}

	public static string Parse(this string current, string start = "", string end = "", int offset = 0,
		bool ignoreCase = true, int endCount = 1)
	{
		var value = current.Cut(start, end, offset, ignoreCase, endCount);
		if (value.IsEmpty())
		{
			return "";
		}

		return value.Substring(start.Length).TrimRight(end).Trim();
	}

	public static string ParseDates(this string current)
	{
		while (current.Contains("{"))
		{
			var format = current.Parse("{", "}");
			var date = DateTime.Now.ToString(format);
			current = current.ReplaceFirst("{" + format + "}", date);
		}

		return current;
	}

	public static string FindFirst(this string current, params string[] values)
	{
		var index = -1;
		var first = "";
		foreach (var value in values)
		{
			var currentIndex = current.IndexOf(value, true);
			if (currentIndex != -1 && (index == -1 || currentIndex < index))
			{
				index = currentIndex;
				first = value;
			}
		}

		return first;
	}

	public static string StripMarkup(this string current)
	{
		return Regex.Replace(current, "<.*?>", string.Empty);
	}

	public static string Pack(this string current)
	{
		return current.Remove("\r", "\n", "'", "\"", "{", "}", "[", "]", "(", ")", "\t", " ");
	}

	public static string Condense(this string current)
	{
		while (current.ContainsAny("\t\t", "  "))
		{
			current = current.Replace("\t\t", "\t").Replace("  ", " ");
		}

		return current;
	}

	public static string Truncate(this string current, int maxLength)
	{
		return current.Length <= maxLength ? current : current.Substring(0, maxLength);
	}

	public static string TrySplit(this string current, string value, int index = 0)
	{
		if (current.Contains(value))
		{
			return current.Split(value)[index];
		}

		return current;
	}

	public static string ValueOr(this string current, string other)
	{
		return current.IsEmpty() ? other : current;
	}

	public static string SetDefault(this string current, string value)
	{
		if (current.IsEmpty())
		{
			return value;
		}

		return current;
	}

	//==================
	// IGNORE
	//==================
	public static string CutIgnore(this string current, string start, string end, string ignoreStart = "`",
		string ignoreEnd = "`")
	{
		if (start.Length == 0 || end.Length == 0 || !current.Contains(start) || !current.Contains(end))
		{
			return "";
		}

		var buffer = new StringBuilder();
		var nestedSkip = 0;
		var toggleSkip = false;
		var startIndex = current.IndexOf(start);
		foreach (var letter in current.Substring(startIndex))
		{
			buffer.Append(letter);
			if (ignoreEnd.Contains(letter) && ignoreStart.Contains(letter))
			{
				toggleSkip = !toggleSkip;
				continue;
			}

			if (toggleSkip)
			{
				continue;
			}

			if (ignoreEnd.Contains(letter))
			{
				nestedSkip -= 1;
			}

			if (ignoreStart.Contains(letter))
			{
				nestedSkip += 1;
			}

			if (nestedSkip > 0)
			{
				continue;
			}

			if (buffer.EndsWith(end))
			{
				return buffer.ToString().TrimRight(start);
			}
		}

		return buffer.ToString();
	}

	public static string[] SplitIgnore(this string current, string value, string ignoreStart = "`",
		string ignoreEnd = "`")
	{
		if (value.Length == 0 || !current.Contains(value))
		{
			return new string[1] { current };
		}

		var buffer = new List<StringBuilder>() { new StringBuilder() };
		var nestedSkip = 0;
		var toggleSkip = false;
		var index = 0;
		foreach (var letter in current)
		{
			buffer[index].Append(letter);
			if (ignoreEnd.Contains(letter) && ignoreStart.Contains(letter))
			{
				toggleSkip = !toggleSkip;
				continue;
			}

			if (toggleSkip)
			{
				continue;
			}

			if (ignoreEnd.Contains(letter))
			{
				nestedSkip -= 1;
			}

			if (ignoreStart.Contains(letter))
			{
				nestedSkip += 1;
			}

			if (nestedSkip > 0)
			{
				continue;
			}

			if (buffer[index].EndsWith(value))
			{
				buffer[index] = new StringBuilder(buffer[index].ToString().TrimRight(value));
				buffer.Add(new StringBuilder());
				index += 1;
			}
		}

		return buffer.Select(x => x.ToString()).ToArray();
	}

	public static ParsedNode SplitNodes(this string current, string start, string end, string ignoreStart = "`",
		string ignoreEnd = "`", string ignoreNext = "\\")
	{
		var depth = new Stack<ParsedNode>();
		var node = new ParsedNode();
		depth.Push(node);
		var skip = false;
		var skipNext = false;
		var ignoreIndex = 0;
		var buildBuffer = new StringBuilder();
		var buildWhole = new StringBuilder();
		foreach (var letter in current)
		{
			buildWhole.Append(letter);
			buildBuffer.Append(letter);
			if (skip || skipNext)
			{
				skip = ignoreEnd[ignoreIndex] != letter;
				skipNext = false;
				continue;
			}

			if (ignoreNext.Contains(letter))
			{
				skipNext = true;
				continue;
			}

			if (ignoreStart.Contains(letter))
			{
				skip = true;
				ignoreIndex = ignoreStart.IndexOf(letter);
			}

			if (buildBuffer.EndsWith(start))
			{
				node.buffer = buildBuffer.ToString().RemoveLast(start);
				node.whole = buildWhole.ToString().RemoveLast(start);
				node = node.children.AddNew();
				depth.Push(node);
				buildBuffer.Clear();
				buildWhole.Clear();
			}
			else if (buildBuffer.EndsWith(end))
			{
				node.buffer = buildBuffer.ToString().RemoveLast(end);
				node.whole = buildWhole.ToString().RemoveLast(end);
				node.value = node.buffer;
				var previous = depth.Pop();
				node = depth.Peek();
				node.whole += start + previous.whole + end;
				buildBuffer = new StringBuilder(node.buffer);
				buildWhole = new StringBuilder(node.whole);
			}
		}

		node.buffer = buildBuffer.ToString();
		node.whole = buildWhole.ToString();
		node.value = node.buffer;
		return node;
	}
}

public class ParsedNode
{
	public string value;
	public string buffer;
	public string whole;
	public List<ParsedNode> children = new List<ParsedNode>();
}