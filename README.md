# payment-api
A financial REST API project using ASP.NET Core, Entity and PostgreSQL.

## Assinatura da API
Para consumir a API, consulte a [documentação](https://github.com/pedr-munhoz/payment-api/blob/master/api-signature.md).


## Configuração para executar

```bash
# Verificar se o dotnet está instalado
$ dotnet -v
# Caso não esteja, instalar o dotnet core https://dotnet.microsoft.com/download

# Clonar o projeto
$ git clone https://github.com/pedr-munhoz/payment-api.git

# Entrar na pasta do projeto
$ cd payment-api
	
# Restaurar dependências do projeto
$ dotnet restore --no-cache

# Verificar se o docker está instalado 
$ docker -v
# Caso não esteja, instalar o docker https://www.docker.com/get-started

# Subir o banco no docker
$ docker pull postgres
$ docker run --name postgres_test_db -e POSTGRES_PASSWORD=123456 -d -p 5432:5432 postgres
$ docker start postgres_test_db

# Verificar se o EF Core está instalado 
$ dotnet-ef -v
# Caso não esteja, instalar a ferramenta 
$ dotnet tool install --global dotnet-ef

# Aplicar migrations no banco
$ dotnet ef database update --context ServerDbContext

# Servir o projeto em http://localhost:5000/
$ dotnet run
  ```
