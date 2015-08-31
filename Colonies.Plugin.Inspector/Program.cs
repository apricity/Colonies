namespace Wacton.Colonies.Plugin.Inspector
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Colonies.Domain.Plugins;

    public class Program
    {
        public static void Main(string[] args)
        {
            var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginPath = Path.Combine(executingAssemblyPath, "Plugins");
            Console.WriteLine("Searching for plugins in ...");
            Console.WriteLine(pluginPath);

            var colonyPluginData = new List<ColonyPluginData>();
            if (Directory.Exists(pluginPath))
            {
                var pluginImporter = new PluginImporter();
                colonyPluginData = pluginImporter.Import();
            }

            Console.WriteLine("... {0} plugins found", colonyPluginData.Count);

            foreach (var pluginData in colonyPluginData)
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine("Description: {0}", pluginData.PluginDescription);
                Console.WriteLine("Name: {0}", pluginData.ColonyName);
                Console.WriteLine("Color: {0}", pluginData.ColonyColor);

                var weightedLogicTypes = pluginData.ColonyLogicTypes;
                var totalWeight = weightedLogicTypes.Sum(weightedItem => weightedItem.Weight);

                foreach (var weightedLogicType in weightedLogicTypes)
                {
                    var logicType = weightedLogicType.Item;
                    var weight = weightedLogicType.Weight;
                    var logic = (IOrganismLogic)Activator.CreateInstance(logicType);

                    var percentage = Math.Round((weight / totalWeight) * 100, 2);
                    Console.WriteLine("Logic: {0} | Chance: {1}/{2} ({3}%)", 
                        logic.Description, weightedLogicType.Weight, totalWeight, percentage);
                }

            }

            Console.WriteLine("==============================");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
