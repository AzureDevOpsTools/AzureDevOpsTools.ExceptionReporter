using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Inmeta.Exception.Reporter
{
    public class PluginsFinder
    {
        /// <summary>
        /// Locates plugins implementation for the provided interface
        /// </summary>
        /// <typeparam name="I">Interface for which plugin implementation is looked</typeparam>
        /// <param name="path">Path to DLLs, if empty provided then Environment.CurrentDirectory</param>
        /// <returns>List of types that match the interface</returns>
        public List<Type> FindPlugins<I>(string path) 
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    path = Environment.CurrentDirectory;

                // Do a search in a separate app domain to avoid memory loading with unnessesary types
                var tempDomain = AppDomain.CreateDomain("TempPluginLoader");
                var finder = (Finder)tempDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(Finder).FullName);
                var foundPluginTypes = finder.SearchForPlugins<I>(path);
                AppDomain.Unload(tempDomain);
                return foundPluginTypes;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }

    public class Finder
    {
        private List<Type> _matchingTypes = new  List<Type>();
		internal Finder()
		{}

		internal List<Type> SearchForPlugins<T>(string path)
		{
			_matchingTypes.Clear();
			foreach(string file in Directory.GetFiles(path,"*.dll"))
			{
				TryLoadingPlugin(file, typeof(T));
			}
			return _matchingTypes;
		}

		private void TryLoadingPlugin(string path, Type type)
		{
			try
			{
				var file = new FileInfo(path);
				path = file.Name.Replace(file.Extension,"");
                
                Assembly asm= AppDomain.CurrentDomain.Load(path);
			    var types = asm.GetTypes().Where(t => t.GetInterfaces().Contains(type));

                _matchingTypes.AddRange(types);
			}
			catch(System.Exception e)
			{
			    // do nothing here
			}
		}
	}
}
