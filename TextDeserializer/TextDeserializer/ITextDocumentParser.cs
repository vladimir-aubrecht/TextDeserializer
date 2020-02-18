namespace ASoft.TextDeserializer
{
	public interface ITextDocumentParser<TDocumentDescriptor> where TDocumentDescriptor : new()
	{
		TDocumentDescriptor Parse(ITextSource textSource);
	}
}