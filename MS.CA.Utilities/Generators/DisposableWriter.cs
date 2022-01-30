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
        }

        public IDisposable WriteBegin()
        {
            _builder.Append(GetStartText());
            return this;
        }

        public void Dispose()
        {
            _builder.Append(GetEndText());
        }

        protected abstract string GetStartText();
        protected abstract string GetEndText();
    }
}
