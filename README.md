# Projeto base para Avaliação A1

## Orientações para rodar o backend

a) Fazer o download do arquivo .zip ou clonar o repositório:

```
git clone https://gitlab.com/gilbriatore/2024/prj-a1-csharp-react.git
```

b) Abrir o Visual Studio Code e selecionar o folder prj-a1-csharp-react/back.

c) Configurar a conexão com o MySQL no arquivo Banco.cs informando usuário e senha.

d) Rodar os comandos de instalação de pacotes no folder back:

```
dotnet tool install --global dotnet-ef --version 7.*
dotnet ef migrations add Inicializar
dotnet ef database update
```

e) Rodar a API:

```
dotnet run
```

## Orientações para rodar o frontend

a) Abrir o Visual Studio Code e selecionar o folder prj-a1-csharp-react/front.

b) Rodar o comando para instalar as dependências do Node.js:

```
npm install
```

c) Rodar o frontend:

```
npm start
```
