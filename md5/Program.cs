using System.Security.Cryptography;
using System.Text;

if (args is not { Length: > 0 })
    args = [Environment.CurrentDirectory];

var pause_on_complete = false;
var logging = false;

foreach (var arg in args)
    if (File.Exists(arg))
        ProcessFile(new(arg));
    else if (Directory.Exists(arg))
        ProcessDirectory(new(arg));
    else if (!string.IsNullOrEmpty(arg))
        Process(arg, ref logging, ref pause_on_complete);

if (!pause_on_complete) 
    return;

Console.WriteLine("Расчёт сумм завершён.");
Console.WriteLine("Нажмите Enter для продолжения...");
Console.ReadLine();

return;

static void ProcessFile(FileInfo file)
{
    using var data = file.OpenRead();
    var hash = MD5.HashData(data);

    FormattableString inf = $"{file.Name} ({file.Length:N0}b)";
    PrintHash(inf.ToString(Consts.Culture), hash);
}

static void ProcessDirectory(DirectoryInfo directory)
{
    Console.WriteLine("dir:{0} - files:{1}", directory.FullName, directory.EnumerateFiles().Count());

    foreach (var file in directory.EnumerateFiles())
        ProcessFile(file);

    var sub_dirs = directory.GetDirectories();

    if (sub_dirs.Length == 0)
        return;

    Console.WriteLine("dir:{0} - subdirs:{1}", directory.FullName, sub_dirs.Length);

    foreach (var sub_dir in sub_dirs)
        ProcessDirectory(sub_dir);
}

static void Process(string str, ref bool Logging, ref bool PauseOnComplete)
{
    if (str is ['-' or '/', ..] && ProcessCommand(str, ref Logging, ref PauseOnComplete))
        return;

    var hash = MD5.HashData(Encoding.Default.GetBytes(str));

    PrintHash($"str:\"{str}\"", hash);
}

static void PrintHash(string Source, byte[] hash, string comment = null, bool logging = false)
{
    var str = new StringBuilder(Source.Length + 5 + 16 * 2 + (comment is { Length: > 0 } ? comment.Length + 1 : 0));
    str.Append(Source).Append(" md5:");

    foreach (var b in hash)
        str.Append($"{b:X2}");

    if (!string.IsNullOrWhiteSpace(comment))
        str.Append(' ').Append(comment);

    Console.WriteLine(str);

    if (!logging) return;
    
    using var log = File.AppendText("md5.log");
    log.WriteLine(str);
}

static bool ProcessCommand(string cmd, ref bool Logging, ref bool PauseOnComplete)
{
    Logging = false;
    PauseOnComplete = false;

    if (cmd.EndsWith("log", StringComparison.InvariantCultureIgnoreCase))
    {
        Logging = true;
        return true;
    }

    if (cmd.EndsWith("pause", StringComparison.InvariantCultureIgnoreCase))
    {
        PauseOnComplete = true;
        return true;
    }

    return false;
}
