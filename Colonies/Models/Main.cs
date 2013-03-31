namespace Colonies.Models
{
    using System.Drawing;

    public class Main
    {
        public Ecosystem Ecosystem { get; set; }

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
