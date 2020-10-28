# payment-api
A financial REST API project using ASP.NET Core, Entity and PostgreSQL.

## Configuração para executar

```bash
# verificar se o dotnet está instalado
$ dotnet -v
# caso não esteja, instalar o dotnet core https://dotnet.microsoft.com/download

# clonar o projeto
$ git clone https://github.com/pedr-munhoz/payment-api.git

# entrar na pasta do projeto
$ cd payment-api
	
# restaurar dependências do projeto
$ dotnet restore --no-cache

# verificar se o docker está instalado 
$ docker -v
# caso não esteja, instalar o docker https://www.docker.com/get-started

# subir o banco no docker
$ docker pull postgres
$ docker run --name postgres_test_db -e POSTGRES_PASSWORD=123456 -d -p 5432:5432 postgres
$ docker start postgres_test_db

# verificar se o dotnet-ef está instalado 
$ dotnet-ef -v
# caso não esteja, instalar a ferramenta 
$ dotnet tool install --global dotnet-ef

# aplicar migrations no banco
$ dotnet ef database update --context ServerDbContext

# servir o projeto em http://localhost:5000/
$ dotnet run
  ```
