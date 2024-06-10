using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.Menus;

internal class MenuMostrarMusicasAnoLancamento : Menu
{
    protected readonly ScreenSoundContext _context;

    public MenuMostrarMusicasAnoLancamento(ScreenSoundContext context)
    {
        _context = context;
    }

    public override void Executar(DAL<Artista> artistaDAL)
    {
        base.Executar(artistaDAL);
        ExibirTituloDaOpcao("Exibir detalhes do artista");
        Console.Write("Digite ano de lançamento das músicas que deseja conhecer melhor: ");
        string sAnoLancamento = Console.ReadLine()!;

        int anoLancamento = 0;
        if (!Int32.TryParse(sAnoLancamento, out anoLancamento))
        {
            Console.WriteLine($"\nO ano {anoLancamento} não foi encontrada!");
            Console.WriteLine("Digite uma tecla para voltar ao menu principal");
            Console.ReadKey();
            Console.Clear();
        }
        else
        {
            DAL<Musica> musicaDAL = new DAL<Musica>(_context);
            var listaAnoLancamento = musicaDAL.ListarPor(a => a.AnoLancamento == anoLancamento);

            if (listaAnoLancamento.Any())
            {
                Console.WriteLine($"\nMusicas do Ano {anoLancamento}:");
                foreach (var musica in listaAnoLancamento)
                {
                    musica.ExibirFichaTecnica();
                }
                Console.WriteLine("\nDigite uma tecla para voltar ao menu principal");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine($"\nO ano {anoLancamento} não foi encontrada!");
                Console.WriteLine("Digite uma tecla para voltar ao menu principal");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
