using System.Collections.Generic;

namespace ASoft.TextDeserializer
{
	public interface ITextSource
	{
		IEnumerable<string> GetPagesText();
	}
}
