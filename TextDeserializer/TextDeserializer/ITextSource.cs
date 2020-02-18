using System;
using System.Collections.Generic;

namespace ASoft.TextDeserializer
{
	public interface ITextSource : IDisposable
	{
		IEnumerable<string> GetPagesText();
	}
}
