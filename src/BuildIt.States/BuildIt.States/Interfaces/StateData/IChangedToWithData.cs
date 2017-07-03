using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    public interface IChangedToWithData
    {
        Task ChangedToWithData(string dataAsJson);
    }
}