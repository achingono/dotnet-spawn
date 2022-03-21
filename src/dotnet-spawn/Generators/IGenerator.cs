using System.Threading.Tasks;

namespace Spawn.Generators
{
    public interface IGenerator
    {
        string Name { get; }
        Task GenerateAsync(string targetNamespace, string targetPath);
    }
}