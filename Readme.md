# 💳 BankApp - Gerenciamento de Contas Bancárias

Projeto completo desenvolvido com foco em **boas práticas de arquitetura**, **testes automatizados**, **Clean Code**, e **performance escalável**. Esta aplicação simula a gestão de contas bancárias e integra autenticação com JWT, filtros globais, paginação, testes automatizados, consumo de APIs externas e muito mais.

---

## 🚀 Tecnologias e Ferramentas Utilizadas

* ✅ **.NET 8 (C#)**
* 🧱 **Entity Framework Core** (sem migrations automáticas)
* 🌐 **ASP.NET Core Web API**
* 🧭 **MediatR** – aplicação do padrão **CQRS**
* 🔁 **Refit** – cliente HTTP para integração com a **API de Compliance da Cubos.io**
* 🧪 **xUnit & Moq** – testes automatizados com mocking
* 🔐 **Autenticação JWT** – com Claims e validação personalizada
* 🗂️ **Repository Pattern** – abstração da camada de dados
* 🧼 **Validações robustas com DTOs**
* 📦 **AutoMapper** – mapeamento entre entidades e DTOs
* 🧰 **Injeção de Dependência (DI)** – nativa do ASP.NET Core
* 📄 **Swagger UI** – documentação interativa da API com suporte completo a testes

---

## 🛠️ Requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [PostgreSQL 17+](https://www.postgresql.org/)
* [Visual Studio 2022+ ou Visual Studio Code](https://visualstudio.microsoft.com/)
* [Git](https://git-scm.com/)

---

## ⚠️ Passo Inicial Obrigatório

Antes de qualquer comando, **crie o banco de dados manualmente** no PostgreSQL:

```sql
CREATE DATABASE bankapp;
```

---

## 📥 Como Clonar o Projeto

```bash
git clone https://github.com/EduardoCaversan/bank-api.git
cd bank-api
```

---

## ⚙️ Como Rodar o Projeto Localmente

1. **Configure o `appsettings.Development.json`**

No projeto `BankApp.WebApi`, configure sua connection string apontando para seu PostgreSQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=bankapp;Username=seu_usuario;Password=sua_senha"
  }
}
```

> Lembre-se de substituir com suas credenciais locais.

2. **Restaure os pacotes**

```bash
dotnet restore
```

3. **Gere as migrations e atualize o banco**

> O projeto **não utiliza migrations automáticas**, então é necessário gerá-las manualmente:

```bash
dotnet ef migrations add InitialCreate -p BankApp.Infrastructure -s BankApp.WebApi
dotnet ef database update -p BankApp.Infrastructure -s BankApp.WebApi
```

4. **Execute o projeto**

```bash
dotnet run --project BankApp.WebApi
```

Ou abra no Visual Studio e pressione **F5**.

---

## 🧪 Como Rodar os Testes Automatizados

```bash
dotnet test
```

Os testes estão localizados no projeto `BankApp.Tests` e cobrem controladores, repositórios e fluxos de negócio com cobertura real via mocks.

---

## 📄 Documentação da API

Assim que o projeto estiver rodando, acesse:

```
https://localhost:5257/swagger
```

Você verá a documentação interativa da API com todos os endpoints disponíveis, parâmetros, bodies esperados e códigos de retorno.

---

## 📚 Principais Funcionalidades

* ✅ Cadastro e gerenciamento de contas bancárias
* 💸 Registro de transações (crédito/débito)
* 🧾 Integração com a **API externa de Compliance (Cubos.io)**
* 🔐 Autenticação via JWT com validação de Claims
* ⚙️ Separação clara de camadas: `Domain`, `Application`, `Infrastructure`, `WebApi`, `Tests`
* 🔍 Filtros globais de exceção e validação
* 🧼 Validações estruturadas com FluentValidation
* 📄 Paginação customizada com metadados
* 🧪 Testes unitários com alta cobertura
* 🧭 Aplicação do padrão CQRS com **MediatR**

---

## ✅ Checklist de Qualidade

* [x] Clean Architecture
* [x] Clean Code
* [x] Paginação e ordenação eficiente
* [x] Testes unitários com mocks
* [x] Autenticação segura via JWT
* [x] Integração com serviço externo (Compliance)
* [x] DTOs e validação robusta
* [x] Filtros globais para logging e erros
* [x] Projeto pronto para deploy

---

## 📫 Contato

Criado por **Eduardo Caversan**
📧 [educaversan.dev@gmail.com](mailto:educaversan.dev@gmail.com)
🔗 [linkedin.com/in/deveduardocaversan](https://linkedin.com/in/deveduardocaversan)