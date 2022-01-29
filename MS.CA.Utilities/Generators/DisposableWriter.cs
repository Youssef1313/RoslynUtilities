using System;
using System.Text;

namespace MS.CA.Utilities.Generators
{
    internal abstract class DisposableWriter : IDisposable
    {
        private readonly StringBuilder _builder;

        public DisposableWriter(IGeneratorWriter generatorWriter)
        {
            _builder = generatorWriter.Builder;
            _builder.AppendLine(GetStartText());
        }

        public void Dispose()
        {
            _builder.AppendLine(GetEndText());
        }

        protected abstract string GetStartText();
        protected abstract string GetEndText();
    }
}
