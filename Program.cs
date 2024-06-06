using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var configuracaoProjeto = app.Configuration;
ListagemDeUsuarios.ProdutosIniciais(configuracaoProjeto);



app.MapGet("/", () => "Olá Mundo!"); //retornando mensagem

app.MapGet("/usuario", () => //retornando JSON
    new {
        Nome = "Ana Carolina",
        Idade = 18,
        Email = "carolina@gmail.com"
    }
); 

app.MapGet("/cabecalho", (HttpResponse response) => response.Headers.Add("Teste","Pedro Santana")); //Adicionando Header (Postman)

app.MapGet("/cabecalho2", (HttpResponse response) => //Adicionando Vários Headers
    {
    response.Headers.Add("teste1","Pedro Santana");
    response.Headers.Add("teste2","Ana Carolina");
    }
);

app.MapGet("/cabecalhoRetorno", (HttpResponse response) => //Retornando um usuário
    {
    response.Headers.Add("teste1","Pedro Santana");
    return new {
        Nome = "Pedro Santana",
        Gatos = 2,
        Email = "pedro@gmail.com"
    };
    }
);

//Passando parâmetro através do Body
app.MapPost("/novousuario", (Usuario usuario) => //Inserindo novo produto 
    {
        return $"{usuario.Nome} - {usuario.Email} - {usuario.Idade}";
    }
);

//Passando parâmetro através do Link

//http://localhost:5247?dataInicial={data}&dataFinal={data} -> Não é Dinâmico
app.MapGet("/usuarioData", ([FromQuery] string dataInicial, [FromQuery] string dataFinal) => //Colocando Pamaetros por Query
        {
            return $"Data Inicial: {dataInicial} \nData Final:{dataFinal}";
        }
);

//http://localhost:5247/{Nome} -> É dinâmico
app.MapGet("/usuarioNome/{nome}", ([FromRoute] string nome) =>  //Colocando Parâmetros por Rota 
    {
        return nome;
    }
);

//Passando parâmetros através do Header

app.MapGet("/obterInformacao", (HttpRequest request) => //Pegando um valor
    {
        return request.Headers["ID_Usuario"].ToString();
    }
);




//EndPoints utilizando o padrão Rest

app.MapPost("/Usuarios", (Usuario usuario) => 
    {
        ListagemDeUsuarios.Add(usuario);
        return Results.Created($"/Usuarios/{usuario.ID_Usuario}", usuario);
    }
);

app.MapGet("/Usuarios/{idUsuario}", ([FromRoute] int idUsuario) => 
    {
        var usuario = ListagemDeUsuarios.GetBy(idUsuario);

        if(usuario == null)
            return Results.NotFound();

        return Results.Ok(usuario);
    }
);

app.MapPut("/Usuarios", (Usuario usuario) =>
    {
        var usuarioSalvo = ListagemDeUsuarios.GetBy(usuario.ID_Usuario);
        if(usuarioSalvo == null)
            return Results.NotFound();

        usuarioSalvo.Nome = usuario.Nome;
        return Results.Created($"/Usuarios/{usuario.ID_Usuario}",usuarioSalvo);
    }
);

app.MapDelete("/Usuarios/{idUsuario}", ([FromRoute] int idUsuario) =>
    {
        var usuarioSalvo = ListagemDeUsuarios.GetBy(idUsuario);
        if(usuarioSalvo == null)
            return Results.NotFound();

        ListagemDeUsuarios.Remove(usuarioSalvo);
        return Results.Ok();
    }
);

app.Run();





public static class ListagemDeUsuarios
{
    public static List<Usuario>? Usuarios { get; set; } = Usuarios = new List<Usuario>();


    public static void ProdutosIniciais(IConfiguration configuracao)
    {
        var usuarios = configuracao.GetSection("Usuarios").Get<List<Usuario>>();
        Usuarios = usuarios;
    }

    public static void Add(Usuario usuario)
    {
        Usuarios.Add(usuario);
    }   
    
    public static Usuario GetBy(int idUsuario)
    {
        return Usuarios.FirstOrDefault(p => p.ID_Usuario == idUsuario);
    }

    public static void Remove(Usuario usuario)
    {
        if(usuario != null)
            Usuarios.Remove(usuario);
    }
}

public class Usuario 
{
    public int ID_Usuario {get;set;}
    public string Nome {get;set;}
    public string Email {get;set;}
    public int Idade {get;set;}

}
