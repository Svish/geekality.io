using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geekality.IO
{
    public class EmbeddedResource
    {
        /// <summary>
        /// Get an embedded resource from the currently loaded assembly where type T is specified.
        /// </summary>
        /// <typeparam name="T">A type specified in wanted assembly.</typeparam>
        /// <param name="name">Name of the resource.</param>
        /// <param name="useNamespaceOfType">If true, the namespace of the type will be prefixed to the name. Defaults to true.</param>
        /// <returns>Stream with embedded resourc.e</returns>
        public static Stream Get<T>(string name, bool useNamespaceOfType = true)
        {
            var assembly = Assembly.GetAssembly(typeof(T));
            var s = useNamespaceOfType
                ? assembly.GetManifestResourceStream(typeof(T), name)
                : assembly.GetManifestResourceStream(name);

            if (s != null)
                return s;
            throw new ArgumentException("Embedded resource not found.");
        }


        /// <summary>
        /// Get an embedded resource from the calling assembly.
        /// </summary>
        /// <typeparam name="T">A type specified in wanted assembly.</typeparam>
        /// <param name="name">Name of the resource.</param>
        /// <returns>Stream with embedded resource.</returns>
        public static Stream Get(string name)
        {
            var s = Assembly.GetCallingAssembly().GetManifestResourceStream(name);
            if (s != null)
                return s;
            throw new ArgumentException("Embedded resource not found.");
        }
    }
}
