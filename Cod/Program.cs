using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CriptografiaConsole
{
    class Program
    {
        static Dictionary<char, int[]> mapaCriptografia = new Dictionary<char, int[]>()
        {
            {'A', new int[]{10}}, {'B', new int[]{52}}, {'C', new int[]{93}}, {'D', new int[]{49}}, {'E', new int[]{54}},
            {'F', new int[]{259}}, {'G', new int[]{825}}, {'H', new int[]{61}}, {'I', new int[]{47}},
            {'J', new int[]{333}}, {'K', new int[]{347}}, {'L', new int[]{482}}, {'M', new int[]{29}}, {'N', new int[]{31}},
            {'O', new int[]{96}}, {'P', new int[]{231}}, {'Q', new int[]{799}}, {'R', new int[]{568}},
            {'S', new int[]{178}}, {'T', new int[]{873}}, {'U', new int[]{993}}, {'V', new int[]{22}},
            {'W', new int[]{76}}, {'X', new int[]{37}}, {'Y', new int[]{64}}, {'Z', new int[]{17}},
            {' ', new int[]{973, 555, 888}}
        };

        static decimal chaveSecreta = 123.456m;
        static long chaveXOR = 987654321;
        static int tamanhoBloco = 13;
        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("Digite o texto para criptografar:");
            string texto = Console.ReadLine().ToUpper();

            string textoCriptografado = Criptografar(texto);
            Console.WriteLine($"\nTexto Criptografado:\n{textoCriptografado}");

            string textoDescriptografado = Descriptografar(textoCriptografado);

            Console.WriteLine($"\n🔓 SENHA DESCOBERTA: {textoDescriptografado}");

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static string Criptografar(string texto)
        {
            StringBuilder resultadoFinal = new StringBuilder();

            foreach (char c in texto)
            {
                if (!mapaCriptografia.ContainsKey(c))
                {
                    resultadoFinal.Append(new string('9', tamanhoBloco));
                    continue;
                }

                if (c == ' ')
                {
                    int[] valoresEspaco = mapaCriptografia[c];
                    int valor = valoresEspaco[random.Next(valoresEspaco.Length)];

                    int aleatorio = random.Next(0, 1000);

                    decimal calc = ((valor + chaveSecreta) * (1000 + aleatorio)) + 5555m;
                    long resultadoCriptografado = (long)Math.Round(calc);
                    resultadoCriptografado ^= chaveXOR;

                    string aleatorioStr = aleatorio.ToString().PadLeft(3, '0');
                    string resultadoStr = resultadoCriptografado.ToString().PadLeft(tamanhoBloco - 3, '0');

                    resultadoFinal.Append(aleatorioStr + resultadoStr);
                }
                else
                {
                    int valor = mapaCriptografia[c][0];

                    decimal passo1 = (valor * 483m) + chaveSecreta;
                    decimal passo2 = passo1 * passo1;
                    decimal passo3 = passo2 + 312;
                    decimal passo4 = passo3 / 67m;
                    decimal passo5 = (decimal)Math.Sqrt((double)passo4);
                    decimal passo6 = passo5 * 32;
                    decimal passo7 = passo6 + 157;
                    decimal passo8 = passo7 / 3m;
                    decimal passo9 = passo8 * 11;
                    decimal passo10 = passo9 - 812;
                    decimal passo11 = passo10 * passo10;
                    decimal passo12 = passo11 / 97m;
                    decimal passo13 = passo12 + 589;
                    decimal passo14 = passo13 * 1.7m;

                    long resultadoCriptografado = (long)Math.Round(passo14);
                    resultadoCriptografado ^= chaveXOR;

                    string numeroStr = resultadoCriptografado.ToString().PadLeft(tamanhoBloco, '0');
                    resultadoFinal.Append(numeroStr);
                }
            }

            return resultadoFinal.ToString();
        }

        static string Descriptografar(string textoCriptografado)
        {
            Dictionary<int, char> mapaDescriptografia = new Dictionary<int, char>();
            foreach (var kvp in mapaCriptografia)
                foreach (var val in kvp.Value)
                    if (!mapaDescriptografia.ContainsKey(val))
                        mapaDescriptografia.Add(val, kvp.Key);

            StringBuilder textoOriginal = new StringBuilder();

            if (textoCriptografado.Length % tamanhoBloco != 0)
                return "Texto criptografado inválido (tamanho incorreto).";

            for (int i = 0; i < textoCriptografado.Length; i += tamanhoBloco)
            {
                string bloco = textoCriptografado.Substring(i, tamanhoBloco);

                if (bloco.All(c => c == '9'))
                {
                    textoOriginal.Append('?');
                    continue;
                }

                if (bloco.Substring(0, 3).All(char.IsDigit) && bloco.Substring(3).All(char.IsDigit))
                {
                    int aleatorio = int.Parse(bloco.Substring(0, 3));
                    string parteValor = bloco.Substring(3);

                    if (!long.TryParse(parteValor, out long valorXOR))
                    {
                        textoOriginal.Append('?');
                        continue;
                    }

                    long valorCriptoOriginal = valorXOR ^ chaveXOR;

                    decimal calc = valorCriptoOriginal;
                    decimal valor = (calc - 5555m) / (1000 + aleatorio) - chaveSecreta;

                    bool encontrouEspaco = false;
                    foreach (int valEspaco in mapaCriptografia[' '])
                    {
                        if (Math.Abs(valor - valEspaco) < 2)
                        {
                            textoOriginal.Append(' ');
                            encontrouEspaco = true;
                            break;
                        }
                    }

                    if (!encontrouEspaco)
                        textoOriginal.Append('?');

                    continue;
                }
                else
                {
                    if (!long.TryParse(bloco, out long valorXOR))
                    {
                        textoOriginal.Append('?');
                        continue;
                    }

                    long valorCriptoOriginal = valorXOR ^ chaveXOR;

                    decimal passo14 = valorCriptoOriginal;
                    decimal passo13 = passo14 / 1.7m - 589;
                    decimal passo12 = passo13 * 97m;
                    if (passo12 < 0) { textoOriginal.Append('?'); continue; }
                    decimal passo11 = (decimal)Math.Sqrt((double)passo12);
                    decimal passo10 = passo11 + 812;
                    decimal passo9 = passo10 / 11m;
                    decimal passo8 = passo9 * 3m;
                    decimal passo7 = passo8 - 157;
                    decimal passo6 = passo7 / 32m;
                    if (passo6 < 0) { textoOriginal.Append('?'); continue; }
                    decimal passo5 = passo6 * passo6;
                    decimal passo4 = passo5 * 67m;
                    decimal passo3 = passo4 - 312;
                    if (passo3 < 0) { textoOriginal.Append('?'); continue; }
                    decimal passo2 = (decimal)Math.Sqrt((double)passo3);
                    decimal passo1 = (passo2 - chaveSecreta) / 483m;

                    decimal valorDecimal = passo1;
                    int valorInt = (int)Math.Round(valorDecimal);

                    int valorUsado = valorInt;

                    if (!mapaDescriptografia.ContainsKey(valorInt))
                    {
                        int chaveMaisProxima = mapaDescriptografia.Keys.OrderBy(k => Math.Abs(k - valorInt)).First();
                        if (Math.Abs(chaveMaisProxima - valorInt) <= 3)
                            valorUsado = chaveMaisProxima;
                        else
                        {
                            textoOriginal.Append('?');
                            continue;
                        }
                    }

                    textoOriginal.Append(mapaDescriptografia[valorUsado]);
                }
            }

            return textoOriginal.ToString();
        }
    }
}
