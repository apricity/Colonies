namespace Wacton.Colonies.Models
{
    using Wacton.Colonies.Ancillary;

    public class Main
    {
        public Ecosystem Ecosystem { get; private set; }

        public Main(Ecosystem ecosystem)
        {
            this.Ecosystem = ecosystem;
        }

        public override string ToString()
        {
            return this.Ecosystem.ToString();
        }

        public UpdateSummary UpdateOnce()
        {
            return this.Ecosystem.Update();
        }
    }
}
