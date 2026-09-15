using System;
using Edelstahl.Services.Cryptography;

namespace Edelstahl.Test
{
    public static class AesEncryptionTest
    {
        public static void Ejecutar()
        {
            Console.WriteLine(
                "PRUEBA DE CIFRADO SIMETRICO AES");

            Console.WriteLine(
                "--------------------------------");

            const string cuitOriginal =
                "30-66778899-2";

            string cuitCifrado =
                CryptographyService.Encrypt(
                    cuitOriginal);

            string cuitDescifrado =
                CryptographyService.Decrypt(
                    cuitCifrado);

            Console.WriteLine();

            Console.WriteLine(
                "CUIT original:");

            Console.WriteLine(
                cuitOriginal);

            Console.WriteLine();

            Console.WriteLine(
                "CUIT cifrado:");

            Console.WriteLine(
                cuitCifrado);

            Console.WriteLine();

            Console.WriteLine(
                "CUIT descifrado:");

            Console.WriteLine(
                cuitDescifrado);

            Console.WriteLine();

            bool resultadoCorrecto =
                string.Equals(
                    cuitOriginal,
                    cuitDescifrado,
                    StringComparison.Ordinal);

            if (resultadoCorrecto)
            {
                Console.ForegroundColor =
                    ConsoleColor.Green;

                Console.WriteLine(
                    "RESULTADO: PRUEBA CORRECTA");
            }
            else
            {
                Console.ForegroundColor =
                    ConsoleColor.Red;

                Console.WriteLine(
                    "RESULTADO: PRUEBA INCORRECTA");
            }

            Console.ResetColor();

            Console.WriteLine();

            Console.WriteLine(
                "La clave AES fue obtenida desde " +
                "el archivo App.config.");

            Console.WriteLine(
                "El valor descifrado debe ser " +
                "igual al valor original.");

            Console.WriteLine();

            Console.WriteLine(
                "Presione una tecla para finalizar.");

            Console.ReadKey();
        }
    }
}