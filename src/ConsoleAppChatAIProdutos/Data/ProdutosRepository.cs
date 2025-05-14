namespace ConsoleAppChatAIProdutos.Data;

public static class ProdutosRepository
{
    public static List<Produto> ListAll()
    {
        var context = new CatalogoContext();
        return context.Produtos!.ToList();
    }

    public static Produto? GetProdutoByCodigoBarras(string codigoBarras)
    {
        var context = new CatalogoContext();
        return context.Produtos!.Where(p => p.CodigoBarras == codigoBarras).FirstOrDefault();
    }

    public static decimal GetPrecoMedio()
    {
        var context = new CatalogoContext();
        return context.Produtos!.Average(p => p.Preco);
    }
}