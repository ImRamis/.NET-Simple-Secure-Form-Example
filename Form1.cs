#region

using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

#endregion

namespace pw_work
{
    public partial class Form1 : Form
    {
        private static readonly Random R = new Random();
        private readonly UnicodeEncoding _byteConverter = new UnicodeEncoding();

        private readonly string _ip = Dns.GetHostEntry("localhost")
            .AddressList
            .FirstOrDefault(t => t.AddressFamily == AddressFamily.InterNetwork)
            ?.ToString();
        private readonly string _pw;
        private readonly string _un;


        private int _logintimes;
        private SmtpClient _mailClient;

        private RSAParameters _rsaParams = new RSAParameters
        {
            Modulus = new byte[]
            {
                22, 126, 210, 83, 160, 73, 40, 39, 201, 155, 19, 202, 3, 11, 191, 178, 56,
                74, 20, 36, 248, 103, 18, 144, 170, 163, 145, 87, 54, 61, 34, 220, 222,
                207, 137, 139, 173, 14, 92, 120, 206, 222, 158, 28, 40, 24, 30, 16, 175,
                108, 128, 35, 230, 118, 40, 121, 113, 125, 216, 130, 11, 24, 90, 48, 194,
                240, 105, 44, 76, 34, 57, 249, 228, 125, 80, 38, 9, 136, 29, 117, 207, 139,
                168, 181, 85, 137, 126, 10, 126, 242, 120, 247, 121, 8, 100, 12, 201, 171,
                38, 226, 193, 180, 190, 117, 177, 87, 143, 242, 213, 11, 44, 180, 113, 93,
                106, 99, 179, 68, 175, 211, 164, 116, 64, 148, 226, 254, 172, 147
            }
        };

        public Form1()
        {
            InitializeComponent();
            label5.Text = @"IP : " + _ip;
            _un = "theU6neal";
            _pw = "objk43n$!@";
            textBox6.Text = @"Username" + Environment.NewLine + RsaEncrypt(_un) + Environment.NewLine +
                            Environment.NewLine;
            textBox6.Text += @"Password" + Environment.NewLine + RsaEncrypt(_pw) + Environment.NewLine;
        }

        public string CodeGen(string n = "")
        {
            return n.Length >= 5 ? n : CodeGen(n + (char) R.Next(65, 90));
        }

        private void SendMail(string code)
        {
            _mailClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("mailrandom16@gmail.com", "cryptography"),
                EnableSsl = true
            };
            var message =
                new MailMessage("mailrandom16@gmail.com", "mailrandom16@gmail.com")
                {
                    Subject = "Verify your login from unknown location",
                    Body = @"Your Verification Code is " + code
                };
            try
            {
                _mailClient.Send(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == _un && textBox3.Text == _pw)
            {
                if (checkBox1.Checked)
                {
                    MessageBox.Show(@"Login Successfull!");
                    textBox5.Text += @"Login Successfull from IP : " + _ip + Environment.NewLine +
                                     @"SMS : You Logged in at example" + Environment.NewLine;
                }
                else
                {
                    if (MessageBox.Show(@"Verify by SMS ? Yes and No for Email ", @"Verification Method",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        textBox5.Text += @"SMS : Your Verification Code is '58AB'" + Environment.NewLine;
                        while (Prompt.ShowDialog("Enter SMS Code", "Verify") != "58AB") ;
                        textBox5.Text += @"Login Successfull from IP : " + _ip + Environment.NewLine +
                                         @"SMS : You Logged in at example" + Environment.NewLine;
                        MessageBox.Show(@"Login Successfull!");
                    }
                    else
                    {
                        var verificationcode = CodeGen();
                        textBox5.Text += @"EMAIL Sent Check your email for verification code;" + Environment.NewLine;
                        SendMail(verificationcode);
                        while (Prompt.ShowDialog("Enter EMAIL Code", "Verify") != verificationcode) ;
                        textBox5.Text += @"Login Successfull from IP : " + _ip + Environment.NewLine +
                                         @"SMS : You Logged in at example" + Environment.NewLine;
                        MessageBox.Show(@"Login Successfull!");
                    }
                }
            }
            else
            {
                if (_logintimes++ < 3)
                    textBox5.Text += @"Login Failed from IP : " + _ip + Environment.NewLine + @"Email Notified " +
                                     Environment.NewLine + @"SMS : Suspicious Activity" + Environment.NewLine;
                else
                    textBox5.Text += @"Account Temporary Locked!" + Environment.NewLine;
                ;
            }
        }


        private string RsaEncrypt(string value)
        {
            var plaintext = Encoding.Unicode.GetBytes(value);

            var cspParams = new CspParameters();
            cspParams.KeyContainerName = _byteConverter.GetString(_rsaParams.Modulus);
            using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
            {
                var encryptedData = rsa.Encrypt(plaintext, false);
                return Convert.ToBase64String(encryptedData);
            }
        }
        private string RsaDecrypt(string value)
        {
            var encryptedData = Convert.FromBase64String(value);
            var cspParams = new CspParameters {KeyContainerName = _byteConverter.GetString(_rsaParams.Modulus)};
            using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
            {
                var decryptedData = rsa.Decrypt(encryptedData, false);
                return Encoding.Unicode.GetString(decryptedData);
            }
        }
    }
}