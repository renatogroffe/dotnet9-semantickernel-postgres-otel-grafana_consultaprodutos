using LinqToDB.Mapping;

namespace ConsoleAppChatAIProdutos.Data.Fake;

//[Table("public.\"Produtos\"")]
[Table("Produtos")]
public class Produto
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column]
    public string? CodigoBarras { get; set; }

    [Column]
    public string? Nome { get; set; }

    [Column]
    public decimal Preco { get; set; }
}