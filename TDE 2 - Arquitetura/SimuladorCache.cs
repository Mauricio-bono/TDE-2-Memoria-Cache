using System;
using System.IO;
using System.Linq;
using System.Globalization;
class SimuladorCache
{
    static void Main()
    {

        int associatividade = 0;

        int politicaSubstituicao = 0;

        Console.WriteLine("Arquivo de entrada: ");
        string arquivoEntrada = Console.ReadLine();
        Console.WriteLine("Arquivo de saída: ");
        string arquivoSaida = Console.ReadLine();

        Console.WriteLine("Política de escrita (0-write-through, 1-write-back): ");
        int politicaEscrita = int.Parse(Console.ReadLine());

        Console.WriteLine("Tamanho da linha (em bytes): ");
        int tamanhoLinha = int.Parse(Console.ReadLine());

        Console.WriteLine("Número de linhas: ");
        int numeroLinhas = int.Parse(Console.ReadLine());

        while (associatividade < 1 || associatividade > numeroLinhas)
        {
            Console.WriteLine("Assosiatividade (Entre 1 e Número de Linhas): ");
            associatividade = int.Parse(Console.ReadLine());
        }

        Console.WriteLine("Tempo de acesso (hit time em nanossegundos): ");
        int tempoAcesso = int.Parse(Console.ReadLine());

        while (politicaSubstituicao < 1 || politicaSubstituicao > 2)
        {
            Console.WriteLine("Política de substituição (1 - LRU ou 2 - Aleatória): ");
            politicaSubstituicao = int.Parse(Console.ReadLine());
        }

        Console.WriteLine("Tempo de leitura da memória principal (nanossegundos): ");
        int tempoLeituraMemoria = int.Parse(Console.ReadLine());
        
        Console.WriteLine("Tempo de escrita da memória principal (nanossegundos): ");
        int tempoEscritaMemoria = int.Parse(Console.ReadLine());

    }
}