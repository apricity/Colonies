namespace Wacton.Colonies.Interfaces
{
    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Models;

    public interface IMeasurable
    {
        Measurement Measurement { get; }
    }
}
