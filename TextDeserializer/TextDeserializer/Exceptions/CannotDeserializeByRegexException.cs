using System.Text.RegularExpressions;

namespace ASoft.TextDeserializer.Exceptions
{
	internal class CannotDeserializeByRegexException : TextException
	{
		public CannotDeserializeByRegexException(string content, Regex regex) : base($"Couldn't deserialize content by regex pattern: {regex}. Content: {content}")
		{
		}
	}
}