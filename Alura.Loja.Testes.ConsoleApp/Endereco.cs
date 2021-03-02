namespace Alura.Loja.Testes.ConsoleApp
{
    public class Endereco
    {
        //Nao necessariamente precisa de chave primaria (relacionamento 1 para 1)
        public int Numero { get; internal set; }
        public string Logradouro { get; internal set; }
        public string Complemento { get; internal set; }
        public string Bairro { get; internal set; }
        public string Cidade { get; internal set; }
        public Cliente Cliente { get; set; }
    }
}