using System.Text;
using MS.CA.Utilities.Services;

namespace MS.CA.Utilities.Generators
{
    public interface IGeneratorWriter : ILanguageService
    {
        StringBuilder Builder { get; }

        GeneratorWriterOptions Options { get; }

        void IncreaseIndentationLevel();

        void DecreaseIndentationLevel();

        string GetIndentation();
    }
}
