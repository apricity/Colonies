namespace Wacton.Colonies.Domain.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Wacton.Tovarisch.Types;

    public class PluginImporter
    {
        // this is assigned by the container.ComposeParts() in ImportUsingMef()
        [ImportMany(typeof(IColonyPlugin))]
        private IEnumerable<IColonyPlugin> colonyPlugins;

        public List<ColonyPluginData> Import(bool useMef = true)
        {
            var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginPath = Path.Combine(executingAssemblyPath, "Plugins");
            return useMef ? this.ImportUsingMef(pluginPath) : this.ImportUsingReflection(pluginPath);
        }

        private List<ColonyPluginData> ImportUsingMef(string pluginPath)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(pluginPath));

            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            return this.colonyPlugins.Select(colonyPlugin => new ColonyPluginData(colonyPlugin)).ToList();
        }

        private List<ColonyPluginData> ImportUsingReflection(string pluginPath)
        {
            var pluginDatas = new List<ColonyPluginData>();

            if (!Directory.Exists(pluginPath))
            {
                return pluginDatas;
            }

            var pluginFiles = Directory.GetFiles(pluginPath, "*.dll");

            foreach (var pluginFile in pluginFiles)
            {
                var pluginAssembly = Assembly.LoadFile(pluginFile);
                foreach (var exportedType in pluginAssembly.GetExportedTypes())
                {
                    if (!exportedType.IsImplementationOf<IColonyPlugin>())
                    {
                        continue;
                    }

                    var colonyPlugin = (IColonyPlugin)Activator.CreateInstance(exportedType);
                    pluginDatas.Add(new ColonyPluginData(colonyPlugin));
                }
            }
            
            return pluginDatas;
        }
    }
}
