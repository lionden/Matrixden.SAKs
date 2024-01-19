/**
 * 邮件相关Helper.
 */

namespace Matrixden.Utils
{
    using Matrixden.Utils.Logging;
    using Matrixden.Utils.Extensions;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;

    public class Mail
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the from address for this e-mail message.
        /// </summary>
        /// <returns>A <see cref="T:System.Net.Mail.MailAddress" /> that contains the from address information.</returns>
        public string From
        {
            get
            {
                return MailMessage.From.Address;
            }
            set
            {
                MailMessage.From = new MailAddress(value);
            }
        }

        /// <summary>
        /// 显示的发件人名称
        /// </summary>
        public string DisplayName
        {
            get
            {
                return MailMessage.From.DisplayName;
            }
            set
            {
                MailMessage.From = new MailAddress(From, value);
            }
        }

        /// <summary>
        /// Gets the address collection that contains the recipients of this e-mail message.
        /// </summary>
        /// <returns>A writable <see cref="T:System.Net.Mail.MailAddressCollection" /> object.</returns>
        public string ToList
        {
            get
            {
                return MailMessage.To.ToString();
            }
            set
            {
                MailMessage.To.Add(value);
            }
        }

        /// <summary>
        /// Gets the address collection that contains the carbon copy (CC) recipients for this e-mail message.
        /// </summary>
        /// <returns>A writable <see cref="T:System.Net.Mail.MailAddressCollection" /> object.</returns>
        public string CCList
        {
            get
            {
                return MailMessage.CC.ToString();
            }
            set
            {
                MailMessage.CC.Add(value);
            }
        }

