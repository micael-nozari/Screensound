using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class MusicaExtensions
    {
        private static ICollection<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicaList)
        {
            return musicaList.Select(a => EntityToResponse(a)).ToList();
        }

        private static MusicaResponse EntityToResponse(Musica musica)
        {
            return new MusicaResponse(musica.Id, musica.Nome!, musica.AnoLancamento, musica.Artista!.Id, musica.Artista.Nome);
        }

        public static void AddEndpointsMusicas(this WebApplication app)
        {
            var groupBuilder = app.MapGroup("musicas").RequireAuthorization().WithTags("Músicas");

            groupBuilder.MapGet("", ([FromServices] DAL<Musica> dal) =>
            {
                var musicas = dal.Listar();

                return Results.Ok(EntityListToResponseList(musicas));
            });

            groupBuilder.MapGet("{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
            {
                var musica = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
                if (musica is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityToResponse(musica));

            });

            groupBuilder.MapPost("", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero, [FromBody] MusicaRequest musicaRequest) =>
            {
                var musica = new Musica(musicaRequest.nome) 
                {                     
                    ArtistaId = musicaRequest.ArtistaId,
                    AnoLancamento = musicaRequest.anoLancamento,
                    Generos = musicaRequest.Generos is not null ? GeneroRequestConverter(dalGenero, musicaRequest.Generos) : new List<Genero>(),
                };

                dal.Adicionar(musica);
                return Results.Ok();
            });

            groupBuilder.MapDelete("{id}", ([FromServices] DAL<Musica> dal, int id) => {
                var musica = dal.RecuperarPor(a => a.Id == id);
                if (musica is null)
                {
                    return Results.NotFound();
                }
                dal.Deletar(musica);
                return Results.NoContent();

            });

            groupBuilder.MapPut("", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequestEdit musicaRequest) => {
                var musicaAAtualizar = dal.RecuperarPor(a => a.Id == musicaRequest.Id);
                if (musicaAAtualizar is null)
                {
                    return Results.NotFound();
                }
                musicaAAtualizar.Nome = musicaRequest.nome;
                musicaAAtualizar.AnoLancamento = musicaRequest.anoLancamento;

                dal.Atualizar(musicaAAtualizar);
                return Results.Ok();
            });
        }

        private static ICollection<Genero> GeneroRequestConverter(DAL<Genero> dalGenero, ICollection<GeneroRequest> generos)
        {
            var listaGeneros = new List<Genero>();
            foreach (var item in generos)
            {
                var entity = RequestToEntity(item);
                var genero = dalGenero.RecuperarPor(g => g.Nome.ToUpper().Equals(item.Nome));

                if(genero is not null)
                {
                    listaGeneros.Add(genero);
                }
                else
                {
                    listaGeneros.Add(entity);
                }
            }
            return listaGeneros;
        }

        private static Genero RequestToEntity(GeneroRequest request)
        {
            return new Genero()
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
            };
        }
    }
}
