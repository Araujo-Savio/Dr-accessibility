# Dr. Accessibility

Aplicação console escrita em C#/.NET para auxiliar médicas e médicos que atuam em regiões com pouca ou nenhuma conectividade com a internet. Todo o fluxo foi pensado para funcionar totalmente offline em uma máquina local, com possibilidade futura de sincronização dos dados com a nuvem quando a conexão estiver disponível.

## Funcionalidades principais

- **Cadastro completo de pacientes** com informações de contato, endereço, documentos e observações clínicas.
- **Agendamento e acompanhamento de consultas**, incluindo registro de anamnese importada de planilhas Excel.
- **Modelos reutilizáveis de anamnese e de receitas**, facilitando o preenchimento rápido das fichas.
- **Geração e armazenamento de receitas personalizadas** por paciente, com histórico completo e exportação direta para PDF contendo os dados do profissional de saúde.
- **Persistência local com SQLite**, garantindo funcionamento offline e facilitando uma futura sincronização com serviços online.

## Estrutura do projeto

```
src/
  DrAccessibility.App/
    Program.cs                 # Interface de linha de comando
    DrAccessibility.App.csproj # Projeto .NET 7
    Models/                    # Entidades do domínio
    Data/                      # Inicialização do banco SQLite
    Services/                  # Serviços de acesso a dados, importação e PDF
```

## Como executar

1. Instale o [.NET 7 SDK](https://dotnet.microsoft.com/pt-br/download).
2. No diretório raiz do repositório execute o build da solução para garantir que todas as dependências foram restauradas:

   ```bash
   dotnet build DrAccessibility.sln
   ```

3. Para iniciar a aplicação em modo interativo utilize:

   ```bash
   dotnet run --project src/DrAccessibility.App/DrAccessibility.App.csproj
   ```

4. O arquivo `clinic.db` será criado automaticamente na primeira execução na pasta de dados do usuário (por exemplo, `%LOCALAPPDATA%/DrAccessibility` no Windows), armazenando todos os dados localmente.

## Uso rápido

1. Configure o perfil do profissional na opção `1` do menu.
2. Cadastre pacientes e, se desejar, importe modelos de anamnese a partir de planilhas Excel (`.xlsx` ou `.xls`).
3. Agende consultas, registre receitas (manuais ou com base em modelos) e gere PDFs personalizados diretamente pelo menu.
4. Utilize marcadores nos modelos de receita para preencher dados automaticamente:
   - `{{Paciente.Nome}}`
   - `{{Paciente.Idade}}`
   - `{{Paciente.DataNascimento}}`

## Próximos passos sugeridos

- Implementar um serviço de sincronização com a nuvem para replicar os dados do `clinic.db` quando houver conexão.
- Disponibilizar uma interface gráfica amigável (web ou desktop) consumindo os mesmos serviços e banco de dados.
- Embutir mecanismos de backup automático em mídias removíveis para ampliar a resiliência em ambientes remotos.

## Licença dos pacotes utilizados

A aplicação utiliza bibliotecas de terceiros, como `ExcelDataReader` para leitura de planilhas e `QuestPDF` para geração de arquivos PDF. Consulte as licenças de cada dependência antes de distribuir o software.
