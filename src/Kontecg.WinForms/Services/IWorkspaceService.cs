namespace Kontecg.Services
{
    public interface IWorkspaceService
    {
        void SetupDefaultWorkspace();

        void SaveWorkspace(string workspaceName);

        void RestoreWorkspace(string workspaceName);

        void ResetWorkspace(string workspaceName);
    }
}