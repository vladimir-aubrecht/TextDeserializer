using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ASoft.TextDeserializer.Attributes;
using ASoft.TextDeserializer.Extensions;

namespace ASoft.TextDeserializer
{
	public class TextDocumentParser<TDocumentDescriptor> : ITextDocumentParser<TDocumentDescriptor> where TDocumentDescriptor : new()
	{
		private readonly ITextRegexDeserializator textRegexDeserializator;
		private readonly Dictionary<PropertyInfo, DeserializeCollectionByRegexAttribute> deserializeCollectionByRegexAttributes;

		public TextDocumentParser(ITextRegexDeserializator textRegexDeserializator = null)
		{
			this.textRegexDeserializator = textRegexDeserializator ?? new TextTypeByRegexDeserializator();

			deserializeCollectionByRegexAttributes = CollectCollectionByRegexAttributes();
		}

		public TDocumentDescriptor Parse(ITextSource textSource)
		{
			_ = textSource ?? throw new ArgumentNullException(nameof(textSource));

			var output = new TDocumentDescriptor();

			foreach (var property in typeof(TDocumentDescriptor).GetProperties())
			{
				var body = ParseTableContent(textSource.GetPagesText(), deserializeCollectionByRegexAttributes[property]?.BodyRegex);

				var value = this.textRegexDeserializator.Deserialize(
					body,
					property.PropertyType,
					typeof(TDocumentDescriptor),
					deserializeCollectionByRegexAttributes[property]?.CollectionRegex,
					GetDeserializationRegexByType(typeof(TDocumentDescriptor)),
					GetDeserializationRegexByType(property.PropertyType),
					GetDeserializationRegexByType(property.PropertyType.GetCollectionElementType()));

				if (this.textRegexDeserializator.IsSimpleType(property.PropertyType))
				{
					property.SetValue(output, property.GetValue(value));
				}
				else
				{
					property.SetValue(output, value);
				}
			}

			return output;
		}

		private Dictionary<PropertyInfo, DeserializeCollectionByRegexAttribute> CollectCollectionByRegexAttributes()
		{
			return typeof(TDocumentDescriptor).GetProperties()
				.ToDictionary(k => k, i => i.GetCustomAttribute<DeserializeCollectionByRegexAttribute>(false));
		}

		private Regex GetDeserializationRegexByType(Type type)
		{
			return type?.GetCustomAttribute<DeserializeByRegexAttribute>(true)?.DeserizalizationRegex;
		}

		private string ParseTableContent(IEnumerable<string> content, Regex pageBodyRegex)
		{
			if (pageBodyRegex == null)
			{
				return String.Join("", content);
			}

			var contents = content
				.Where(pageText => pageBodyRegex.Match(pageText).Success)
				.Select(pageText => pageBodyRegex.Match(pageText).Groups[1].Value);

			return String.Join("", contents); //Lets merge tables splitted cross multiple pages
		}
	}
}
