using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CriptografiaDiv3Correta
{
    class Program
    {
        static Dictionary<char, char> mapaSubstituicao = new Dictionary<char, char>()
        {
            {'A', 'Q'}, {'B', 'W'}, {'C', 'E'}, {'D', 'R'}, {'E', 'T'},
            {'F', 'Y'}, {'G', 'U'}, {'H', 'I'}, {'I', 'O'}, {'J', 'P'},
            {'K', 'A'}, {'L', 'S'}, {'M', 'D'}, {'N', 'F'}, {'O', 'G'},
            {'P', 'H'}, {'Q', 'J'}, {'R', 'K'}, {'S', 'L'}, {'T', 'Z'},
            {'U', 'X'}, {'V', 'C'}, {'W', 'V'}, {'X', 'B'}, {'Y', 'N'},
            {'Z', 'M'}, {' ', '_'}
        };

        static Dictionary<char, int> mapaCharParaNumero = new Dictionary<char, int>()
        {
            {'Q', 10}, {'W', 11}, {'E', 12}, {'R', 13}, {'T', 14},
            {'Y', 15}, {'U', 16}, {'I', 17}, {'O', 18}, {'P', 19},
            {'A', 20}, {'S', 21}, {'D', 22}, {'F', 23}, {'G', 24},
            {'H', 25}, {'J', 26}, {'K', 27}, {'L', 28}, {'Z', 29},
            {'X', 30}, {'C', 31}, {'V', 32}, {'B', 33}, {'N', 34},
            {'M', 35}, {'_', 36}
        };

        static Dictionary<int, char> mapaNumeroParaChar = mapaCharParaNumero.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        static int[] ordemTransposicao = { 2, 0, 3, 1 };

        static int chave = 13;

        // Usamos módulo primo para garantir inversos
        static int mod = 997;

        static void Main(string[] args)
        {
            Console.WriteLine("Digite o texto para criptografar:");
            string texto = Console.ReadLine().ToUpper();

            string textoCriptografado = Criptografar(texto);
            Console.WriteLine($"\nTexto Criptografado:\n{textoCriptografado}");

            string textoDescriptografado = Descriptografar(textoCriptografado);
            Console.WriteLine($"\nTexto Descriptografado:\n{textoDescriptografado}");

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static string Criptografar(string texto)
        {
            // Substituição
            StringBuilder substituido = new StringBuilder();
            foreach (char c in texto)
            {
                if (mapaSubstituicao.ContainsKey(c))
                    substituido.Append(mapaSubstituicao[c]);
                else
                    substituido.Append(c);
            }

            // Transposição
            string textoTransposto = Transpor(substituido.ToString(), ordemTransposicao);

            // Converter para números e aplicar operações
            StringBuilder resultadoNumerico = new StringBuilder();
            foreach (char c in textoTransposto)
            {
                if (mapaCharParaNumero.ContainsKey(c))
                {
                    int numero = mapaCharParaNumero[c];
                    int numeroCripto;

                    if (numero % 3 == 0)
                        numeroCripto = OperacoesDiv3(numero, chave);
                    else
                        numeroCripto = OperacoesNaoDiv3(numero, chave);

                    resultadoNumerico.Append(numeroCripto.ToString("D3"));
                }
                else
                    resultadoNumerico.Append("000");
            }

            return resultadoNumerico.ToString();
        }

        static string Descriptografar(string textoNumerico)
        {
            List<char> chars = new List<char>();

            for (int i = 0; i < textoNumerico.Length; i += 3)
            {
                string numStr = textoNumerico.Substring(i, 3);
                if (int.TryParse(numStr, out int num))
                {
                    // Testa ambas as reversões
                    int tentativaDiv3 = ReverterOperacoesDiv3(num, chave);
                    int tentativaNaoDiv3 = ReverterOperacoesNaoDiv3(num, chave);

                    bool div3Valido = mapaNumeroParaChar.ContainsKey(tentativaDiv3) && (tentativaDiv3 % 3 == 0);
                    bool naoDiv3Valido = mapaNumeroParaChar.ContainsKey(tentativaNaoDiv3) && (tentativaNaoDiv3 % 3 != 0);

                    if (div3Valido)
                        chars.Add(mapaNumeroParaChar[tentativaDiv3]);
                    else if (naoDiv3Valido)
                        chars.Add(mapaNumeroParaChar[tentativaNaoDiv3]);
                    else
                        chars.Add('?');
                }
                else
                {
                    chars.Add('?');
                }
            }

            string textoTransposto = new string(chars.ToArray());
            string textoDestransposto = Destranspor(textoTransposto, ordemTransposicao);

            // Inversa da substituição
            Dictionary<char, char> mapaInverso = mapaSubstituicao.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            StringBuilder resultadoOriginal = new StringBuilder();
            foreach (char c in textoDestransposto)
            {
                if (mapaInverso.ContainsKey(c))
                    resultadoOriginal.Append(mapaInverso[c]);
                else if (c == '_')
                    resultadoOriginal.Append(' ');
                else
                    resultadoOriginal.Append(c);
            }

            return resultadoOriginal.ToString();
        }

        // Operações para números divisíveis por 3
        static int OperacoesDiv3(int num, int chave)
        {
            int val = num;
            val = (val + chave) % mod;
            val = (val * 3) % mod;
            val = (val - chave + mod) % mod;
            val = (val + 7) % mod;
            val = (val * 2) % mod;
            return val;
        }

        static int ReverterOperacoesDiv3(int num, int chave)
        {
            int val = num;
            val = (val * MultiplicativoInverso(2, mod)) % mod;   // inverso de *2
            val = (val - 7 + mod) % mod;
            val = (val + chave) % mod;
            val = (val * MultiplicativoInverso(3, mod)) % mod;   // inverso de *3
            val = (val - chave + mod) % mod;
            return val;
        }

        // Operações para números não divisíveis por 3
        static int OperacoesNaoDiv3(int num, int chave)
        {
            int val = num;
            val = (val * 7) % mod;
            val = (val + chave) % mod;
            val = (val - 9 + mod) % mod;
            val = (val * 4) % mod;
            val = (val + 1) % mod;
            return val;
        }

        static int ReverterOperacoesNaoDiv3(int num, int chave)
        {
            int val = num;
            val = (val - 1 + mod) % mod;
            val = (val * MultiplicativoInverso(4, mod)) % mod;
            val = (val + 9) % mod;
            val = (val - chave + mod) % mod;
            val = (val * MultiplicativoInverso(7, mod)) % mod;
            return val;
        }

        // Função para calcular inverso multiplicativo mod m
        static int MultiplicativoInverso(int a, int m)
        {
            int m0 = m, t, q;
            int x0 = 0, x1 = 1;

            if (m == 1)
                return 0;

            while (a > 1)
            {
                q = a / m;
                t = m;
                m = a % m; a = t;
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }

            if (x1 < 0)
                x1 += m0;

            return x1;
        }

        static string Transpor(string texto, int[] ordem)
        {
            if (texto.Length % ordem.Length != 0)
                texto = texto.PadRight(((texto.Length / ordem.Length) + 1) * ordem.Length, '_');

            StringBuilder resultado = new StringBuilder();

            for (int i = 0; i < texto.Length; i += ordem.Length)
            {
                char[] bloco = new char[ordem.Length];
                for (int j = 0; j < ordem.Length; j++)
                    bloco[j] = texto[i + ordem[j]];

                resultado.Append(new string(bloco));
            }

            return resultado.ToString();
        }

        static string Destranspor(string texto, int[] ordem)
        {
            if (texto.Length % ordem.Length != 0)
                return texto;

            char[] resultado = new char[texto.Length];

            for (int i = 0; i < texto.Length; i += ordem.Length)
            {
                for (int j = 0; j < ordem.Length; j++)
                    resultado[i + ordem[j]] = texto[i + j];
            }

            return new string(resultado);
        }
    }
}
