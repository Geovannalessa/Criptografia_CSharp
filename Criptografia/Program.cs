using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.NetworkInformation;
using System.Globalization;

namespace Criptografia
{
	class Program
	{
		static void Main(string[] args)
		{
			string fraseCriptografar, fraseDescriptografar;
			byte[] criptografado;
			bool repete = true;

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("----------------------");
			Console.WriteLine("Bem vindo ao Criptando");
			Console.WriteLine("----------------------");
			Console.ResetColor();
			using (Aes aes = Aes.Create())
			{
				//converte a chave que o usuario escolher para base 64
				aes.GenerateKey();
				//protege a criptografiaçao 
				aes.GenerateIV();

				while (repete)
				{
					Console.Write(
						"Deseja: \n" +
						"1- Criptografar \n" +
						"2- Descriptografar\n"
					 );
					Console.Write("- ");
					string resposta = Console.ReadLine();
					Console.WriteLine();
					try
					{
						if (resposta == "1" || resposta == "Criptografar")
						{
							Console.Write("Escreva o que deseja criptografar: ");
							fraseCriptografar = Console.ReadLine();
							//de fato criptografando
							criptografado = Criptografar(fraseCriptografar, aes.Key, aes.IV);
							string base64Criptografado = Convert.ToBase64String(criptografado);
							
							Console.Write("Frase Criptografado: " );
							ColorirFrase($"{base64Criptografado}", 2);
							Console.WriteLine();
						}
						else if (resposta == "2" || resposta == "Descriptografar")
						{
							Console.Write("Escreva o que deseja descriptografar: ");
							fraseDescriptografar = Console.ReadLine();
							byte[] textoBytes = Convert.FromBase64String(fraseDescriptografar);
							string fraseDescriptografada = Descriptografar(textoBytes, aes.Key, aes.IV);
							Console.Write("Frase Descriptografado: ");
							ColorirFrase($"{fraseDescriptografada}", 2);
							Console.WriteLine();
						}
						else
						{
							Console.Write("Escolha inválida!");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Ocorreu um erro: " + ex.Message);
					}
					

					Console.WriteLine();
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine("deseja repetir? sim/ nao ");
					Console.ResetColor();
					Console.Write("- ");
					string respostaRepeticao = Console.ReadLine();

					if (respostaRepeticao == "sim" || respostaRepeticao == "s")
					{
						repete = true;
					}
					else if (respostaRepeticao == "nao" || respostaRepeticao == "n")
					{
						repete = false;
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.WriteLine();
						Console.WriteLine("----------------------");
						Console.WriteLine("    Até a próxima!");
						Console.WriteLine("----------------------");
						Console.ResetColor();
					}
					else
					{
						repete = false;
					}
					Console.WriteLine();
				}
			}
		}

		public static byte[] Criptografar(string texto, byte[] chave, byte[] IV)
		{
			try
			{
				//testando se o todos os paramentros possuem valor
				if (texto == null || texto.Length <= 0)
				{
					throw new Exception("Não foi enviado o texto");
				}
				if (chave == null || chave.Length <= 0)
				{
					throw new Exception("Não foi enviado a chave");
				}
				if (IV == null || IV.Length <= 0)
				{
					throw new Exception("Não foi enviado o IV");
				}

				byte[] criptografado;

				using (Aes aes = Aes.Create())
				{
					aes.Key = chave;
					aes.IV = IV;
					//criptografando
					ICryptoTransform criptografa = aes.CreateEncryptor(aes.Key, aes.IV);

					//instancia um lugar na memoria onde vai ficar a criptografia
					using (MemoryStream memoria = new MemoryStream())
					{
						using (CryptoStream processoDeCriptografia = new CryptoStream(memoria, criptografa, CryptoStreamMode.Write))
						{
							using (StreamWriter sw = new StreamWriter(processoDeCriptografia))
							{
								//texto no fluxo criptografado
								sw.Write(texto);
							}
							criptografado = memoria.ToArray();
						}
					}
				}
				return criptografado;
			}
			catch (Exception ex)
			{
				throw new Exception("durante a criptografia: " + ex.Message);
			}
		}

		public static string Descriptografar(byte[] textoCriptografado, byte[] chave, byte[] IV)
		{
			try
			{
				//testando se o todos os paramentros possuem valor
				if (textoCriptografado == null || textoCriptografado.Length <= 0)
				{
					throw new Exception("Não foi enviado o texto");
				}
				if (chave == null || chave.Length <= 0)
				{
					throw new Exception("Não foi enviado a chave");
				}
				if (IV == null || IV.Length <= 0)
				{
					throw new Exception("Não foi enviado o IV");
				}

				string textoDescriptografado = null;

				using (Aes aes = Aes.Create())
				{
					aes.Key = chave;
					aes.IV = IV;

					ICryptoTransform descripta = aes.CreateDecryptor(aes.Key, aes.IV);
					//memoria
					using (MemoryStream memoria = new MemoryStream(textoCriptografado))
					{
						using (CryptoStream processoDeDescriptografia = new CryptoStream(memoria, descripta, CryptoStreamMode.Read))
						{
							using (StreamReader sr = new StreamReader(processoDeDescriptografia))
							{
								textoDescriptografado = sr.ReadToEnd();
							}
						}
					}
				}
				return textoDescriptografado;
			}
			catch (Exception ex)
			{

				throw new Exception("durante a descriptografia: " + ex.Message);
			}
		
		}

		public static void ColorirFrase(string frase, int cor)
		{
			switch (cor)
			{
				case 1:
					Console.ForegroundColor = ConsoleColor.Cyan;
					break;
				case 2:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
			}
			Console.WriteLine(frase);
			Console.ResetColor();
		}
	}
}