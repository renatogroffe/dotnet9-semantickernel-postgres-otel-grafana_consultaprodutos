CREATE DATABASE basecatalogo;

\c basecatalogo;

CREATE TABLE "Produtos" (
    "Id" SERIAL NOT NULL,
    "CodigoBarras" VARCHAR(13) NOT NULL,
    "Nome" VARCHAR(100) NOT NULL,
    "Preco" NUMERIC(19, 4) NOT NULL,
    CONSTRAINT "PK_Funcionarios" PRIMARY KEY ("Id")
);
