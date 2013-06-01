namespace Wacton.Colonies.Models
{
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
    }
}
