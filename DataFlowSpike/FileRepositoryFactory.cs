namespace DataFlowSpike
{
    public static class FileRepositoryFactory
    {
        public static IFileRepository GetFileRepository()
        {
            return new FileRepositoryLocalSystem();
        }
    }
}
