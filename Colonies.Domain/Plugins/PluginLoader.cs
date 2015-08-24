namespace Wacton.Colonies.Domain.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Reflection;

    using Wacton.Tovarisch.Types;

    public class PluginImporter
    {
        [ImportMany(typeof(IColonyPlugin))]
        private IEnumerable<IColonyPlugin> colonyPlugins;

        public List<ColonyPluginData> Import(bool useMef = true)
        {
            var pluginDatas = new List<ColonyPluginData>();
            var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginPath = Path.Combine(executingAssemblyPath, "Plugins");
            pluginDatas = useMef ? this.ImportUsingMef(pluginPath) : this.ImportUsingReflection(pluginPath);
            return pluginDatas;
        }

        private List<ColonyPluginData> ImportUsingMef(string pluginPath)
        {
            var pluginDatas = new List<ColonyPluginData>();

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(pluginPath));

            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            foreach (var colonyPlugin in colonyPlugins)
            {
                pluginDatas.Add(new ColonyPluginData(colonyPlugin));
            }

            return pluginDatas;
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
