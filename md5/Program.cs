using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace md5
{
    internal class Program
    {
        private static bool __Logging;
        private static bool __PauseOnComplete;

        private static void Main(string[] args)
        {
            if (args is null || args.Length == 0)
                args = new[] { Environment.CurrentDirectory };
            foreach (var arg in args)
            {
                if (File.Exists(arg)) Process(new FileInfo(arg));
                else if (Directory.Exists(arg)) Process(new DirectoryInfo(arg));
                else if (!string.IsNullOrEmpty(arg)) Process(arg);
            }

            if (!__PauseOnComplete) return;
            Console.WriteLine("Расчёт сумм завершён.");
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private static void Process(FileInfo file)
        {
            using var md5 = new MD5CryptoServiceProvider();
            using var data = file.OpenRead();
            var hash = md5.ComputeHash(data);
            PrintHash($"{file.Name} ({file.Length}b)", hash);
        }

        private static void Process(DirectoryInfo directory)
        {
            Console.WriteLine("dir:{0} - files:{1}", directory.FullName, directory.EnumerateFiles().Count());
            foreach (var file in directory.EnumerateFiles())
                Process(file);
            var sub_dirs = directory.GetDirectories();
            if (sub_dirs.Length == 0) return;
            Console.WriteLine("dir:{0} - subdirs:{1}", directory.FullName, sub_dirs.Length);
            foreach (var sub_dir in sub_dirs)
                Process(sub_dir);
        }

        private static void Process(string str)
        {
            if (str.StartsWith("-") || str.StartsWith("/") && ProcessCommand(str)) return;
            using var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(Encoding.Default.GetBytes(str));

            PrintHash($"str:\"{str}\"", hash);
        }

        private static void PrintHash(string Source, byte[] hash, string comment = null)
        {
            var str = $"{Source} - {string.Join("", hash.Select(b => b.ToString("X2")))}";
            if (!string.IsNullOrWhiteSpace(comment)) str += $" {comment}";
            Console.WriteLine(str);
            if (!__Logging) return;
            using var log = File.AppendText("md5.log");
            log.WriteLine(str);
        }

        private static bool ProcessCommand(string cmd)
        {
            if (cmd.EndsWith("log", StringComparison.InvariantCultureIgnoreCase))
                return __Logging = true;
            if (cmd.EndsWith("pause", StringComparison.InvariantCultureIgnoreCase))
                return __PauseOnComplete = true;
            return false;
        }
    }
}
