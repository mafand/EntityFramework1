namespace Alura.Loja.Testes.ConsoleApp
{
    public class Compra
    {
        
        public int Id { get; set; }
        public int Quantidade { get; internal set; }
        public int ProdutoId { get; set; } // Por ser int (default 0 - nao permite null), o campo na tabela sera obrigatorio
        public Produto Produto { get; internal set; }
        public double Preco { get; internal set; }
    }
}