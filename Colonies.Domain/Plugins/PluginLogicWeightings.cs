namespace Wacton.Colonies.Domain.Plugins
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Tovarisch.Types;

    public class PluginLogicWeightings
    {
        private TypeDictionary<int> typeDictionary;

        public PluginLogicWeightings()
        {
            this.typeDictionary = new TypeDictionary<int>();
        }

        public void Add<T>(int weight) where T : class, IOrganismLogic
        {
            this.typeDictionary.Add<T>(weight);
        }

        public TypeDictionary<int> Get()
        {
            return this.typeDictionary;
        }
    }
}
