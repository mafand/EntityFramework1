using Microsoft.EntityFrameworkCore;
using System;

namespace Alura.Loja.Testes.ConsoleApp
{
    public class LojaContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; } // Nome da propriedade e o nome da tabela no banco de dados
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Promocao> Promocaos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configura chave primaria composta
            modelBuilder.Entity<PromocaoProduto>().HasKey(pp => new { pp.PromocaoId, pp.ProdutoId });

            //muda o nome da tabela que seria Endereco (baseado no nome da propriedade no contexto) para Enderecos
            modelBuilder.Entity<Endereco>().ToTable("Enderecos");

            //criar uma coluna na tabela que não esta mapeada na classe (Shadow Property)
            modelBuilder.Entity<Endereco>().Property<int>("ClienteId");

            //configura chave primaria
            modelBuilder.Entity<Endereco>().HasKey("ClienteId");

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = LojaDB; Trusted_Connection = true; ");
        }

    }
}