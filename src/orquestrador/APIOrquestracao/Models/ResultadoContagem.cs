namespace APIOrquestracao.Models;

public class ResultadoOrquestracao
{
    public string? Horario { get; set; }
    public ResultadoContagem? ContagemPostgres { get; set; }
    public ResultadoContagem? ContagemMySql { get; set; }
}

public class ResultadoContagem
{
    public int ValorAtual { get; set; }
    public string? Producer { get; set; }
    public string? Kernel { get; set; }
    public string? Framework { get; set; }
    public string? Mensagem { get; set; }
}