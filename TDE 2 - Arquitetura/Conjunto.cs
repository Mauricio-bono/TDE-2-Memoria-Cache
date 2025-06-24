public class Conjunto
{
    public LinhaCache[] Linhas;

    public Conjunto(int associatividade)
    {
        Linhas = new LinhaCache[associatividade];
        for (int i = 0; i < associatividade; i++)
        {
            Linhas[i] = new LinhaCache();
        }
    }
}