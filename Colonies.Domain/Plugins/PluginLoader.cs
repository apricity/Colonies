namespace Wacton.Colonies.Domain.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Wacton.Tovarisch.Types;

    public class PluginLoader
    {
        public List<ColonyPluginData> LoadPlugins()
        {
            var pluginDatas = new List<ColonyPluginData>();

            var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginPath = Path.Combine(executingAssemblyPath, "Plugins");

            if (!Directory.Exists(pluginPath))
            {
                return new List<ColonyPluginData>();
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
