//  
//  Hanna Shniukava (c) 2012 - 2013
// 

using System.Collections.Generic;
using IronPython.Modules;
using IronPython.Runtime.Types;

namespace SimAgile.Core
{
    public class PyConfig
    {
        private readonly IEnumerable<string> _pathes;

        public static readonly PyConfig Default = new PyConfig(
            new[]
                {
                    "C:\\Python27",
                    "C:\\Python27\\Lib",
                    "C:\\Python27\\Lib\\site-packages\\simpy-2.3.1-py2.7.egg"
                });

        // ReSharper disable NotAccessedField.Local
        // ReSharper disable UnusedMember.Local
        // TODO: to enforce dependency on IronPython.Modules
        private PythonType _arrayModule = ArrayModule.ArrayType;
        // ReSharper restore UnusedMember.Local
        // ReSharper restore NotAccessedField.Local

        public PyConfig(IEnumerable<string> pathes)
        {
            _pathes = pathes;
        }

        public IEnumerable<string> Pathes
        {
            get { return _pathes; }
        }
    }
}