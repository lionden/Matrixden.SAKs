using Matrixden.Utils;
using Matrixden.Utils.Extensions;
using Matrixden.Utils.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives
{
    public class EmailHelper
    {
        public static OperationResult Validate(string mailAddress)
        {
            //// 1. 对Email地址字符串进行简单的格式检验
            Regex reg = new Regex(Constants.EMAIL_PATTERN);
            if (!reg.IsMatch(mailAddress))
            {
                return new OperationResult("Email Format error!");
            }

            //// 2. 通过nslookup程序查询MX记录，获取域名对应的mail服务器
            string mailServer = string.Empty,
             strDomain = mailAddress.Split('@')[1];
            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.FileName = "nslookup";
            info.CreateNoWindow = true;
            info.Arguments = $"-type=mx {strDomain}";
            Process ns = Process.Start(info);
            StreamReader sout = ns.StandardOutput;
            reg = new Regex("mail exchanger = (?<mailServer>[^\\s].*)");
            string strResponse = "";
            while ((strResponse = sout.ReadLine()) != null)
            {
                Match amatch = reg.Match(strResponse);
                if (reg.Match(strResponse).Success)
                    mailServer = amatch.Groups["mailServer"].Value;
            }

            if (mailServer.IsNullOrEmptyOrWhiteSpace())
            {
                return new OperationResult("Email Server error!");
            }

            //// 3. 连接邮件服务器，确认服务器的可用性和用户是否存在
            TcpClient tcpc = new TcpClient();
            tcpc.NoDelay = true;
            tcpc.ReceiveTimeout = 3000;
            tcpc.SendTimeout = 3000;
            try
            {
                tcpc.Connect(mailServer, 25);
                NetworkStream s = tcpc.GetStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                StreamWriter sw = new StreamWriter(s, Encoding.Default);
                string strTestFrom = mailAddress;
                //// 3.1 写入HELO命令
                sw.WriteLine($"helo {mailServer}");
                strResponse = sr.ReadLine();

                //// 3.2 写入Mail From命令
                sw.WriteLine($"mail from:<{mailAddress}>");
                strResponse = sr.ReadLine();

                //// 3.3 写入RCPT命令，这是关键的一步，后面的参数便是查询的Email的地址
                sw.WriteLine($"rcpt to:<{strTestFrom}>");
                strResponse = sr.ReadLine();
                if (!strResponse.StartsWith("2"))
                {
                    return new OperationResult("UserName error!");
                }
                sw.WriteLine("quit");

                return OperationResult.True;
            }
            catch (Exception ex)
            {
                return new OperationResult(ex.Message);
            }
        }
    }
}
