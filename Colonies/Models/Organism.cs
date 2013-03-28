namespace Colonies
{
    public class Organism
    {
        private readonly string organismID;

        public Organism(string organismID)
        {
            this.organismID = organismID; 
        }

        public override string ToString()
        {
            return this.organismID;
        }
    }
}
