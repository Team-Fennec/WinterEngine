namespace ModelCompiler;

public class ModelConfig
{
	public const string FormatName = "wemodelconfig";
	public const int FormatVersion = 1;

	public string Name;
	public string ReferenceModel;
	public List<String> Animations;

	public ModelConfig(string filename)
	{
		// read model info from keyvalues v2 file
	}
}
