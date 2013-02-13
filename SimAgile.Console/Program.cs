//  
//  Hanna Shniukava (c) 2012 - 2013
// 

using SimAgile.Core;

namespace SimAgile.Console
{
	class Program
	{
		static void Main(string[] args)
		{
		    var answer = new SimulationEngine(@"..\..\..\SimAgile.Py", PyConfig.Default).Run(new SimulationParameters());
			System.Console.WriteLine(answer);
		}
	}
}
