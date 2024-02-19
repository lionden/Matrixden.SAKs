using Matrixden.SwissArmyKnives.Models;
using Matrixden.Utils;
using Matrixden.Utils.Extensions;
using Matrixden.Utils.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Matrixden.SwissArmyKnives
{
    public class EmailHelper
    {
        private static readonly ILog log = Utils.Logging.LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Regex-based email validation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string value)
        {
            if (value.IsNullOrEmptyOrWhiteSpace())
                return false;

            value = value.Trim();

            var options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
            var _emailRegex = new Regex(Constants.EMAIL_PATTERN, options, TimeSpan.FromSeconds(2.0));

            return _emailRegex.IsMatch(value);
        }


        const string _fromAddr = "Reactor@matrixden.top";
        private SmtpClient _SmtpClient { get; set; }
        private MailMessage _MailMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toList">
        /// The e-mail addresses to add to the System.Net.Mail.MailAddressCollection.
        /// Multiple e-mail addresses must be separated with a comma character (",").
        /// </param>
        public EmailHelper(string toList)
        {
            _SmtpClient = new SmtpClient("smtp.qiye.aliyun.com", 25)
            {
                Credentials = new NetworkCredential(_fromAddr, "X7kzDChRDDtfc3Bv"),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            _MailMessage = new MailMessage(_fromAddr, toList);
        }

        public EmailHelper(string toList, string displayName) : this(toList)
        {
            _MailMessage.From = new MailAddress(_fromAddr, displayName);
        }

        public OperationResult Send(string subject, string body, bool isBodyHtml = false)
        {
            try
            {
                _MailMessage.Subject = subject;
                _MailMessage.Body = body;
                _MailMessage.IsBodyHtml = isBodyHtml;

                _SmtpClient.Send(_MailMessage);

                return OperationResult.True;
            }
            catch (SmtpFailedRecipientsException)
            {
                return new OperationResult($"Failed to send Email to: '{_MailMessage.To}'.");
            }
            catch (SmtpException smtpEx)
            {
                return new OperationResult(smtpEx.Message);
            }
            catch (Exception ex)
            {
                log.FatalException(String.Empty, ex);
                return new OperationResult(ex.Message);
            }
        }

        public OperationResult Send(string body, bool isBodyHtml = false) => Send("Send from Matrix Bot", body, isBodyHtml);

        public OperationResult Send() => Send("This mail is used for testing.");

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