        /// <summary>
        /// Gets the address collection that contains the blind carbon copy (BCC) recipients for this e-mail message.
        /// </summary>
        /// <returns>A writable <see cref="T:System.Net.Mail.MailAddressCollection" /> object.</returns>
        public string BccList
        {
            get
            {
                return MailMessage.Bcc.ToString();
            }
            set
            {
                MailMessage.Bcc.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the priority of this e-mail message.
        /// </summary>
        /// <returns>A <see cref="T:System.Net.Mail.MailPriority" /> that contains the priority of this message.</returns>
        public MailPriority Priority
        {
            get
            {
                return MailMessage.Priority;
            }
            set
            {
                MailMessage.Priority = value;
            }
        }

        /// <summary>
        /// Gets or sets the subject line for this e-mail message.
        /// </summary>
        /// <returns>A <see cref="T:System.String" /> that contains the subject content.</returns>
        public string Subject
        {
            get
            {
                return MailMessage.Subject;
            }
            set
            {
                MailMessage.Subject = value;
            }
        }

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        /// <returns>A <see cref="T:System.String" /> value that contains the body text.</returns>
        public string Body
        {
            get
            {
                return MailMessage.Body;
            }
            set
            {
                MailMessage.Body = value;
            }
        }

        /// <summary>
        /// Gets or sets the encoding used to encode the message body.
        /// </summary>
        /// <returns>An <see cref="T:System.Text.Encoding" /> applied to the contents of the <see cref="P:System.Net.Mail.MailMessage.Body" />.</returns>
        public Encoding BodyEncoding { get; set; }

        private MailMessage _mailMessage;
        /// <summary>
        /// 发送邮件的内容（如：收发人地址、标题、主体、图片等等）
        /// </summary>
        public MailMessage MailMessage
        {
            get
            {
                if (_mailMessage == null)
                {
                    _mailMessage = new MailMessage();
                }

                return _mailMessage;
            }
            private set
            {
                _mailMessage = value;
            }
        }

        private SmtpClient _smtpClient;
        /// <summary>
        /// 用smtp方式发送此邮件的配置信息（如：邮件服务器、发送端口号、验证方式等等）
        /// </summary>
        private SmtpClient SmtpClient
        {
            get
            {
                if (_smtpClient == null)
                {
                    _smtpClient = new SmtpClient();
                }

                return _smtpClient;
            }
            set
            {
                _smtpClient = value;
            }
        }

        /// <summary>
        /// 发件箱的邮件服务器地址（IP形式或字符串形式均可）
        /// </summary>
        public string SenderServerHost
        {
            get
            {
                return SmtpClient.Host;
            }
            set
            {
                SmtpClient.Host = value;
            }
        }

        /// <summary>
        /// 邮件所用的端口号（http协议默认为25）
        /// </summary>
        public int SenderServerPort
        {
            get
            {
                return SmtpClient.Port;
            }
            set
            {
                SmtpClient.Port = value;
            }
        }

        /// <summary>
        /// 是否启用SSL加密
        /// </summary>
        public bool EnableSsl
        {
            get
            {
                return SmtpClient.EnableSsl;
            }
            set
            {
                SmtpClient.EnableSsl = value;
            }
        }

        /// <summary>
        /// 发件箱的用户名（即@符号前面的字符串，例如：hello@world.com，用户名为：hello）
        /// </summary>
        public string SenderUsername { get; set; }
        /// <summary>
        /// 发件箱的密码
        /// </summary>
        public string SenderPassword { get; set; }
        /// <summary>
        /// 是否对发件人邮箱进行密码验证
        /// </summary>
        public bool EnablePwdAuthentication { get; set; }

        public Mail() : this(null, null, default(bool)) { }
        public Mail(string subject, string mailBody, bool isBodyHtml) : this(null, null, subject, mailBody) { MailMessage.IsBodyHtml = isBodyHtml; }
        public Mail(string from, string toList, string subject, string mailBody, string cCList = null)
            : this(from, toList, cCList, null, subject, mailBody, null, 25, false) { }

        public Mail(string fromAddress, string toList, string cCList, string bccList, string subject, string mailBody, string host, int port, bool enableSsl)
        {
            if (StringHelper.IsNullOrEmptyOrWhiteSpace(toList, fromAddress))
            {
                return;
            }

            MailMessage.From = new MailAddress(fromAddress);
            MailMessage.To.Add(toList);

            if (!StringHelper.IsNullOrEmptyOrWhiteSpace(cCList))
            {
                MailMessage.CC.Add(cCList);
            }
            if (!StringHelper.IsNullOrEmptyOrWhiteSpace(bccList))
            {
                MailMessage.Bcc.Add(bccList ?? string.Empty);
            }

            MailMessage.Subject = subject;
            MailMessage.Body = mailBody;
            MailMessage.IsBodyHtml = true;

            if (!StringHelper.IsNullOrEmptyOrWhiteSpace(host))
            {
                SmtpClient.Host = host;
            }
            SmtpClient.Port = port;
            SmtpClient.EnableSsl = enableSsl;
        }

        ///<summary>
        /// 添加附件
        ///</summary>
        ///<param name="attachmentsPath">附件的路径集合，以分号(;)分隔</param>
        public void AddAttachments(string attachmentsPath)
        {
            if (attachmentsPath.IsNullOrEmptyOrWhiteSpace())
                return;

            try
            {
                string[] path = attachmentsPath.Split(';'); //以什么符号分隔可以自定义
                Attachment data;
                ContentDisposition disposition;
                for (int i = 0; i < path.Length; i++)
                {
                    data = new Attachment(path[i], MediaTypeNames.Application.Octet);
                    disposition = data.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(path[i]);
                    disposition.ModificationDate = File.GetLastWriteTime(path[i]);
                    disposition.ReadDate = File.GetLastAccessTime(path[i]);
                    MailMessage.Attachments.Add(data);
                }
            }
            catch (IOException ioEx)
            {
                log.ErrorException("Failed to add attachments, attachments' path=[{0}].", ioEx, attachmentsPath);
            }
            catch (Exception ex)
            {
                log.FatalException("Unknown exception occured.", ex);
            }
        }

        public void Send(bool isHtmlBody)
        {
            MailMessage.IsBodyHtml = isHtmlBody;
            Send();
        }

        ///<summary>
        /// 发送邮件
        ///</summary>
        public void Send()
        {
            try
            {
                if (MailMessage == null)
                {
                    log.Error("Mail message is null.");
                    return;
                }

                if (this.EnablePwdAuthentication)
                {
                    if ((this.SenderUsername ?? this.From).IsNullOrEmptyOrWhiteSpace() || this.SenderPassword.IsNullOrEmptyOrWhiteSpace())
                    {
                        log.Fatal("User name or password is null.");
                        return;
                    }

                    NetworkCredential nc = new NetworkCredential(this.SenderUsername ?? this.From, this.SenderPassword);
                    SmtpClient.Credentials = nc.GetCredential(SmtpClient.Host, SmtpClient.Port, "NTLM");
                }
                else
                {
                    SmtpClient.Credentials = new NetworkCredential(this.From ?? this.From, this.SenderPassword);
                }
                SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpClient.Send(MailMessage);
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to send E-Mail.", ex);
            }
        }
    }
}