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

        int numeroConjuntos = numeroLinhas / associatividade;
        int palavraBits = (int)Math.Log2(tamanhoLinha);
        int conjuntoBits = (int)Math.Log2(numeroConjuntos);

        Conjunto[] cache = new Conjunto[numeroConjuntos];

        for (int i = 0; i < numeroConjuntos; i++)
        {
            cache[i] = new Conjunto(associatividade);
        }

        int totalR = 0, totalW = 0, hitR = 0, hitW = 0;
        int memAccessR = 0, memAccessW = 0;

        string[] linhas = File.ReadAllLines(arquivoEntrada);

        foreach (string linha in linhas)
        {
            string[] partes = linha.Split(' ');
            uint endereco = Convert.ToUInt32(partes[0], 16);
            char op = partes[1][0];

            uint index = (endereco >> palavraBits) & (uint)(numeroConjuntos - 1);
            uint tag = endereco >> (palavraBits + conjuntoBits);
            bool hit = false;
            Conjunto conjunto = cache[index];

            int linhaUsada = -1;
            for (int i = 0; i < associatividade; i++)
            {
                if (conjunto.Linhas[i].Valida && conjunto.Linhas[i].Tag == tag)
                {
                    hit = true;
                    linhaUsada = i;
                    conjunto.Linhas[i].LRU = 0;
                    break;
                }
            }

            for (int i = 0; i < associatividade; i++)
                if (conjunto.Linhas[i].Valida && i != linhaUsada)
                    conjunto.Linhas[i].LRU++;

            if (op == 'R') totalR++;
            else totalW++;

            if (hit)
            {
                if (op == 'R') hitR++;
                else hitW++;

                if (politicaEscrita == 0 && op == 'W')
                    memAccessW++;
            }
            else
            {
                if (op == 'R') memAccessR++;
                else memAccessR++;

                int linhaSubstituir = Array.FindIndex(conjunto.Linhas, l => !l.Valida);
                if (linhaSubstituir == -1)
                {
                    if (politicaSubstituicao == 1)
                    {
                        linhaSubstituir = Array.FindIndex(conjunto.Linhas, l => l.LRU == conjunto.Linhas.Max(x => x.LRU));
                    }
                    else
                    {
                        linhaSubstituir = new Random().Next(associatividade);
                    }
                    if (politicaEscrita == 1 && conjunto.Linhas[linhaSubstituir].Dirty)
                    {
                        memAccessW++;
                    }
                }

                var linhaCache = conjunto.Linhas[linhaSubstituir];
                linhaCache.Tag = tag;
                linhaCache.Valida = true;
                linhaCache.LRU = 0;
                linhaCache.Dirty = (politicaEscrita == 1 && op == 'W');

                if (politicaEscrita == 0 && op == 'W')
                    memAccessW++;
            }
        }

        int total = totalR + totalW;
        double taxaHitR = (double)hitR / totalR;
        double taxaHitW = (double)hitW / totalW;
        double taxaGlobal = (double)(hitR + hitW) / total;

        double tempoMedio = ((hitR + hitW) * tempoAcesso + (memAccessR * tempoLeituraMemoria) + (memAccessW * tempoEscritaMemoria)) / (double)total;

        var linhasSaida = new List<string>();

        linhasSaida.Add("Parâmetros:");
        linhasSaida.Add($"Política de escrita: {politicaEscrita}");
        linhasSaida.Add($"Tamanho da linha: {tamanhoLinha}");
        linhasSaida.Add($"Número de linhas: {numeroLinhas}");
        linhasSaida.Add($"Associatividade: {associatividade}");
        linhasSaida.Add($"Hit time: {tempoAcesso} ns");
        string politicaSubstituicaoStr = politicaSubstituicao == 1 ? "LRU" : "Aleatória";
        linhasSaida.Add($"Substituição: {politicaSubstituicaoStr}");
        linhasSaida.Add($"Memória principal leitura: {tempoLeituraMemoria} ns");
        linhasSaida.Add($"Memória principal escrita: {tempoEscritaMemoria} ns");

        linhasSaida.Add("Resultados:");
        linhasSaida.Add($"Total leituras: {totalR}");
        linhasSaida.Add($"Total escritas: {totalW}");
        linhasSaida.Add($"Acessos à memória principal (leitura): {memAccessR}");
        linhasSaida.Add($"Acessos à memória principal (escrita): {memAccessW}");

        linhasSaida.Add($"Taxa de acerto leitura: {taxaHitR:F4} ({hitR})");
        linhasSaida.Add($"Taxa de acerto escrita: {taxaHitW:F4} ({hitW})");
        linhasSaida.Add($"Taxa de acerto global: {taxaGlobal:F4} ({hitR + hitW})");

        linhasSaida.Add($"Tempo médio de acesso: {tempoMedio:F4} ns");

        File.WriteAllLines(arquivoSaida, linhasSaida);
    }
}