namespace Colonies.Models
{
    using System.Collections.Generic;

    public class UpdateSummary
    {
        // TODO: might be overengineering for now, could just list all coordinates that have been affected
        public Dictionary<string, Coordinates> PreUpdateSummary { get; private set; }
        public Dictionary<string, Coordinates> PostUpdateSummary { get; private set; }

        public UpdateSummary(Dictionary<string, Coordinates> preUpdateSummary, Dictionary<string, Coordinates> postUpdateSummary)
        {
            this.PreUpdateSummary = preUpdateSummary;
            this.PostUpdateSummary = postUpdateSummary;
        }
    }
}
