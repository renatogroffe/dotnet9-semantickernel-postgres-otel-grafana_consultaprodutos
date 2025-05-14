using ConsoleAppChatAIProdutos.Data;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ConsoleAppChatAIProdutos.Plugins;

public class ProdutosPlugin
{
    [KernelFunction, Description("Retorne uma lista com nomes de produtos, além de seus respectivos códigos de barra e preços.")]
    [return: Description("Uma lista com nomes, código de barras e preços de produtos")]
    public async Task<List<Produto>> GetProdutos()
    {
        return await Task.Run(() => ProdutosRepository.ListAll());
    }

    [KernelFunction, Description("Retorne o nome de um Produto a partir de seu Código de Barras.")]
    [return: Description("Nome do Produto")]
    public async Task<string?> GetProdutoByCodigoBarras(
        [Description("Código de Barras")] string codigoBarras)
    {
        return await Task.Run(() => ProdutosRepository.GetProdutoByCodigoBarras(codigoBarras)?.Nome);
    }

    [KernelFunction, Description("Retorne o Preço Médio dos Produtos que compõem o catálogo.")]
    [return: Description("Preço Médio dos Produtos")]
    public async Task<decimal> GetPrecoMedioProdutos()
    {
        return await Task.Run(() => ProdutosRepository.GetPrecoMedio());
    }
}