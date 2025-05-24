using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Reax.Core.Ast;

namespace Reax.Interpreter.Cache;

public static class ReaxBinSerializer
{
    private const string FILE_TYPE = "REAX_BIN";
    private const int COMPILER_VERSION = 1;
    private const string EXT_REAX_BIN = ".reax-bin";
    private const string FOLDER_BIN = ".reax-cache";

    public static void SerializeAstToBinary(string filename, ReaxNode[] ast)
    {
        var outputFile = GetOutputFilePath(filename);
        var sourceHash = CalculateHash(File.ReadAllText(filename));

        if (File.Exists(outputFile))
            File.Delete(outputFile);

        using var fs = new FileStream(outputFile, FileMode.Create);
        using var bw = new BinaryWriter(fs);

        bw.Write(FILE_TYPE);                           // Magic Header
        bw.Write(COMPILER_VERSION);                    // Versão do compilador
        bw.Write(Convert.FromHexString(sourceHash));   // Hash (32 bytes)

        var cached = new CachedAst(ast);
        cached.Serialize(bw);
    }

    public static ReaxNode[]? TryLoadAstIfHashMatches(string filename)
    {
        var outputFile = GetOutputFilePath(filename);
        if (!File.Exists(outputFile))
            return null;

        using var fs = new FileStream(outputFile, FileMode.Open);
        using var br = new BinaryReader(fs);

        var fileType = br.ReadString();
        if (fileType != FILE_TYPE)
            return null;

        var version = br.ReadInt32();
        if (version != COMPILER_VERSION)
            return null;

        var sourceHash = Convert.ToHexString(br.ReadBytes(32));
        var sourceCode = File.ReadAllText(filename);
        if (CalculateHash(sourceCode) != sourceHash)
            return null;

        var payloadSize = br.ReadInt32();
        var payload = br.ReadBytes(payloadSize);
        var json = Encoding.UTF8.GetString(payload);

        return JsonSerializer.Deserialize<ReaxNode[]>(json);
    }

    private static string GetOutputFilePath(string filename)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var cacheDirectory = Path.Combine(currentDirectory, FOLDER_BIN);
        if (!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory);
        }

        return Path.Combine(cacheDirectory, Path.GetFileNameWithoutExtension(filename) + EXT_REAX_BIN);
    }

    private static string CalculateHash(string sourceCode)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(sourceCode);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}
