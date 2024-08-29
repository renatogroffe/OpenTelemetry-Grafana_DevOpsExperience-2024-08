using Microsoft.AspNetCore.Mvc;
using APIContagem.Data;
using APIContagem.Models;
using APIContagem.Logging;
using APIContagem.Tracing;

namespace APIContagem.Controllers;

[ApiController]
[Route("[controller]")]
public class ContadorController : ControllerBase
{
    private static readonly Contador _CONTADOR = new();
    private readonly ILogger<ContadorController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ContagemRepository _repository;

    public ContadorController(ILogger<ContadorController> logger,
        IConfiguration configuration,
        ContagemRepository repository)
    {
        _logger = logger;
        _configuration = configuration;
        _repository = repository;
    }

    [HttpGet]
    public ResultadoContador Get()
    {
        int valorAtualContador;

        using var activity1 = OpenTelemetryExtensions.ActivitySource
            .StartActivity("GerarValorContagem")!;

        lock (_CONTADOR)
        {
            _CONTADOR.Incrementar();
            valorAtualContador = _CONTADOR.ValorAtual;
        }

        activity1.SetTag("valorAtual", valorAtualContador);
        _logger.LogValorAtual(valorAtualContador);

        var resultado = new ResultadoContador()
        {
            ValorAtual = valorAtualContador,
            Producer = _CONTADOR.Local,
            Kernel = _CONTADOR.Kernel,
            Framework = _CONTADOR.Framework,
            Mensagem = _configuration["MensagemVariavel"]
        };
        activity1.Stop();

        using var activity2 = OpenTelemetryExtensions.ActivitySource
            .StartActivity("RegistrarRetornarValorContagem")!;
        
        _repository.Insert(resultado);
        _logger.LogInformation($"Registro inserido com sucesso! Valor: {valorAtualContador}");
        
        activity2.SetTag("valorAtual", valorAtualContador);
        activity2.SetTag("horario", $"{DateTime.UtcNow.AddHours(-3):HH:mm:ss}");

        return resultado;
    }
}