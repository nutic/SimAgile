//  
//  Hanna Shniukava (c) 2012 - 2013
// 

using System;
using IronPython.Hosting;
using IronPython.Modules;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using MoreLinq;
using System.Linq;

namespace SimAgile.Core
{
    public class SimulationEngine
    {
        private const string Source = @"from simulator import run
run(parameters)";

        private readonly string _simEnginePath;
        private readonly PyConfig _pyConfig;

        public SimulationEngine(string simEnginePath, PyConfig pyConfig)
        {
            _simEnginePath = simEnginePath;
            _pyConfig = pyConfig;
        }

        public object Run(SimulationParameters parameters)
        {
            return CreateEngine().Execute(Source, CreateScope(parameters));
        }

        private ScriptEngine CreateEngine()
        {
	        var runtime = Python.CreateRuntime();
	        var engine = runtime.GetEngine("py");

            engine.SetSearchPaths(engine.GetSearchPaths().Concat(_pyConfig.Pathes).Concat(_simEnginePath).ToList());
            
            return engine;
        }

        private ScriptScope CreateScope(SimulationParameters parameters)
        {
            var scope = CreateEngine().CreateScope();

            var parametersDict = new PythonDictionary
                                     {
                                         {"theseed", DateTime.Now.Ticks},
                                         {"initialBacklogSize", parameters.InitialBacklogSize},
                                         {"initialCodedSize", parameters.InitialCodedSize},
                                         {"maxTime", parameters.DateRange},
                                         {"meanDevTime", parameters.MeanDevTime},
                                         {"varDevTime", parameters.VarDevTime},
                                         {"meanTestTime", parameters.TestTimeRate},
                                         {"meanUsArrival", parameters.BacklogGrowthRate},
                                         {"developerCount", parameters.DeveloperCount},
                                         {"qaCount", parameters.QaCount},
                                         {"quality", parameters.Quality},
                                     };

            scope.SetVariable("parameters", parametersDict);

            return scope;
        }
    }
}