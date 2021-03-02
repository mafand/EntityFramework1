using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Alura.Loja.Testes.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //GravarUsandoAdoNet();
            //GravarUsandoEntity();
            //RecuperarProdutos();
            //ExcluirProdutos();
            //RecuperarProdutos();
            //AtualizarProduto();
            //Inserir();
            //UmParaMuitos();
            //MuitosParaMuitos();
            //UmParaUm();
            //IncluirPromocao();
            //ExibeProdutosDaPromocao();

            ExibeComprasDoProduto();

            Console.ReadLine();
        }

        private static void ExibeComprasDoProduto()
        {
            using (var contexto = new LojaContext())
            {
                // 1 para 1 => cliente/endereco
                var cliente = contexto
                    .Clientes
                    .Include(c => c.EnderecoDeEntrega)
                    .FirstOrDefault();

                Console.WriteLine($"Endereco de entrega: { cliente.EnderecoDeEntrega.Logradouro}");

                // 1 para N => produto/compras
                var produto = contexto
                    .Produtos
                    //.Include(p => p.Compras)
                    .Where(p => p.Id == 1002)
                    .FirstOrDefault();

                // filtrar entidade relacionada
                // produto 'pao' com compras com preco acima de 10 
                contexto.Entry(produto)
                    .Collection(p => p.Compras)
                    .Query()
                    .Where(c => c.Preco > 1)
                    .Load(); // carregar relacionamento em instancia ja recuperada

                Console.WriteLine($"Monstrando as compras do produto {produto.Nome}");

                foreach (var item in produto.Compras)
                {
                    Console.WriteLine(item);
                }

                
            }
        }

        private static void ExibeProdutosDaPromocao()
        {
            using (var contexto2 = new LojaContext())
            {
                var promocao = contexto2
                    .Promocaos
                    .Include(p => p.Produtos) // tabela relacionada
                    .ThenInclude(pp => pp.Produto) // tabela relacionada segundo nivel
                    .FirstOrDefault();

                Console.WriteLine("Monstrando os produtos da promocao...");

                foreach (var item in promocao.Produtos)
                {
                    Console.WriteLine(item.Produto);
                }
            }
        }

        private static void IncluirPromocao()
        {
            using (var contexto = new LojaContext())
            {
                var promocao = new Promocao();
                promocao.Descricao = "Queima total janeiro 2017";
                promocao.DataInicio = new DateTime(2017, 1, 1);
                promocao.DataTermino = new DateTime(2017, 1, 31);

                var produtos = contexto
                    .Produtos
                    .Where(p => p.Categoria == "Bebidas")
                    .ToList();

                foreach (var item in produtos)
                {
                    promocao.IncluiProduto(item);
                }

                contexto.Promocaos.Add(promocao);

                contexto.SaveChanges();
            }

            ExibeProdutosDaPromocao();
        }

        private static void UmParaMuitos()
        {
            var paoFrances = new Produto() { Nome = "Pão Frances", Categoria = "Padaria", PrecoUnitario = 0.40, Unidade = "Unidade" };

            var compra = new Compra();
            compra.Quantidade = 6;
            compra.Produto = paoFrances;
            compra.Preco = paoFrances.PrecoUnitario * compra.Quantidade;

            using (var contexto = new LojaContext())
            {
                contexto.Compras.Add(compra);

                contexto.SaveChanges();
            }


        }

        private static void UmParaUm()
        {
            var fulano = new Cliente();
            fulano.Nome = "Fulalinho de tal";
            fulano.EnderecoDeEntrega = new Endereco()
            {
                Numero = 12,
                Logradouro = "Rua dos Inválidos",
                Complemento = "Sobrado",
                Bairro = "Centro",
                Cidade = "Cidade"
            };

            using (var contexto = new LojaContext())
            {
                contexto.Clientes.Add(fulano);
                contexto.SaveChanges();
            }
        }


        private static void MuitosParaMuitos()
        {
            var p1 = new Produto() { Nome = "Suco de laranja", Categoria = "Bebidas", PrecoUnitario = 8.79, Unidade = "Kilos" };
            var p2 = new Produto() { Nome = "Café", Categoria = "Bebidas", PrecoUnitario = 12.45, Unidade = "Gramas" }; ;
            var p3 = new Produto() { Nome = "Macarrão", Categoria = "Alimentos", PrecoUnitario = 4.23, Unidade = "Gramas" }; ;


            var promocaoDePascoa = new Promocao();
            promocaoDePascoa.Descricao = "Pascoa Feliz";
            promocaoDePascoa.DataInicio = DateTime.Now;
            promocaoDePascoa.DataTermino = DateTime.Now.AddMonths(3);
            promocaoDePascoa.IncluiProduto(p1);
            promocaoDePascoa.IncluiProduto(p2);
            promocaoDePascoa.IncluiProduto(p3);

            using (var contexto = new LojaContext())
            {
                // Como o objeto compra faz referência a um produto que não existe no banco de dados, caneta será inserida no banco de dados e logo após a compra também será
                //contexto.Promocaos.Add(promocaoDePascoa);

                var promocao = contexto.Promocaos.Find(1);
                contexto.Promocaos.Remove(promocao);

                contexto.SaveChanges();
            }
        }

        private static void Inserir()
        {
            using (var contexto = new LojaContext())
            {


                var produtos = contexto.Produtos.ToList();

                foreach (var p in produtos)
                {
                    Console.WriteLine(p);
                }

                Console.WriteLine("========");

                foreach (var c in contexto.ChangeTracker.Entries())
                {
                    Console.WriteLine(c.State);
                }

                /*var p1 = produtos.First();
                p1.Nome = "Harry Potter";

                contexto.SaveChanges();*/

                var novoProduto = new Produto()
                {
                    Nome = "Desinfetante",
                    Categoria = "Limpeza",
                    PrecoUnitario = 2.99

                };

                contexto.Produtos.Add(novoProduto);

                contexto.SaveChanges();
            }
        }

        private static void AtualizarProduto()
        {
            //incluir um produto
            GravarUsandoEntity();
            RecuperarProdutos();

            //atualizar o produto
            using (var repo = new ProdutoDAOEntity())
            {
                Produto primeiro = repo.Produtos().First();
                primeiro.Nome = "Harry Potter e a Ordem da Fênix Editado";

                repo.Atualizar(primeiro);
            }
            RecuperarProdutos();
        }

        private static void ExcluirProdutos()
        {
            using (var repo = new ProdutoDAOEntity())
            {
                IList<Produto> produtos = repo.Produtos();

                foreach (var item in produtos)
                {
                    repo.Remover(item);
                }
            }


        }

        private static void RecuperarProdutos()
        {
            using (var repo = new ProdutoDAOEntity())
            {
                IList<Produto> produtos = repo.Produtos();

                Console.WriteLine("Foram encontrados {0} produto(s).", produtos.Count());

                foreach (var item in produtos)
                {
                    Console.WriteLine(item.Nome);
                }
            }

        }

        private static void GravarUsandoEntity()
        {
            Produto p = new Produto();
            p.Nome = "Harry Potter e a Ordem da Fênix";
            p.Categoria = "Livros";
            p.PrecoUnitario = 19.89;

            using (var repo = new ProdutoDAOEntity())
            {
                repo.Adicionar(p);
            }
        }

        private static void GravarUsandoAdoNet()
        {
            Produto p = new Produto();
            p.Nome = "Harry Potter e a Ordem da Fênix";
            p.Categoria = "Livros";
            p.PrecoUnitario = 19.89;

            using (var repo = new ProdutoDAO())
            {
                repo.Adicionar(p);
            }
        }
    }
}
