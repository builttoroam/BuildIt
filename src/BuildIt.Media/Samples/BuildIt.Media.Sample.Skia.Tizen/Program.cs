using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace BuildIt.Media.Sample.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new BuildIt.Media.Sample.App(), args);
		host.Run();
	}
}
}
