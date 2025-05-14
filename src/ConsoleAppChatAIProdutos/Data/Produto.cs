using System.ComponentModel.DataAnnotations;

namespace ConsoleAppChatAIProdutos.Data;

public class Produto
{
    [Key]
    public int Id { get; set; }
    public string? CodigoBarras { get; set; }
    public string? Nome { get; set; }
    public decimal Preco { get; set; }
}