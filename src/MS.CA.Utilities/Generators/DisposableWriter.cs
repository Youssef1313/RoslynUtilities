using System;
using System.Text;

namespace MS.CA.Utilities.Generators
{
    internal abstract class DisposableWriter : IDisposable
    {
        private readonly StringBuilder _builder;

        protected DisposableWriter(IGeneratorWriter generatorWriter)
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

        internal abstract string GetStartText();
        internal abstract string GetEndText();
    }
}
