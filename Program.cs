using System.Diagnostics;
using OtpNet;

class Program
{
    static void Main(string[] args)
    {
        string Issuer = "test";
        string Account = "test1";
        // var generateKey = KeyGeneration.GenerateRandomKey();
        var secret = "JZOVONT73PWXTREDRNY2RFDZCEADOLJM";//Base32Encoding.ToString(generateKey);
        var genQrCode =
            $"\"otpauth://totp/{Account}?secret={Uri.EscapeDataString(secret)}&issuer={Uri.EscapeDataString(Issuer)}\"";

        Console.WriteLine("QR Code Content: " + genQrCode);

        // 執行 curl 指令
        ExecuteCurlCommand($"qrenco.de/{genQrCode}");

        // 驗證 TOTP
        while(true){
            Console.WriteLine("Enter TOTP to validate:");
            string inputTotp = Console.ReadLine();
            string validationMessage = ValidateTotp(inputTotp, secret);
            Console.WriteLine(validationMessage);
        }
    }

    static void ExecuteCurlCommand(string url)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true
        };

        Process process = new Process { StartInfo = startInfo };
        process.Start();

        process.StandardInput.WriteLine($"curl {url}");
        process.StandardInput.Flush();
        process.StandardInput.Close();

        process.WaitForExit();
        process.Close();
    }

    static Totp totpInstance = null;
    static string ValidateTotp(string totp, string secret)
    {
        if (totpInstance == null)
        {
            totpInstance = new Totp(Base32Encoding.ToBytes(secret));
        }

        long timedWindowUsed;
        if (totpInstance.VerifyTotp(totp, out timedWindowUsed))
        {
            return $"驗證通過 - {timedWindowUsed}";
        }
        else
        {
            return "驗證失敗";
        }
    }
}
