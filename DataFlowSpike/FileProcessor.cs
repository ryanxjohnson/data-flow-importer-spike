using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowSpike
{
    public class FileProcessor
    {
        public static void Start()
        {
            // Prepare
            var query = new TransformBlock<string, string>(async uri =>
            {
                Console.WriteLine($"Downloading '{uri}'...");

                return await GetFile(uri);
            });

            var transform = new TransformBlock<string, string>(async text =>
            {
                Console.WriteLine($"Transforming downloaded file: {text}");

                return await TransformFile(text);
            });

            var importReady = new TransformBlock<string, bool>(async text =>
            {
                Console.WriteLine($"Validating file is import ready");

                return await ValidateImportReady(text);
            });

            var import = new TransformBlock<bool, bool>(async ready =>
            {
                Console.WriteLine($"Importing data {ready}");

                return await ImportFile(ready);
            });

            var importComplete = new TransformBlock<bool, bool>(async ready =>
            {
                Console.WriteLine($"Validating import complete");

                return await ValidateImportComplete(ready);
            });

            var archive = new TransformBlock<bool, bool>(async ready =>
            {
                Console.WriteLine($"Archiving data {ready}");

                return await ImportFile(ready);
            });

            var complete = new ActionBlock<bool>(status =>
            {
                Console.WriteLine($"Finished: {status}");
            });

            //
            // Connect the dataflow blocks to form a pipeline.
            //

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            query.LinkTo(transform, linkOptions);
            transform.LinkTo(importReady, linkOptions);
            importReady.LinkTo(import, linkOptions);
            import.LinkTo(importComplete, linkOptions);
            importComplete.LinkTo(archive, linkOptions);
            archive.LinkTo(complete, linkOptions);

            query.Post("google drive");
            query.Complete();

            complete.Completion.Wait();
            Console.WriteLine("The end");            
        }

        public static async Task<string> GetFile(string uri)
        {
            Task<string> getFile = GetFileByTechnique();

            Console.WriteLine("Downloading file");

            return await getFile;
        }

        public static async Task<string> TransformFile(string text)
        {
            Task<string> transform = TransformMethod();

            Console.WriteLine("Transforming file");

            return await transform;
        }

        public static async Task<bool> ImportFile(bool ready)
        {
            if (!ready) return false;

            Task<bool> import = ImportMethod();

            Console.WriteLine("Importing data {data}");

            return await import;
        }

        public static async Task<bool> ValidateImportReady(string text)
        {
            Task<bool> import = ValidateMethod();

            Console.WriteLine("Validating import ready");

            return await import;
        }

        public static async Task<bool> ValidateImportComplete(bool ready)
        {
            Task<bool> import = ValidateMethod();

            Console.WriteLine("Validating import complete");

            return await import;
        }

        private static Task<string> GetFileByTechnique()
        {
            var repo = FileRepositoryFactory.GetFileRepository();

            TaskCompletionSource<string> tcs1 = new TaskCompletionSource<string>();
            Task<string> t1 = tcs1.Task;

            // Start a background task that will complete tcs1.Task
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                tcs1.SetResult(repo.GetFile());
            });

            return t1;
        }

        private static Task<string> TransformMethod()
        {
            TaskCompletionSource<string> tcs1 = new TaskCompletionSource<string>();
            Task<string> t1 = tcs1.Task;

            // Start a background task that will complete tcs1.Task
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                tcs1.SetResult("transform complete");
            });

            return t1;
        }

        private static Task<bool> ImportMethod()
        {
            TaskCompletionSource<bool> tcs1 = new TaskCompletionSource<bool>();
            Task<bool> t1 = tcs1.Task;

            // Start a background task that will complete tcs1.Task
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                tcs1.SetResult(true);
            });

            return t1;
        }

        private static Task<bool> ValidateMethod()
        {
            TaskCompletionSource<bool> tcs1 = new TaskCompletionSource<bool>();
            Task<bool> t1 = tcs1.Task;

            // Start a background task that will complete tcs1.Task
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                tcs1.SetResult(true);
            });

            return t1;
        }
    }
}
