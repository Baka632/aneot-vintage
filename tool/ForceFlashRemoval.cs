namespace AnEoT.Vintage.Tool;

public sealed class ForceflashRemoval
{
    public ForceflashRemoval(string projectPath)
    {
        if (!Path.Exists(projectPath))
        {
            throw new IOException();
        }

        ProjectPath = projectPath;
    }

    public string ProjectPath { get; }
}
